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
        private ChargeControl _uut;
        private ChargingStateEventArgs _receivedEventArgs;
        private IUsbCharger _usbSource;

        [SetUp]
        public void Setup()
        {
            _usbSource = Substitute.For<IUsbCharger>();
            _uut = new ChargeControl(_usbSource);

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
            //Function start usbcharger, that then sends an event to control
            //_uut.StartCharge();

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