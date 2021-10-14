using NUnit.Framework;
using NSubstitute;
using System;

namespace LadeskabLibrary.Tests
{
    class DoorTests
    {
        private Door Uut;
        [SetUp]
        public void Setup()
        {
            Uut = new Door(true,false);
        }

        #region Door() Tests
        [Test]
        public void Door_DoorWithDoorOpenFalseLockStatausFalse_AssertStates()
        {
            Uut = new Door(false, false);
            Assert.That(Uut.doorOpen is false & Uut.lockedStatus is false);
        }
        [Test]
        public void Door_DoorWithDoorOpenTrueLockStatausFalse_AssertStates()
        {
            Uut = new Door(true, false);
            Assert.That(Uut.doorOpen is true & Uut.lockedStatus is false);
        }
        [Test]
        public void Door_DoorWithDoorOpenFalseLockStatausTrue_AssertStates()
        {
            Uut = new Door(false, true);
            Assert.That(Uut.doorOpen is false  & Uut.lockedStatus is true);
        }
        [Test]
        public void Door_DoorWithDoorOpenTrueLockStatausTrue_AssertExceptionThrown()
        {
            var exc = Assert.Throws<Exception>(() => Uut = new Door(true, true));
        }

        #endregion
        #region LockDoor() Tests
        [Test]
        public void Door_LockDoorWithDoorClosedDoorUnlocked_AssertDoorLockedTrue()
        {
            Uut.doorOpen = false;
            Uut.lockedStatus = false;
            Uut.LockDoor();
            Assert.That(Uut.lockedStatus is true);
        }
        [Test]
        public void Door_LockDoorWithDoorOpenDoorUnlocked_AssertExceptionThrown()
        {
            Uut.doorOpen = true;
            Uut.lockedStatus = false;
            var exc = Assert.Throws<Exception>(() => Uut.LockDoor());
        }
        #endregion
        #region OnDoorOpen() Tests
        #endregion
    }
}
