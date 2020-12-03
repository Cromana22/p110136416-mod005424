using System;
using System.Collections.Generic;
using System.Text;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    class Lane
    {
        public List<Pump> lanePumps = new List<Pump>();

        public Lane(int numberPumps)
        {
            for (int i = 0; i < numberPumps; i++)
            {
                lanePumps.Add(new Pump());
            }
        }
    }
}
