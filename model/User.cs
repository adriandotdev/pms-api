using System.ComponentModel.DataAnnotations.Schema;

public class User {

    public int Id {get; set;}

    public string Name { get; set;} = null!;

    public string Username { get; set;} = null!;

    public string Password { get; set;} = null!;

    public string Role { get; set;} = null!;
    
    public DateOnly DateCreated { get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);
}