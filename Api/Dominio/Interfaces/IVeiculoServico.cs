using Microsoft.AspNetCore.Mvc;
using minimalsAPIs.Dominio.Entidades;

namespace minimalsAPIs.Dominio.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> Todos(int? pagina=1, string? nome=null, string? marca=null);

        Veiculo? BuscaPorId(int id);

        void Incluir(Veiculo veiculo);

        void Atualizar(Veiculo veiculo);

        void Apagar(Veiculo veiculo);
    }
}