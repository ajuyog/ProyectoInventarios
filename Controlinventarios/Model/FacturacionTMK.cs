using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Model
{
    public class FacturacionTMK
    {
        public int Id { get; set; }

        public float VlrNeto { get; set; }
 
        public int IdEnsamble { get; set; }

        public string Descripcion { get; set; }

        public DateOnly? Fecha { get; set; }
    }
}
