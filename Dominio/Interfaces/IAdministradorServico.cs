using minimalsAPIs.Dominio.DTOs;
using minimalsAPIs.Dominio.Entidades;

namespace minimalsAPIs.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO usuario);
    }
}