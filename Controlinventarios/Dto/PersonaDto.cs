using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class PersonaDto
    {
        public string UserId { get; set; }
        public int IdArea { get; set; }
        public string Identificacion { get; set; }
        public bool Estado { get; set; }
        public int IdEmpresa { get; set; }
        public string UserName { get; set; }
        public string AreaName { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}
