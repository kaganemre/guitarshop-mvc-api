using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using GuitarShopApp.WebUI.ApiService;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.WebUI.Models;

namespace GuitarShopApp.WebUI.Controllers;

[Authorize(Roles = "admin")]
public class HomeController : Controller
{
    private readonly ProductApiService _productApiService;
    private readonly CategoryApiService _categoryApiService;
    public HomeController(ProductApiService productApiService, CategoryApiService categoryApiService)
    {
        _productApiService = productApiService;
        _categoryApiService = categoryApiService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await _productApiService.GetAll());
    }

    [AllowAnonymous]
    public async Task<IActionResult> List(string category)
    {
        ViewBag.Categories = await _categoryApiService.GetAll();
        ViewBag.SelectedCategory = RouteData?.Values["category"];

        return View(await _productApiService.GetProductsByCategory(category));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        
        await CategoryList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _productApiService.CreateAsync(model);
                return RedirectToAction("List");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        await CategoryList();
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var model = await _productApiService.GetById(id);
        await CategoryList();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = await _productApiService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                await _productApiService.UpdateAsync(model);
                return RedirectToAction("List");
            }
            catch (Exception)
            {
                throw;
            }

        }
        
        await CategoryList();
        return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> Delete([FromBody] ProductDTO model)
    {
        var entity = await _productApiService.GetById(model.Id);

        if (entity != null)
        {
            await _productApiService.DeleteAsync(entity);
        }

        return Json(new { model.Id });
    }
    private async Task<string> AddImage(IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName);
        var randomFileName= string.Format($"{Guid.NewGuid()}{extension}");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
        using var stream = new FileStream(path, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return randomFileName;
    }
    private async Task CategoryList()
    {
        ViewBag.Categories = new SelectList(await _categoryApiService.GetAll(), "Id", "Name");
    }
    


}