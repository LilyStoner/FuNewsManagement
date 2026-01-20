using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232.Repositories;
using Assigment1_PRN232.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddOData(options =>
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<NewsArticle>("News");
    options.AddRouteComponents("odata", odataBuilder.GetEdmModel()).Filter().OrderBy().Expand().Select().SetMaxTop(100).Count();
});

// DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Repositories and services
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
