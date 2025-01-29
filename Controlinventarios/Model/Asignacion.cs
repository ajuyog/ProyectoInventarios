using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Model
{
    public class Asignacion
    {
        public int id { get; set; }
        public string IdPersona { get; set; }
        public int IdEnsamble { get; set; }
    }
}
