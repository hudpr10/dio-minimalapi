using minimalsAPIs.Dominio.Enums;

namespace minimalsAPIs.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public PerfilEnum? Perfil { get; set; } = default!;   
    }
}