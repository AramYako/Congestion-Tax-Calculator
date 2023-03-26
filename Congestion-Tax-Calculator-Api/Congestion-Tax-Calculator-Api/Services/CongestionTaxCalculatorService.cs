using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Interfaces;
using Congestion_Tax_Calculator_Api.Models;

public class CongestionTaxCalculatorService : ICongestionTaxCalculatorService
{

    private readonly ITaxCongestionPersistance _taxCongestionPersistance;
    public CongestionTaxCalculatorService(ITaxCongestionPersistance taxCongestionPersistance)
    {
        _taxCongestionPersistance = taxCongestionPersistance;
    }

    public List<TollFees> GetTax(Cities city, VehicleType vehicleType, List<DateTime> dates)
    {
        List<TollFees> tollFees = GetTollFees(city, vehicleType, dates)
        .OrderBy(x => x.Date)
            .ToList();

        tollFees = ApplySingleChargeRule(city, tollFees);

        tollFees = ApplyMaxChargePerDay(city, tollFees);

        return tollFees;

    }

    private List<TollFees> GetTollFees(Cities city, VehicleType vehicleType, List<DateTime> dates)
    {
        List<TollFees> tollFees = new List<TollFees>();

        TollFreeDates tollfreeDates = new TollFreeDates();
        
        if (dates.Count>0)
            tollfreeDates = _taxCongestionPersistance.GetTollFreeDates(city, dates.First().Year);

        foreach (var date in dates)
            tollFees.Add(new TollFees { Date = date, Fee = GetTollFee(date, city, vehicleType, tollfreeDates) });

        return tollFees;
    }

    private int GetTollFee(DateTime date, Cities city, VehicleType vehicleType, TollFreeDates tollfreeDates)
    {
        if (IsTollFreeDate(tollfreeDates, date) || IsTollFreeVehicle(vehicleType))
            return 0;

        return _taxCongestionPersistance.GetTollFee(city, date);
    }

    private bool IsTollFreeVehicle(VehicleType vehicleType)
    {
        return vehicleType.Equals(VehicleType.Emergency) ||
            vehicleType.Equals(VehicleType.Bus) ||
            vehicleType.Equals(VehicleType.Diplomat) ||
            vehicleType.Equals(VehicleType.Motorcycle) ||
            vehicleType.Equals(VehicleType.Military) ||
            vehicleType.Equals(VehicleType.Foreign);
    }

    private Boolean IsTollFreeDate(TollFreeDates tollfreeDates, DateTime date)
    {

        if (tollfreeDates.DayOfWeeks.Any(x => x.Equals(date.DayOfWeek)))
            return true;

        if (tollfreeDates.Months.Any(x => x.Equals(date.Month)))
            return true;

        return tollfreeDates.Days
            .Where(x => x.Equals(date.Month))
            .Where(x => x.Equals(date.Day))
            .Any();
    }

    private List<TollFees> ApplySingleChargeRule(Cities city, List<TollFees> tollFees)
    {
        int chargeTimeMinutes = _taxCongestionPersistance.GetSingleChargeRuleInMinutes(city);

        if (chargeTimeMinutes == 0)
            return tollFees;

        tollFees = tollFees.OrderBy(x => x.Date).ToList();

        List<TollFees> singleChargeTollFees = new List<TollFees>(); // this will contain the final groups of dates

        HashSet<DateTime> usedDates = new HashSet<DateTime>(); // keep track of dates already used in a group

        for (int i = 0; i < tollFees.Count; i++)
        {
            if (!usedDates.Contains(tollFees[i].Date)) // check if date has already been used in a group
            {
                singleChargeTollFees.Add(tollFees[i]);

                for (int j = i + 1; j < tollFees.Count; j++)
                {
                    if (!usedDates.Contains(tollFees[j].Date) && (tollFees[j].Date - tollFees[i].Date).TotalMinutes <= 60)
                    {
                        if (singleChargeTollFees.Last().Fee < tollFees[j].Fee)
                        {
                            singleChargeTollFees.Remove(singleChargeTollFees.Last());
                            singleChargeTollFees.Add(tollFees[j]);
                        }

                        usedDates.Add(tollFees[j].Date); // mark date as used
                    }
                    else
                        break;
                }
            }
        }

        return singleChargeTollFees;
    }

    private List<TollFees> ApplyMaxChargePerDay(Cities city, List<TollFees> tollFees)
    {
        int maxChargePerDay = _taxCongestionPersistance.GetSingleDayMaxChargePerDay(city);

        if (maxChargePerDay == 0)
            return tollFees;

        List<TollFees> appliedMaxChargeToolFee = new List<TollFees>();

        List<int> chargeDays = tollFees.Select(x => x.Date).Select(x => x.Day)
            .Distinct()
            .ToList();

        foreach (int day in chargeDays)
        {
            int totalDayFee = tollFees
                .Where(x => x.Date.Day == day)?
                .Select(x => x.Fee)?
                .Sum() ?? 0;

            if (totalDayFee > maxChargePerDay)
                appliedMaxChargeToolFee.Add(new TollFees { Fee = maxChargePerDay, Date = tollFees.Where(x => x.Date.Day == day).Select(x => x.Date).First() });
            else
                appliedMaxChargeToolFee.Add(new TollFees { Fee = totalDayFee, Date = tollFees.Where(x => x.Date.Day == day).Select(x => x.Date).First() });
        }

        return appliedMaxChargeToolFee;
    }

}