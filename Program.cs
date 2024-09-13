using minimalsAPIs.Dominio.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Olá, mundo!");

app.MapPost("/login", (LoginDTO usuario) => {
    if(usuario.Email == "adm@teste.com" && usuario.Senha == "123456")
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();