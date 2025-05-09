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