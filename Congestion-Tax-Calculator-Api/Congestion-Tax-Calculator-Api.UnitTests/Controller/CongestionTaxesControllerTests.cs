using Congestion_Tax_Calculator_Api.Controllers;
using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Congestion_Tax_Calculator_Api.UnitTests.Controller
{
    [TestFixture]
    public class CongestionTaxesControllerTests
    {
        private Mock<ICongestionTaxCalculatorService> _congestionTaxCalculatorServiceMock;
        private CongestionTaxesController _controller;

        [SetUp]
        public void Setup()
        {
            _congestionTaxCalculatorServiceMock = new Mock<ICongestionTaxCalculatorService>();
            _controller = new CongestionTaxesController(_congestionTaxCalculatorServiceMock.Object);
        }

        [Test]
        public void GetVehicles_ReturnsOkResult_WhenServiceReturnsTollFees()
        {
            // Arrange
            var vehicleType = VehicleType.Car;
            var city = Cities.Gothenburg;
            var dates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(2) };
            var expectedTollFee =8;
            var tollFees = new List<TollFees> { new TollFees { Fee = 5 }, new TollFees { Fee = 3 } };
            _congestionTaxCalculatorServiceMock.Setup(s => s.GetTax(city, vehicleType, dates)).Returns(tollFees);

            // Act
            var result = _controller.GetVehicles(vehicleType, city, dates) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTollFee, result.Value);
        }

    }
}
