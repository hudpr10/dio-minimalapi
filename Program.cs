using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

app.MapGet("/", () => "Olá, mundo!");

app.MapPost("/login", (LoginDTO usuario) => {
    if(usuario.Email == "adm@teste.com" && usuario.Senha == "123456")
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();