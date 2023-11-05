using Microsoft.EntityFrameworkCore;
using CRUD_Imagenes.Models;

namespace CRUD_Imagenes.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string Documento, string Contraseña);
        Task<Usuario> SaveUsuario(Usuario modelo);

    }
}
