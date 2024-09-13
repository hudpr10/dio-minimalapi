using Microsoft.EntityFrameworkCore;
using minimalsAPIs.Dominio.Entidades;

namespace minimalsAPIs.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuracaoAppSettings;

        public DbContexto(IConfiguration configuration)
        {
            _configuracaoAppSettings = configuration;
        }

        public DbContexto(DbContextOptions options) : base(options)
        {

        }
 
        public DbSet<Administrador> Administradores { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var stringConexao = _configuracaoAppSettings.GetConnectionString("ConexaoPadrao")?.ToString();

                if(!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseSqlServer(stringConexao);
                }
            }
        }
    }
}