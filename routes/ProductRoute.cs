using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Route {

    public class ProductRoute {

        public static void Map(WebApplication app) {
            var products = app.MapGroup("/api/v1/products");

            products.MapGet("/test", TestAPI);
            
            products.MapGet("/", GetProducts).RequireAuthorization("admin_auth");

            products.MapPost("/", CreateProduct);

            products.MapDelete("/{id}", DeleteProduct);

            products.MapPut("/{id}", UpdateProduct);
        }

        private static async Task<IResult> TestAPI() {

            return TypedResults.Ok(new {

                message = "It is working"
            });
        }
        private static async Task<IResult> DeleteProduct(int id, ProductDb db)
        {
            var product = await db.Products.FindAsync(id);

            if (product is null) {
                return TypedResults.NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            
            return TypedResults.Ok();
        }

       private static async Task<IResult> GetProducts(ProductDb db, int pageSize = 10, int pageNumber = 1, string sortDirection = "asc", string filter = "") {
           var totalProducts = await db.Products.CountAsync();
            var totalFilteredProducts = 0;

            var columnValue = new NpgsqlParameter("columnValue", $"%{filter}%");

            IQueryable<Product> query;

            if (!string.IsNullOrEmpty(filter)) {

                totalFilteredProducts = await db.Products.FromSqlInterpolated($@"
                SELECT 
                    p.""Id"", 
                    p.""Name"", 
                    p.""Price"",
                    p.""CreatedAt"",
                    p.""CategoryId"",
                    p.""Description"",
                    p.""ExpirationDate""
                FROM 
                    ""Products"" AS p
                INNER JOIN ""Categories"" AS c ON c.""Id"" = p.""CategoryId""
                WHERE p.""Name"" ILIKE {'%' + filter + '%'}").CountAsync();
                
                query = db.Products.FromSqlInterpolated($@"
                SELECT 
                    p.""Id"", 
                    p.""Name"", 
                    p.""Price"",
                    p.""CreatedAt"",
                    p.""CategoryId"",
                    p.""Description"",
                    p.""ExpirationDate""
                FROM 
                    ""Products"" AS p
                INNER JOIN ""Categories"" AS c ON c.""Id"" = p.""CategoryId""
                WHERE p.""Name"" ILIKE {'%' + filter + '%'}").Select(product => new Product() {
                    Id = product.Id, 
                    Name = product.Name, 
                    Price = product.Price, 
                    CreatedAt = product.CreatedAt, 
                    ExpirationDate = product.ExpirationDate, 
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Category = product.Category
                });
            }
            else {
                query = db.Products
                    .Select(product => 
                        new Product() {
                            Id = product.Id, 
                            Name = product.Name, 
                            Price = product.Price, 
                            CreatedAt = product.CreatedAt, 
                            ExpirationDate = product.ExpirationDate, 
                            Description = product.Description,
                            CategoryId = product.CategoryId,
                            Category = product.Category
                });
            }
       
            query = sortDirection == "desc"
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id);
            
          var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

           return TypedResults.Ok(new {
             products,
             totalProducts,
             totalFilteredProducts
           });
        }

       private static async Task<IResult> CreateProduct(Product product, ProductDb db) {

            try {
                    await db.Products.AddAsync(product);
                    await db.SaveChangesAsync();
                    return TypedResults.Ok(new {
                        message = "Product added successfully",
                        product
                    });
            }
            catch(Exception e) {
                return TypedResults.BadRequest(new {
                    message = "Error adding product",
                    error = e.Message
                });
            }
        }

       private static async Task<IResult> UpdateProduct(int id, Product product, ProductDb db) {
            var productToUpdate = await db.Products.FindAsync(id);

            if (productToUpdate is null) {
                return TypedResults.NotFound(new {
                    message = "Product not found"
                });
            }

            productToUpdate.Name = product.Name ?? productToUpdate.Name;
            productToUpdate.Price = product.Price != 0 ? product.Price : productToUpdate.Price;
            productToUpdate.CategoryId = product.CategoryId != 0 ? product.CategoryId : productToUpdate.CategoryId;

            await db.SaveChangesAsync();

            return TypedResults.Ok(new {
                message = "Product updated successfully",
                product = productToUpdate
            });
        }
    }
}