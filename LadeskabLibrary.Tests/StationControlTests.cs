﻿using System;
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
        [TestCase]

        public void CheckId_CurrntIdNullIdNotNull_AssertFalse(int id)
        {
            Uut.currentId = null;
            Assert.That(Uut.CheckId(id) is false);
        }
        #endregion
        #region HandleIdDetected tests
        [Test]

        #endregion
    }
}
