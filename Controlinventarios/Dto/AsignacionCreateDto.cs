namespace Controlinventarios.Dto
{
    public class AsignacionCreateDto
    {
        public string IdPersona { get; set; }
        public int IdEnsamble { get; set; }
        public DateOnly FechaRegistro { get; set; }
    }
}
