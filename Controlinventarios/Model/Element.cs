namespace Controlinventarios.Model
{
    public class Element
    {
        public int id { get; set; }
        public string Marca { get; set; }
        public int idElementType { get; set; }
        public string Descripcion { get; set; }
        public string NumeroSerial { get; set; }
    }
}
