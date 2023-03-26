using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Interfaces;
using Congestion_Tax_Calculator_Api.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Globalization;

namespace Congestion_Tax_Calculator_Api.UnitTests.Services
{
    [TestFixture]

    public class CongestionTaxCalculatorServiceTests
    {
        private Mock<ITaxCongestionPersistance> _taxCongestionPersistanceMock;
        private CongestionTaxCalculatorService _service;

        [SetUp]
        public void SetUp()
        {
            _taxCongestionPersistanceMock = new Mock<ITaxCongestionPersistance>();

            _service = new CongestionTaxCalculatorService(_taxCongestionPersistanceMock.Object);
        }

        [TestCase(VehicleType.Diplomat)]
        [TestCase(VehicleType.Emergency)]
        [TestCase(VehicleType.Motorcycle)]
        [TestCase(VehicleType.Foreign)]
        [TestCase(VehicleType.Bus)]
        [TestCase(VehicleType.Military)]
        public void GetTax_VehicleAreExemptInGothenburg_VerifyNoFee(VehicleType vehicleType)
        {
            //Arrange
            Cities city = Cities.Gothenburg;
            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.Now,
                DateTime.Now.AddDays(3)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, DateTime.Now.Year))
                .Returns(tollFree);


            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(2));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(0));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, DateTime.Now.Year), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        [TestCase(VehicleType.Other)]
        [TestCase(VehicleType.Car)]
        public void GetTax_OtherVehicleAreNotExemptInGothenburg_VerifyFee(VehicleType vehicleType)
        {
            //Arrange
            Cities city = Cities.Gothenburg;
            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.Now,
                DateTime.Now.AddDays(3)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, DateTime.Now.Year))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFee(city, dates[0]))
                .Returns(3);

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFee(city, dates[1]))
                .Returns(5);



            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(2));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(8));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, DateTime.Now.Year), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, dates[0]), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, dates[1]), Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_TollFreeWeekends_VerifyNoFee(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                 DateTime.Today.AddDays((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek),
                DateTime.Today.AddDays((int)DayOfWeek.Sunday - (int)DateTime.Today.DayOfWeek)
            };

            TollFreeDates tollFree = new TollFreeDates()
            {
                DayOfWeeks = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
            };

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, DateTime.Now.Year))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(5);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            //Assert
            Assert.That(tax.Count, Is.EqualTo(2));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(0));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, DateTime.Now.Year), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_TollFreeJuly_VerifyNoFee(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                new DateTime(2023, 07, 04),
                new DateTime(2023, 07, 10),
                new DateTime(2023, 07, 12)
            };

            TollFreeDates tollFree = new TollFreeDates()
            {
                DayOfWeeks = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                Months = new List<int>() { 07 }
            };

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, DateTime.Now.Year))
                    .Returns(tollFree);

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(5);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(3));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(0));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, DateTime.Now.Year), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_SpecificDaysAreTollFree_VerifyNoFee(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                new DateTime(2023, 09, 04),
                new DateTime(2023, 10, 10),
                new DateTime(2023, 12, 12)
            };

            TollFreeDates tollFree = new TollFreeDates()
            {
                DayOfWeeks = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                Months = new List<int>() { 07 },
                Days = new List<DateTime> { new DateTime(2023, 09, 04), new DateTime(2023, 10, 10), new DateTime(2023, 12, 12) }
            };

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, DateTime.Now.Year))
                    .Returns(tollFree);

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(5);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(3));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(0));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, DateTime.Now.Year), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }



        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_VerifySingleChargeRule_VerifyFees(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.ParseExact("2013-01-14 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 21:30:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:15:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:16:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, It.IsAny<int>()))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.SetupSequence(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(3)
                .Returns(65)
                .Returns(67)
                .Returns(14)
                .Returns(18)
                .Returns(20);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleChargeRuleInMinutes(city))
                .Returns(60);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(4));
            Assert.That(tax.Select(x => x.Fee).Sum, Is.EqualTo(170));

            Assert.That(tax[0].Fee, Is.EqualTo(65));
            Assert.That(tax[1].Fee, Is.EqualTo(67));
            Assert.That(tax[2].Fee, Is.EqualTo(18));
            Assert.That(tax[3].Fee, Is.EqualTo(20));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, It.IsAny<int>()), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, It.IsAny<DateTime>()), Times.Exactly(6));

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_VerifyNoSingleChargeRule_VerifyFees(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.ParseExact("2013-01-14 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 21:30:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:15:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:16:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, It.IsAny<int>()))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.SetupSequence(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(3)
                .Returns(65)
                .Returns(67)
                .Returns(14)
                .Returns(18)
                .Returns(20);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleChargeRuleInMinutes(city))
                .Returns(0);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(6));
            Assert.That(tax[0].Fee, Is.EqualTo(3));
            Assert.That(tax[1].Fee, Is.EqualTo(65));
            Assert.That(tax[2].Fee, Is.EqualTo(67));
            Assert.That(tax[3].Fee, Is.EqualTo(14));
            Assert.That(tax[4].Fee, Is.EqualTo(18));
            Assert.That(tax[5].Fee, Is.EqualTo(20));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, It.IsAny<int>()), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, It.IsAny<DateTime>()), Times.Exactly(6));

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }


        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_VerifyMaxDayCharge_VerifyFees(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.ParseExact("2013-01-14 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 21:30:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:15:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:16:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 13:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 15:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 15:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 17:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, It.IsAny<int>()))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.SetupSequence(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(3)
                .Returns(65)
                .Returns(67)
                .Returns(14)
                .Returns(50)
                .Returns(30)
                .Returns(30)
                .Returns(20);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleChargeRuleInMinutes(city))
                .Returns(60);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleDayMaxChargePerDay(city))
                .Returns(60);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(3));

            Assert.That(tax[0].Fee, Is.EqualTo(60));
            Assert.That(tax[1].Fee, Is.EqualTo(60));
            Assert.That(tax[2].Fee, Is.EqualTo(50));

            Assert.That(tax[0].Date.Day, Is.EqualTo(14));
            Assert.That(tax[1].Date.Day, Is.EqualTo(15));
            Assert.That(tax[2].Date.Day, Is.EqualTo(16));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, It.IsAny<int>()), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, It.IsAny<DateTime>()), Times.Exactly(8));

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }



        [TestCase(VehicleType.Other, Cities.Gothenburg)]
        [TestCase(VehicleType.Car, Cities.Gothenburg)]
        public void GetTax_VerifyNoMaxDayCharge_VerifyFees(VehicleType vehicleType, Cities city)
        {
            //Arrange

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.ParseExact("2013-01-14 21:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 21:30:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:15:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-14 22:16:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 13:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-15 15:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 15:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTime.ParseExact("2013-01-16 17:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            TollFreeDates tollFree = new TollFreeDates();

            _taxCongestionPersistanceMock.Setup(x => x.GetTollFreeDates(city, It.IsAny<int>()))
                .Returns(tollFree);

            _taxCongestionPersistanceMock.SetupSequence(x => x.GetTollFee(city, It.IsAny<DateTime>()))
                .Returns(3)
                .Returns(65)
                .Returns(67)
                .Returns(14)
                .Returns(50)
                .Returns(30)
                .Returns(30)
                .Returns(20);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleChargeRuleInMinutes(city))
                .Returns(60);

            _taxCongestionPersistanceMock.Setup(x => x.GetSingleDayMaxChargePerDay(city))
                .Returns(0);

            //Act
            var tax = _service.GetTax(city, vehicleType, dates);

            //Assert
            Assert.That(tax.Count, Is.EqualTo(6));

            Assert.That(tax[0].Fee, Is.EqualTo(65));
            Assert.That(tax[1].Fee, Is.EqualTo(67));
            Assert.That(tax[2].Fee, Is.EqualTo(50));
            Assert.That(tax[3].Fee, Is.EqualTo(30));
            Assert.That(tax[4].Fee, Is.EqualTo(30));
            Assert.That(tax[5].Fee, Is.EqualTo(20));

            Assert.That(tax[0].Date.Day, Is.EqualTo(14));
            Assert.That(tax[1].Date.Day, Is.EqualTo(14));
            Assert.That(tax[2].Date.Day, Is.EqualTo(15));
            Assert.That(tax[3].Date.Day, Is.EqualTo(15));
            Assert.That(tax[4].Date.Day, Is.EqualTo(16));
            Assert.That(tax[5].Date.Day, Is.EqualTo(16));

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFreeDates(city, It.IsAny<int>()), Times.Exactly(1));

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleChargeRuleInMinutes(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetSingleDayMaxChargePerDay(city), Times.Once);

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, It.IsAny<DateTime>()), Times.Exactly(8));

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }


        //more test....................................

    }
}

