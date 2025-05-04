using Microsoft.EntityFrameworkCore;

class ProductDb : DbContext {
    
    public ProductDb(DbContextOptions<ProductDb> options) : base(options) {}

    public DbSet<Product> Products {get; set;}
    public DbSet<Category> Categories { get; set; }

    public DbSet<User> Users { get; set; }
}