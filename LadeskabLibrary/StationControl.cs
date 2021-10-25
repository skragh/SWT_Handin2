using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public class StationControl : IStationControl
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
            this.reader.NewRead += HandleIdDetected;
            chargeControl = _chargeControl;
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
            newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Indlæs RFID" });
        }

        public bool CheckId(int id)
        {
            return currentId == id;
        }
        public void HandleIdDetected(object sender, IReader.ReaderEventArgs e)
        {
            if (!inUse)
            {
                if (!chargeControl.IsConnected())
                    newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Tilslutningsfejl" });
                else
                {
                    try
                    {
                        door.LockDoor();
                        chargeControl.StartCharge();
                        currentId = e.idRead;
                        logger.LogDoorLocked(e.idRead);
                        newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Ladeskab Optaget" });
                        inUse = true;
                    }
                    catch (Exception exc)
                    {
                        ErrorHandler(exc.Message);
                    }
                }
            }
            else
            {
                if (!CheckId(e.idRead))
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
        public void ErrorHandler(string message)
        {
            newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = $"ERROR: {message}" });
        }
    }

    public interface IStationControl
    {
        event EventHandler<StationControl.StationMessageEventArgs> newDisplayMessage;
        IChargeControl chargeControl { get; set; }
        IDoor door { get; set; }
        IReader reader { get; set; }
        ILogger logger { get; set; }
        bool isDoorOpen { get; set; }
        bool inUse { get; set; }
        int? currentId { get; set; }
        void HandleDoorOpened(object sender, IDoor.DoorEventArgs e);
        void HandleDoorClosed(object sender, IDoor.DoorEventArgs e);
        bool CheckId(int id);
        void HandleIdDetected(object sender, IReader.ReaderEventArgs e);
        void ErrorHandler(string message);

    }
}
