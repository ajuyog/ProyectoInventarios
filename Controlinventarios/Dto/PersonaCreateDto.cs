namespace Controlinventarios.Dto
{
    public class PersonaCreateDto
    {
        public string userId { get; set; }
        public string identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime? FechaCumpleaños { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
        public string TelefonoMovil { get; set; }
        public int idEmpresa { get; set; }
        public int IdArea { get; set; }
        public bool Estado { get; set; }
    }
}
