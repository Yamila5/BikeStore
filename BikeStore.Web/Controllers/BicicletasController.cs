using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;
public class BicicletasController(BikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? marca, string? modelo, int? idCategoria)
    {
        ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias");
        var query = $"api/bicicletas/buscar?marca={Uri.EscapeDataString(marca ?? "")}&modelo={Uri.EscapeDataString(modelo ?? "")}" + (idCategoria.HasValue ? $"&idCategoria={idCategoria}" : "");
        return View(await api.GetListAsync<BicicletaDto>(query));
    }
    public async Task<IActionResult> Create() { ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(new BicicletaDto()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BicicletaDto model)
    {
        if (!ModelState.IsValid) { ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(model); }
        var r = await api.SendAsync<BicicletaDto>(HttpMethod.Post, "api/bicicletas", model);
        if (r.Success) { TempData["Ok"] = "Bicicleta registrada correctamente."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError("", r.Error ?? "No se pudo registrar la bicicleta."); ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(model);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var model = await api.GetAsync<BicicletaDto>($"api/bicicletas/{id}"); if (model is null) return NotFound();
        ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BicicletaDto model)
    {
        if (id != model.IdBicicleta) return BadRequest();
        if (!ModelState.IsValid) { ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(model); }
        var r = await api.SendAsync<BicicletaDto>(HttpMethod.Put, $"api/bicicletas/{id}", model);
        if (r.Success) { TempData["Ok"] = "Bicicleta actualizada correctamente."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError("", r.Error ?? "No se pudo actualizar."); ViewBag.Categorias = await api.GetListAsync<CategoriaDto>("api/categorias"); return View(model);
    }
    public async Task<IActionResult> Delete(int id) { var model = await api.GetAsync<BicicletaDto>($"api/bicicletas/{id}"); return model is null ? NotFound() : View(model); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    { var r = await api.SendAsync<object>(HttpMethod.Delete, $"api/bicicletas/{id}"); if (r.Success) { TempData["Ok"] = "Bicicleta eliminada."; return RedirectToAction(nameof(Index)); } TempData["Error"] = r.Error; return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> StockBajo() => View(await api.GetListAsync<BicicletaDto>("api/bicicletas/stock-bajo"));
}
