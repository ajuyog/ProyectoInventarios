namespace Controlinventarios.Dto
{
    public class EnsambleDto
    {
        public int Id { get; set; }//

        public int IdElementType { get; set; }//

        public int IdMarca { get; set; }//

        public string NumeroSerial { get; set; }//

        public bool Estado { get; set; }

        public string Descripcion { get; set; }

        public bool Renting { get; set; }
            
        public string TipoElemento { get; set; }
        public string NumeroFactura { get; set; }

        public string NombreMarca { get; set; }
        public string PropiedadesConcatenadas { get; set; }
    }
}
