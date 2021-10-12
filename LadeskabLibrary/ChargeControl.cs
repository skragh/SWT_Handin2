using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UsbSimulator;

namespace LadeskabLibrary
{
    interface IChargeControl
    {
        //EventHandler ChargeMessage();
        bool IsConnected();
        void StartCharge();
        void StopCharge();
    }

    public class ChargeControl : IChargeControl
    {
        public class ChargeEventArgs : EventArgs
        {
            public string chargeStatus { get; }
        }

        public IUsbCharger UsbCharger { get; set; }
        public event EventHandler<ChargeEventArgs> ChargeStatusChange;
        public ChargeControl(IUsbCharger usbCharger)
        {
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

        //May need a tolorance of 0.1A.
        public void HandleCurrentChanged(object sender, CurrentEventArgs e)
        {
            var currentValue = e.Current;
            if (currentValue == 0)
            {
                //Write "please connect"
            }
            else if (currentValue > 0 && currentValue <= 5)
            {
                //Write "Fully charged"
            }
            else if (currentValue > 5 && currentValue <= 500)
            {
                //Write "Charging"
            }
            else if (currentValue > 500)
            {
                UsbCharger.StopCharge();
                //Write "Overload error!"
            }
            else
            {
                throw new Exception("Negative amp error");
            }

        }

        public void StopCharge()
        {
            throw new NotImplementedException();
        }
    }
    
}
