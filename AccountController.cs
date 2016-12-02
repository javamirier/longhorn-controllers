using LonghornMusic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace LonghornMusic.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private AppSignInManager _signInManager;
        private AppUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(AppUserManager userManager, AppSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AppSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ActionResult MusicOwnedGet()
        {
            AppUser Customer = db.Users.Find(User.Identity.GetUserId());
            List<Song> songList = Customer.MusicOwned;
            return RedirectToAction("MusicOwned");
        }

        public ActionResult MusicOwned()
        {
            AppUser Customer = db.Users.Find(User.Identity.GetUserId());
            List<Song> songList = Customer.MusicOwned;
            return View(songList);

        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)//NOTE: User has been re-directed here from a page they're not authorized to see
            {
                return View("Error", new string[] { "Access Denied" });
            }
            AuthenticationManager.SignOut();  //this removes any old cookies hanging around
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //Send to UsersList
        [Authorize]
        public ActionResult UsersList()
        {
            //What the hell is this 
            //var usersInRole = db.Users.Where(m => m.Roles.RoleId == role.Id)).ToList();

            List<AppUser> AllUsers = new List<AppUser>();

            var query = from u in db.Users
                        select u;
            AllUsers = query.ToList();

            return View(AllUsers);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId, FName, LName, MI, Email, PhoneNumber, Address, City, State, Zip")] AppUser user, string FName, string LName, string MI, string Email, string PhoneNumber, string Address, string City, string State, string Zip)
        {
            if (ModelState.IsValid)
            {
                AppUser userToChange = db.Users.Find(user.Id);

                userToChange.FName = user.FName;
                userToChange.LName = user.LName;
                userToChange.MI = user.MI;
                userToChange.Email = user.Email;
                userToChange.PhoneNumber = user.PhoneNumber;
                userToChange.City = user.City;
                userToChange.State = user.State;
                userToChange.Zip = user.Zip;
                userToChange.Address = user.Address;

                db.Entry(userToChange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View("UserList");
        }

        //GET: Send to UserMenu
        [Authorize]
        public ActionResult UserMenu()
        {
            return View();
        }

        //GET: Manage Credit Cards
        [Authorize]
        public ActionResult CreditCardsGet()
        {
            return View("CreditCards");
        }
        //POST: Manage Credit Cards
        [Authorize]
        public ActionResult CreditCards(CreditCardsViewModel model)
        {
            //Change to use two string properties for credit card instead of a list 
            string id = User.Identity.GetUserId();
            AppUser user = db.Users.Find(id);
            string toAdd = model.CardNumber;
            if (user.CreditCard1 == null || user.CreditCard1 == "")
            {
                user.CreditCard1 = toAdd;
            }
            else if (user.CreditCard2 == null || user.CreditCard2 == "")
            {
                user.CreditCard2 = toAdd;
            }
            else
            {
                return RedirectToAction("UserMenu");
            }
            //Maybe it's on strike until it gets cost of living adjustments 
            //string nonsense = user.Id;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            //string bullshit = "Changes saved";
            return RedirectToAction("UserMenu");
        }

        //Send to AccountDetails
        [Authorize]
        public ActionResult UserDetailsGet()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            ViewBag.FName = user.FName;
            ViewBag.MI = user.MI;
            ViewBag.LName = user.LName;
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = user.PhoneNumber;
            ViewBag.CreditCard1 = user.CreditCard1;
            ViewBag.CreditCard1Type = user.CreditCard1Type;
            ViewBag.CreditCard2 = user.CreditCard2;
            ViewBag.CreditCard2Type = user.CreditCard2Type;
            ViewBag.Address = user.Address;
            ViewBag.Zip = user.Zip;
            ViewBag.City = user.City;
            ViewBag.State = user.State;

            return View("UserDetails");
        }

        //GET: Manage Credit Cards
        [Authorize]
        public ActionResult ChangeInformationGet()
        {
            return View("ChangeInformation");
        }

        //POST: Change Account Information
        //i believe
        [Authorize]
        public ActionResult ChangeInformation(ChangeInformationViewModel model)
        {
            string id = User.Identity.GetUserId();
            AppUser user = db.Users.Find(id);

            if (model.Email != null && model.Email != "")
            {
                user.Email = model.Email;
            }
            if (model.FName != null && model.FName != "")
            {
                user.FName = model.FName;
            }
            if (model.LName != null && model.LName != "")
            {
                user.LName = model.LName;
            }
            if (model.PhoneNumber != null && model.PhoneNumber != "")
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (model.Address != null && model.Address != "")
            {
                user.Address = model.Address;
            }
            if (model.City != null && model.City != "")
            {
                user.City = model.City;
            }
            if (model.State != null && model.State != "")
            {
                user.State = model.State;
            }
            if (model.Zip != null && model.Zip != "")
            {
                user.Zip = model.Zip;
            }
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("UserMenu");
        }


        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Add fields to user here so they will be saved to the database
                //Create a new user with all the properties you need for the class
                var user = new AppUser { UserName = model.Email, Email = model.Email, FName = model.FName, LName = model.LName, PhoneNumber = model.PhoneNumber, Address = model.Address, City = model.City, State = model.State, Zip = model.Zip };

                //Add the new user to the database
                var result = await UserManager.CreateAsync(user, model.Password);

                //TODO: Once you get roles working, you may want to add users to roles upon creation
                //await UserManager.AddToRoleAsync(user.Id, "User"); //adds user to role called "User"
                // --OR--
                //await UserManager.AddToRoleAsync(user.Id, "Employee"); //adds user to role called "Employee"

                if (result.Succeeded) //user was created successfully
                {
                    //sign the user in
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //send them to the home page
                    return RedirectToAction("Index", "Home");
                }

                //if there was a problem, add the error messages to what we will display
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }


        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        // GET: /Account/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}