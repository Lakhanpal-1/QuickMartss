using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickMart.Data.DbContext;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Data.Repository;
using QuickMart.Services.Helpers;
using QuickMart.Services.Mapper;
using QuickMart.Services.Services.IServices;
using QuickMart.Services.Services;
using System.Text;
using QuickMart.Data.Repositories;

namespace QuickMart.Extension
{
    public static class ConfigureService
    {
        public static void AddQuickMartServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ===================== Database Configuration =====================
            ConfigureDatabase(services, configuration);

            // ===================== Identity Configuration =====================
            ConfigureIdentity(services);

            // =====================  Repositories =====================
            ConfigureRepositories(services);

            // ===================== Services =====================
            ConfigureServices(services, configuration);

            // ===================== AutoMapper Configuration =====================
            services.AddAutoMapper(typeof(MappingProfile));

            // ===================== JWT Configuration =====================
            ConfigureJwtAuthentication(services, configuration);

            // ===================== Authorization Policies Configuration =====================
            ConfigureAuthorization(services);

            // ===================== CORS Configuration =====================
            ConfigureCors(services);

            // ===================== Swagger Configuration =====================
            ConfigureSwagger(services);
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<JwtHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            ConfigureEmailService(services, configuration);
        }

        private static void ConfigureEmailService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>(sp =>
            {
                var emailSettings = configuration.GetSection("EmailSettings");
                return new EmailService(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["Port"]),
                    emailSettings["Email"], // Sender email used for authentication
                    emailSettings["Password"] // Sender's email password
                );
            });
        }

        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
                options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
                options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
                options.AddPolicy("AdminOrManager", policy => policy.RequireRole("Admin", "Manager"));
            });
        }

        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuickMartAPI", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
