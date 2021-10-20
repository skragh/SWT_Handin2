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
        public IChargeControl chargeControl { get; set; }
        public IDoor door { get; set; }
        public bool isDoorOpen { get; set; }

        public StationControl(IDoor door, IChargeControl _chargeControl)
        {
            this.door = door;
            door.DoorOpened += HandleDoorOpened;
            door.DoorClosed += HandleDoorClosed;
            chargeControl = _chargeControl;
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


    }
}
