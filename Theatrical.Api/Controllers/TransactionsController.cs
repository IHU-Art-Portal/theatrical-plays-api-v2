using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Services;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _repo;
    private readonly ITransactionValidationService _validation;
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionRepository repository, ITransactionValidationService validationService, ITransactionService transactionService)
    {
        _repo = repository;
        _validation = validationService;
        _service = transactionService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> PostTransaction([FromBody]TransactionDto transactionDto)
    {
        try
        {
            var transaction = await _service.PostTransaction(transactionDto);
            
            var response = new ApiResponse<Transaction>(transaction, "Transaction Successful!");
            
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

            var transactionDto = _service.TransactionToDto(transaction!);
            
            var response = new ApiResponse<TransactionDtoFetch>(transactionDto);
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

            var transactionDtos = _service.TransactionListToDto(transactions!);
            
            var response = new ApiResponse<List<TransactionDtoFetch>>(transactionDtos);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.InnerException.Message);
            return new ObjectResult(exceptionResponse) { StatusCode = 500 };
        }
    }
}