namespace BikeStore.DTOs
{
    public class BicicletaDTO
    {
        public int IdBicicleta { get; set; }
        public int IdCategoria { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; } = "Disponible";
        public string? NombreCategoria { get; set; } // Opcional: para mostrar el nombre de la categoría en la lectura
    }
    public class CrearBicicletaDTO
    {
        public int IdCategoria { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}

