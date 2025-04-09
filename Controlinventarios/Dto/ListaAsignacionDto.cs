namespace Controlinventarios.Dto
{
    public class ListaAsignacionDto
    {
        public string IdPersona { get; set; }
        public string NombrePersona { get; set; }
        public string ApellidoPersona { get; set; }
        public string Email { get; set; }
        public string CCPersonas { get; set; }
        public string AreaPersona { get; set; }
        public string EmpresaPersona { get; set; }
        public int CantidadEquiposAsignados { get; set; }
        public List<ListaEnsambleDto> EquiposAsignados { get; set; }
    }
}
