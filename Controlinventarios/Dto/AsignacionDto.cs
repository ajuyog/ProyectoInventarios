using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class AsignacionDto
    {
        public string IdPersona { get; set; }
        public int IdEnsamble { get; set; }
        public DateOnly FechaRegistro { get; set; }
        public string NombrePersona { get; set; }
        public string ApellidoPersona { get; set; }
        public string Email { get; set; }
        public string CCPersonas { get; set; }
        public string AreaPersona { get; set; }
        public int Id { get; set; }//
        public string NumeroSerial { get; set; }//
        public bool Estado { get; set; }
        public bool Renting { get; set; }
        public string TipoElemento { get; set; }
        public string NombreMarca { get; set; }
        public DateOnly FechaRegistroEquipo { get; set; }
        public List<ListaEnsambleDto> EquiposAsignados { get; set; }
    }
}
