using Congestion_Tax_Calculator_Api.Enums;
using Congestion_Tax_Calculator_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Congestion_Tax_Calculator_Api.Controllers
{
    [ApiController]
    public class CongestionTaxesController : ControllerBase
    {
        private readonly ICongestionTaxCalculatorService _congestionTaxCalculatorService;
        public CongestionTaxesController(ICongestionTaxCalculatorService congestionTaxCalculatorService)
        {
            _congestionTaxCalculatorService = congestionTaxCalculatorService;
        }

        [HttpGet]
        [Route("api/vehicles")]
        public IActionResult GetVehicles(VehicleType vehicleType, Cities city, List<DateTime> dates)
        {
            List<TollFees> tollFees = _congestionTaxCalculatorService.GetTax(city, vehicleType, dates);

            return Ok(tollFees.Select(x => x.Fee).Sum());
        }
    }
}
