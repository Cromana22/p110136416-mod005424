using System;
using System.Collections.Generic;
using System.Text;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    public class Vehicle
    {
        int maxFuel;
        int currentFuel;

        public int LaneLocation
        { get; set; }

        public int PumpLocation
        { get; set; }

        public DateTime FilledTime //time the vehicle will finish fuelling
        { get; set; }

        public DateTime LeaveTime //time the vehicle will get fed up and leave
        { get; set; }

        public int FillAmount
        { get; set; }

        public int FillTime //duration to fill the tank
        { get; set; }

        public string Type //e.g. car or van
        { get; set; }

        public EnumFuel FuelType
        { get; set; }

        public Vehicle()
        {
            Type = RandomVehicleType();
            int tempFuelType = RandomFuel();
            FuelType = (EnumFuel)tempFuelType;
            maxFuel = CapacityChecker();
            currentFuel = Program.rngNo.Next(maxFuel / 4, maxFuel);
            FillAmount = maxFuel - currentFuel;
            FillTime = Convert.ToInt32(FillAmount / 1.5 * 1000);  //fillTime is kept in milliseconds.
            LeaveTime = DateTime.Now.AddMilliseconds(Program.rngNo.Next(2000, 3001));
        }

        string RandomVehicleType()
        //This method sets a random vehicle type based on a random number generator.
        {
            string[] vehicleTypes = { "Car", "Van", "HGV" };
            int tempType = Program.rngNo.Next(0, vehicleTypes.Length);
            return vehicleTypes[tempType];
        }

        int RandomFuel()
        //This method selects a random fuel type from choices based on the vehicle type.
        //Depends on RandomVehicleType.
        {
            int[] tempFuelType;

            if (Type == "Car")
            {
                tempFuelType = new int[] { 0, 1, 2 };
            }
            else if (Type == "Van")
            {
                tempFuelType = new int[] { 0, 1 };
            }
            else
            {
                tempFuelType = new int[] { 0 };
            }

            int intType = Program.rngNo.Next(0, tempFuelType.Length);
            return tempFuelType[intType];
        }

        int CapacityChecker()
        //This method sets the max fuel capacity based on vehicle type.
        //Depends on RandomVehicleType.
        {
            int tempFuelMaxCapacity;

            if (Type == "Car")
                tempFuelMaxCapacity = 50;
            else if (Type == "Van")
                tempFuelMaxCapacity = 80;
            else
                tempFuelMaxCapacity = 150;

            return tempFuelMaxCapacity;
        }
    }
}