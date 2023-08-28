using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Curators;
using Theatrical.Services.Curators.Responses;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuratorController : ControllerBase
{
    private readonly IContributionRepository _repo;

    public CuratorController(IContributionRepository repo)
    {
        _repo = repo;
    }
    
    [HttpGet]
    [Route("curateExisting")]
    public async Task<ActionResult<ApiResponse>> CurateExistingData()
    {
        var contributions = await _repo.Get();
        
        Curator curator = new Curator();
        
        var contributionsProcessed = curator.CleanData(contributions);

        var currateResponse = new CurateResponse
        {
            Contributions = contributionsProcessed,
            CorrectedObjects = contributionsProcessed.Count
        };
        
        var response = new ApiResponse<CurateResponse>(currateResponse);
        
        return Ok(response);
    }
    
    [HttpGet]
    [Route("curateNew")]
    public async Task<ActionResult> CurateNewData()
    {
        return Ok();
    }
    
}