using Info_ToolsCRUD_SP_Imgs.Data;
using Info_ToolsCRUD_SP_Imgs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Info_ToolsCRUD_SP_Imgs.Controllers
{
    public class LibroController : Controller
    {
        private readonly ApplicationDBContext _DbContext;
        public LibroController(ApplicationDBContext _dbContext)
        {
            _DbContext = _dbContext;
        }

        public IActionResult Index()
        {
            using (SqlConnection conexion = new SqlConnection(_DbContext.Conexion))
            {
                List<Imagen> ListaImagenes = new List<Imagen>();
                using (SqlCommand cmd = new SqlCommand("sp_Listar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();

                    var rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        ListaImagenes.Add(new Imagen
                        {
                            Id_Imagen = (int)rd["Id_Imagen"],
                            Fecha_Creacion = (DateTime)rd["Fecha_Creacion"],
                            Nombre = rd["Nombre"].ToString(),
                            Image = rd["Imagen"].ToString()
                        });
                    }
                }
                return View(ListaImagenes);
            }
        }

        [HttpGet]
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

                            using (SqlConnection conexion = new SqlConnection(_DbContext.Conexion))
                            {
                                using (SqlCommand cmd = new SqlCommand("sp_Crear", conexion))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("Fecha_Creacion", imagen.Fecha_Creacion);
                                    cmd.Parameters.AddWithValue("Nombre", imagen.Nombre);
                                    cmd.Parameters.AddWithValue("Imagen", imagen.Image);
                                    conexion.Open();
                                    cmd.ExecuteNonQuery();
                                    conexion.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View(imagen);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            using (SqlConnection conexion = new(_DbContext.Conexion))
            {
                Imagen registro = new Imagen();
                using (SqlCommand cmd = new("sp_Buscar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id_Imagen", id);
                    conexion.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    registro.Id_Imagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][2].ToString();
                    registro.Image = dt.Rows[0][3].ToString();
                }
                return View(registro);
            }
        }

        [HttpPost]
        public IActionResult Editar(Imagen imagen)
        {
            try
            {
                using (SqlConnection conexion = new(_DbContext.Conexion))
                {
                    string i; // Validacion si una imagen ha sido cargada
                    if (imagen.File == null)
                        i = "null";
                    else
                    {
                        byte[] bytes;
                        // Conversión de imagen a formato que se pueda almacenar
                        using (Stream fs = imagen.File.OpenReadStream())
                        {
                            using (BinaryReader br = new(fs))
                            {
                                bytes = br.ReadBytes((int)fs.Length);
                                i = Convert.ToBase64String(bytes, 0, bytes.Length);
                            }
                        }
                    }

                    using (SqlCommand cmd = new("sp_Actualizar", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("Id_Imagen", imagen.Id_Imagen);
                        cmd.Parameters.AddWithValue("Nombre", imagen.Nombre);
                        cmd.Parameters.AddWithValue("Imagen", i);
                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        conexion.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View(imagen);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection conexion = new(_DbContext.Conexion))
            {
                Imagen registro = new Imagen();
                using (SqlCommand cmd = new("sp_Buscar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id_Imagen", id);
                    conexion.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    registro.Id_Imagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][2].ToString();
                    registro.Image = dt.Rows[0][3].ToString();
                }
                return View(registro);
            }
        }

        [HttpPost]
        public IActionResult Eliminar(Imagen imagen)
        {
            using (SqlConnection conexion = new(_DbContext.Conexion))
            {
                using SqlCommand cmd = new("sp_Eliminar", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Id_Imagen", imagen.Id_Imagen);
                conexion.Open();
                cmd.ExecuteNonQuery();
                conexion.Close();
            }
            return RedirectToAction("Index");
        }
    }
}
