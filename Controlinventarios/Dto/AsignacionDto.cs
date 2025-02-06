using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class AsignacionDto
    {
        public string IdPersona { get; set; }
        public int IdEnsamble { get; set; }
        public string NombrePersona { get; set; }
        public string Numeroserial { get; set; }
        public DateOnly FechaRegistro { get; set; }
    }
}
