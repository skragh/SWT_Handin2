using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public interface IDisplay
    {
        public void DisplayChargeMessage(object sender, ChargingStateEventArgs e);
        public void DisplayStationMessage(object sender, StationControl.StationMessageEventArgs e);
        public void Print();
    }
}
