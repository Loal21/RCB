using System;
using System.Collections.Generic;

namespace CRUD_Imagenes.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Telefono { get; set; }
    public string? Documento { get; set; }

    public string? Contraseña { get; set; }

    public string? Rol { get; set; }

    public Usuario()
    {
        Rol = "Usuario"; 
    }
  
}