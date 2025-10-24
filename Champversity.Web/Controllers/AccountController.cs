using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Champversity.DataAccess.Models;
using Champversity.Web.Models;

namespace Champversity.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
     _signInManager = signInManager;
_userManager = userManager;
      }

  [HttpGet]
     public IActionResult Login(string returnUrl = null)
      {
     ViewData["ReturnUrl"] = returnUrl;
return View();
  }

   [HttpPost]
  public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
   if (ModelState.IsValid)
     {
       var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
      if (result.Succeeded)
   {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
   {
         return Redirect(returnUrl);
       }
       return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
           }
      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
       return View(model);
    }

     [HttpPost]
      public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
     }

 public IActionResult AccessDenied()
  {
        return View();
        }
    }
}