using Application.MapperProfiles;
using Application.Services.Auth;
using Application.Services.Dashboard;
using Application.Services.OrgAdmin;
using Application.Services.PlatformAdmin;
using Application.Services.Sales;
using Application.Services.Security;
using Infrastructure.Contexts;
using Infrastructure.Contracts;
using Infrastructure.Hashers;
using Infrastructure.MiddlewareExtenstions;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Contracts;
using Infrastructure.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ErpApplicationApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- DB Context ---
            builder.Services.AddDbContext<SalesErpDbContext>((sp, options) => {
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .UseSnakeCaseNamingConvention();
                options.AddInterceptors(sp.GetRequiredService<TenantConnectionInterceptor>());
            });
            
            // Add services to the container.
            builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
            builder.Services.AddScoped<TenantConnectionInterceptor>();

            // --- Auth-related services ---
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            // --- Application services ---
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPlatformAdminService, PlatformAdminService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILeadService, LeadService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // --- Repositories ---
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            builder.Services.AddScoped<ILeadRepository, LeadRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<ILeadStatusHistoryRepository, LeadStatusHistoryRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

            // --- JWT bearer authentication ---
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30) // small leeway, not the 5-minute default
                    };
                });

            builder.Services.AddAuthorizationBuilder()
                // Reused via [Authorize(Policy = "PlatformAdminOnly")] on every Platform
                // Admin controller/action. Checks the is_platform_admin claim emitted by
                // JwtTokenService — same claim TenantResolutionMiddleware reads, so the
                // two stay in sync by construction rather than by convention.
                .AddPolicy("PlatformAdminOnly", policy => policy.RequireClaim("is_platform_admin", "true"))

                // Reused via [Authorize(Policy = "OrgAdminOnly")] on write actions within
                // an organization (creating/editing Roles, Teams, Users). Checks the
                // is_org_admin claim — only present on the token when User.IsOrgAdmin
                // was true at login, set via JwtTokenService.
                .AddPolicy("OrgAdminOnly", policy => policy.RequireClaim("is_org_admin", "true"));

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<AuthMappingProfile>();
                config.AddProfile<PlatformAdminMappingProfile>();
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // --- Middleware order matters here ---
            // 1. UseAuthentication: validates the JWT signature/expiry, populates
            //    HttpContext.User with claims. Nothing tenant-related happens yet.
            // 2. UseAuthorization: enforces [Authorize] attributes based on that
            //    validated identity.
            // 3. UseTenantResolution: OUR custom middleware, runs AFTER authentication
            //    has populated HttpContext.User, reads the claims, and populates
            //    ICurrentTenantService for this request. Must run before any
            //    controller action executes a DB query, and must run after
            //    authentication (it has nothing to read otherwise).
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseTenantResolution();

            app.MapControllers();

            app.Run();
        }
    }
}
