namespace Controlinventarios.Dto
{
    public class PersonaCreateDto
    {
        public string userId { get; set; }
        public int IdArea { get; set; }
        public string identificacion { get; set; }
        public bool Estado { get; set; }
        public int idEmpresa { get; set; }
    }
}
