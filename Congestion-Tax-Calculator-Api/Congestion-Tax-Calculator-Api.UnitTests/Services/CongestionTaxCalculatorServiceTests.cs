using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Interfaces;
using Congestion_Tax_Calculator_Api.Models;
using Moq;
using NUnit.Framework;

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
            Assert.That(tax.Select(x=>x.Fee).Sum, Is.EqualTo(0));

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

            _taxCongestionPersistanceMock.Verify(x => x.GetTollFee(city, dates[1]),Times.Once);

            _taxCongestionPersistanceMock.VerifyNoOtherCalls();
        }

        //more test....................................

    }
}

