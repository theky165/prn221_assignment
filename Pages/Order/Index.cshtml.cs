using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly PRN221_DBContext dbContext;

        public IndexModel(PRN221_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        

        [BindProperty]
        public Models.Account Auth { get; set; }

        public List<Models.Order> orderList;

        public void OnGet()
        {
            String jsonString = HttpContext.Session.GetString("CustSession");
            if (jsonString == null)
            {
                RedirectToPage("index");
            }
            else
            {
                var account = JsonSerializer.Deserialize<Models.Account>(jsonString);
                orderList = getAllOrder(account.CustomerId);
                ViewData["custName"] = account.Email;
            }

        }

        private List<Models.Order> getAllOrder(string customerID)
        {
            List<Models.Order> orders = dbContext.Orders.Select(o => new Models.Order
            {
                CustomerId = o.CustomerId,
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShippedDate = o.ShippedDate,
                RequiredDate = o.RequiredDate,
                ShipAddress = o.ShipAddress,
                Freight = o.Freight,
                ShipCity = o.ShipCity,
                ShipCountry = o.ShipCountry,
                ShipPostalCode = o.ShipPostalCode,
                ShipName = o.ShipName,
                ShipRegion = o.ShipRegion

            }).Where(o => o.CustomerId.Equals(customerID) && o.RequiredDate != null).ToList();
            foreach (Models.Order o in orders)
            {
                o.OrderDetails = dbContext.OrderDetails.Select(od => new OrderDetail
                {
                    OrderId = od.OrderId,
                    Discount = od.Discount,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Product = dbContext.Products.FirstOrDefault(p => p.ProductId == od.ProductId)
                }).Where(od => od.OrderId == o.OrderId).ToList();
            }

            return orders;
        }
    }
}
