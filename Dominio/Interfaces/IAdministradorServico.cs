using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Entidades;

namespace minimalsAPIs.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO usuario);

        Administrador Incluir(Administrador adm);

        Administrador? BuscaPorId(int id);

        List<Administrador> Todos(int? pagina);
    }
}