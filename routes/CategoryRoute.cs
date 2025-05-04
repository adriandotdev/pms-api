using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Route {

    public class CategoryRoute {

        public static void Map(WebApplication app) {

            var categories = app.MapGroup("/api/v1/categories");

            categories.MapGet("/", GetCategories).RequireAuthorization("user_auth");
            categories.MapPost("/", CreateCategory);
        }

        private static async Task<IResult> GetCategories(ProductDb db, int pageSize = 10, int pageNumber = 1) {

            var categories = await db.Categories
                .Select(category => 
                    new Category() {
                        Id = category.Id, 
                        Name = category.Name, 
                        Description = category.Description, 
                        CreatedAt = category.CreatedAt, 
                        Products = category.Products 
                })
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCategories = db.Categories.Count();

            return TypedResults.Ok(new {
                categories,
                totalCategories
            });
        }

        private static async Task<IResult> CreateCategory(Category category, ProductDb db) {

            await db.Categories.AddAsync(category);
            await db.SaveChangesAsync();

            return TypedResults.Ok();
        }
    }
}