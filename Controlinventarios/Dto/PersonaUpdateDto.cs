namespace Controlinventarios.Dto
{
    public class PersonaUpdateDto
    {
        public int IdArea { get; set; }
        public string identificacion { get; set; }
        public bool Estado { get; set; }
        public int idEmpresa { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}
