using NetTopologySuite.Operation.Polygonize;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controlinventarios.Dto
{
    public class PropiedadesDto
    {
        public int id { get; set; }
        public string Propiedad { get; set; }
        public int IdEnsamble { get; set; }
        public string EnsambleName { get; set; }
        public List<string> Propiedades { get; set; } //Esta propiedad es una nueva para poder sacar la lista de las propiedades de cada ensamble
    }
}