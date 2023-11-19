using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.TransactionDtos.PurchaseDtos;
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

    /// <summary>
    /// Use this endpoint to purchase the premium package.
    /// Costs and gives 5.99 in credits.
    /// Not fully implemented yet.
    /// </summary>
    /// <param name="customerInformation"></param>
    /// <returns></returns>
    [HttpPost("PurchasePremium")]
    public async Task<ActionResult<ApiResponse>> PurchasePremium(CustomerInformation customerInformation)
    {
        try
        {
            var (userValidated, user) = await _validation.ValidateForPurchase(customerInformation.Email);

            if (!userValidated.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)userValidated.ErrorCode!, userValidated.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var (result, transaction) = await _service.PurchaseSubscription(customerInformation, user!);

            var transactionResponseDto = _service.TransactionToResponseDto(transaction!);

            var transactionResponse = new TransactionResponse
            {
                CreateTransactionResponse = result,
                TransactionResponseDto = transactionResponseDto
            };

            var response = new ApiResponse<TransactionResponse>(transactionResponse);
            
            return new ObjectResult(response);
        
        }
        catch (Exception e)
        {
            return new ObjectResult(new ApiResponse<Exception>(e));
        }
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