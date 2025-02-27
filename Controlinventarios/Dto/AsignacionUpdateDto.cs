using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class AsignacionUpdateDto
    {
        public string IdPersona { get; set; }
        public int IdEnsamble { get; set; }
        public DateOnly FechaRegistro { get; set; }
    }
}
