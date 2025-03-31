namespace Controlinventarios.Utildad
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int registrosPorPagina = 10;
        private readonly int cantidadMaxPorPagina = 150; /* Límite Restricción */

        public int RegistrosPorPagina
        {
            get { return registrosPorPagina; }
            set { registrosPorPagina = value > cantidadMaxPorPagina ? cantidadMaxPorPagina : value; }
        }
    }
}