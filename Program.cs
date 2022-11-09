using EstudosAPI.Models;
using EstudosAPI.Repository;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Mensagem Teste!");
app.MapPost("/", () => new { Name = "Teste", Age = 29 });

app.MapGet("/users", (HttpResponse response) =>
{
    response.Headers.Add("CabecalhoTeste", "Teste");
    return new { Name = "Teste", Age = 29 };
});

//api.app.com/user?datastart={date}&dateend={date} //parâmetros inseridos na url
app.MapGet("getuserdate", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return $"{dateStart} - {dateEnd}";
});

app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product);
});

//api.app.com/user/{code} //parâmetros via rota
app.MapGet("/products/{code}", ([FromRoute] int code) =>
{
    Product product = ProductRepository.GetProduct(code);
    
    if(product == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(product);
    //caso tenha um retorno de Results em algum momento, todos os retornos tem que ser do tipo Results
});

app.MapGet("/products", (HttpRequest request) => 
{
    return ProductRepository.GetProducts();
});

app.MapPut("/products", (Product product) => {
    Product productSaved = ProductRepository.GetProduct(product.Code);
    productSaved.Name = product.Name;
    return productSaved;
});

app.MapDelete("/products/{code}", ([FromRoute] int code) =>
{
    Product productSaved = ProductRepository.GetProduct(code);
    ProductRepository.RemoveProduct(productSaved);
});

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
