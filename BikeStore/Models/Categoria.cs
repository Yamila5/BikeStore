namespace BikeStore.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }

        public ICollection<Bicicleta> Bicicletas { get; set; } = new List<Bicicleta>();
    }
}