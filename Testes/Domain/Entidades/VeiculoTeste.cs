using minimalsAPIs.Dominio.Entidades;

namespace Testes.Domain.Entidades
{
    [TestClass]
    public class VeiculoTeste
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            Veiculo v1 = new Veiculo();

            v1.Id = 1;
            v1.Nome = "Nome";
            v1.Marca = "Marca";
            v1.Ano = 2004;

            Assert.AreEqual(1, v1.Id);
            Assert.AreEqual("Nome", v1.Nome);
            Assert.AreEqual("Marca", v1.Marca);
            Assert.AreEqual(2004, v1.Ano);
        }
    }
}