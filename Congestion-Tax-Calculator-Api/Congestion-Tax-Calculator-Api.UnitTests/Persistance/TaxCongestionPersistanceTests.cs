using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Models;
using Congestion_Tax_Calculator_Api.Persistance;
using NUnit.Framework;

namespace Congestion_Tax_Calculator_Api.UnitTests.Persistance
{
    [TestFixture]

    public class TaxCongestionPersistanceTests
    {
        [TestCase("2013-01-01 06:00:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 06:29:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 06:30:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 06:59:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 07:00:00", Cities.Gothenburg, ExpectedResult = 18)]
        [TestCase("2013-01-01 07:59:00", Cities.Gothenburg, ExpectedResult = 18)]
        [TestCase("2013-01-01 08:00:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 08:15:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 08:29:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 08:30:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 12:30:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 14:59:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 15:00:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 15:29:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 15:30:00", Cities.Gothenburg, ExpectedResult = 18)]
        [TestCase("2013-01-01 15:45:00", Cities.Gothenburg, ExpectedResult = 18)]
        [TestCase("2013-01-01 16:59:00", Cities.Gothenburg, ExpectedResult = 18)]
        [TestCase("2013-01-01 17:00:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 17:59:00", Cities.Gothenburg, ExpectedResult = 13)]
        [TestCase("2013-01-01 18:00:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 18:29:00", Cities.Gothenburg, ExpectedResult = 8)]
        [TestCase("2013-01-01 19:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 20:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 22:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 23:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 01:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 03:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 04:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 05:00:00", Cities.Gothenburg, ExpectedResult = 0)]
        [TestCase("2013-01-01 05:59:00", Cities.Gothenburg, ExpectedResult = 0)]

        public int GetTollFee_GetTollForDate_ReturnFee(DateTime date, Cities city)
        {
            // Arrange
            var taxCongestionPersistance = new TaxCongestionPersistance();

            // Act
            var actualFee = taxCongestionPersistance.GetTollFee(city, date);

            // Assert
            return actualFee;
        }

        [Test]
        public void GetTollFreeDates_GetForGothenburg_ReturnsTollFreeDates()
        {
            // Arrange
            var taxCongestionPersistance = new TaxCongestionPersistance();
            var expectedTollFreeDates = new TollFreeDates
            {
                DayOfWeeks = new List<DayOfWeek>() { DayOfWeek.Saturday, DayOfWeek.Sunday },
                Months = new List<int>() { 7 },
                Days = new List<DateTime> {
                    new DateTime(2023, 1, 1),
                    new DateTime(2023, 3, 28),
                    new DateTime(2023, 3, 29),
                    new DateTime(2023, 4, 1),
                    new DateTime(2023, 4, 30),
                    new DateTime(2023, 5, 1),
                    new DateTime(2023, 5, 8),
                    new DateTime(2023, 5, 9),
                    new DateTime(2023, 6, 5),
                    new DateTime(2023, 6, 6),
                    new DateTime(2023, 6, 21),
                    new DateTime(2023, 11, 1),
                    new DateTime(2023, 12, 24),
                    new DateTime(2023, 12, 25),
                    new DateTime(2023, 12, 26),
                    new DateTime(2023, 12, 31),
                },
            };

            // Act
            var actualTollFreeDates = taxCongestionPersistance.GetTollFreeDates(Cities.Gothenburg, 2013);

            // Assert
            Assert.AreEqual(expectedTollFreeDates.DayOfWeeks, actualTollFreeDates.DayOfWeeks);
            Assert.AreEqual(expectedTollFreeDates.Months, actualTollFreeDates.Months);
            CollectionAssert.AreEqual(expectedTollFreeDates.Days, actualTollFreeDates.Days);
        }

        [TestCase(Cities.Gothenburg, ExpectedResult = 60)]
        [TestCase(Cities.Other, ExpectedResult = 0)]
        public int GetSingleChargeRuleInMinutes_GetMaxSingleChargeInMinuteForCity_ReturnsCorrectValue(Cities city)
        {
            // Arrange
            var taxCongestionPersistance = new TaxCongestionPersistance();

            // Act
            var actualValue = taxCongestionPersistance.GetSingleChargeRuleInMinutes(city);

            // Assert
            return actualValue; 
        }

        [TestCase(Cities.Gothenburg, ExpectedResult = 60)]
        [TestCase(Cities.Other, ExpectedResult = 0)]
        public int GetSingleDayMaxChargePerDay_GetMaxChargeForCity_ReturnsCorrectValue(Cities city)
        {
            // Arrange
            var taxCongestionPersistance = new TaxCongestionPersistance();
            var expectedValue = 60;

            // Act
            var actualValue = taxCongestionPersistance.GetSingleDayMaxChargePerDay(city);

            // Assert
            return actualValue;
        }
    }
}
