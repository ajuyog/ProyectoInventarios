﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Controlinventarios.Model
{
    public class Propiedades
    {
        public int id { get; set; }
        public string Propiedad { get; set; }
        public int IdEnsamble { get; set; }
        //public List<string> Propiedadess { get; set; }
    }    
}
