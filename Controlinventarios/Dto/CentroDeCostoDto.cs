using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Controlinventarios.Dto
{
    public class CentroDeCostoDto
    {
        public string Descripcion { get; set; }
        public float VlrNeto { get; set; }
        public int IdEnsamble { get; set; }
        public string NumeroSerial { get; set; }
        public string NombreArea { get; set; }
    }

    //public class CentroDeCostoDto
    //{
    //    public List<FacturacionTMKDto> Valor { get; set; }
    //    public List<EnsambleDto> Equipo { get; set; }
    //    public List<AreaDto> Area { get; set; }
    //}
}

