using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

[Authorize]
[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Show cart items
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToAction("Index", "Items");

        var cartItems = await _context.CartItems
            .Include(c => c.Item)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }

    // Add item to cart (optional: can also be in ItemsController)
    [HttpPost]
    public async Task<IActionResult> Add(int itemId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToAction("Index", "Items");

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId);

        if (cartItem == null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ItemId = itemId,
                Quantity = 1
            });
        }
        else
        {
            cartItem.Quantity++;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // Remove item from cart
    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // Update quantity of an item in cart
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        if (quantity < 1) quantity = 1;

        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // Helper method to get current user ID
    private string? GetCurrentUserId()
    {
        return User.Identity?.IsAuthenticated == true ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
    }
}