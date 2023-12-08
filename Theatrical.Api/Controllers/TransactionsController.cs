using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionValidationService _validation;
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionValidationService validationService, ITransactionService transactionService)
    {
        _validation = validationService;
        _service = transactionService;
    }

    /*/// <summary>
    /// Endpoint to making a new transaction.
    /// </summary>
    /// <param name="transactionDto"></param>
    /// <returns></returns>
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
    }*/

    /// <summary>
    /// Get a specific transaction
    /// </summary>
    /// <param name="id">transaction id</param>
    /// <returns>TransactionDtoFetch</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionDtoFetch), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Get all the transactions of a specific user.
    /// </summary>
    /// <param name="id">user's id</param>
    /// <returns>List&lt;TransactionsDtoFetch&gt;</returns>
    [HttpGet("user/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TransactionDtoFetch), StatusCodes.Status200OK)]
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