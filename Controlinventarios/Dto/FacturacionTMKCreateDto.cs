namespace Controlinventarios.Dto
{
    public class FacturacionTMKCreateDto
    {
        public float VlrNeto { get; set; }

        public int IdEnsamble { get; set; }

        public string Descripcion { get; set; }

        public DateOnly? Fecha { get; set; }
    }
}
