namespace Controlinventarios.Dto
{
    public class EnsambleCreateDto
    {
        public int IdElementType { get; set; }

        public int IdMarca { get; set; }

        public string NumeroSerial { get; set; }

        public bool Estado { get; set; }

        public string Descripcion { get; set; }

        public bool Renting { get; set; }
    }
}
