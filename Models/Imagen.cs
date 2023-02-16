using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Info_ToolsCRUD_SP_Imgs.Models
{
    public class Imagen
    {
        [Key]
        public int Id_Imagen { get; set; }
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
        public string? Nombre { get; set; }
        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; } // Indicador oara saber si la imagen ha sido cargada
    }
}
