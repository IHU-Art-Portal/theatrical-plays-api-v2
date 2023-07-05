﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PersonDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Curators;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPersonValidationService
{
    Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId);
    Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role);
    Task<(ValidationReport report, List<Person>? person)> ValidateForInitials(string initials);
    Task<(ValidationReport report, List<PersonProductionsRoleInfo>? productions)> ValidatePersonsProductions(int personId);
    Task<(ValidationReport report, List<Image>? images)> ValidatePersonsPhotos(int personId);
    Task<ValidationReport> ValidateForCreate(string fullName);
}

public class PersonValidationService : IPersonValidationService
{
    private readonly IPersonRepository _repository;
    private readonly IDataCurator _curator;

    public PersonValidationService(IPersonRepository repository, IDataCurator curator)
    {
        _repository = repository;
        _curator = curator;
    }

    public async Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId)
    {
        var person = await _repository.Get(performerId);
        var report = new ValidationReport();

        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists";
        return (report, person);
    }

    public async Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId)
    {
        var person = await _repository.Get(performerId);
        var report = new ValidationReport();
        
        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists and marked for deletion";
        return (report, person);
    }

    public async Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role)
    {
        var persons = await _repository.GetByRole(role);
        var report = new ValidationReport();

        if (!persons.Any())
        {
            report.Success = false;
            report.Message = "Not Found any people with the provided role";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Found people with the provided role";
        return (report, persons);
    }

    public async Task<(ValidationReport report, List<Person>? person)> ValidateForInitials(string initials)
    {
        var persons = await _repository.GetByLetter(initials);
        var report = new ValidationReport();

        if (!persons.Any())
        {
            report.Success = false;
            report.Message = $"Not Found any people with the initials: {initials}";
            return (report, null);
        }
        
        report.Success = true;
        report.Message = "Found people with the provided criteria";
        return (report, persons);
    }

    public async Task<(ValidationReport report, List<PersonProductionsRoleInfo>? productions)> ValidatePersonsProductions(int personId)
    {
        var (report, person) = await ValidateAndFetch(personId);

        if (!report.Success)
        {
            report.Message = $"Person with the provided ID not found";
            return (report, null);
        }

        var productions = await _repository.GetProductionsOfPerson(person!.Id);

        if (!productions.Any())
        {
            report.Message = $"This person does not participate in any productions";
            return (report, null);
        }

        report.Message = "Successful";
        report.Success = true;

        return (report, productions);
    }

    public async Task<(ValidationReport report, List<Image>? images)> ValidatePersonsPhotos(int personId)
    {
        var person = await _repository.Get(personId);
        var report = new ValidationReport();
        
        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        var images = await _repository.GetPersonsImages(personId);

        if (!images.Any())
        {
            report.Success = false;
            report.Message = "This person does not have any photos";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Successful";

        return (report, images);
    }

    public async Task<ValidationReport> ValidateForCreate(string fullName)
    {
        bool isValid = _curator.ValidateFullName(fullName);
        var report = new ValidationReport();
        
        if (!isValid)
        {
            report.Success = false;
            report.Message = "Curation failed. Person's name must follow the template: {Name Surname}, {Name}, or {Surname}";
            report.ErrorCode = ErrorCode.CurationFailure;
            return report;
        }

        var person = await _repository.GetByName(fullName);

        if (person is not null)
        {
            report.Success = false;
            report.Message = "Person with a similar name already exists.";
            report.ErrorCode = ErrorCode.AlreadyExists;
            return report;
        }
        
        report.Message = "Success";
        report.Success = true;
        return report;
    }
}