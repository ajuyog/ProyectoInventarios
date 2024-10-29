namespace Controlinventarios.Dto
{
    public class ElementDto
    {
        public int id { get; set; }
        public string Marca { get; set; }
        public int idElementType { get; set; }
        public string Descripcion { get; set; }
        public string NumeroSerial { get; set; }
    }
}
