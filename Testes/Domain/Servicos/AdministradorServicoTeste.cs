using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Dominio.Servicos;
using minimalsAPIs.Infraestrutura.Db;
using NuGet.Frameworks;

namespace Testes.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicoTeste
    {
        private DbContexto CriarContextoDeTeste()
        {
            // Configurar o ConfigurationBuilder
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory()) // path ?? Directory.GetCurrentDirectory()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContexto(configuration);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange - Testa a instância
            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var admServico = new AdministradorServico(context);

            // Act - Testa o SET
            admServico.Incluir(adm);

            // Assert - Testa o GET
            Assert.AreEqual(1, admServico.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscarAdministrado()
        {
            var context = CriarContextoDeTeste();
            var admServico = new AdministradorServico(context);
            var admEncontrado = admServico.BuscaPorId(1);

            Assert.AreEqual(1, admEncontrado.Id);
        }
    }
}