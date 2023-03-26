using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Interfaces;
using Congestion_Tax_Calculator_Api.Models;

namespace Congestion_Tax_Calculator_Api.Persistance
{
    public class TaxCongestionPersistance : ITaxCongestionPersistance
    {
        public int GetTollFee(Cities city, DateTime time)
        {
            int hour = time.Hour;
            int minute = time.Minute;

            if (city == Cities.Gothenburg)
            {
                if (hour == 6 && minute >= 0 && minute <= 29)
                    return 8;
                else if (hour == 6 && minute >= 30 && minute <= 59)
                    return 13;
                else if (hour == 7)
                    return 18;
                else if (hour == 8 && minute >= 0 && minute <= 29)
                    return 13;
                else if ((hour >= 8 && minute >= 30) && (hour <=14  && minute <= 59))
                    return 8;
                else if (hour == 15 && minute >= 0 && minute <= 29)
                    return 13;
                else if ((hour >= 15 && minute >= 30) && (hour <= 16 && minute <= 59))
                    return 18;
                else if (hour == 17 && minute >= 0 && minute <= 59)
                    return 13;
                else if (hour == 18 && minute >= 0 && minute <= 29)
                    return 8;
            }

            return 0;

        }

        public TollFreeDates GetTollFreeDates(Cities city, int year)
        {
            if (city == Cities.Gothenburg)
            {
                if (year == 2013)
                {
                    return new TollFreeDates
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
                        new DateTime(2023, 12, 31), },
                    };
                }
            }

            return new TollFreeDates();
        }

        public int GetSingleChargeRuleInMinutes(Cities city)
        {
            if (city == Cities.Gothenburg)
                return 60;

            return 0;
        }

        public int GetSingleDayMaxChargePerDay(Cities city)
        {
            if (city == Cities.Gothenburg)
                return 60;

            return 0;
        }
    }
}
