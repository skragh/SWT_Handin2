using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public class DisplayImplementation : IDisplay
    {
        public DisplayImplementation()
        {
            ChargingMessageDeclaration = "CHARGING INFORMATION - Display information for customer";
            StationMessageDeclaration = "STATION INFORMATION - Display information for technician";
        }
        public void DisplayChargeMessage(object sender, ChargingStateEventArgs e)
        {
            switch (e._chargingState)
            {
                case ChargingState.CHARGING:
                    ChargeMessage = "Charging";
                    break;
                case ChargingState.FULL:
                    ChargeMessage = "Fully Charged";
                    break;
                case ChargingState.IDLE:
                    ChargeMessage = "Disconnected";
                    break;
                case ChargingState.OVERLOAD:
                    ChargeMessage = "Overload Error - Too much power being transferred - Disconnecting";
                    break;
                default:
                    ChargeMessage = "Error exception. Internal charging information is invalid";
                    break;
            }
        }

        public void DisplayStationMessage(object sender, StationControl.StationMessageEventArgs e)
        {
            StationMessage = e.message;
            print();
        }

        public void print()
        {
            printChargingMessage();
            printStationMessage();
        }

        private void printChargingMessage()
        {
            Console.WriteLine(ChargingMessageDeclaration);
            Console.WriteLine(ChargeMessage);
        }
        private void printStationMessage()
        {
            Console.WriteLine(StationMessageDeclaration);
            Console.WriteLine(StationMessage);
        }

        public string ChargingMessageDeclaration { get; private set; }
        public string StationMessageDeclaration { get; private set; }
        public string ChargeMessage { get; set; }
        public string StationMessage { get; set; }


        //public void DisplayChargeMessage(string message)
        //{
        //    switch (message)
        //    {
        //        case "currently charging":
        //            Console.WriteLine(message);
        //            break;
        //        case "fully charged":
        //            Console.WriteLine(message);
        //            break;
        //        case "no connection":
        //            Console.WriteLine(message);
        //            break;
        //        default:
        //            Console.WriteLine("Error exception. Charging information is invalid. Noget er gået galt, eks. kortslutning. Opladning stoppes straks");
        //            break;
        //    }
        //}
    }
}
