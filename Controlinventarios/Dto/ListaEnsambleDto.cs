namespace Controlinventarios.Dto
{
    public class ListaEnsambleDto
    {
        public int Id { get; set; }//
        public string NumeroSerial { get; set; }//
        public bool Estado { get; set; }
        public bool Renting { get; set; }
        public string TipoElemento { get; set; }
        public string NombreMarca { get; set; }
        public DateOnly FechaRegistroEquipo { get; set; }
    }
}
