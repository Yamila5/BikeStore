using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;
public class ClientesController(BikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? cedula, string? apellido) => View(await api.GetListAsync<ClienteDto>($"api/clientes/buscar?cedula={Uri.EscapeDataString(cedula ?? "")}&apellido={Uri.EscapeDataString(apellido ?? "")}"));
    public IActionResult Create() => View(new ClienteDto());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(ClienteDto model) => await Save(HttpMethod.Post, "api/clientes", model, "Cliente registrado correctamente.");
    public async Task<IActionResult> Edit(int id) { var m = await api.GetAsync<ClienteDto>($"api/clientes/{id}"); return m is null ? NotFound() : View(m); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id, ClienteDto model) { if (id != model.IdCliente) return BadRequest(); return await Save(HttpMethod.Put, $"api/clientes/{id}", model, "Cliente actualizado correctamente."); }
    public async Task<IActionResult> Delete(int id) { var m = await api.GetAsync<ClienteDto>($"api/clientes/{id}"); return m is null ? NotFound() : View(m); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id) { var r = await api.SendAsync<object>(HttpMethod.Delete, $"api/clientes/{id}"); TempData[r.Success ? "Ok" : "Error"] = r.Success ? "Cliente eliminado." : r.Error; return RedirectToAction(nameof(Index)); }
    private async Task<IActionResult> Save(HttpMethod method, string url, ClienteDto model, string ok) { if (!ModelState.IsValid) return View(model); var r = await api.SendAsync<ClienteDto>(method, url, model); if (r.Success) { TempData["Ok"] = ok; return RedirectToAction(nameof(Index)); } ModelState.AddModelError("", r.Error ?? "No se pudo guardar."); return View(model); }
}
