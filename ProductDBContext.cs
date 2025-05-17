using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

class ProductDb : DbContext
{

    private readonly string _name;

    private readonly string _username;
    private readonly string _password;

    public ProductDb(DbContextOptions<ProductDb> options) : base(options)
    {

        var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // optional now
           .AddEnvironmentVariables()
           .Build();

        _name = config["User:Name"]!;
        _username = config["User:Username"]!;
        _password = config["User:Password"]!;

        Console.WriteLine($"{_username}:{_password}");
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
    optionsBuilder.UseNpgsql()
    .UseSeeding((context, _) =>
    {
        var userSet = context.Set<User>();
        var set = context.Set<Category>();

        int count = set.Count();
        int userCount = userSet.Count();

        if (userCount == 0)
        {

            Console.WriteLine($"ewwwww");
            Console.WriteLine($"{_username}:{_password}");
            userSet.Add(new User()
            {
                Name = _name,
                Username = _username,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(_password, HashType.SHA512),
                Role = "admin"
            });
        }

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
        }
        context.SaveChanges();
    })
    .UseAsyncSeeding(async (context, _, cancellationToken) =>
    {
        var userSet = context.Set<User>();
        var set = context.Set<Category>();

        int count = set.Count();
        int userCount = userSet.Count();

        if (userCount == 0)
        {
            userSet.Add(new User()
            {
                Name = _name,
                Username = _username,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(_password, HashType.SHA512),
                Role = "admin"
            });
        }

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

        }
        
        await context.SaveChangesAsync();
    });
}