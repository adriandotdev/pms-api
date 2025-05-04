public class Category {

    public int Id { get; set;}
    public string Name { get; set;} = "";

    public string? Description {get; set;}
    
    public DateOnly CreatedAt {get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);
    
    public ICollection<Product> Products { get; set;} = [];
}