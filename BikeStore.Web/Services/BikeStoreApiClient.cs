using System.Net;
using System.Net.Http.Json;
using BikeStore.Web.Models;

namespace BikeStore.Web.Services;

public class BikeStoreApiClient(HttpClient http)
{
    public async Task<List<T>> GetListAsync<T>(string url)
    {
        try
        {
            return await http.GetFromJsonAsync<List<T>>(url) ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            return await http.GetFromJsonAsync<T>(url);
        }
        catch (HttpRequestException)
        {
            return default;
        }
    }

    public async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null) request.Content = JsonContent.Create(body);
            using var response = await http.SendAsync(request);
            var message = response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
            var data = response.Content.Headers.ContentLength > 0 ? await response.Content.ReadFromJsonAsync<T>() : default;
            return new ApiResult<T>(response.IsSuccessStatusCode, data, message, response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new ApiResult<T>(false, default, "No se pudo conectar con el servidor API. Asegúrate de que el proyecto BikeStore (API) esté en ejecución.", HttpStatusCode.ServiceUnavailable);
        }
    }
}
public record ApiResult<T>(bool Success, T? Data, string? Error, HttpStatusCode StatusCode);

