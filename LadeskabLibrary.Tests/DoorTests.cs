using NUnit.Framework;
using NSubstitute;

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
        [Test]
        public void Door_LockDoorWithDoorClosedDoorUnlocked_AssertDoorLockedTrue()
        {
            Uut = new Door(false, false);
            Uut.LockDoor();
            Assert.That(Uut.lockedStatus is true);
        }
        [Test]
        public void Door_LockDoorWithDoorOpenDoorUnlocked_AssertDoorLockedFalse()
        {
            Uut = new Door(true, false);
            Uut.LockDoor();
            Assert.That(Uut.lockedStatus is false);
        }
        [Test]
        public void Door_LockDoorWithDoorClosedDoorLocked_AssertDoorLockedFalse()
        {
            Uut = new Door(true, true);
            Uut.LockDoor();
            Assert.That(Uut.lockedStatus is true);
        }
    }
}
