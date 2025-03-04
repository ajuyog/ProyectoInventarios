namespace Controlinventarios.Dto
{
    public class ListaAsignacionDto
    { 
        public string IdPersona { get; set; }
        public string NombrePersona { get; set; }
        public string ApellidoPersona { get; set; }
        public string Email { get; set; }
        public List<ListaEnsambleDto> EquiposAsignados { get; set; }
    }
}
