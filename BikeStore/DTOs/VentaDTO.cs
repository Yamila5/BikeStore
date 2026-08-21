namespace BikeStore.DTOs
{
    public class VentaDTO
    {
            public int IdVenta { get; set; }
            public DateTime Fecha { get; set; }
            public int IdCliente { get; set; }
            public decimal Subtotal { get; set; }
            public decimal IVA { get; set; }
            public decimal Total { get; set; }
            public List<DetalleVentaDTO> Detalles { get; set; } = new();
        }

        public class CrearVentaDTO
        {
            public int IdCliente { get; set; }
            public List<CrearDetalleVentaDTO> Detalles { get; set; } = new();
        }
}
