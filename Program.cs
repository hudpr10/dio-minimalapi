using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Dominio.ModelViews;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", ([FromBody] LoginDTO usuario, IAdministradorServico admServico) => {
    if(admServico.Login(usuario) != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();