using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly PRN221_DBContext dbContext;

        public IndexModel(PRN221_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Category> Categories { get; set; }
        public List<Models.Product> products { get; set; }

        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int categoryChoose { get; set; }
        public int totalPages { get; set; }

        public const int pageSize = 5;
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            int totalOrders = getTotalProducts();

            totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            products = getAllProducts();

            Categories = dbContext.Categories.ToList();
            return Page();

        }

        private List<Models.Product> getAllProducts()
        {
            var list = (from p in dbContext.Products.Include(c => c.Category) select p).ToList();

            List<Models.Product> products = new List<Models.Product>();

            if (categoryChoose > 0 && !String.IsNullOrEmpty(search))
            {
                products = list.Where(p => p.CategoryId == categoryChoose && p.ProductName.ToLower().Contains(search.ToLower()))
                    .Skip((currentPage - 1) * pageSize).Take(pageSize)
                    .ToList();
            }
            else if (categoryChoose < 1 && !String.IsNullOrEmpty(search))
            {
                products = list.Where(p => p.ProductName.ToLower().Contains(search.ToLower()))
                    .Skip((currentPage - 1) * pageSize).Take(pageSize)
                    .ToList();
            }
            else if (categoryChoose > 0 && String.IsNullOrEmpty(search))
            {
                products = list.Where(p => p.CategoryId == categoryChoose)
                   .Skip((currentPage - 1) * pageSize).Take(pageSize)
                   .ToList();
            }
            else
            {
                products = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }

            //foreach (var product in products)
            //{
            //    product.Category = dbContext.Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            //}
            return products;
        }

        private int getTotalProducts()
        {
            var list = (from p in dbContext.Products select p).ToList();
            if (categoryChoose > 0 && !String.IsNullOrEmpty(search))
            {
                return list.Where(p => p.CategoryId == categoryChoose && p.ProductName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            else if (categoryChoose < 1 && !String.IsNullOrEmpty(search))
            {
                return list.Where(p => p.ProductName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            else if (categoryChoose > 0 && String.IsNullOrEmpty(search))
            {
                return list.Where(p => p.CategoryId == categoryChoose).ToList().Count;
            }
            else
            {
                return list.Skip((currentPage - 1) * pageSize).ToList().Count;
            }
        }
    }
}
