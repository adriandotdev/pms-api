using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Route;

var AllowedOrigins = "allowed_origins";

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Services
builder.Services
    .AddDbContext<ProductDb>(
        opt => opt.UseNpgsql(dbConnectionString
    ));

// CORS
builder.Services
    .AddCors(
        options => {
            options.AddPolicy(name: AllowedOrigins, policy => {
            policy.WithOrigins("http://localhost:3000").WithMethods("POST", "GET", "PUT", "DELETE", "OPTIONS").AllowAnyHeader();
        });
    });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yanyan-pms",
            ValidAudience = "pms",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JwtSecretKey"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("admin_auth", 
        policy => policy
            .RequireRole("admin")
            .RequireClaim("scope", "admin_scope")
    )
    .AddPolicy("user_auth", 
        policy => policy
            .RequireRole("user")
            .RequireClaim("scope", "user_scope")
    )
    .AddPolicy("admin_or_user_auth",
        policy => policy
            .RequireRole("user", "admin")
            .RequireClaim("scope", "admin_scope", "user_scope")
        );

var app = builder.Build();

app.UseCors(AllowedOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () =>
{
    return TypedResults.Ok(new
    {
        message = "Ok"
    });
});

// Routes
ProductRoute.Map(app);
CategoryRoute.Map(app);
DashboardRoute.Map(app);
AuthRoute.Map(app);

app.Run();
