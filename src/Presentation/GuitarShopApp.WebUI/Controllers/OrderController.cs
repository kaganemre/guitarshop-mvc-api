using System.Globalization;
using GuitarShopApp.WebUI.ApiService;
using GuitarShopApp.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebUI.Controllers;

public class OrderController : Controller
{
    private readonly CartViewModel cart;
    private readonly OrderApiService _orderApiService;
    private readonly UserApiService _userApiService;
    public OrderController(CartViewModel cartService, 
    OrderApiService orderApiService,
    UserApiService userApiService)
    {
        cart = cartService;
        _orderApiService = orderApiService;
        _userApiService = userApiService;
    }
    public IActionResult Checkout()
    {
        return View(new OrderModel { Cart = cart });
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(OrderModel model, string email)
    {
        if (cart.Items.Count == 0)
        {
            ModelState.AddModelError("", "There are no items in your cart.");
        }

        if (ModelState.IsValid)
        {
            var user = await _userApiService.GetByEmail(email);
            
            model.Cart = cart;
            var payment = await ProcessPayment(model);
            
            if (payment.Status == "success")
            {
                var currentOrder = await _orderApiService.CreateAsync(model, user.Id.ToString(), cart.Items);
                cart.Clear();
                return RedirectToAction("Completed", new { OrderId = currentOrder.Id });
            }
            
            model.Cart = cart;
            return View(model);
        }
        else
        {
            model.Cart = cart;
            return View(model);
        }

    }
    private async Task<Payment> ProcessPayment(OrderModel model)
    {
        Options options = new Options();
        options.ApiKey = "";
        options.SecretKey = "";
        options.BaseUrl = "https://sandbox-api.iyzipay.com";

        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = new Random().Next(111111111, 999999999).ToString();
        request.Price = model?.Cart?.CalculateTotal().ToString("F2", CultureInfo.InvariantCulture);
        request.PaidPrice = model?.Cart?.CalculateTotal().ToString("F2", CultureInfo.InvariantCulture); ;
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.BasketId = "B67832";
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = model?.CartName;
        paymentCard.CardNumber = model?.CartNumber;
        paymentCard.ExpireMonth = model?.ExpirationMonth;
        paymentCard.ExpireYear = model?.ExpirationYear;
        paymentCard.Cvc = model?.Cvc;
        paymentCard.RegisterCard = 0;
        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = Guid.NewGuid().ToString();
        buyer.Name = model?.Name;
        buyer.Surname = "Doe";
        buyer.GsmNumber = model?.Phone;
        buyer.Email = model?.Email;
        buyer.IdentityNumber = "74300864791";
        buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        buyer.RegistrationDate = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss");
        buyer.RegistrationAddress = model?.AddressLine;
        buyer.Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        buyer.City = model?.City;
        buyer.Country = "Turkey";
        buyer.ZipCode = "34732";
        request.Buyer = buyer;

        Address shippingAddress = new Address();
        shippingAddress.ContactName = model?.Name;
        shippingAddress.City = model?.City;
        shippingAddress.Country = "Turkey";
        shippingAddress.Description = model?.AddressLine;
        shippingAddress.ZipCode = "34742";
        request.ShippingAddress = shippingAddress;

        Address billingAddress = new Address();
        billingAddress.ContactName = model?.Name;
        billingAddress.City = model?.City;
        billingAddress.Country = "Turkey";
        billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        billingAddress.ZipCode = "34742";
        request.BillingAddress = billingAddress;

        List<BasketItem> basketItems = new List<BasketItem>();

        foreach (var item in model?.Cart?.Items ?? Enumerable.Empty<CartItemModel>())
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                BasketItem basketItem = new BasketItem
                {
                    Id = $"{item.Product.Id}_{i}",
                    Name = item.Product.Name,
                    Category1 = item.Product.CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = item.Product.Price.ToString("F2", CultureInfo.InvariantCulture)
                };
                basketItems.Add(basketItem);
            }
        }



        request.BasketItems = basketItems;

        var payment = await Payment.Create(request, options);
        
        var request2 = new RetrievePaymentRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = payment.ConversationId,
            PaymentId = payment.PaymentId
        };

        var result = await Payment.Retrieve(request2, options);
        
        return payment;
    }
    public IActionResult Completed(int orderId)
    {
        return View(orderId);
    }
}