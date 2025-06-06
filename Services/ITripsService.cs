﻿using Tutorial8.Models.DTOs;

namespace Tutorial8.Services
{
    public interface ITripsService
    {
        Task<List<TripDTO>> GetTrips();
        Task<TripDTO?> GetTrip(int id); 
        Task<List<ClientTripDTO>> GetTripsForClient(int clientId);
        Task<bool> AddClient(ClientDTO client);
        Task<bool> AssignClientToTrip(int clientId, int tripId, ClientTripAssignDTO data);
        Task<bool> RemoveClientFromTrip(int clientId, int tripId);


    }
}