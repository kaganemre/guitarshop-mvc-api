using GuitarShopApp.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebUI.Components;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly CartViewModel cart;
    public CartSummaryViewComponent(CartViewModel cartService)
    {
        cart = cartService;
    }

    public IViewComponentResult Invoke()
    {
        return View(cart);
    }
}