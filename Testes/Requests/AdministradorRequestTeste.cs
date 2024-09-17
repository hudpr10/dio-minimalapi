using System.Net;
using System.Text;
using System.Text.Json;
using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Enums;
using minimalsAPIs.Dominio.ModelViews;
using Testes.Helpers;

namespace Testes.Requests
{
    [TestClass]
    public class AdministradorRequestTeste
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestarGetSetPropriedades()
        {
            // Arrange - Testa a instância
            var login = new LoginDTO 
            {
                Email = "adm@teste.com",
                Senha = "123456"
            };

            var adicionarAdm = new AdministradorDTO
            {
                Email = "adm2@email.com",
                Senha = "123456",
                Perfil = PerfilEnum.Adm
            };

            var content = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "Application/json");
            var contentAdcionar = new StringContent(JsonSerializer.Serialize(adicionarAdm), Encoding.UTF8, "Application/json");

            // Act - Testa o SET
            var response = await Setup.client.PostAsync("/adm/login", content);
            var responseAdicionar = await Setup.client.PostAsync("/adm/login", contentAdcionar);

            // Assert - Testa o GET
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, responseAdicionar.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogado?.Email ?? "");
            Assert.IsNotNull(admLogado?.Perfil ?? "");
            Assert.IsNotNull(admLogado?.Token ?? "");

            // Console.WriteLine(admLogado?.Token);
        }
    }
}