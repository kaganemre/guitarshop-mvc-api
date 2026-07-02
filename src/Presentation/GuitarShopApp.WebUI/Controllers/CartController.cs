using AutoMapper;
using GuitarShopApp.Domain.Entities;
using GuitarShopApp.WebUI.ApiService;
using GuitarShopApp.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebUI.Controllers;

public class CartController : Controller
{
    private readonly ProductApiService _productApiService;
    public CartController(ProductApiService productApiService, CartViewModel cartService)
    {
        _productApiService = productApiService;
        Cart = cartService;
    }

    public CartViewModel? Cart { get; set; }
    public IActionResult Basket()
    {
        return View(Cart);
    }

    [HttpPost]
    public async Task<IActionResult> Basket(int Id)
    {
        var product = await _productApiService.GetById(Id);
        
        if (product != null)
        {
            Cart?.AddItem(product, 1);
        }

        return View(Cart);
    }
    public IActionResult Remove(int Id)
    {
        Cart?.RemoveItem(Cart.Items.First(p => p.Product.Id == Id).Product);
        return RedirectToAction("Basket");
    }
}