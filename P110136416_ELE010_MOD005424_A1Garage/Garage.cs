using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    public class Garage
    {
        string username = "user1";
        string password = "password1";
        DateTime shiftStart;
        DateTime shiftEnd;
        List<Lane> lanes = new List<Lane>();
        public Vehicle[] queue = new Vehicle[5];
        List<Vehicle> pumps = new List<Vehicle>();
        List<Fuel> fuels = new List<Fuel>();
        int vehiclesServiced = 0;
        int vehiclesNotServiced = 0;
        public DateTime nextCreateTime = DateTime.Now;

        //display UI uses these
        int noLanes;
        int pumpsPerLane;
        string[] queueDisplay = new string[5];
        string[] pumpDisplay;
        float commissionRate = 0.01F;
        float wageRate = 5.90F;

        public bool LoggedIn
        { get; set; }

        public Garage(int noLanes, int pumpsPerLane)
        {
            pumpDisplay = new string[noLanes * pumpsPerLane]; //set the size of pump display array
            this.noLanes = noLanes;
            this.pumpsPerLane = pumpsPerLane;
            
            for (int i = 0; i < noLanes; i++) //create the lanes
            {
                lanes.Add(new Lane(pumpsPerLane));
            }

            //create the fuels
            fuels.Add(new Fuel(EnumFuel.Diesel, 1.09F));
            fuels.Add(new Fuel(EnumFuel.LPG, 1.10F));
            fuels.Add(new Fuel(EnumFuel.Unleaded, 1.12F));
        }

        public void Login()
        {
            Console.Clear();
            Console.Write("Please input your username or type \"quit\" to close: ");
            string tempUsername = Console.ReadLine();

            if (tempUsername == "quit") //check if they want to close program
            {
                Program.running = false;
            }

            else
            {
                Console.Write("Please input your password or type \"quit\" to close: ");
                string tempPassword = Console.ReadLine();

                if (tempPassword == "quit") //check if they want to close program
                {
                    Program.running = false;
                }

                else
                {
                    if (tempUsername == username && tempPassword == password) //login check
                    {
                        LoggedIn = true;
                        shiftStart = DateTime.Now;
                    }
                    else
                    {
                        Console.WriteLine("There was an error with your credentials. Please try again.");
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public void Logout()
        {
            //set status then clear the garage of cars and update
            LoggedIn = false;
            shiftEnd = DateTime.Now;
            Array.Clear(queue, 0, queue.Length);

            foreach (Vehicle vehicle in pumps)
            {
                vehicle.FilledTime = DateTime.Now.AddMinutes(-1);
            }

            CheckVehicleStatus();

            //calculate wages etc
            double wages = Math.Round((shiftEnd - shiftStart).TotalMinutes / 60 * wageRate, 2);
            float commission = 0F;

            foreach (Fuel fuel in fuels)
            {
                commission = (float)Math.Round(commission + (fuel.ReturnSessionPrice() * commissionRate), 2);
            }

            //Write to shiftLog file
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string shiftFile = @"\ShiftLog.txt";
            string shiftLog = $"\n{DateTime.Now} : {username} : {shiftStart} - {shiftEnd} : £{wages} wages and £{commission} commission." +
                $"\n{vehiclesServiced} vehicles were serviced and {vehiclesNotServiced} left without service.";

            foreach (Fuel fuel in fuels)
            {
                shiftLog = shiftLog + $"\n{fuel.Name}: {fuel.SessionCount} litres sold.";
            }

            File.AppendAllText(path + shiftFile, shiftLog + Environment.NewLine);

            //Display on console the same information
            Console.Clear();
            Console.WriteLine($"Thank you for your shift, {username}.");
            Console.WriteLine($"You earned £{wages} and £{commission} commission.");
            Thread.Sleep(5000);

            //clear session counters
            foreach (Fuel fuel in fuels)
            {
                fuel.SessionCount = 0;
            }
        }

        public void CheckVehicleStatus()
        {
            for (int i = 0; i < queue.Length; i++)  //check if a car in queue has passed its leave time, clear it if so and increment counter
            {
                if (queue[i] != null)
                {
                    if (queue[i].LeaveTime < DateTime.Now)
                    {
                        vehiclesNotServiced++;
                        Array.Clear(queue, i, 1);
                    }
                }
            }

            for (int i = 0; i < pumps.Count; i++)
            {
                if (pumps[i].FilledTime < DateTime.Now)   //go through pumps and check if vehicles finished fuelling, increment if so
                {
                    vehiclesServiced++;

                    foreach (Fuel fuel in fuels)
                    {
                        if (fuel.Name == pumps[i].FuelType) //find the fuel with matching name to the car fuel type
                        {
                            fuel.TotalCount += pumps[i].FillAmount;
                            fuel.SessionCount += pumps[i].FillAmount;
                        }
                    }
                    //Write to VehicleLog file
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string vehicleFile = @"\VehicleLog.txt";
                    string vehicleLog = $"{ DateTime.Now} : {pumps[i].Type} : {pumps[i].FuelType} : Pump {pumps[i].LaneLocation * 3 + (pumps[i].PumpLocation + 1)} : {pumps[i].FillAmount} litres.";
                    File.AppendAllText(path + vehicleFile, vehicleLog + Environment.NewLine);

                    //set all busy pumps back to available, remove cars from pumps, reset blocked pumps
                    lanes[pumps[i].LaneLocation].lanePumps[pumps[i].PumpLocation].PumpStatus = status.Available;
                    pumps.RemoveAt(i);
                    UpdateBlockedPumps();
                }
            }
        }

        public void CreateVehicleInstance()
        {
            if (DateTime.Now >= nextCreateTime) //if create time has passed
            {
                int emptyQueue = -1;
                int i;
                for (i = 0; i < queue.Length; i++) //find the next empty queue slot
                {
                    if (queue[i] == null)
                    {
                        emptyQueue = i;
                        break;
                    }
                }

                if (emptyQueue == -1) //if no slots left, increment vehicles left counter
                {
                    vehiclesNotServiced++;
                }
                else //otherwise, create new vehicle and reset the create time
                {
                    queue[i] = new Vehicle();
                }

                nextCreateTime = DateTime.Now.AddMilliseconds(Program.rngNo.Next(1500, 2201));
            }
        }

        public void UpdateBlockedPumps()
        {
            foreach (Lane lane in lanes)
            {
                foreach (Pump pump in lane.lanePumps)
                {
                    if ((status)pump.PumpStatus == status.Blocked) //if pump is blocked, set it to available
                    {
                        pump.PumpStatus = status.Available;
                    }

                    if ((status)pump.PumpStatus == status.Busy)  //if pump is busy...
                    {
                        for (int i = 0; i < lane.lanePumps.IndexOf(pump); i++)  //for the pumps with index less than the busy one...
                        {

                            if ((status)lane.lanePumps[i].PumpStatus == status.Available)   //if status is available...
                            {
                                lane.lanePumps[i].PumpStatus = status.Blocked;   //set status to blocked
                            }
                        }
                    }
                }
            }
        }

        public void SelectPump(ConsoleKeyInfo key)
        {
            int input = -1;
            int inputLane;
            int inputPump;

            //convert the input into a number
            try
            {
                input = Convert.ToInt32(Convert.ToString(key.KeyChar));
            }

            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
                Thread.Sleep(1000);
            }

            if (input > 0)
            {
                //figure out the lane and pump
                float tempFloat = (float)input / 2;
                if (tempFloat < 2)
                {
                    inputLane = 0;
                    inputPump = input - 1;
                }
                else if (tempFloat < 3.5F)
                {
                    inputLane = 1;
                    inputPump = input % 4;
                }
                else
                {
                    inputLane = 2;
                    inputPump = input % 7;
                }

                bool isAvailable = (status)lanes[inputLane].lanePumps[inputPump].PumpStatus == status.Available;

                if (isAvailable == true && queue[0] != null) //if the pump is available, set attributes, move to pumps and set pump status, and clear its space in queue
                {
                    queue[0].LaneLocation = inputLane;
                    queue[0].PumpLocation = inputPump;
                    queue[0].FilledTime = DateTime.Now.AddMilliseconds(queue[0].FillTime);
                    pumps.Add(queue[0]);
                    lanes[inputLane].lanePumps[inputPump].PumpStatus = status.Busy;
                    Array.Clear(queue, 0, 1);

                    for (int i = 0; i < queue.Length - 1; i++) //move the cars in queue up one space
                    {
                        queue[i] = queue[i + 1];
                        Array.Clear(queue, i + 1, 1);
                    }

                    UpdateBlockedPumps();
                }
            }
        }

        public void DisplayUI()
        {
            string queueVehicle = "VEH ";
            string queueEmpty = "    ";
            int sessionLtr = 0;
            float sessionPrice = 0.00F;
            float sessionCommission;
            int totalLtr = 0;
            float totalPrice = 0.00F;

            foreach (Fuel fuel in fuels)
            {
                sessionPrice += (float)Math.Round(fuel.ReturnSessionPrice(), 2);
                totalPrice += (float)Math.Round(fuel.ReturnTotalPrice(), 2);
                sessionLtr += fuel.SessionCount;
                totalLtr += fuel.TotalCount;
            }

            sessionCommission = (float)Math.Round((sessionPrice * commissionRate), 2);

            for (int i = 0; i < queue.Length; i++) //sets the queue display based on queue value
            {
                if (queue[i] != null)
                {
                    queueDisplay[i] = queueVehicle;
                }
                else
                {
                    queueDisplay[i] = queueEmpty;
                }
            }

            for (int iLanes = 0; iLanes < noLanes; iLanes++) //sets the display text per pump in an array based on their status
            {
                for (int iLanePumps = 0; iLanePumps < pumpsPerLane; iLanePumps++)
                {
                    if ((status)lanes[iLanes].lanePumps[iLanePumps].PumpStatus == status.Available)
                    {
                        pumpDisplay[(iLanes * pumpsPerLane) + iLanePumps] = "FREE";
                    }
                    
                    else if ((status)lanes[iLanes].lanePumps[iLanePumps].PumpStatus == status.Busy)
                    {
                        pumpDisplay[(iLanes * pumpsPerLane) + iLanePumps] = "BUSY";
                    }
                    
                    else
                    {
                        pumpDisplay[(iLanes * pumpsPerLane) + iLanePumps] = "BLOK";
                    }

                }
            }

            Console.Clear();
            Console.WriteLine($"Welcome To A1 Garage, {username}. Input a pump number to send vehicle to a pump.\n");
            Console.WriteLine($"  _ _ _ _  _ _ _ _ _ _ _ _ _ _ _ _ _ _ ");
            Console.WriteLine($"| QUEUE: |         |         |         |");

            Console.Write($"|        | 3: ");      //Ends up looking like this: "| VEH   |  3: FREE   |  4: BUSY   |  5: BLOK   |"
            PumpColourText(pumpDisplay[2]);
            Console.Write(" | 2: ");
            PumpColourText(pumpDisplay[1]);
            Console.Write(" | 1: ");
            PumpColourText(pumpDisplay[0]);
            Console.WriteLine(" |");

            Console.WriteLine($"| {queueDisplay[0]}   | _ _ _ _ | _ _ _ _ | _ _ _ _ |");
            Console.WriteLine($"| {queueDisplay[1]}   |         |         |         |");

            Console.Write($"| {queueDisplay[2]}   | 6: ");
            PumpColourText(pumpDisplay[5]);
            Console.Write(" | 5: ");
            PumpColourText(pumpDisplay[4]);
            Console.Write(" | 4: ");
            PumpColourText(pumpDisplay[3]);
            Console.WriteLine(" |");

            Console.WriteLine($"| {queueDisplay[3]}   | _ _ _ _ | _ _ _ _ | _ _ _ _ |");
            Console.WriteLine($"| {queueDisplay[4]}   |         |         |         |");

            Console.Write($"|        | 9: ");
            PumpColourText(pumpDisplay[8]);
            Console.Write(" | 8: ");
            PumpColourText(pumpDisplay[7]);
            Console.Write(" | 7: ");
            PumpColourText(pumpDisplay[6]);
            Console.WriteLine(" |");

            Console.WriteLine($"| _ _ _ _| _ _ _ _ | _ _ _ _ | _ _ _ _ |");

            Console.WriteLine();
            Console.WriteLine(String.Format("{0,-12} : {1,-6} | {2,-11} : {3,-9} | {4,-10} : {5,-8}", "Sesh Ltr", sessionLtr, "Sesh Price", $"£{sessionPrice}", "Sesh Comm", $"£{sessionCommission}"));
            Console.WriteLine(String.Format("{0,-12} : {1,-6} | {2,-11} : {3,-9}", "Total Ltr", totalLtr, "Total Price", $"£{totalPrice}"));
            Console.WriteLine(String.Format("{0,-12} : {1,-6} : {2,-11} : {3,-9}", "Veh Serviced", vehiclesServiced, "Veh Left", $"{vehiclesNotServiced}"));

            foreach (Fuel fuel in fuels)
            {
                Console.WriteLine($"{fuel.Name}: {fuel.SessionCount} litres dispensed this shift");
            }

        }

        void PumpColourText(string pump) //set the font colour based on pump status
        {
            if (pump == "FREE")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(pump);
                Console.ResetColor();
            }
            else if (pump == "BUSY")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(pump);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(pump);
                Console.ResetColor();
            }
        }
    }
}
