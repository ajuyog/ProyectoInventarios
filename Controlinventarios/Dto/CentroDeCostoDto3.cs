namespace Controlinventarios.Dto
{
    public class CentroDeCostoDto3
    {
        public string Factura { get; set; }
        public float TotalVlrNeto { get; set; }
        public int totalEquipos { get; set; }
        public float TotalIva { get; set; }
        public float Retencion { get; set; }
        public float Total_A_Pagar { get; set; }
        public DateTime? Fecha { get; set; }

        // propiedad para obtener formateado con comas
        public string TotalVlrNetoConFormato
        {
            get
            {
                // se convierte a un tipo de dato string
                return TotalVlrNeto.ToString("N2");// el "N2" le da dos decimales con comas
            }
        }

        public string TotalIvaConFormato
        {
            get
            {
                return TotalIva.ToString("N2");
            }
        }

        public string TotalRetencionConFormato 
        { 
            get 
            {
                return Retencion.ToString("N2");
            } 
        }

        public string TotalAPagarConFormato
        {
            get
            {
                // formatea el Total_A_Pagar como moneda
                return Total_A_Pagar.ToString("N2");  // el "N2" le da dos decimales con comas
            }
        }
    }
}
