﻿using System.ComponentModel.DataAnnotations;

namespace Controlinventarios.Dto
{
    public class AsignacionDto
    {
        [Key]
        public int id { get; set; }
        public int IdPersona { get; set; }
        public int IdEnsamble { get; set; }
        public string NombrePersona { get; set; }
        public string Numeroserial { get; set; }
    }
}
