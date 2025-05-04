using Microsoft.EntityFrameworkCore;

namespace Route {
    public class DashboardRoute {

        public static void Map(WebApplication app) {
            var dashboard = app.MapGroup("/api/v1/dashboard").RequireAuthorization("admin_or_user_auth");

            dashboard.MapGet("/", GetDashboard);
        }

        private static async Task<IResult> GetDashboard(ProductDb db) {

           var totalProducts = await db.Products.CountAsync();
           var totalCategories = await db.Categories.CountAsync();
           var totalUsers  = await db.Users.CountAsync();
           
           return TypedResults.Ok(new {
                totalProducts,
                totalCategories,
                totalUsers
            });
        }
    }

}