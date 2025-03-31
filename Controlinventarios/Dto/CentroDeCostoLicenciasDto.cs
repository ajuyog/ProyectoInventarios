using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Controlinventarios.Dto
{
    public class CentroDeCostoLicenciasDto
    {
        public string Descripcion { get; set; }
        public float VlrNeto { get; set; }
        public string NumeroSerial { get; set; }
        public string NombreArea { get; set; }
        public string IdEmpresa { get; set; }
        public bool Renting { get; set; }
        public int total { get; set; }
        public string TipoDeElemento { get; set; }
        public DateOnly Fecha { get; set; }
        public string Marca { get; set; }
        public string NombreMarca { get; set; }
    }
}