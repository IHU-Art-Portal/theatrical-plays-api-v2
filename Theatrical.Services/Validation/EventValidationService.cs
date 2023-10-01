﻿using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IEventValidationService
{
    Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto);
    Task<(ValidationReport report, List<Event>? events)> FetchAndValidate();
    Task<(ValidationReport, Event?)> ValidateForFetch(int eventId);
    Task<(ValidationReport validation, List<Event>? events)> FetchEventsForPerson(int id);
    Task<(ValidationReport validation, List<Event>? events)> FetchEventsForProduction(int id);
}

public class EventValidationService : IEventValidationService
{
    private readonly IEventRepository _repository;
    private readonly IProductionRepository _productionRepo;
    private readonly IVenueRepository _venueRepo;

    public EventValidationService(IEventRepository repository, IProductionRepository productionRepository, IVenueRepository venueRepository)
    {
        _repository = repository;
        _productionRepo = productionRepository;
        _venueRepo = venueRepository;
    }

    public async Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto)
    {
        var report = new ValidationReport();
        var production = await _productionRepo.GetProduction(createEventDto.ProductionId);
        
        if (production is null)
        {
            report.Success = false;
            report.Message = $"Production with ID: {createEventDto.ProductionId} does not exist";
            return report;
        }

        var venue = await _venueRepo.Get(createEventDto.VenueId);

        if (venue is null)
        {
            report.Success = false;
            report.Message = $"Venue with Id: {createEventDto.VenueId} does not exist";
            return report;
        }

        report.Success = true;
        report.Message = "Success";
        return report;
    }

    public async Task<(ValidationReport report, List<Event>? events)> FetchAndValidate()
    {
        var events = await _repository.Get();
        var report = new ValidationReport();
        
        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found any Events";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Events Found";
        return (report, events);
    }

    public async Task<(ValidationReport, Event?)> ValidateForFetch(int eventId)
    {
        var eventFromDb = await _repository.GetEvent(eventId);
        var report = new ValidationReport();
        
        if (eventFromDb is null)
        {
            report.Success = false;
            report.Message = $"Event with id: {eventId} not found";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Event found";

        return (report, eventFromDb);
    }

    public async Task<(ValidationReport validation, List<Event>? events)> FetchEventsForPerson(int id)
    {
        var events = await _repository.GetEventsForPerson(id);
        var report = new ValidationReport();

        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found events for this person";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Found events";
        
        return (report, events);
    }

    public async Task<(ValidationReport validation, List<Event>? events)> FetchEventsForProduction(int id)
    {
        var events = await _repository.GetEventsForProduction(id);
        var report = new ValidationReport();
        
        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found events for this production";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        foreach (var @event in events)
        {
            Console.WriteLine(@event.Id);
        }

        report.Success = true;
        report.Message = "Found events";
        
        return (report, events);
    }
}

