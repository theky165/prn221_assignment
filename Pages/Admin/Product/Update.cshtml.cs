using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Product
{
    public class UpdateModel : PageModel
    {
        private readonly PRN221_DBContext _context;

        private readonly IHubContext<HubServer> hubContext;
        public UpdateModel(PRN221_DBContext context, IHubContext<HubServer> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category).FirstOrDefaultAsync(m => m.ProductId == id);

            if (Product == null)
            {
                return NotFound();
            }
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Product.UnitsInStock == null
                || Product.QuantityPerUnit == null
                || Product.UnitPrice == null
                || Product.UnitsOnOrder == null
                || Product.ReorderLevel == null)
            {
                ViewData["msg"] = "Please fill all information of product!";
                return Page();
            }
           
            try
            {
                _context.Attach(Product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("ReloadProduct");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
