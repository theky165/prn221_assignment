using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazor.Pages.Admin
{
    public class ExportModel : PageModel
    {

        public IActionResult OnGet(string? dataTable, DateTime? dateFrom, DateTime? dateTo)
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(dataTable))
            {
                return BadRequest();
            }

            if (!dataTable.Equals("product") && (dateFrom == null || dateTo == null
                || (dateFrom == null && dateTo == null)))
            {
                TempData["msg"] = "Date is invalid!";
                return Redirect("Order");
            }

            string fileName;
            if (dataTable.Equals("product"))
            {
                fileName = "ProductExport.xlsx";
            }
            else
            {
                string dateF = dateFrom.Value.ToString("dd/MM/yyyy");
                string dateT = dateTo.Value.ToString("dd/MM/yyyy");
                fileName = "OrderExport_" + dateF + "_To_" + dateT + ".xlsx";
            }
            var data = Helper.ExportFile.ExportExcelFile(dataTable, dateFrom, dateTo);
            data.Position = 0;

            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
