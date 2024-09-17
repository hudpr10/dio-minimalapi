using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Entidades;
using minimalsAPIs.Dominio.Interfaces;

namespace Testes.Mocks
{
    public class AdministradorServicoMock : IAdministradorServico
    {
        private static List<Administrador> administradores = new List<Administrador>()
        {
            new Administrador 
            {
                Id = 1,
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            },

            new Administrador 
            {
                Id = 2,
                Email = "editor@teste.com",
                Senha = "123456",
                Perfil = "Editor"
            }
        };
        public Administrador? BuscaPorId(int id)
        {
            return administradores.Find(a => a.Id == id);
        }

        public Administrador Incluir(Administrador adm)
        {
            adm.Id = administradores.Count();
            administradores.Add(adm);
            return adm;
        }

        public Administrador? Login(LoginDTO usuario)
        {
            return administradores.Find(x => x.Email == usuario.Email && x.Senha == usuario.Senha);
        }

        public List<Administrador> Todos(int? pagina)
        {
            return administradores;
        }
    }
}