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
        public IActionResult Index()
        {
            using (SqlConnection con = new(_contexto.Conexion))
            {
                List<Imagen> listaImagenes = new();
                using (SqlCommand cmd = new("sp_listar_imagenes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    var rd = cmd.ExecuteReader();
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
                ViewBag.listado = listaImagenes;
                return View();
            }
        }

        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Imagen imagen)
        {
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
            return RedirectToAction("Index");
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

                }
                return View(Index);
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
