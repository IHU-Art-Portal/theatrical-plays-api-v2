using AuthorizeNet.Api.Contracts.V1;

namespace Theatrical.Dto.TransactionDtos.PurchaseDtos;

public class TransactionResponse
{
    public createTransactionResponse? CreateTransactionResponse { get; set; }
    public TransactionResponseDto? TransactionResponseDto { get; set; }
}