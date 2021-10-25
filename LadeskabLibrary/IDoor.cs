using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public interface IDoor
    {
        public class DoorEventArgs : EventArgs
        {
            public bool doorStatus { get; set; }
        }
        void LockDoor();
        void UnlockDoor();
        void OnDoorOpen();
        void OnDoorClose();
        event EventHandler<IDoor.DoorEventArgs> DoorOpened;
        event EventHandler<IDoor.DoorEventArgs> DoorClosed;    }

    public class Door : IDoor
    {
        public event EventHandler<IDoor.DoorEventArgs> DoorOpened;
        public event EventHandler<IDoor.DoorEventArgs> DoorClosed;
        public bool OpenStatOpen
        { get; set;
        }
        public bool lockedStatus 
        { get; set;}

        public Door(bool openStat, bool lockStat)
        {
            if (openStat & lockStat)
                throw new Exception("Cant Lock open openStat");
            else
            {
                OpenStatOpen = openStat;
                lockedStatus = lockStat;
            }
        }

        public void LockDoor()
        {
            if (!OpenStatOpen)
                lockedStatus = true;
            else
                //throw exception on OpenStatOpen = true and lockedStatus= true
                throw new Exception("DoorIsOpen");
        }

        public void UnlockDoor()
        {
            lockedStatus = false;
        }

        public void OnDoorOpen()
        {
            if (lockedStatus)
                throw new Exception("DoorIsLocked");
            else if (OpenStatOpen)
            {
                throw new Exception("DoorIsOpen");
            }
            //throw exception
            else
            {
                OpenStatOpen = true;
                DoorOpened?.Invoke(this, new IDoor.DoorEventArgs() { doorStatus = true });
            }
        }

        public void OnDoorClose()
        {
            if (!OpenStatOpen)
                throw new Exception("Door allready closed");
            else
            {
                OpenStatOpen = false;
                DoorClosed?.Invoke(this, new IDoor.DoorEventArgs() { doorStatus = false });
            }
        }
    }
}
