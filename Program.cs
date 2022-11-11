using EstudosAPI.Data;
using EstudosAPI.Models;
using EstudosAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//criação do serviço para a instância do banco de dados
string connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

var configuration = app.Configuration;
ProductRepository.Init(configuration);

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

//Checando o ambiente de desenvolvimento para poder mapear a rota em questão.
if(app.Environment.IsStaging())
{
    app.MapGet("/configuration/database", (IConfiguration configuration) =>
    {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });
}
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
    List<Product> products = ProductRepository.GetProducts();
    
    if(products == null)
    {
        Results.NotFound();
    }
    
    return Results.Ok(products);
});

app.MapPut("/products", (Product product) => {
    Product productSaved = ProductRepository.GetProduct(product.Code);

    if (productSaved == null)
    {
        return Results.NotFound();
    }

    productSaved.Name = product.Name;
    
    return Results.Ok(productSaved);
});

app.MapDelete("/products/{code}", ([FromRoute] int code) =>
{
    Product productSaved = ProductRepository.GetProduct(code);
    
    if(productSaved == null)
    {
        return Results.NotFound();
    }
    
    ProductRepository.RemoveProduct(productSaved);
    
    return Results.Ok();
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
