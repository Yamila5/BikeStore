using BikeStore.DTOs;

namespace BikeStore.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> ObtenerTodosAsync();
        Task<ClienteDTO?> ObtenerPorIdAsync(int id);
        Task<ClienteDTO> CrearAsync(CrearClienteDTO dto);
        Task<bool> ActualizarAsync(int id, CrearClienteDTO dto);
        Task<bool> EliminarAsync(int id);
    }
}
