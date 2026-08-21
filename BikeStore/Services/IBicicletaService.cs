using BikeStore.DTOs;

namespace BikeStore.Services
{
    public interface IBicicletaService
    {
        Task<IEnumerable<BicicletaDTO>> ObtenerTodasAsync();
        Task<BicicletaDTO?> ObtenerPorIdAsync(int id);
        Task<BicicletaDTO> CrearAsync(CrearBicicletaDTO dto);
        Task<bool> ActualizarAsync(int id, CrearBicicletaDTO dto);
        Task<bool> EliminarAsync(int id);
    }
}
