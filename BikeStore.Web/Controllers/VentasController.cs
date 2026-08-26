using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;
public class VentasController(BikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(int? idCliente)
    {
        ViewBag.Clientes = await api.GetListAsync<ClienteDto>("api/clientes");
        return View(await api.GetListAsync<VentaDto>(idCliente.HasValue ? $"api/ventas/cliente/{idCliente}" : "api/ventas"));
    }
    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new VentaDto { Detalles = [new DetalleVentaDto { Cantidad = 1 }] });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VentaDto model)
    {
        model.Detalles = model.Detalles.Where(d => d.IdBicicleta > 0 && d.Cantidad > 0).ToList();
        if (!ModelState.IsValid || model.Detalles.Count == 0) { ModelState.AddModelError("", "Complete al menos un producto válido."); await LoadLists(); return View(model); }
        var r = await api.SendAsync<VentaDto>(HttpMethod.Post, "api/ventas", model);
        if (r.Success) { TempData["Ok"] = "Venta registrada y stock actualizado."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError("", r.Error ?? "No se pudo registrar la venta."); await LoadLists(); return View(model);
    }
    public async Task<IActionResult> Details(int id) { var m = await api.GetAsync<VentaDto>($"api/ventas/{id}"); return m is null ? NotFound() : View(m); }
    private async Task LoadLists() { ViewBag.Clientes = await api.GetListAsync<ClienteDto>("api/clientes"); ViewBag.Bicicletas = await api.GetListAsync<BicicletaDto>("api/bicicletas"); }
}
