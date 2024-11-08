namespace Controlinventarios.Dto
{
    public class EnsambleDto
    {
        public int id { get; set; }
        public int IdElementType { get; set; }
        public int IdMarca { get; set; }
        public string NumeroSerial { get; set; }
        public bool Estado { get; set; }
        public string Descripcion { get; set; }
    }
}
