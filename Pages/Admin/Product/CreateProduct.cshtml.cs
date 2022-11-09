using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Product
{
    public class CreateProductModel : PageModel
    {
        private readonly PRN221_DBContext _context;

        private readonly IHubContext<HubServer> hubContext;
        public CreateProductModel(PRN221_DBContext context, IHubContext<HubServer> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(Product.UnitsInStock == null 
                || Product.QuantityPerUnit == null
                || Product.UnitPrice == null)
            {
                ViewData["msg"] = "Please fill all information of product!";
                return Page(); 
            }


            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadProduct");

            return RedirectToPage("./Index");
        }
    }
}
