using System.ComponentModel.DataAnnotations.Schema;

public class Product {
   
    public int Id {get; set;}
    public string Name {get; set;} = null!;

    [Column(TypeName = "decimal(6, 2)")]
    public decimal Price {get; set;}

    public int CategoryId { get; set; }  
    public Category Category {get; set;} = null!;

    public DateOnly CreatedAt {get; set;} =DateOnly.FromDateTime(DateTime.UtcNow);

    public string? Description {get; set;} 
    
    public DateOnly? ExpirationDate {get; set;}
}