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


        public Administrador Login(LoginDTO usuario)
        {
            var adm = _contexto.Administradores.Where(x => x.Email == usuario.Email && x.Senha == usuario.Senha).FirstOrDefault();
            return adm;
        }
    }
}