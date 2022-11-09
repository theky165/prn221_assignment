using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Product
{
    public class DeleteModel : PageModel
    {
        private readonly PRN221_DBContext _context;

        private readonly IHubContext<HubServer> hubContext;
        public DeleteModel(PRN221_DBContext context, IHubContext<HubServer> hubContext)
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

            Product = await _context.Products.FindAsync(id);

            if (Product != null)
            {
                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("ReloadProduct");
            }

            return RedirectToPage("./Index");
        }
    }
}
