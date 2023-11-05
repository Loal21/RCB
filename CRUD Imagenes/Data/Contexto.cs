using Microsoft.EntityFrameworkCore;

namespace CRUD_Imagenes.Data
{
    public class Contexto
    {
        public string Conexion { get; }
        public Contexto(string valor)
        {
            Conexion = valor;
        }
    }
    public partial class DbpruebaContext : DbContext
    {
        private readonly string _connectionString;

        public DbpruebaContext(Contexto contexto, DbContextOptions<DbpruebaContext> options)
            : base(options)
        {
            _connectionString = contexto.Conexion;
        }
    }
}
