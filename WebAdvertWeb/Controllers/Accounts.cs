using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using WebAdvertWeb.Models.Accounts;

namespace WebAdvertWeb.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CognitoSignInManager<CognitoUser> signInManager;
        private readonly CognitoUserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public AccountsController(CognitoSignInManager<CognitoUser> signInManager, CognitoUserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.pool = pool;
        }

        public IActionResult Signup()
        {
            var model = new SignUpModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                userManager.PasswordValidators.Clear();
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                   return RedirectToAction("Confirm");
                }
                else
                {
                    foreach (var identityError in result.Errors)
                    {
                        ModelState.AddModelError(identityError.Code, identityError.Description);
                    }
                }
            }

            return View();
        }

        public IActionResult Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmSend(ConfirmModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Confirm", model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("NotFound", "A user with given email was not found");
                return View("Confirm", model);
            }

            await signInManager.SignInAsync(user, true);                       
            var result = await userManager.ConfirmSignUpAsync(user, model.Code, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(item.Code, item.Description);
            }



            return View("Confirm", model);
        }

    }
}