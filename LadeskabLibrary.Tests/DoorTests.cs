using NUnit.Framework;
using NSubstitute;
using System;

namespace LadeskabLibrary.Tests
{
    class DoorTests
    {
        private Door Uut;
        private IDoor.DoorEventArgs _receivedDoorOpenEventArgs;
        private IDoor.DoorEventArgs _receivedDoorClosedEventArgs;
        [SetUp]
        public void Setup()
        {
            _receivedDoorOpenEventArgs = null;
            Uut = new Door(true,false);

            Uut.DoorClosed += (o, args) => { _receivedDoorClosedEventArgs = args; };
            Uut.DoorOpened += (o, args) => { _receivedDoorOpenEventArgs = args; };
        }

        #region Door() Tests
        [Test]
        public void Door_DoorWithDoorOpenFalseLockStatausFalse_AssertStates()
        {
            Uut = new Door(false, false);
            Assert.That(Uut.OpenStatOpen is false & Uut.lockedStatus is false);
        }
        [Test]
        public void Door_DoorWithDoorOpenTrueLockStatausFalse_AssertStates()
        {
            Uut = new Door(true, false);
            Assert.That(Uut.OpenStatOpen is true & Uut.lockedStatus is false);
        }
        [Test]
        public void Door_DoorWithDoorOpenFalseLockStatausTrue_AssertStates()
        {
            Uut = new Door(false, true);
            Assert.That(Uut.OpenStatOpen is false  & Uut.lockedStatus is true);
        }
        [Test]
        public void Door_DoorWithDoorOpenTrueLockStatausTrue_AssertExceptionThrown()
        {
            var exc = Assert.Throws<Exception>(() => Uut = new Door(true, true));
        }

        #endregion
        #region LockDoor() Tests
        [Test]
        public void LockDoor_LockDoorWithDoorClosedDoorUnlocked_AssertDoorLockedTrue()
        {
            Uut.OpenStatOpen = false;
            Uut.lockedStatus = false;
            Uut.LockDoor();
            Assert.That(Uut.lockedStatus is true);
        }
        [Test]
        public void LockDoor_LockDoorWithDoorOpenDoorUnlocked_AssertExceptionThrown()
        {
            Uut.OpenStatOpen = true;
            Uut.lockedStatus = false;
            var exc = Assert.Throws<Exception>(() => Uut.LockDoor());
        }
        #endregion
        #region UnlockDoor() Tests
        [Test]
        public void UnlockDoor_UnlockDoorWithDoorClosedDoorLocked_AssertDoorUnlocked()
        {
            Uut.lockedStatus = true;
            Uut.OpenStatOpen = false;
            Uut.UnlockDoor();
            Assert.That(Uut.lockedStatus is false);
        }

        [Test]
        public void UnlockDoor_UnlockDoorWithDoorClosedDoorUnlocked_AssertDoorUnlocked()
        {
            Uut.lockedStatus = false;
            Uut.OpenStatOpen = false;
            Uut.UnlockDoor();
            Assert.That(Uut.lockedStatus is false);
        }

        #endregion
        #region OnDoorOpen() Tests
        [Test]
        public void OnDoorOpen_OpenDoorWithDoorClosedDoorUnlocked_AssertArgsTrue()
        {
            Uut.lockedStatus = false;
            Uut.OpenStatOpen = false;
            Uut.OnDoorOpen();
            Assert.That(_receivedDoorOpenEventArgs.doorStatus is true);
        }

        [Test]
        public void OnDoorOpen_OpenDoorWithDoorOpenDoorUnlocked_AssertExceptionThrown()
        {
            Uut.lockedStatus = false;
            Uut.OpenStatOpen = true;
            var exc = Assert.Throws<Exception>(() => Uut.OnDoorOpen());
        }

        [Test]
        public void OnDoorOpen_OpenDoorWithDoorClosedDoorLocked_AssertExceptionThrown()
        {
            Uut.lockedStatus = true;
            Uut.OpenStatOpen = false;
            var exc = Assert.Throws<Exception>(() => Uut.OnDoorOpen());
        }
        #endregion
        #region OnDoorClose() Tests
        [Test]
        public void OnDoorClose_CloseDoorWithDoorOpenDoorUnlocked_AssertArgsTrue()
        {
            Uut.lockedStatus = false;
            Uut.OpenStatOpen = true;
            Uut.OnDoorClose();
            Assert.That(_receivedDoorClosedEventArgs.doorStatus is false);
        }

        [Test]
        public void OnDoorClose_CloseDoorWithDoorClosedDoorUnlocked_AssertExceptionThrown()
        {
            Uut.lockedStatus = false;
            Uut.OpenStatOpen = false;
            var exc = Assert.Throws<Exception>(() => Uut.OnDoorClose());
        }
        #endregion

    }
}
