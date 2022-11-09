using EstudosAPI.Models;
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

app.MapPost("/productadd", (Product product) =>
{
    return $"{product.Code} - {product.Name}";
});

//api.app.com/user?datastart={date}&dateend={date} //parâmetros inseridos na url
app.MapGet("getuserdate", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return $"{dateStart} - {dateEnd}";
});

//api.app.com/user/{code} //parâmetros via rota
app.MapGet("/getuser/{code}", ([FromRoute] string code) =>
{
    return code;
});

app.MapGet("/getproduct", (HttpRequest request) => 
{
    return request.Headers["product-code"].ToString();
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
