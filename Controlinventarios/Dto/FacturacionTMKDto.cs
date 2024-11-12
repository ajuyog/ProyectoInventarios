namespace Controlinventarios.Dto
{
    public class FacturacionTMKDto
    {
        public int id { get; set; }
        public int item { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public float vlrUnitario { get; set; }
    }
}
