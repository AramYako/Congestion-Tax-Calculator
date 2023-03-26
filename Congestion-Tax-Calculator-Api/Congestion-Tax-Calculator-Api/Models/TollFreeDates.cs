namespace Congestion_Tax_Calculator_Api.Models
{
    public class TollFreeDates
    {
        public TollFreeDates()
        {
            DayOfWeeks = new List<DayOfWeek>();
            Months = new List<int>();
            Days = new List<DateTime>();
        }
        public List<DayOfWeek> DayOfWeeks { get; set; }

        public List<int> Months { get; set; }

        public List<DateTime> Days { get; set; }
    }
}
