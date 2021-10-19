using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    class StationControl
    {
        public class StationMessageEventArgs : EventArgs
        {
            public string message { get; set; }
        }
        public event EventHandler<StationMessageEventArgs> newDisplayMessage;
        public IChargeControl chargeControl { get; set; }
        public IDoor door { get; set; }

        StationControl(IDoor door, IChargeControl _chargeControl)
        {
            this.door = door;
            door.DoorOpened += HandleDoorOpened;
            door.DoorClosed += HandleDoorClosed;
            chargeControl = _chargeControl;
        }
        
        private void HandleDoorOpened(object sender, IDoor.DoorEventArgs e)
        {
                newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Tilslut Telefon" });
        }
        private void HandleDoorClosed(object sender, IDoor.DoorEventArgs e)
        {
                newDisplayMessage?.Invoke(this, new StationMessageEventArgs() { message = "Indlæse RFID" });
        }


    }
}
