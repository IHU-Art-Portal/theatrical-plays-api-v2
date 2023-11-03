using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using Theatrical.Data.Models;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.TransactionDtos.PurchaseDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface ITransactionService
{
    TransactionDtoFetch TransactionToDto(Transaction transaction);
    TransactionResponseDto TransactionToResponseDto(Transaction transcation);
    List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions);
    Task<Transaction> PostTransaction(PaymentDetailsDto paymentDetailsDto);
    Task<(createTransactionResponse?, Transaction?)> PurchaseSubscription(CustomerInformation customerInfo, User user);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Transaction> PostTransaction(PaymentDetailsDto paymentDetailsDto)
    {
        var transaction = new Transaction
        {
            UserId = paymentDetailsDto.UserId,
            CreditAmount = paymentDetailsDto.CreditAmount,
            Reason = paymentDetailsDto.Reason,
            TransactionId = paymentDetailsDto.TransactionId,
            AuthCode = paymentDetailsDto.AuthCode,
            NetworkTransactionId = paymentDetailsDto.NetworkTransactionId,
            AccountNumber = paymentDetailsDto.AccountNumber,
            AccountType = paymentDetailsDto.AccountType
        };
        
        var newTransaction = await _repository.PostTransaction(transaction);
        return newTransaction;
    }
    
    public TransactionDtoFetch TransactionToDto(Transaction transaction)
    {

        var transactionDtoFetch = new TransactionDtoFetch
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
            TransactionId = transaction.TransactionId.ToString(),
            AuthCode = transaction.AuthCode,
            AccountNumber = transaction.AccountNumber,
            AccountType = transaction.AccountType
        };

        return transactionDtoFetch;
    }

    public TransactionResponseDto TransactionToResponseDto(Transaction transaction)
    {
        var transactionResponseDto = new TransactionResponseDto
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
            TransactionId = transaction.TransactionId.ToString(),
            AuthCode = transaction.AuthCode,
            AccountNumber = transaction.AccountNumber,
            AccountType = transaction.AccountType,
            DatabaseTransactionId = transaction.Id
        };

        return transactionResponseDto;
    }

    public List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions)
    {
        List<TransactionDtoFetch> transactionDtoFetches = 
            transactions.Select(transaction => new TransactionDtoFetch 
                { CreditAmount = transaction.CreditAmount, 
                    Reason = transaction.Reason, 
                    UserId = transaction.UserId, 
                    DateCreated = transaction.DateCreated,
                    TransactionId = transaction.TransactionId.ToString(),
                    AuthCode = transaction.AuthCode,
                    AccountNumber = transaction.AccountNumber,
                    AccountType = transaction.AccountType
                }).ToList();
        
        return transactionDtoFetches;
    }
    
    public async Task<(createTransactionResponse?, Transaction?)> PurchaseSubscription(CustomerInformation customerInfo, User user)
    {
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
        
        // define the merchant information (authentication / transaction id)
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = "485cQaeSQ", //ApiLoginID
            ItemElementName = ItemChoiceType.transactionKey,
            Item = "43Esm2s8mFY4k8EL", //ApiTransactionKey
        };
        
        var creditCard = new creditCardType
        {
            cardNumber = "4111111111111111",
            expirationDate = "0823",
            cardCode = "123"
        };

        var billingAddress = new customerAddressType
        {
            firstName = customerInfo.FirstName,
            lastName = customerInfo.LastName,
            address = customerInfo.Address,
            city = customerInfo.City,     
            zip = customerInfo.Zip,       
            country = customerInfo.Country,
            email = customerInfo.Email,
            company = customerInfo.Company,
            state = customerInfo.State,
            phoneNumber = customerInfo.PhoneNumber
        };

        //standard api call to retrieve response
        var paymentType = new paymentType { Item = creditCard };

        var lineItems = new lineItemType[1];
        lineItems[0] = new lineItemType { itemId = "1", name = "Subscription", quantity = 1, unitPrice = new Decimal(5.99) };
        
        
        var transactionRequest = new transactionRequestType
        {
            transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),   // charge the card
            amount = 5.99m,
            payment = paymentType,
            billTo = billingAddress,
            lineItems = lineItems
        };
			
        var request = new createTransactionRequest { transactionRequest = transactionRequest };
			
        // instantiate the controller that will call the service
        var controller = new createTransactionController(request);
        controller.Execute();
			
        // get the response from the service (errors contained if any)
        var response = controller.GetApiResponse();
        
        
        if (response != null)
        {
            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                if(response.transactionResponse.messages != null)
                {
                    var transactionId = response.transactionResponse.transId;
                    var authCode = response.transactionResponse.authCode;
                    var accountNumber = response.transactionResponse.accountNumber;
                    var accountType = response.transactionResponse.accountType;
                    var networkTransId = response.transactionResponse.networkTransId;

                    var paymentDetails = new PaymentDetailsDto
                    {
                        UserId = user.Id,
                        CreditAmount = 5.99m,
                        Reason = "Premium Purchase",
                        TransactionId = long.Parse(transactionId),
                        AuthCode = authCode,
                        NetworkTransactionId = networkTransId,
                        AccountNumber = accountNumber,
                        AccountType = accountType
                    };

                    var transaction = await PostTransaction(paymentDetails);
                    
                    Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                    Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                    Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                    Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
					Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);

                    return (response, transaction);
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed Transaction.");
                if (response.transactionResponse != null && response.transactionResponse.errors != null)
                {
                    Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                    Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                }
                else
                {
                    Console.WriteLine("Error Code: " + response.messages.message[0].code);
                    Console.WriteLine("Error message: " + response.messages.message[0].text);
                }
            }
        }
        else
        {
            Console.WriteLine("Null Response.");
        }

        return (response, null);
    }
}

