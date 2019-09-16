using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocelot.Models;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Ocelot.Service;
using Ocelot.Logging;
using Ocelot.SQLHELPER;

namespace Ocelot.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account4
        [System.Web.Http.HttpGet]
       // [OcelotAuthorizationAttribute(Roles = "LOGIN")]
        public ActionResult Index()
        {
            return View();
        }
        [System.Web.Http.HttpGet]
       // [OcelotAuthorizationAttribute(Roles = "LOGIN")]
        public ActionResult Register()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "LOGIN")]
        public ActionResult Login(LoginModel login)
        {
            if (AccountService.IsUser(login) && AccountService.IsLoggedIn(login))
            {
                // loggen in
                var _signIn = new SignInService();
                _signIn.IdentitySignin(AccountService.User(login.Email));
                OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} signed in.", "AccountCOntroller", "Login");
                return RedirectToAction("../Vendor/Index");
            }
            else
            {
                @TempData["LOGIN"] = "LOGIN";
                @TempData["message"] = "Invalid email or password";
                @TempData["email"] = login.Email;
                OcelotLog.ErrorLogs($"{login.Email} at {DateTime.Now} attempted signed in.", "AccountCOntroller", "Login");
                return RedirectToAction("Index");
            }
            
        }
        [OcelotAuthorizationAttribute(Roles = "LOGIN")]
        [System.Web.Mvc.HttpPost]
        public ActionResult Register(UsersListModel register)
        {
            if(register.Password != register.ConfirmPassword)
            {
                @TempData["FirstName"] = register.FirstName;
                @TempData["LastName"] = register.LastName;
                @TempData["Email"] = register.Email;
                @TempData["PhoneNumber"] = register.PhoneNumber;
                @TempData["REGISTER"] = "REGISTER";
                @TempData["message"] = "Passwords dont match";

                return RedirectToAction("Register");
            }
            var mes = UsersSQL.AddUser(register);
            if(mes.Status == "success")
            {
                return RedirectToAction("../Home/Index");
            }
            else
            {

                @TempData["FirstName"] = register.FirstName;
                @TempData["LastName"] = register.LastName;
                @TempData["Email"] = register.Email;
                @TempData["PhoneNumber"] = register.PhoneNumber;
                @TempData["REGISTER"] = "REGISTER";
                @TempData["message"] = "Failed! Check details and then retry";

                return RedirectToAction("Register");
            }

        }
        [OcelotAuthorizationAttribute(Roles = "LOGIN")]
        public ActionResult LogOut()
        {
            var _singIn = new SignInService();
            _singIn.IdentitySignout();
            OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} signed out.", "AccountCOntroller", "Login");
            return RedirectToAction("Index");
        }
    }
}