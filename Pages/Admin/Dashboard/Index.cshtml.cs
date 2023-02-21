using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WebRazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebRazor.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        public PRN221_DBContext db;
        public double wsale { get; set; }
        public long totalOrder { get; set; }
        public long totalCus { get; set; }
        public long totalGuest { get; set; }

        //[BindProperty(SupportsGet = true)]
        //public int yearChoose { get; set; }

        public IndexModel(PRN221_DBContext dBContext)
        {
            db = dBContext;
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return RedirectToPage("/Forbiden");
            }
            decimal wsales = 0;
            DateTime from = DateTime.Now.AddDays(-7);
            List<Models.Order> lo = db.Orders.Include(o => o.OrderDetails)
                .Where(o => o.ShippedDate >= from && o.ShippedDate <= DateTime.Now).ToList()
                ;

            lo.ForEach(o =>
            {
                List<Models.OrderDetail> lod = o.OrderDetails.ToList();
                wsales += lod.Sum(o => o.Quantity * o.UnitPrice * (1 - (decimal)o.Discount));
            });
            ViewData["totalO"] = db.Orders.Count();
            ViewData["totalC"] = db.Customers.Count();
            ViewData["totalG"] = db.Accounts.Count();
            ViewData["newC"] = db.Customers.Where(o => o.CreateDate >= DateTime.Now.AddMonths(-1)).Count();
            ViewData["wsales"] = wsales;

            Dictionary<int, decimal> longs = db.Orders.Include(o => o.OrderDetails).ToList()
                .GroupBy(o => o.OrderDate.Value.Month)
                .ToDictionary(e => e.Key, e => e.Sum(e => e.OrderDetails.Sum(e => e.UnitPrice * e.Quantity * (1 - (decimal)e.Discount))));

            ViewData["m1"] = GetTotalPriceOfOrderDetailsByMonth(1, longs);
            ViewData["m2"] = GetTotalPriceOfOrderDetailsByMonth(2, longs);
            ViewData["m3"] = GetTotalPriceOfOrderDetailsByMonth(3, longs);
            ViewData["m4"] = GetTotalPriceOfOrderDetailsByMonth(4, longs);
            ViewData["m5"] = GetTotalPriceOfOrderDetailsByMonth(5, longs);
            ViewData["m6"] = GetTotalPriceOfOrderDetailsByMonth(6, longs);
            ViewData["m7"] = GetTotalPriceOfOrderDetailsByMonth(7, longs);
            ViewData["m8"] = GetTotalPriceOfOrderDetailsByMonth(8, longs);
            ViewData["m9"] = GetTotalPriceOfOrderDetailsByMonth(9, longs);
            ViewData["m10"] = GetTotalPriceOfOrderDetailsByMonth(10, longs);
            ViewData["m11"] = GetTotalPriceOfOrderDetailsByMonth(11, longs);
            ViewData["m12"] = GetTotalPriceOfOrderDetailsByMonth(12, longs);
            return Page();
        }

        private decimal GetTotalPriceOfOrderDetailsByMonth(int month, Dictionary<int, decimal> listPrice)
        {
            foreach (var item in listPrice)
            {
                if (item.Key == month) return item.Value;
            }
            return 0;
        }
    }
}