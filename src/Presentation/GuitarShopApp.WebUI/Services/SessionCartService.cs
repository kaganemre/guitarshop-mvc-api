using System.Text.Json.Serialization;
using GuitarShopApp.WebUI.Models;

namespace GuitarShopApp.WebUI.Services;
public class SessionCartService : CartViewModel
{
    public static CartViewModel GetCart(IServiceProvider services)
    {
        ISession? session = services.CreateScope().ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
        SessionCartService cart = session?.GetJson<SessionCartService>("Cart") ?? new SessionCartService();
        cart.Session = session;
        return cart;
    }

    [JsonIgnore]
    public ISession? Session { get; set; }
    public override void AddItem(ProductViewModel product, int quantity)
    {
        base.AddItem(product, quantity);
        Session?.SetJson("Cart", this);
    }
    public override void RemoveItem(ProductViewModel product)
    {
        base.RemoveItem(product);
        Session?.SetJson("Cart", this);
    }
    public override void Clear()
    {
        base.Clear();
        Session?.Remove("Cart");
    }
}