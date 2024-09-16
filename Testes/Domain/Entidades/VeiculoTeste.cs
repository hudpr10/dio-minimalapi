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
        }
    }
}