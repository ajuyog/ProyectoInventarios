namespace Controlinventarios.Dto
{
    public class FacturacionTMKDto
    {
        public int Id { get; set; }

        public float VlrNeto { get; set; }

        public int IdEnsamble { get; set; }

        public string Descripcion { get; set; }

        public DateOnly? Fecha { get; set; }

        public string EnsambleName { get; set; }
    }
}
