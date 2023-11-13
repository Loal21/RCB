using CRUD_Imagenes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using Microsoft.Win32;
using CRUD_Imagenes.Data;

namespace CRUD_Imagenes.Controllers
{

    public class ImagenController : Controller
    {
        public readonly Contexto _contexto;
        public ImagenController(Contexto contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index(int documento)
        {
            int idUsuario = -1;
            int idUsuarioImagenes = -1;
            using (SqlConnection con = new(_contexto.Conexion))
            {
                List<Imagen> listaImagenes = new();
                using (SqlCommand cmd = new("sp_listar_imagenes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlCommand GetUserID = new SqlCommand("Select IdUsuario from USUARIO where Documento = @Documento", con)) // obtiene el valor de id de la tabla USUARIO
                    {
                        GetUserID.Parameters.AddWithValue("@Documento", documento);
                        con.Open();

                        using (SqlDataReader reader = GetUserID.ExecuteReader())
                        {
                            if (reader.Read()) // Verifica si se encontró un resultado.
                            {
                                idUsuario = reader.GetInt32(0); // Obtiene el valor de la columna "IdUsuario" del primer resultado.
                            }
                            con.Close();
                        }
                        using (SqlCommand GetUserIDImagenes = new SqlCommand("Select ID_UsuarioCreador from Imagenes where Documento = @Documento", con)) // obtiene el valor de ID de la tabla imagenes
                        {
                            GetUserIDImagenes.Parameters.AddWithValue("@Documento", documento);
                            con.Open();

                            using (SqlDataReader reader = GetUserIDImagenes.ExecuteReader())
                            {
                                if (reader.Read()) // Verifica si se encontró un resultado.
                                {
                                    idUsuarioImagenes = reader.GetInt32(0); // Obtiene el valor de la columna "IdUsuario" del primer resultado.
                                }
                                con.Close();
                            }
                        }
                        if (idUsuario == idUsuarioImagenes)
                        {
                            using (SqlCommand ListarImagenes = new SqlCommand("select Imagenes.* from usuario inner join Imagenes on USUARIO.idUsuario = Imagenes.ID_UsuarioCreador " +
                                "WHERE USUARIO.IdUsuario = @IdUsuario and Imagenes.ID_UsuarioCreador = @Id_UsuarioCreador", con))
                            {
                                ListarImagenes.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                                ListarImagenes.Parameters.Add("@Id_UsuarioCreador", SqlDbType.Int).Value = idUsuarioImagenes;
                                con.Open();
                                var rd = ListarImagenes.ExecuteReader();
                                while (rd.Read())
                                {
                                    listaImagenes.Add(new Imagen
                                    {
                                        Id_Imagen = (int)rd["Id_Imagen"],
                                        Nombre = rd["Nombre"].ToString(),
                                        Apellido = rd["Apellido"].ToString(),
                                        Image = rd["Imagen"].ToString(),
                                        Documento = rd["Documento"].ToString(),
                                        Telefono = rd["Telefono"].ToString()
                                    });

                                }
                            }

                        }

                    }
                    ViewBag.listado = listaImagenes;
                    return View();
                }
            }
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Imagen imagen, int documento)
        {
            int idUsuario = -1;
            try
            {
                byte[] bytes;
                if (imagen.File != null && imagen.Nombre != null)
                {
                    using (Stream fs = imagen.File.OpenReadStream())
                    {
                        using (BinaryReader br = new(fs))
                        {
                            bytes = br.ReadBytes((int)fs.Length);
                            imagen.Image = Convert.ToBase64String(bytes, 0, bytes.Length);
                            using (SqlConnection con = new(_contexto.Conexion))
                            {
                                using (SqlCommand cmd = new("sp_insertar_imagen", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = imagen.Nombre;
                                    cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar).Value = imagen.Apellido;
                                    cmd.Parameters.Add("@Imagen", SqlDbType.NVarChar).Value = imagen.Image;
                                    cmd.Parameters.Add("@Documento", SqlDbType.Int).Value = imagen.Documento;
                                    cmd.Parameters.Add("@Telefono", SqlDbType.Int).Value = imagen.Telefono;
                                    using (SqlCommand cmd2 = new SqlCommand("Select IdUsuario from USUARIO where Documento = @Documento", con))
                                    {
                                        cmd2.Parameters.AddWithValue("@Documento", documento);
                                        con.Open();

                                        using (SqlDataReader reader = cmd2.ExecuteReader())
                                        {
                                            if (reader.Read()) // Verifica si se encontró un resultado.
                                            {
                                                idUsuario = reader.GetInt32(0); // Obtiene el valor de la columna "IdUsuario" del primer resultado.
                                            }
                                        }
                                    }
                                    con.Close();
                                    cmd.Parameters.Add("@ID_UsuarioCreador", SqlDbType.Int).Value = idUsuario;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                ViewBag.Error = e.Message;
                return View();
            }
            return RedirectToAction("Index", new { documento = documento });
        }
        public IActionResult Editar(int id)
        {
            using (SqlConnection con = new(_contexto.Conexion))
            {
                Imagen registro = new();
                using (SqlCommand cmd = new("sp_buscar_imagen", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    con.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    registro.Id_Imagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][1].ToString();
                    registro.Image = dt.Rows[0][2].ToString();
                    registro.Documento = dt.Rows[0][3].ToString();
                    registro.Apellido = dt.Rows[0][4].ToString();
                    registro.Telefono = dt.Rows[0][5].ToString();

                }
                return View(registro);
            }
        }
        [HttpPost]
        public IActionResult Editar(Imagen imagen)
        {
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    string i;
                    if (imagen.File == null)
                    {
                        i = "null";
                    }
                    else
                    {
                        byte[] bytes;
                        using (Stream fs = imagen.File.OpenReadStream())
                        {
                            using (BinaryReader br = new(fs))
                            {
                                bytes = br.ReadBytes((int)fs.Length);
                                i = Convert.ToBase64String(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    using (SqlCommand cmd = new("sp_actualizar_imagen", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = imagen.Id_Imagen;
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = imagen.Nombre;
                        cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar).Value = imagen.Apellido;
                        cmd.Parameters.Add("@Imagen", SqlDbType.NVarChar).Value = i;
                        cmd.Parameters.Add("@Documento", SqlDbType.NVarChar).Value = imagen.Documento;
                        cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar).Value = imagen.Telefono;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            using (SqlConnection con = new(_contexto.Conexion))
            {
                Imagen registro = new();
                using (SqlCommand cmd = new("sp_buscar_imagen", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    con.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    registro.Id_Imagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][1].ToString();
                    registro.Image = dt.Rows[0][2].ToString();
                    registro.Documento = dt.Rows[0][3].ToString();
                    registro.Apellido = dt.Rows[0][4].ToString();
                    registro.Telefono = dt.Rows[0][5].ToString();

                }
                return View(registro);
            }
        }
        [HttpPost]
        public IActionResult Eliminar(Imagen img)
        {
            using (SqlConnection con = new(_contexto.Conexion))
            {
                using (SqlCommand cmd = new("sp_eliminar_imagen", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = img.Id_Imagen;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
