using Autologinwithcookies.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Diagnostics;
//using MailKit.Net.Smtp;
//using MimeKit.Text;
using Org.BouncyCastle.Crypto.Macs;
using System.Diagnostics;


namespace Autologinwithcookies.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]

        public IActionResult Email(UserModel user)
        {
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            TempData["DataPass"] = OTP;

            var email = new MimeMessage();
             email.From.Add(MailboxAddress.Parse("reshmasekhar27@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Auto-Generated Password";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = TempData["DataToPass"] as string
            };

            using var smtp = new SmpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
            smtp.Authenticate("reshmasekhar27@gmail.com", "sjzdvjojkjvtmfnv");
            smtp.Send(email);
            smtp.Diconnect(true);
            return RedirectToAction("Pass");
        }

        public IActionResult Pass()
        {
            return View();
        }

        [HttpPost]

        public IActionResult pass(UserModel user)
        {
            string tempapass = TempData["DataToPass"] as string;

            if(tempapass != null)
            {
                if(user.Password  ==  tempapass && user.Username != null)
                {
                    CookieOptions options = new CookieOptions();
                    options.Secure = true;
                    options.Expires = DateTimeOffset.Now.AddMinutes(5);
                    HttpContext.Response.Cookies.Append("Cookie", user.Username.ToString(), options);
                    return RedirectToAction("Success");

                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        public IActionResult Success()
        {
            string Cookie = HttpContext.Request.Cookies["Cookie"];
            if(Cookie != null)
            {
                UserModel user = new UserModel();
                user.Username = Cookie;

                if(user.Username != null)
                {
                    ViewBag.Status = user.Username;
                    return View();
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Cookie");
            return View("Email");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}