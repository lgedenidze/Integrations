using Integrations;
using Integrations.Data;
using Integrations.Model;
using Integrations.Repositories;
using Integrations.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// ✅ **Securely Configure EF Core with PostgreSQL**
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection")));

// ✅ **Secure JWT Authentication Configuration**
var jwtKey = Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
var jwtIssuer = Configuration["Jwt:Issuer"] ?? "YourApiIssuer";
var jwtAudience = Configuration["Jwt:Audience"] ?? "YourApiAudience";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // ✅ Enforce HTTPS for JWT validation
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, // ✅ Now requires correct Issuer
            ValidateAudience = true, // ✅ Now requires correct Audience
            ValidateLifetime = true, // ✅ Token expiration check
            ValidateIssuerSigningKey = true, // ✅ Ensures token is signed
            ValidAudience = jwtAudience,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero, // ✅ Prevents expired tokens from being used
            RoleClaimType = ClaimTypes.Role
        };
    });

// ✅ **Enable Controllers**
builder.Services.AddControllers();

// ✅ **Secure CORS Configuration**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://yourfrontend.com") // ✅ Only allow specific origins
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// ✅ **Register Dependency Injection Services**
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISoonEventRepository, SoonEventRepository>();

// ✅ **Fix PostgreSQL Timestamp Issues**
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ✅ **Secure Swagger Configuration**
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Integrations API",
        Version = "v1",
        Description = "Secure API with JWT Authentication"
    });

    // ✅ Configure Swagger for JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token."
    });
    c.EnableAnnotations();
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
            new string[] {} // ✅ Required for all endpoints
        }
    });

    // ✅ Enable Authorization globally in Swagger UI
    
});
builder.Services.AddAuthorization();
 
// ✅ **Build & Run App**
var app = builder.Build();

// ✅ **Configure Middleware**
// if (app.Environment.IsDevelopment()) // 🔹 Commented out for now
// {
app.UseDeveloperExceptionPage();
// }

// ✅ Secure API with HTTPS & CORS
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

// ✅ **Enable Swagger UI (Now always enabled)**
// if (app.Environment.IsDevelopment()) // 🔹 Commented out for now
// {
app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MakerSpace")
    );
// }

// ✅ **Map Controllers**
app.MapControllers();

// ✅ **Run API**
app.Run();
