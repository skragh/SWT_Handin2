using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public class StationControl
    {
        public class StationMessageEventArgs : EventArgs
        {
            public string message { get; set; }
        }

        public event EventHandler<StationMessageEventArgs> newDisplayMessage;

        #region properties
        public IChargeControl chargeControl { get; set; }
        public IDoor door { get; set; }
        public IReader reader { get; set; }
        public ILogger logger { get; set; }
        public bool isDoorOpen { get; set; }
        public bool inUse { get; set; } = false;
        public int? currentId { get; set; }
        #endregion
        public StationControl(IDoor door, IChargeControl _chargeControl, IReader reader, ILogger logger)
        {
            this.logger = logger;
            this.reader = reader;
            this.door = door;
            this.reader = reader;
            this.door.DoorOpened += HandleDoorOpened;
            this.door.DoorClosed += HandleDoorClosed;
            this.reader.NewRead += HandleIdRead;
            chargeControl = _chargeControl;
            chargeControl.ChargingStateEvent += HandleChargingState;

            currentId = null;
        }
        
        public void HandleDoorOpened(object sender, IDoor.DoorEventArgs e)
        {
            isDoorOpen = e.doorStatus;
            newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Tilslut Telefon" });
        }
        public void HandleDoorClosed(object sender, IDoor.DoorEventArgs e)
        {
            isDoorOpen = e.doorStatus;
            newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Indlæse RFID" });
        }

        public bool CheckId(int id)
        {
            return currentId == id;
        }
        public void HandleIdDetected(object sender,IReader.ReaderEventArgs e)
        {
            if (!inUse)
            {
                if (isDoorOpen)
                    newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Luk Døren og genindlæs RFID" });
                else
                {
                    if (!chargeControl.IsConnected())
                        newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Tilslutningsfejl" });
                    else
                    {
                        chargeControl.StartCharge();
                        door.LockDoor();
                        currentId = e.idRead;
                        logger.LogDoorLockeD(e.idRead);
                        newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Ladeskab Optaget" });
                        inUse = true;
                    }
                }
            }
            else
            {
                if (CheckId(e.idRead))
                    newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "RFID fejl" });
                else
                {
                    chargeControl.StopCharge();
                    door.UnlockDoor();
                    logger.LogDoorUnlocked(e.idRead);
                    newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Fjern telefon" });
                    inUse = false;
                    currentId = null;
                }
            }
        }

        public void HandleChargingState(object sender, ChargingStateEventArgs e)
        {
            //chargingState = e._chargingState;
        }

    }
}
