using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    public int IdClient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}

public class ClientTripDTO
{
    public int IdTrip { get; set; }
    public string TripName { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime RegisteredAt { get; set; } 
    public DateTime? PaymentDate { get; set; }
}

public class CreateClientDTO
{
    [Required]
    public string FirstName { get; set; } 
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    public string Telephone { get; set; }
    
    [Required]
    [RegularExpression("^[0-9]{11}$")]
    public string Pesel { get; set; }
}