using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Models;

public interface ICongestionTaxCalculatorService
{
    List<TollFees> GetTax(Cities city, VehicleType vehicleType, List<DateTime> dates);
}