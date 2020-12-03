using System;
using System.Threading;
using System.IO;

namespace P110136416_ELE010_MOD005424_A1Garage
{
    class Program
    {
        public static Random rngNo = new Random(); //random number generator for the whole program to use.
        public static bool running = true; //control if the user wants to close the app completely.

        static void Main()
        {
            Garage a1garages = new Garage(3, 3); //create garage with 3 lanes, and 3 pumps per lane

            while (running == true)
            {
                a1garages.Login();

                while (a1garages.LoggedIn == true) //while logged in
                {
                    if (Console.KeyAvailable)  //if user presses a key check if esc to logout, or selected a pump
                    {
                        ConsoleKeyInfo keypressed = Console.ReadKey(true);

                        if (keypressed.Key == ConsoleKey.Escape)
                        {
                            a1garages.Logout();
                        }

                        else
                        {
                            a1garages.SelectPump(keypressed);
                        }
                    }

                    a1garages.CreateVehicleInstance();
                    a1garages.CheckVehicleStatus();
                    a1garages.DisplayUI();
                    Thread.Sleep(200); //to stop the screen flickering too badly
                }
            }
        }
    }
}
