using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Order
{
    public class CancelOrderModel : PageModel
    {
        private readonly PRN221_DBContext _context;

        public CancelOrderModel(PRN221_DBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Order order = null;
            try
            {
                order = GetOrder((int)id);
                order.RequiredDate = null;
                _context.Attach(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetOrder(order.OrderId) == null)
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


        private Models.Order GetOrder(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderId == id);
        }
    }
}
