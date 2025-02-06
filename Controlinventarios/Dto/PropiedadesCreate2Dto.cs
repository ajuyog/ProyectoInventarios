namespace Controlinventarios.Dto
{
    public class PropiedadesCreate2Dto
    {
        public string NumeroSerial { get; set; }
        public int IdEnsamble { get; set; }
        public string TagsInput { get; set; } // Cadena separada por comas
        public List<string> Propiedadess { get; set; }
    }
}