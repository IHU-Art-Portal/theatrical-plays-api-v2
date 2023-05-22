﻿using Theatrical.Data.Models;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IVenueService
{
    Task Create(VenueCreateDto venueCreateDto);
    Task Delete(Venue venue);
}

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;

    public VenueService(IVenueRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Create(VenueCreateDto venueCreateDto)
    {
        Venue venue = new Venue
        {
            Title = venueCreateDto.Address,
            Address = venueCreateDto.Address,
            Created = DateTime.UtcNow
        }; 
        
        await _repository.Create(venue);
    }

    public async Task Delete(Venue venue)
    {
        await _repository.Delete(venue);
    }
}