using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class PersonaDto
    {
        [Key]
        public string id { get; set; }
        public string userId { get; set; }
        public int IdArea { get; set; }
        public string identificacion { get; set; }
        public bool Estado { get; set; }
    }
}
