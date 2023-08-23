using System.ComponentModel.DataAnnotations;

namespace Theatrical.Dto.TransactionDtos.PurchaseDtos;

public class CustomerInformation
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Billing address is required")]
    public string Address { get; set; }
    
    [Required(ErrorMessage = "City is required")]
    public string City { get; set; }
    
    [Required(ErrorMessage = "Zip is required")]
    public string Zip { get; set; }
    
    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; }
    
    [Required(ErrorMessage = "Email address is required")]
    public string Email { get; set; }
    public string? Company { get; set; }
    public string? State { get; set; }         //Περιφέρεια
    public string? PhoneNumber { get; set; }
}