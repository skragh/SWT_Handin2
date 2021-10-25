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

        //Event other obj can subscribe to
        public event EventHandler<ChargingStateEventArgs> ChargingStateEvent;

        //Ctor-injection
        public ChargeControl(IUsbCharger usbCharger)
        {
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
            UsbCharger.StartCharge();
        }
        public void StopCharge()
        {
            UsbCharger.StopCharge();
        }
        

        //May need a tolorance of 0.1A.
        public void HandleCurrentChanged(object sender, CurrentEventArgs e)
        {
            var currentValue = e.Current;
            if (currentValue == 0)
            {
                CurrentChargingState = ChargingState.IDLE;
                OnNewChargeState();
            }
            else if (currentValue > 0 && currentValue <= 5 && CurrentChargingState != ChargingState.FULL)
            {
                //Write "Fully charged"
                CurrentChargingState = ChargingState.FULL;
                OnNewChargeState();
            }
            else if (currentValue > 5 && currentValue <= 500 && CurrentChargingState != ChargingState.CHARGING)
            {
                //Write "Charging"
                CurrentChargingState = ChargingState.CHARGING;
                OnNewChargeState();
            }
            else if (currentValue > 500 && CurrentChargingState != ChargingState.OVERLOAD)
            {
                UsbCharger.StopCharge();
                //Write "Overload error!"
                CurrentChargingState = ChargingState.OVERLOAD;
                OnNewChargeState();
            }
            //else
            //{
            //    throw new Exception("Negative amp error");
            //}
        }
        void OnNewChargeState()
        {
            ChargingStateEvent?.Invoke(this, new ChargingStateEventArgs() {_chargingState = this.CurrentChargingState });
        }
    }
}
