using System;
using System.Collections.Generic;
using System.Text;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    enum status
    {
        Available, //0
        Busy,      //1
        Blocked,   //2
    }

    class Pump
    {
        public Enum PumpStatus
        { get; set; }

        public Pump()
        {
            PumpStatus = status.Available;
        }
    }
}
