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
            
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>(false, default, contentString, response.StatusCode);
            }

            T? data = default;
            if (!string.IsNullOrWhiteSpace(contentString) && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                try
                {
                    data = System.Text.Json.JsonSerializer.Deserialize<T>(contentString, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (System.Text.Json.JsonException)
                {
                    data = default;
                }
            }

            return new ApiResult<T>(true, data, null, response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new ApiResult<T>(false, default, "No se pudo conectar con el servidor API. Asegúrate de que el proyecto BikeStore (API) esté en ejecución.", HttpStatusCode.ServiceUnavailable);
        }
    }
}
public record ApiResult<T>(bool Success, T? Data, string? Error, HttpStatusCode StatusCode);

