using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class PersonaDto
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int IdArea { get; set; }
        public string identificacion { get; set; }
        public bool Estado { get; set; }
        public int idEmpresa { get; set; }
        public string UserName { get; set; }
        public string AreaName { get; set; }
        public string NombreEmpresa { get; set; }
    }
}
