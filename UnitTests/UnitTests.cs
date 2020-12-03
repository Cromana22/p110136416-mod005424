using Microsoft.VisualStudio.TestTools.UnitTesting;
using P110136416_ELE010_MOD005424_A1Garage;
using System;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void Fuel_ReturnSessionPrice_ValidPositive()
        {
            //Arrange
            EnumFuel name = EnumFuel.Diesel;
            float pricePerUnit = 0.99F;
            int sessionCount = 57;
            float result;
            float expectedResult = 56.43F;

            //Act
            Fuel fuel = new Fuel(name, pricePerUnit);
            fuel.SessionCount = sessionCount;
            result = fuel.ReturnSessionPrice();

            //Assert
            Assert.AreEqual(expectedResult, result, "Incorrect session price calculated.");
        }

        [TestMethod]
        public void Vehicle_CapacityChecker_ValidCar()
        {
            //Arrange
            int result;
            int expectedResult = 50;

            //Act
            Vehicle vehicle = new Vehicle();
            vehicle.Type = "Car";
            result = vehicle.CapacityChecker();

            //Assert
            Assert.AreEqual(expectedResult, result, "Incorrect maximum capacity for a car.");
        }

        [TestMethod]
        public void Garage_CreateVehicleInstance_ValidCreateTime()
        {
            //Arrange
            DateTime dateTime = DateTime.Now.AddSeconds(-60);

            //Act
            Garage garage = new Garage(3, 3);
            garage.nextCreateTime = dateTime;
            garage.CreateVehicleInstance();

            //Assert
            Assert.IsNotNull(garage.queue[0], "Did not create vehicle as expected.");
        }

        [TestMethod]
        public void Garage_CreateVehicleInstance_InvalidCreateTime()
        {
            //Arrange
            DateTime dateTime = DateTime.Now.AddSeconds(60);

            //Act
            Garage garage = new Garage(3, 3);
            garage.nextCreateTime = dateTime;
            garage.CreateVehicleInstance();

            //Assert
            Assert.IsNull(garage.queue[0], "Created vehicle when it should not have.");
        }

    }
}
