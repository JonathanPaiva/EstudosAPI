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
app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) =>
{
    Category category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    
    Product product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };
    
    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        
        foreach(var tag in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = tag });
        }
    }

    context.Products.Add(product);
    context.SaveChanges();
        
    return Results.Created($"/products/{product.Code}", product);
});

//api.app.com/user/{code} //parâmetros via rota
app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    Product product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();

    if(product == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(product);
    //caso tenha um retorno de Results em algum momento, todos os retornos tem que ser do tipo Results
});

app.MapGet("/products", (HttpRequest request, ApplicationDbContext context) => 
{
    List<Product> products = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .ToList();
    
    if(products == null)
    {
        Results.NotFound();
    }
    
    return Results.Ok(products);
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {

    Product product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();

    if (product == null)
    {
        return Results.NotFound();
    }

    product.Code = productRequest.Code;
    product.Name = productRequest.Name;
    product.Description = productRequest.Description;
    product.CategoryId = productRequest.CategoryId;

    product.Tags = new List<Tag>();

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();

        foreach (var tag in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = tag });
        }
    }

    context.SaveChanges();

    return Results.Ok(product);
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    Product product = context.Products.Where(p => p.Id == id).First();

    if (product == null)
    {
        return Results.NotFound();
    }

    context.Products.Remove(product);

    context.SaveChanges();
    
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
