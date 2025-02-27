﻿using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Model
{
    public class Asignacion
    {
        public string IdPersona { get; set; }
        [Key]
        public int IdEnsamble { get; set; }
        public DateOnly FechaRegistro { get; set; }
    }
}
