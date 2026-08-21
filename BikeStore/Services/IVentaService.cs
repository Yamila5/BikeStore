using BikeStore.DTOs;

namespace BikeStore.Services
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDTO>> ObtenerTodasAsync();
        Task<VentaDTO?> ObtenerPorIdAsync(int id);
        Task<VentaDTO> CrearAsync(CrearVentaDTO dto);
    }
}
