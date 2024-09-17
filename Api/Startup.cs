using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimalsAPIs;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Dominio.Enums;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Dominio.ModelViews;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Infraestrutura.Db;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
            if (string.IsNullOrEmpty(key))
                key = "123456";
        }

        private string key = ""; 

        public IConfiguration Configuration { get; set; } = default!;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAdministradorServico, AdministradorServico>();
            services.AddScoped<IVeiculoServico, VeiculoServico>();

            #region Builder - Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Cole seu token aqui:"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                        new OpenApiSecurityScheme 
                        {
                            Reference = new OpenApiReference 
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            #endregion

            #region Conexão com banco
            services.AddDbContext<DbContexto>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConexaoPadrao")));
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoint => 
            {
                #region Home
                endpoint.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Adm CRUD
                // Gerador de token
                string GerarTokenJwt(Administrador adm) 
                {
                    if (string.IsNullOrEmpty(key)) 
                        return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                    {
                        new Claim("Email", adm.Email),
                        new Claim("Perfil", adm.Perfil),
                        new Claim(ClaimTypes.Role, adm.Perfil)
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                // Login
                endpoint.MapPost("adm/login", ([FromBody] LoginDTO usuario, IAdministradorServico admServico) => {
                    var adm = admServico.Login(usuario);

                    if(adm != null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdministradorLogado 
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    else
                    {
                        return Results.Unauthorized();
                    }
                })
                .AllowAnonymous()
                .WithTags("Administradores");

                // Adicionar Usuario
                endpoint.MapPost("/adm", ([FromBody] AdministradorDTO admDTO, IAdministradorServico admServico) => {
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");

                // GET - Obter todos
                endpoint.MapGet("/adm", ([FromQuery] int? pagina, IAdministradorServico admServico) => {
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");

                // GET - Por Id
                endpoint.MapGet("/adm/{id}", ([FromRoute] int id, IAdministradorServico admServico) => {
                    var adm = admServico.BuscaPorId(id);

                    if (adm == null)
                        return Results.NotFound(); 

                    return Results.Ok(new AdministradorModelView {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");
                #endregion

                #region Veiculos CRUD
                // Função Local para validar os campos
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
                endpoint.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculo, IVeiculoServico veiculoSrv) => {
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

                // GET - Todos
                endpoint.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculo) => {
                    return Results.Ok(veiculo.Todos(pagina));
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

                // GET - Por Id
                endpoint.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculo) => {
                    var veiculoBuscado = veiculo.BuscaPorId(id);

                    if (veiculoBuscado == null)
                        return Results.NotFound(); 

                    return Results.Ok(veiculoBuscado);
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

                // PUT - Atualizar
                endpoint.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO dadosVeiculo, IVeiculoServico servico) => {
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Veiculos");

                // DELETE - Apagar
                endpoint.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico servico) => {
                    var veiculoBuscado = servico.BuscaPorId(id);
                    if (veiculoBuscado == null)
                        return Results.NotFound();

                    servico.Apagar(veiculoBuscado);
                    return Results.Ok("Deletado com sucesso!");
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Veiculos");
                #endregion
            });
        }
    }
}