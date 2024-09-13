using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();

app.MapGet("/", () => "Olá, mundo!");

app.MapPost("/login", ([FromBody] LoginDTO usuario, IAdministradorServico admServico) => {
    if(admServico.Login(usuario) != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();