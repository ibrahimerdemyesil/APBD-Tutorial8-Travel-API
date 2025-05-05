using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _service;

    public ClientsController(ITripsService service)
    {
        _service = service;
    }

    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        var trips = await _service.GetTripsForClient(id);

        if (trips.Count == 0)
            return NotFound();

        return Ok(trips);
    }
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
    {
        var added = await _service.AddClient(client);

        if (!added)
            return StatusCode(500, "Could not insert client.");

        return Created("", null);
    }
    
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> AssignClientToTrip(int id, int tripId, [FromBody] ClientTripAssignDTO data)
    {
        var success = await _service.AssignClientToTrip(id, tripId, data);

        if (!success)
            return Conflict("Client already assigned to this trip.");


        return Ok("Client successfully assigned to trip.");
    }
    
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTrip(int id, int tripId)
    {
        var deleted = await _service.RemoveClientFromTrip(id, tripId);

        if (!deleted)
            return NotFound("No matching client-trip relation found.");

        return Ok("Client removed from trip.");
    }



}