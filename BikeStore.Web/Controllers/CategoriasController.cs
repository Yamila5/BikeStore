using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;
public class CategoriasController(BikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index() => View(await api.GetListAsync<CategoriaDto>("api/categorias"));
    public IActionResult Create() => View(new CategoriaDto());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(CategoriaDto model) => await Save(HttpMethod.Post, "api/categorias", model, "Categoría registrada correctamente.");
    public async Task<IActionResult> Edit(int id) { var m = await api.GetAsync<CategoriaDto>($"api/categorias/{id}"); return m is null ? NotFound() : View(m); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id, CategoriaDto model) { if (id != model.IdCategoria) return BadRequest(); return await Save(HttpMethod.Put, $"api/categorias/{id}", model, "Categoría actualizada correctamente."); }
    public async Task<IActionResult> Delete(int id) { var m = await api.GetAsync<CategoriaDto>($"api/categorias/{id}"); return m is null ? NotFound() : View(m); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id) { var r = await api.SendAsync<object>(HttpMethod.Delete, $"api/categorias/{id}"); TempData[r.Success ? "Ok" : "Error"] = r.Success ? "Categoría eliminada." : r.Error; return RedirectToAction(nameof(Index)); }
    private async Task<IActionResult> Save(HttpMethod method, string url, CategoriaDto model, string ok) { if (!ModelState.IsValid) return View(model); var r = await api.SendAsync<CategoriaDto>(method, url, model); if (r.Success) { TempData["Ok"] = ok; return RedirectToAction(nameof(Index)); } ModelState.AddModelError("", r.Error ?? "No se pudo guardar."); return View(model); }
}
