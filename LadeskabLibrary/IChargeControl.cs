using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LadeskabLibrary
{
    public enum ChargingState
    {
        CHARGING,
        FULL,
        IDLE,
        OVERLOAD
    }

    public class ChargingStateEventArgs : EventArgs
    {
        public ChargingState _chargingState;
    }

    public interface IChargeControl
    {
        //Event triggered when charging state is different
        public event EventHandler<ChargingStateEventArgs> ChargingStateEvent;
        public ChargingState CurrentChargingState {get; }
        bool IsConnected();
        void StartCharge();
        void StopCharge();
    }

    public class ChargeControl : IChargeControl
    {
        //Charger object
        public IUsbCharger UsbCharger { get; set; }

        //The charging state
        public ChargingState CurrentChargingState { get; private set; }

        //If charging is active or not
        private bool _charging;

        //Event other obj can subscribe to
        public event EventHandler<ChargingStateEventArgs> ChargingStateEvent;

        //Ctor-injection
        public ChargeControl(IUsbCharger usbCharger)
        {
            _charging = false;
            CurrentChargingState = ChargingState.IDLE;
            UsbCharger = usbCharger;
            usbCharger.CurrentValueEvent += HandleCurrentChanged;
        }
        
        public bool IsConnected()
        {
            return UsbCharger.Connected;
        }

        public void StartCharge()
        {
            _charging = true;
            UsbCharger.StartCharge();
        }
        public void StopCharge()
        {
            _charging = false;
            UsbCharger.StopCharge();
            CurrentChargingState = ChargingState.IDLE;
        }
        

        //OBS May need a tolorance of 0.01A.
        public void HandleCurrentChanged(object sender, CurrentEventArgs e)
        {
            //Only execute if charging
            if (_charging)
            {
                var currentValue = e.Current;
                if (CurrentChargingState != ChargingState.OVERLOAD)
                {
                    if (currentValue == 0)
                    {
                        CurrentChargingState = ChargingState.IDLE;
                        OnNewChargeState();
                    }
                    else if (currentValue > 0 && currentValue <= 5 &&
                             CurrentChargingState != ChargingState.FULL)
                    {
                        //Write "Fully charged"
                        CurrentChargingState = ChargingState.FULL;
                        OnNewChargeState();
                    }
                    else if (currentValue > 5 && currentValue <= 500 &&
                             CurrentChargingState != ChargingState.CHARGING &&
                             CurrentChargingState != ChargingState.FULL)
                    {
                        //Write "Charging"
                        CurrentChargingState = ChargingState.CHARGING;
                        OnNewChargeState();
                    }
                    else if (currentValue > 500 &&
                             CurrentChargingState != ChargingState.OVERLOAD)
                    {
                        UsbCharger.StopCharge();
                        //Write "Overload error!"
                        CurrentChargingState = ChargingState.OVERLOAD;
                        OnNewChargeState();
                    }
                }
                else
                {
                    if (!UsbCharger.Connected && currentValue == 0)
                    {
                        CurrentChargingState = ChargingState.IDLE;
                        OnNewChargeState();
                    }
                }
            }
        }
        void OnNewChargeState()
        {
            ChargingStateEvent?.Invoke(this, new ChargingStateEventArgs() {_chargingState = this.CurrentChargingState });
        }
    }
}
