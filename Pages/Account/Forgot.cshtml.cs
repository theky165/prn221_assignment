using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Helper;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221_DBContext dbContext;

        public ForgotModel(PRN221_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public string email { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrEmpty(email))
            {
                ViewData["msg"] = "Please enter your email to get password!";
                return Page();
            }

            var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Email.Equals(email));
            if (account == null)
            {
                ViewData["msg"] = "Not found email in for system, please check again!";
                return Page();
            }

            String passwordGenerate = AccountHelper.GeneratePassword(8);
            account.Password = AccountHelper.HashPassWord(passwordGenerate);
            
            dbContext.Update(account);
            if (await dbContext.SaveChangesAsync() <= 0)
            {
                ViewData["msg"] = "System error, please try again!";
                return Page();
            }
            
            string bodyMail = "After login, please change your password! You new password is: " + passwordGenerate;

            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(email);
            mailMessage.Subject = "[Reset Password]";
            mailMessage.Body = bodyMail;
            mailMessage.IsBodyHtml = false;
            mailMessage.From = new MailAddress("conmuangangqua200x@gmail.com");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("******************", "*****");
            await smtp.SendMailAsync(mailMessage);
            ViewData["msg"] = "Please check you email!";
            return Page();
        }
    }
}
