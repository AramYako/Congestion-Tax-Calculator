using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Models;

namespace Congestion_Tax_Calculator_Api.Interfaces
{
    public interface ITaxCongestionPersistance
    {
        int GetTollFee(Cities city, DateTime time);

        TollFreeDates GetTollFreeDates(Cities city, int year);

        int GetSingleChargeRuleInMinutes(Cities city);

        int GetSingleDayMaxChargePerDay(Cities city);
    }
}
