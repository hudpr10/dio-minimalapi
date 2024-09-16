using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Dominio.ModelViews;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Conexão com banco
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));
#endregion

var app = builder.Build();

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Adm Login
app.MapPost("adm/login", ([FromBody] LoginDTO usuario, IAdministradorServico admServico) => {
    if(admServico.Login(usuario) != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");
#endregion

#region Veiculos Adicionar
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculo, IVeiculoServico veiculoSrv) => {
    var veiculoCriado = new Veiculo {
        Nome = veiculo.Nome,
        Marca = veiculo.Marca,
        Ano = veiculo.Ano
    };
    
    veiculoSrv.Incluir(veiculoCriado);
    return Results.Created($"/veiculo/{veiculoCriado.Id}", veiculoCriado);
}).WithTags("Veiculos");
#endregion

#region Veiculos GET Todos
app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculo) => {
    var veiculos = veiculo.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculos");
#endregion

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();