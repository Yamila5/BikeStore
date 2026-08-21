using BikeStore.DTOs;

namespace BikeStore.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> ObtenerTodasAsync();
        Task<CategoriaDTO?> ObtenerPorIdAsync(int id);
        Task<CategoriaDTO> CrearAsync(CrearCategoriaDTO dto);
        Task<bool> ActualizarAsync(int id, CrearCategoriaDTO dto);
        Task<bool> EliminarAsync(int id);

    }
}
