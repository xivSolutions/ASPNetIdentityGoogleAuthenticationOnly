using ASPNetIdentityGoogleAuthenticationOnly.IdentityConfig;
using ASPNetIdentityGoogleAuthenticationOnly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASPNetIdentityGoogleAuthenticationOnly.Controllers
{
    public class AuthenticationController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback", new { returnUrl = "/" })
            };

            //challenge
            Request.RequestContext.HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");

            //if we got here, the above challenge didn't handle it, return unauth.
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            //See if the user exists in our database
            var user = await UserManager.FindByEmailAsync(loginInfo.Email);

            if (user == null)
            {
                //user doesn't exist, so the user needs to be created
                user = new ApplicationUser
                {
                    UserName = loginInfo.Email,
                    Email = loginInfo.Email,
                    EmailConfirmed = true,
                    FirstName = loginInfo.ExternalIdentity.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.GivenName)).Value,
                    LastName = loginInfo.ExternalIdentity.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Surname)).Value
                };

                //create the user
                await UserManager.CreateAsync(user);
                //add the google login to the newly created user
                await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                //add user to roles if required here....
            }

            //if user logins doesn't contain Google, then add it
            if (!user.Logins.Any(x => x.LoginProvider.Equals("Google")))
                await UserManager.AddLoginAsync(user.Id, loginInfo.Login);

            //successfully authenticated with google, so sign them in to our app
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

            return RedirectToLocal(returnUrl);
        }

        public ActionResult LogOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}