namespace Controlinventarios.Model
{
    public class Ensamble
    {
        public int Id { get; set; }

        public int IdElementType { get; set; }

        public int IdMarca { get; set; }

        public string NumeroSerial { get; set; }

        public bool Estado { get; set; }

        public string Descripcion { get; set; }

        public bool Renting { get; set; }
    }
}
