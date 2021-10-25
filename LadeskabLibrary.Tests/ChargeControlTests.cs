using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NSubstitute;
using NUnit.Framework;


namespace LadeskabLibrary.Tests
{
    [TestFixture]
    class ChargeControlTests_ZeroTest
    {
        private IChargeControl _uut;
        private IUsbCharger _usbSource;

        [SetUp]
        public void Setup()
        {
            _usbSource = Substitute.For<IUsbCharger>();
            _uut = new ChargeControl(_usbSource);
        }

        #region ZeroTests
        //OBS May be white-box testing..?
        [Test]
        public void InitialState_Idle()
        {
            Assert.That(_uut.CurrentChargingState, Is.EqualTo(ChargingState.IDLE));
        }

        [Test]
        public void InitialState_NotConnected()
        {
            Assert.That(_uut.IsConnected, Is.False);
        }

        #endregion
    }


    [TestFixture]
    class ChargeControlTests_StartCharge
    {
        private IChargeControl _uut;
        private ChargingStateEventArgs _receivedEventArgs;
        private IUsbCharger _usbSource;
        private int _eventsfired;

        [SetUp]
        public void Setup()
        {
            //Stub
            _usbSource = Substitute.For<IUsbCharger>();
            _uut = new ChargeControl(_usbSource);
            _eventsfired = 0;

            _uut.ChargingStateEvent += (o, args) =>
            {
                _receivedEventArgs = args;
                _eventsfired++;
            };

            //StartCharge has been run.
            //USB is connected.
            _uut.StartCharge();
            _usbSource.Connected.Returns(true);
        }


        #region OneTests
        [Test]
        public void StartCharge_PhoneNotConnected_EventFired()
        {
            //_uut.StartCharge();
            _usbSource.Received().StartCharge();
        }

        [TestCase(0)]
        [TestCase(-2)]
        [TestCase(double.MinValue)]
        public void StartCharge_PhoneNotConnected_Idle(double c)
        {
            //_uut.StartCharge(); Always after this call
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs { Current = c });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.IDLE));
        }

        [TestCase(0.01)]
        [TestCase(5)]
        public void StartCharge_LowCurrent_Full(double c)
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = c});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.FULL));
        }

        [TestCase(5.01)]
        [TestCase(500)]
        public void StartCharge_NormalCurrent_Chargning(double c)
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = c });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.CHARGING));
        }

        [TestCase(500.01)]
        [TestCase(1200)]
        [TestCase(double.MaxValue)]
        public void StartCharge_HighCurrent_Overload(double c)
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = c});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
        }

        #endregion

        #region ManyTests
        //OBS Idle case may be CHARGING, to avoid flickering (current could be 0 <-> 0.001)
        [TestCase(2.2, ChargingState.FULL)]
        [TestCase(520, ChargingState.OVERLOAD)]
        [TestCase(0, ChargingState.IDLE)]
        [TestCase(420, ChargingState.CHARGING)]
        public void StartCharge_FromChargingToNewCurrent_CorrectStateSent(double newCurrent, ChargingState newState)
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = 499});
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = newCurrent});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(newState));
        }
        
        [Test]
        public void StartCharge_FromChargingToChargingNewCurrent_NoNewEvents()
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 499 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 420 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 320 });
            Assert.That(_eventsfired, Is.EqualTo(1));
        }

        [Test]
        public void StartCharge_FromChargingToNoCurrent_Idle()
        {
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 499 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 0 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.IDLE));
        }

        //Only one event going to OVERLOAD
        [TestCase(2.5)]
        [TestCase(0)]
        [TestCase(250)]
        [TestCase(900)]
        [TestCase(501)]
        public void StartCharge_FromOverloadToNewCurrentWithoutDisconnect_StillOverload(double newCurrent)
        {
            _uut.StartCharge();
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 1000 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = newCurrent });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
            Assert.That(_eventsfired, Is.EqualTo(1));
        }

        #endregion



        //Should this be possible, when StartCharge() from StatonControl is not activated?
        [Test]
        public void OnNewCurrentEvent_PhoneNotConnected_EventFired()
        {
            //Control recives an event, which triggers own event
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs { Current = 0 });

            Assert.That(_receivedEventArgs, Is.Not.Null);
        }

        //OnNewCurrent

    }
}