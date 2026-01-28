using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using Assigment1_PRN232_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<NewsArticle>("NewsArticles");
modelBuilder.EntitySet<Category>("Categories");
modelBuilder.EntitySet<Tag>("Tags");

// Explicitly configure SystemAccount key
var systemAccountEntity = modelBuilder.EntitySet<SystemAccount>("SystemAccounts");
systemAccountEntity.EntityType.HasKey(x => x.AccountId);

builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
        options.AddRouteComponents("odata", modelBuilder.GetEdmModel());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// Add AutoMapper - Temporarily disabled
// builder.Services.AddAutoMapper(typeof(MappingProfile));

// DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Register Repository and UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
;

// Register All Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportService, ReportService>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Transform the role claim to use standard ClaimTypes.Role
            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var roleClaim = identity.FindFirst("role");
                if (roleClaim != null && roleClaim.Type != ClaimTypes.Role)
                {
                    identity.RemoveClaim(roleClaim);
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
            }
            
            return Task.CompletedTask;
        }
    };
});

// Authorization policies - Use ClaimTypes.Role for consistency
builder.Services.AddAuthorization(options =>
{
    // StaffOnly: allow Staff (1) or Admin
    options.AddPolicy("StaffOnly", policy => policy.RequireClaim(ClaimTypes.Role, "1", "Admin"));
    // LecturerOrAbove: allow Lecturer (2), Staff (1) or Admin
    options.AddPolicy("LecturerOrAbove", policy => policy.RequireClaim(ClaimTypes.Role, "2", "1", "Admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
