using System;
using System.Collections.Generic;
using System.Text;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    public class Fuel
    {
        float pricePerUnit;

        public int SessionCount //litres sold in session
        { get; set; }

        public int TotalCount //total litres sold while app running
        { get; set; }

        public EnumFuel Name
        { get; set; }

        public Fuel(EnumFuel name, float price)
        {
            Name = name;
            pricePerUnit = price;
        }

        public float ReturnSessionPrice() //returns the value of amount sold in session
        {
            float sessionPrice = SessionCount * pricePerUnit;
            return sessionPrice;
        }

        public float ReturnTotalPrice() //returns the value of amount sold in total app time
        {
            float totalPrice = TotalCount * pricePerUnit;
            return totalPrice;
        }
    }
}
