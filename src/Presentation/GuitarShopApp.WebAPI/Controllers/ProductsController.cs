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
public class ProductsController(IProductService productService, IMapper mapper) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetAll();
        return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetById(id);
        return Ok(_mapper.Map<ProductDTO>(product));
    }
    
    [HttpGet("get-by-category/{name?}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(string name="")
    {
        var products = await _productService.GetProductsByCategory(name);
        return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductDTO model)
    {
        var created = await _productService.CreateAsync(_mapper.Map<Product>(model));
        var result = _mapper.Map<ProductDTO>(created);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(ProductDTO model)
    {
        await _productService.GetById(model.Id);
        await _productService.UpdateAsync(_mapper.Map<Product>(model));
        return Ok(model);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int? id)
    {
        _productService.Delete(await _productService.GetById(id));
        return NoContent();
    }
}