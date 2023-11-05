using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CRUD_Imagenes.Models;
using CRUD_Imagenes.Servicios.Contrato;
using CRUD_Imagenes.Data;

namespace CRUD_Imagenes.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DbpruebaContext _dbContext;
        public UsuarioService(DbpruebaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> GetUsuario(string Documento, string Contraseña)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(u => u.Documento == Documento && u.Contraseña == Contraseña)
                 .FirstOrDefaultAsync();

            return usuario_encontrado;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
