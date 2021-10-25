using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
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

        [Test]
        public void StartCharging_CallsStartChargingOnUSB_FunctionCalled()
        {
            _uut.StartCharge();
            _usbSource.Received(1).StartCharge();
        }

        [Test]
        public void StopCharging_CallsStopChargingOnUSB_FunctionCalled()
        {
            _uut.StopCharge();
            _usbSource.Received(1).StopCharge();
        }
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

            _uut.StartCharge();
            SetInitialState(ChargingState.IDLE);
        }


        #region OneTests
        [Test]
        public void StartCharge_PhoneNotConnected_EventFired()
        {
            _usbSource.Received().StartCharge();
        }

        [TestCase(0)]
        [TestCase(-2)]
        [TestCase(double.MinValue)]
        public void StartCharge_PhoneNotConnected_Idle(double c)
        {
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

        #region Charging
        //OBS Idle case may be CHARGING, to avoid flickering (current could be 0 <-> 0.001)
        [TestCase(2.2, ChargingState.FULL)]
        [TestCase(520, ChargingState.OVERLOAD)]
        [TestCase(0, ChargingState.IDLE)]
        [TestCase(420, ChargingState.CHARGING)]
        public void StartCharge_FromChargingToNewCurrent_CorrectStateSent(double newCurrent, ChargingState newState)
        {
            SetInitialState(ChargingState.CHARGING);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = newCurrent});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(newState));
        }
        
        [Test]
        public void StartCharge_FromChargingToChargingNewCurrent_NoNewEvents()
        {
            SetInitialState(ChargingState.CHARGING);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 420 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 320 });
            Assert.That(_eventsfired, Is.EqualTo(0));
        }

        [Test]
        public void StartCharge_FromChargingToNoCurrent_Idle()
        {
            SetInitialState(ChargingState.CHARGING);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 0 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.IDLE));
        }
        #endregion

        #region Overload
        [TestCase(2.5)]
        [TestCase(0)]
        [TestCase(250)]
        [TestCase(900)]
        [TestCase(501)]
        public void StartCharge_FromOverloadToNewCurrentWithoutDisconnect_StillOverload(double newCurrent)
        {
            SetInitialState(ChargingState.OVERLOAD);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = newCurrent });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
            Assert.That(_eventsfired, Is.EqualTo(0));
        }

        [Test]
        public void StartCharge_FromOverloadToHighCurrent_NoNewEvents()
        {
            SetInitialState(ChargingState.OVERLOAD);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 1200 });
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 501 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
            Assert.That(_eventsfired, Is.EqualTo(0));
        }


        //OBS Potential error, if connect is for some reason false AFTER current set at 0.
        [Test]
        public void StartCharge_FromOverloadToDisconnectAndNoCurrent_Idle()
        {
            SetInitialState(ChargingState.OVERLOAD);
            _usbSource.Connected.Returns(false);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = 0});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.IDLE));
        }

        [Test]
        public void StartCharge_FromOverloadToDisconnectAndHighCurrent_StillOverload()
        {
            SetInitialState(ChargingState.OVERLOAD);
            _usbSource.Connected.Returns(true);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 550 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
        }
        #endregion

        #region FULL

        [Test]
        public void StartCharge_FromFullToNoCurrent_Idle()
        {
            SetInitialState(ChargingState.FULL);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = 0});
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.IDLE));
        }

        //OBS Potential risk of failure, if current starts low, before becoming normal
        [Test]
        public void StartCharge_FromFullToNormalCurrent_StillFull()
        {
            SetInitialState(ChargingState.FULL);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 500 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.FULL));
        }

        [Test]
        public void StartCharge_FromFullToHighCurrent_Overload()
        {
            SetInitialState(ChargingState.FULL);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 1000 });
            Assert.That(_receivedEventArgs._chargingState, Is.EqualTo(ChargingState.OVERLOAD));
        }

        [TestCase(0.001)]
        [TestCase(5)]
        [TestCase(3)]
        public void StartCharge_FromFullToLowCurrent_NoNewEvent(double newCurrent)
        {
            SetInitialState(ChargingState.FULL);
            _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = newCurrent});
            Assert.That(_eventsfired, Is.EqualTo(0));
        }


        #endregion

        #endregion


        //Helper functions
        public void SetInitialState(ChargingState state)
        {
            switch (state)
            {
                case ChargingState.IDLE:
                    _usbSource.Connected.Returns(false);
                    _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 0 });
                    break;

                case ChargingState.CHARGING:
                    _usbSource.Connected.Returns(true);
                    _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 500 });
                    break;

                case ChargingState.FULL:
                    _usbSource.Connected.Returns(true);
                    _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 5 });
                    break;

                case ChargingState.OVERLOAD:
                    _usbSource.Connected.Returns(true);
                    _usbSource.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 501 });
                    break;
            }

            if (_receivedEventArgs._chargingState == state)
            {
                _eventsfired = 0;
            }
            else
            {
                throw new Exception(
                    $"Error setting initial state. \nnew Expected state: {state}\n But state was: {_receivedEventArgs._chargingState}");
            }
        }
    }
}