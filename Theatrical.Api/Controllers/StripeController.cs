using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class StripeController : ControllerBase
{
    private readonly IUserValidationService _userValidation;
    private readonly ITransactionService _transactionService;

    public StripeController(IUserValidationService userValidationService, ITransactionService transactionService)
    {
        _userValidation = userValidationService;
        _transactionService = transactionService;
    }

    [HttpGet("create-checkout-session")]
    public async Task<ActionResult<ApiResponse>> CreateCheckoutSession()
    {
        var domain = $"https://{Request.Host}";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "price_1OKrZFFkSW4xPENMeITAfqUV",
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = domain + "/Success",
            CancelUrl = domain + "/Cancel",
        };
        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        Response.Headers.Add("Location", session.Url);
        return Redirect(session.Url);
    }
    
    [HttpPost("stripe-webhook")]
    public async Task<ActionResult<ApiResponse>> StripeWebhook()
    {
        var stripeSignature = HttpContext.Request.Headers["Stripe-Signature"];
        var endpointSecret = "whsec_KKIzvtVOngMI59473So1rQHecPyQmq2F";

        var reader = new StreamReader(HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();
        var json = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(body));
        
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

            // Handle the event based on its type
            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                // Retrieve information about the completed session
                var paymentStatus = session.PaymentStatus; // Check if the payment was successful
                var customerEmail = session.CustomerDetails.Email; // Get customer email

                var amountTotalPaidInCents = session.AmountTotal;
                var amountTotalInEuros = amountTotalPaidInCents / 100;
                
                if (paymentStatus.Equals("paid"))
                {
                    var (userValidation, user) = await _userValidation.ValidateUser(customerEmail);

                    if (!userValidation.Success)
                    {
                        return new BadRequestObjectResult(new ApiResponse((ErrorCode)userValidation.ErrorCode!, userValidation.Message!));
                    }

                
                    // Perform actions based on the successful payment
                    await _transactionService.PostTransaction(user!, amountTotalInEuros);
                    
                    return new OkObjectResult(new ApiResponse("Credits have been added!"));
                }

                return Ok("Not Paid. Credits have not been added.");
            }
            
            Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            
            return new OkObjectResult(new ApiResponse($"Unhandled event type: {stripeEvent.Type}"));
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new ApiResponse<Exception>(e));
        }
    }
}