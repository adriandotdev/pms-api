using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace Route {

    public class AuthRoute {

        private static string JWT_SECRET_KEY = "";

        public static void Map(WebApplication app) {

            var auth = app.MapGroup("/api/v1/auth");

            JWT_SECRET_KEY = app.Configuration["Authentication:JwtSecretKey"] ?? "";

            auth.MapPost("/login", Login);
            auth.MapPost("/signup", SignUp);
            auth.MapPost("/refresh", RefreshToken);
        }

        private static async Task<IResult> SignUp(User input, ProductDb db) 
        {
            try 
            {
                var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(input.Password, HashType.SHA512);

                User user = new() {
                    Name = input.Name,
                    Username = input.Username,
                    Password = passwordHash,
                    Role = input.Role
                };

                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == input.Username);

                if (existingUser is not null) {

                    return TypedResults.BadRequest(new {
                        message = "Username already exists. Please consider other username."
                    });
                }

                await db.Users.AddAsync(user);

                await db.SaveChangesAsync();

                return TypedResults.Ok(new {
                input.Name,
                message = "User successfully created!"
                });
            }
            catch (Exception e) 
            {
                return TypedResults.InternalServerError(new {
                    message = e.Message
                });
            }
        }

        private static async Task<IResult> Login(User input, ProductDb db) {

           var users = await db.Users.Select(user => 
            new User() {
                Id = user.Id, 
                Username = user.Username, 
                Password = user.Password,
                Role = user.Role,
            })
            .Where(user => user.Username == input.Username)
            .ToListAsync();

            if (users.Count == 0) {
                return TypedResults.NotFound(new {
                    message = "User does not exists"
                }); 
            }
            
            var user = users[0];

            var isPasswordMatch = BCrypt.Net.BCrypt.EnhancedVerify(input.Password, user.Password, HashType.SHA512);

            if (!isPasswordMatch) 
                return TypedResults.Unauthorized();

            var accessToken = GenerateJwtToken([
                new ("role", users[0].Role),
                new ("scope", users[0].Role == "admin" ? "admin_scope" : "user_scope"),
                new ("token_type", "access_token")
            ], 15);

            var refreshToken = GenerateJwtToken([ 
                new ("role", users[0].Role),
                new ("scope", users[0].Role == "admin" ? "admin_scope" : "user_scope"),
                new ("token_type", "refresh_token")
            ], 30);

            return TypedResults.Ok(new {
                accessToken,
                refreshToken
            });
        }

        private static Task<IResult> RefreshToken(RefreshTokenRequest request) 
        {
            try {
                var claimsPrincipal = ValidateJwtToken(request.RefreshToken, JWT_SECRET_KEY);

                string? tokenType = claimsPrincipal?.FindFirst("token_type")?.Value;
                string? role = claimsPrincipal?.FindFirst(ClaimTypes.Role)?.Value;
                string? scope = claimsPrincipal?.FindFirst("scope")?.Value;
                
                if (tokenType != "refresh_token" || role is null || scope is null) 
                    return Task.FromResult<IResult>(TypedResults.BadRequest(new { message = "Invalid token" }));

                var newAccessToken = GenerateJwtToken([
                    new ("role", role ?? ""),
                    new ("scope", scope ?? ""),
                    new ("token_type", "access_token")
                ], 15);

                var newRefreshToken = GenerateJwtToken([
                    new ("role", role ?? ""),
                    new ("scope", scope ?? ""), 
                    new ("token_type", "refresh_token")
                ], 30);

                return Task.FromResult<IResult>(TypedResults.Ok(new {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                }));
            }
            catch(Exception e)
            {
                return Task.FromResult<IResult>(TypedResults.InternalServerError(new {
                    message = e.Message
                }));
            }
        }

        private static string  GenerateJwtToken(List<Claim> claims, double expiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECRET_KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      
            var token = new JwtSecurityToken(
                issuer: "yanyan-pms",
                audience: "pms",
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiration),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal? ValidateJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "yanyan-pms",

                ValidateAudience = true,
                ValidAudience = "pms",

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}