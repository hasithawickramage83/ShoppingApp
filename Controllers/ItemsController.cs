using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

public class ItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchString)
    {
        var itemsQuery = _context.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            itemsQuery = itemsQuery.Where(i => i.Name.Contains(searchString));
        }

        var items = await itemsQuery.ToListAsync();
        ViewData["CurrentFilter"] = searchString;

        return View(items);
    }

    // AddToCart action remains same
}