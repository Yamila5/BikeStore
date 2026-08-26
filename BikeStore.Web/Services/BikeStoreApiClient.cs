using System.Net;
using System.Net.Http.Json;
using BikeStore.Web.Models;

namespace BikeStore.Web.Services;

public class BikeStoreApiClient(HttpClient http)
{
    public async Task<List<T>> GetListAsync<T>(string url) => await http.GetFromJsonAsync<List<T>>(url) ?? [];
    public async Task<T?> GetAsync<T>(string url) => await http.GetFromJsonAsync<T>(url);

    public async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await http.SendAsync(request);
        var message = response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
        var data = response.Content.Headers.ContentLength > 0 ? await response.Content.ReadFromJsonAsync<T>() : default;
        return new ApiResult<T>(response.IsSuccessStatusCode, data, message, response.StatusCode);
    }
}
public record ApiResult<T>(bool Success, T? Data, string? Error, HttpStatusCode StatusCode);
