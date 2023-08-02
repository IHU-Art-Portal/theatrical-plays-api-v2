using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _repo;
    private readonly ITransactionValidationService _validation;

    public TransactionsController(ITransactionRepository repository, ITransactionValidationService validationService)
    {
        _repo = repository;
        _validation = validationService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> PostTransaction([FromBody]TransactionDto transactionDto)
    {
        try
        {
            var transaction = new Transaction
            {
                UserId = transactionDto.UserId,
                CreditAmount = transactionDto.CreditAmount,
                Reason = transactionDto.Reason
            };

            await _repo.PostTransaction(transaction);
            
            var response = new ApiResponse("Transaction Successful!");
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetTransaction([FromRoute] int id)
    {
        try
        {
            var (validation, transaction) = await _validation.ValidateForFetch(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode) validation.ErrorCode!, validation.Message!);
                return new NotFoundObjectResult(errorResponse);
            }
            
            var response = new ApiResponse<Transaction>(transaction);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<ApiResponse>> GetUserTransactions([FromRoute] int id)
    {
        try
        {
            var (validation, transactions) = await _validation.ValidateUserTransactions(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode) validation.ErrorCode!, validation.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var response = new ApiResponse<List<Transaction>>(transactions);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }
}