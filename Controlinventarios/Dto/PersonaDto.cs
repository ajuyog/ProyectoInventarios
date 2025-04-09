using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class PersonaDto
    {
        public string UserId { get; set; }
        public string Identificacion { get; set; }
        public string UserName { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public DateTime? FechaCumpleaños { get; set; }
        public string TelefonoMovil { get; set; }
        public string Cargo { get; set; }
        public int IdArea { get; set; }
        public string AreaName { get; set; }
        public string NombreEmpresa { get; set; }
        public int IdEmpresa { get; set; }
        public bool Estado { get; set; }
    }
}
