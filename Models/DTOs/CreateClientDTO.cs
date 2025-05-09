using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

/* -- CreateClientDTO -- */
/*
    Purpose:
        Used to accept new client data from the request body.

    Used in:
        - POST /api/clients
 */
public class CreateClientDTO
{
    [Required]
    public string FirstName { get; set; }           // Client's First Name (required)
    
    [Required] 
    public string LastName { get; set; }            // Client's Last Name (required)

    [Required]
    [EmailAddress]                                  // Annotation [EmailAddress] is used to validate email format
    public string Email { get; set; }               // Client's Email (required)
    
    public string Telephone { get; set; }           // Client's Telephone Number (optional)
    
    [Required]
    [RegularExpression("^[0-9]{11}$")]       // Annotation [RegularExpression()] is used to apply regular expression to PESEL Number (which contains 11 symbols)
    public string Pesel { get; set; }               // Client's Pesel Number (required)
}