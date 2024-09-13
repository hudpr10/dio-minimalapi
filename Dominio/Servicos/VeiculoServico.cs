using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Infraestrutura.Db;

namespace minimalsAPIs.Dominio.Servicos
{
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbContexto _contexto;
        public VeiculoServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void Incluir(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            // var carro = _contexto.Veiculos.Find(id);
            // return carro;

            var carro = _contexto.Veiculos.Where(x => x.Id == id).FirstOrDefault();
            return carro;
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public void Apagar(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }


        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var carros = _contexto.Veiculos.AsQueryable();
            if(!string.IsNullOrEmpty(nome))
            {
                carros = carros.Where(x => EF.Functions.Like(x.Nome.ToLower(), $"%{nome.ToLower()}%"));
            }

            int itensPorPagina = 10;

            carros = carros.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return carros.ToList();
        }
    }
}