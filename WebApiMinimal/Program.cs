using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Catalog API", 
    Description = "Find the products you deserve" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API");
    });
}

var products = new List<Product>();

app.MapGet("/products", () => products);
app.MapGet("/products/{id}", Results<Ok<Product>, NotFound> (string id) =>
{
    var product = products.SingleOrDefault(p => p.id == id);
    if (product == null)
    {
        return TypedResults.NotFound();
    }
    else
    {
        return TypedResults.Ok(product);
    }
});
app.MapPost("/products", (Product product) =>
{
    products.Add(product);
    return TypedResults.Created($"/products/{product.id}", product);
});
app.MapDelete("/products/{id}", (string id) =>
{
    products.RemoveAll(p => p.id == id);
    return TypedResults.NoContent();
});

app.Run();

record Product(string id, string name, decimal price, bool inStock);
