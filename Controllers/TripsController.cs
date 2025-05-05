using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        // GET /api/trips
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();

            if (trips == null || trips.Count == 0)
                return NotFound(); // 404

            return Ok(trips); // 200
        }

        // GET /api/trips/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _tripsService.GetTrip(id);

            if (trip == null)
                return NotFound(); // 404

            return Ok(trip); // 200
        }
    }
}