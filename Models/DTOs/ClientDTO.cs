using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

/* -- ClientDTO -- */
/*
    Purpose:
        Used to return full client info.
    
    Used in:
        - GET /api/clients/{id}
 */
public class ClientDTO
{
    public int IdClient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}

/* -- ClientTripDTO -- */
/*
    Purpose:
        Used to return trip registration details for a given client.
    
    Used in:
        - GET /api/clients/{id}/trips 
 */
public class ClientTripDTO
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public string TripName { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int RegisteredAt { get; set; } 
    public int? PaymentDate { get; set; }
}

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
    [RegularExpression("^[0-9]{11}$")]       // Annotation [RegularExpression()] is used to apply regular expression to 
    public string Pesel { get; set; }               // Client's Pesel Number (required)
}