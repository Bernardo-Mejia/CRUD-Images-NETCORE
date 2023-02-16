using Microsoft.EntityFrameworkCore;
using Info_ToolsCRUD_SP_Imgs.Models;

namespace Info_ToolsCRUD_SP_Imgs.Data
{
    public class ApplicationDBContext : DbContext
    {
        /*
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<Imagen> Imagen { get; set; }
        */
        public string Conexion { get; }

        public ApplicationDBContext(string valor)
        {
            Conexion = valor;
        }
    }
}
