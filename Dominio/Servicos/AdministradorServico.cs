using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Dominio.Interfaces;
using minimalsAPIs.Infraestrutura.Db;

namespace minimalsAPIs.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {   
            _contexto = contexto;
        }

        public Administrador? Login(LoginDTO usuario)
        {
            var adm = _contexto.Administradores.Where(x => x.Email == usuario.Email && x.Senha == usuario.Senha).FirstOrDefault();
            return adm;
        }

        public Administrador Incluir(Administrador adm)
        {
            _contexto.Administradores.Add(adm);
            _contexto.SaveChanges();

            return adm;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var adms = _contexto.Administradores.AsQueryable();
            int itensPorPagina = 10;

            if(pagina != null)
                adms = adms.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return adms.ToList();
        }

        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}