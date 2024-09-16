using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Dominio.ModelViews;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Infraestrutura.Db;
using minimalsAPIs.Dominio.Enums;

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

#region Adm CRUD
app.MapPost("adm/login", ([FromBody] LoginDTO usuario, IAdministradorServico admServico) => {
    if(admServico.Login(usuario) != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/adm", ([FromBody] AdministradorDTO admDTO, IAdministradorServico admServico) => {
    var validacao = new ErrosDeValidacao(){
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(admDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");

    if(string.IsNullOrEmpty(admDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia");

    if(admDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil não pode ser vazio");

    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var adm = new Administrador {
        Email = admDTO.Email,
        Senha = admDTO.Senha,
        Perfil = admDTO.Perfil.ToString() ?? PerfilEnum.Editor.ToString()
    };

    admServico.Incluir(adm);

    return Results.Created($"/adm/{adm.Id}", new AdministradorModelView {
        Id = adm.Id,
        Email = adm.Email,
        Perfil = adm.Perfil
    });
}).WithTags("Administradores");

app.MapGet("/adm", ([FromQuery] int? pagina, IAdministradorServico admServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = admServico.Todos(pagina);

    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Administradores");

app.MapGet("/adm/{id}", ([FromRoute] int id, IAdministradorServico admServico) => {
    var adm = admServico.BuscaPorId(id);

    if (adm == null)
        return Results.NotFound(); 

    return Results.Ok(new AdministradorModelView {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
}).WithTags("Administradores");
#endregion

#region Veiculos CRUD
ErrosDeValidacao ValidaDTO(VeiculoDTO veiculo)
{
    var validacao = new ErrosDeValidacao();
    validacao.Mensagens = [];

    if(string.IsNullOrEmpty(veiculo.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio");

    if(string.IsNullOrEmpty(veiculo.Marca))
        validacao.Mensagens.Add("A marca não pode ficar em branco");

    if(veiculo.Ano <= 1950)
        validacao.Mensagens.Add("Veículo muito antigo, aceito somentes anos acima de 1950");

    return validacao;
}

// POST - Adicionar
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculo, IVeiculoServico veiculoSrv) => {
    var validacao = ValidaDTO(veiculo);

    if(validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var veiculoCriado = new Veiculo {
        Nome = veiculo.Nome,
        Marca = veiculo.Marca,
        Ano = veiculo.Ano
    };
    
    veiculoSrv.Incluir(veiculoCriado);
    return Results.Created($"/veiculo/{veiculoCriado.Id}", veiculoCriado);
}).WithTags("Veiculos");

// GET - Todos
app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculo) => {
    return Results.Ok(veiculo.Todos(pagina));
}).WithTags("Veiculos");

// GET - Por Id
app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculo) => {
    var veiculoBuscado = veiculo.BuscaPorId(id);

    if (veiculoBuscado == null)
        return Results.NotFound(); 

    return Results.Ok(veiculoBuscado);
}).WithTags("Veiculos");

// PUT - Atualizar
app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO dadosVeiculo, IVeiculoServico servico) => {
    var veiculoBuscado = servico.BuscaPorId(id);
    var validacao = ValidaDTO(dadosVeiculo);
    
    if (veiculoBuscado == null)
        return Results.NotFound(); 

    if(validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    veiculoBuscado.Nome = dadosVeiculo.Nome;
    veiculoBuscado.Marca = dadosVeiculo.Marca;
    veiculoBuscado.Ano = dadosVeiculo.Ano;

    servico.Atualizar(veiculoBuscado);
    return Results.Ok(veiculoBuscado);
}).WithTags("Veiculos");

// DELETE - Apagar
app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico servico) => {
    var veiculoBuscado = servico.BuscaPorId(id);
    if (veiculoBuscado == null)
        return Results.NotFound();

    servico.Apagar(veiculoBuscado);
    return Results.Ok("Deletado com sucesso!");
}).WithTags("Veiculos");
#endregion

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();