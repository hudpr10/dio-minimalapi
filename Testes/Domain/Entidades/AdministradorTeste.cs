using minimalsAPIs.Dominio.Entidades;

namespace Testes.Domain.Entidades
{
    [TestClass]
    public class AdministradorTeste
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange - Testa a instância
            var adm = new Administrador();
            adm.Id = 1;
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            // Act - Testa o SET

            // Assert - Testa o GET
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("teste", adm.Senha);
            Assert.AreEqual("Adm", adm.Perfil);
        }
    }
}