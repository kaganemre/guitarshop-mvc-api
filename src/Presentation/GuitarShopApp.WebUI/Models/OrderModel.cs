using System.ComponentModel.DataAnnotations;
using GuitarShopApp.Domain.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GuitarShopApp.WebUI.Models;

public class OrderModel : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Phone { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    [BindNever]
    public CartViewModel? Cart { get; set; } = null!;

    public string? CartName { get; set; }
    public string? CartNumber { get; set; }
    public string? ExpirationMonth { get; set; }
    public string? ExpirationYear { get; set; }
    public string? Cvc { get; set; }

}