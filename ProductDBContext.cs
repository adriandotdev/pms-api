using Microsoft.EntityFrameworkCore;

class ProductDb : DbContext
{

    public ProductDb(DbContextOptions<ProductDb> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
    optionsBuilder.UseNpgsql()
    .UseSeeding((context, _) =>
    {
        var set = context.Set<Category>();

        int count = set.Count();

        if (count == 0)
        {
            set.Add(new Category()
            {
                Name = "Snacks & Junked Food",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Beverages",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Canned & Packaged Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Rice & Dry Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Personal Care & Toiletries",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Household Essentials",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Cigarettes & Alcohol",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Frozen Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            context.SaveChanges();
        }
    })
    .UseAsyncSeeding(async (context, _, cancellationToken) =>
    {
        var set = context.Set<Category>();

        int count = set.Count();

        if (count == 0)
        {
            set.Add(new Category()
            {
                Name = "Snacks & Junked Food",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Beverages",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Canned & Packaged Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Rice & Dry Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Personal Care & Toiletries",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Household Essentials",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Cigarettes & Alcohol",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            set.Add(new Category()
            {
                Name = "Frozen Goods",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            await context.SaveChangesAsync();
        }
    });
}