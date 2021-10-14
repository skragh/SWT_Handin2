using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;


namespace LadeskabLibrary.Tests
{
    [TestFixture]
    class ChargeControlTest
    {
        private IChargeControl _uut;
        private ChargingStateEventArgs _receivedEventArgs;
        private IUsbCharger _usbSource;

        [SetUp]
        public void Setup()
        {
            _usbSource = Substitute.For<IUsbCharger>();
            _uut = new IChargeControl(_usbSource);

            _uut.ChargingStateEvent += (o, args) =>
            {
                _receivedEventArgs = args;
            };
        }

        #region ZeroTests
        [Test]
        public void InitialStateIsDisconnected()
        {
            Assert.That(_uut.IsConnected, Is.False);
        }
        #endregion

        [Test]
        public void StartCharge_PhoneNotConnected_EventFired()
        {
            _uut.StartCharge();
            _usbSource.Received().StartCharge();
        }

        //Should this be possible, when StartCharge() is not activated?
        [Test]
        public void OnNewCurrentEvent_PhoneNotConnected_EventFired()
        {
            //Control recives an event, which triggers own event
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs { Current = 0 });

            Assert.That(_receivedEventArgs, Is.Not.Null);
        }

        [Test]
        public void StartCharge_PhoneNotConnected_CorrectStateSent()
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs { Current = 0 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.DISCONNECTED));
        }
    }
}