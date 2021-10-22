using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace LadeskabLibrary.Tests
{
    class StationControlTests
    {
        #region Setup
        private IDoor door;
        private IChargeControl chargeControl;
        private IReader reader;
        private ILogger logger;
        private StationControl Uut;
        private StationControl.StationMessageEventArgs msgArgs;
        [SetUp]
        public void Setup()
        {
            msgArgs = null;
            chargeControl = Substitute.For<IChargeControl>();
            door = Substitute.For<IDoor>();
            reader = Substitute.For<IReader>();
            logger = Substitute.For<ILogger>();
            Uut = new StationControl(door, chargeControl, reader, logger);
            Uut.newDisplayMessage += (o, args) => { msgArgs = args; };
        }
        #endregion
        #region DoorOpened/closed events
        [Test]
        public void HandleDoorOpened_EventTriggered_AssertMethodCalled()
        {
            door.DoorOpened += Raise.EventWith(new IDoor.DoorEventArgs { doorStatus = true });
            Assert.That(Uut.isDoorOpen is true);
        }

        [Test]
        public void HandleDoorOpened_EventTriggered_AssertNewDisplayMesssageEventTriggered()
        {
            Uut.HandleDoorOpened(door, new IDoor.DoorEventArgs { doorStatus = true });
            Assert.That(msgArgs, Is.Not.Null);
        }

        [Test]
        public void HandleDoorClosed_EventTriggered_AssertMethodCalled()
        {
            door.DoorClosed += Raise.EventWith(new IDoor.DoorEventArgs { doorStatus = false });
            Assert.That(Uut.isDoorOpen is false);
        }

        [Test]
        public void HandleDoorClosed_EventTriggered_AssertNewDisplayMesssageEventTriggered()
        {
            Uut.HandleDoorOpened(door, new IDoor.DoorEventArgs { doorStatus = false });
            Assert.That(msgArgs, Is.Not.Null);
        }
        #endregion
        #region CheckId tests
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        public void CheckId_CurrntIdNullIdNotNull_AssertFalse(int id)
        {
            Uut.currentId = null;
            Assert.That(Uut.CheckId(id) is false);
        }
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        public void CheckId_CurrentIdNotNullIdIdentical_AssertTrue(int id)
        {
            Uut.currentId = id;
            Assert.That(Uut.CheckId(id) is true);
        }
        [TestCase(5)]
        [TestCase(0)]
        [TestCase(-1)]
        public void CheckId_CurrentIdNotNullIdDifferent_AssertFalse(int id)
        {
            Uut.currentId = 3;
            Assert.That(Uut.CheckId(id) is false);
        }

        #endregion
        #region HandleIdDetected tests
        #region InUse == False
        [Test]
        public void HandIdDetected_NotInUseChargeConnectedFalse_AssertMsgSent()
        {
            chargeControl.IsConnected().Returns(false);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(msgArgs, Is.Not.Null);
        }
        [Test]
        public void HandIdDetected_NotInUseChargeConnectedFalse_AssertNotInUse()
        {
            chargeControl.IsConnected().Returns(false);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(Uut.inUse is false);
        }

        [Test]
        public void HandIdDetected_NotInUseChargeConnectedTrueDoorOpenTrue_AssertExceptionCaught()
        {
            chargeControl.IsConnected().Returns(true);
            string text = "FakeException";
            door.When(x => x.LockDoor()).Do(context => { throw new Exception(text); });
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(msgArgs.message, Contains.Substring(text));
        }
        [Test]
        public void HandIdDetected_NotInUseChargeConnectedTrueDoorOpenTrue_AssertInUseFalse()
        {
            chargeControl.IsConnected().Returns(true);
            string text = "FakeException";
            door.When(x => x.LockDoor()).Do(context => { throw new Exception(text); });
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(Uut.inUse is false);
        }
        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertId()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader,new IReader.ReaderEventArgs() { idRead=5 });
            Assert.That(Uut.currentId is 5);
        }
        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertLockDoorCalled()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            door.Received().LockDoor();
        }

        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertInUse()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(Uut.inUse is true);
        }
        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertMsgSent()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(msgArgs, Is.Not.Null);
        }
        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertLoggerCalled()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            logger.Received().LogDoorLockeD(5);
        }
        [Test]
        public void HandleIdDetected_NotInUseChargeConnectedTrueDoorClosed_AssertChargerCalled()
        {
            chargeControl.IsConnected().Returns(true);
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            chargeControl.Received().StartCharge();
        }

        #endregion
        #region InUse = true
        [Test]
        public void HandIdDetected_InUseTrueCheckIdFalse_AssertInUseTrue()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 3 });
            Assert.That(Uut.inUse = true);
        }
        [Test]
        public void HandIdDetected_InUseTrueCheckIdFalse_AssertMsgSent()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 3 });
            Assert.That(msgArgs, Is.Not.Null);
        }

        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_AssertInUseFalse()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(Uut.inUse is false);
        }

        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_AssertCurrentIdNull()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(Uut.currentId is null);
        }
        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_AssertMsgSent()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            Assert.That(msgArgs,Is.Not.Null);
        }
        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_StopChargeCalled()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            chargeControl.Received().StopCharge();
        }
        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_UnlockDoorCalled()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            door.Received().UnlockDoor();
        }
        [Test]
        public void HandIdDetected_InUseTrueCheckIdTrue_LogDoorUnlockedCalled()
        {
            Uut.currentId = 5;
            Uut.inUse = true;
            Uut.HandleIdDetected(reader, new IReader.ReaderEventArgs() { idRead = 5 });
            logger.Received().LogDoorUnlocked(5);
        }

        #endregion
        #endregion
    }
}
