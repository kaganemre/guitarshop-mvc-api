using AutoMapper;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.Application.Interfaces.Services;
using GuitarShopApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    public OrdersController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int? id)
    {
        return Ok(_mapper.Map<OrderDTO>(await _orderService.GetById(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDTO model)
    {
        var entity = _mapper.Map<Order>(model);
        await _orderService.CreateAsync(entity);
        model.Id = entity.Id;
        return CreatedAtAction(nameof(GetOrder), new { id = model.Id }, model);
    }
}