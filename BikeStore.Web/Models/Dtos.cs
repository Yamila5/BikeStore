using System.ComponentModel.DataAnnotations;

namespace BikeStore.Web.Models;

public class CategoriaDto
{
    public int IdCategoria { get; set; }
    [Required, StringLength(80)] public string Nombre { get; set; } = string.Empty;
    [StringLength(250)] public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
public class BicicletaDto
{
    public int IdBicicleta { get; set; }
    [Range(1, int.MaxValue)] public int IdCategoria { get; set; }
    [Required, StringLength(80)] public string Marca { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Modelo { get; set; } = string.Empty;
    [Range(0.01, 9999999)] public decimal Precio { get; set; }
    [Range(0, int.MaxValue)] public int Stock { get; set; }
    [StringLength(30)] public string Estado { get; set; } = "Disponible";
    public CategoriaDto? Categoria { get; set; }
}
public class ClienteDto
{
    public int IdCliente { get; set; }
    [Required, StringLength(10, MinimumLength = 10)] public string Cedula { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Nombres { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Apellidos { get; set; } = string.Empty;
    [StringLength(20)] public string? Telefono { get; set; }
    [EmailAddress, StringLength(120)] public string? Correo { get; set; }
}
public class DetalleVentaDto
{
    public int IdDetalle { get; set; }
    [Range(1, int.MaxValue)] public int IdBicicleta { get; set; }
    [Range(1, int.MaxValue)] public int Cantidad { get; set; }
    [Range(0, 9999999)] public decimal Precio { get; set; }
    public decimal Subtotal { get; set; }
    public BicicletaDto? Bicicleta { get; set; }
}
public class VentaDto
{
    public int IdVenta { get; set; }
    public DateTime Fecha { get; set; }
    [Range(1, int.MaxValue)] public int IdCliente { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public ClienteDto? Cliente { get; set; }
    [MinLength(1, ErrorMessage = "Agregue al menos una bicicleta.")]
    public List<DetalleVentaDto> Detalles { get; set; } = [];
}
