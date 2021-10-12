using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    interface IDoor
    {
        public class DoorEventArgs : EventArgs
        {
            public bool doorStatus { get; set; }
        }
        void LockDoor();
        void UnlockDoor();
        void OnDoorOpen();
        void OnDoorClose();
        event EventHandler DoorOpened;
        event EventHandler DoorClosed;
    }

    public class Door : IDoor
    {
        public event EventHandler DoorOpened;
        public event EventHandler DoorClosed;
        public bool doorOpen { get; set; }
        public bool lockedStatus { get; set; }

        public Door(bool door, bool lockStat)
        {
            doorOpen = door;
            lockedStatus = lockStat;
            //throw exception on doorOpen = true and lockedStatus= true
        }

        public void LockDoor()
        {
            if (!doorOpen)
                lockedStatus = true;
            else
                Console.WriteLine("Exception");
                //throw exception door isnt closed
        }

        public void UnlockDoor()
        {
            lockedStatus = false;
        }

        public void OnDoorOpen()
        {
            if (lockedStatus)
                Console.WriteLine("didnt happen");
            //throw exception
            else
            {
                doorOpen = true;
                DoorOpened?.Invoke(this, new IDoor.DoorEventArgs() { doorStatus = true });
            }
        }

        public void OnDoorClose()
        {
            doorOpen = false;
            DoorOpened?.Invoke(this, new IDoor.DoorEventArgs() { doorStatus = false });
        }
    }
}
