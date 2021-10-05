﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskoMask.Application.Core.ViewModels.Users;
using TaskoMask.Web.Area.Admin.Controllers;
using TaskoMask.Application.Core.Dtos.Users;
using TaskoMask.Web.Common.Controllers;
using TaskoMask.Web.Common.Services.Authentication.CookieAuthentication;
using TaskoMask.Application.Team.Members.Services;
using DNTCaptcha.Core;
using TaskoMask.Domain.Core.Models;
using AutoMapper;

namespace TaskoMask.Web.Controllers
{
    //TODO adding forget password- 2fa - external login
    public class AccountController : BaseMvcController
    {
        #region Fields

        private readonly IMemberService _memberService;
        private readonly ICookieAuthenticationService _cookieAuthenticationService;

        #endregion

        #region Ctors

       

        public AccountController(IMemberService memberService, ICookieAuthenticationService cookieAuthenticationService, IMapper mapper):base(mapper)
        {
            _memberService = memberService;
            _cookieAuthenticationService = cookieAuthenticationService;
        }


        #endregion

        #region Public Methods



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await _cookieAuthenticationService.SignOutAsync();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }



        /// <summary>
        ///  
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Please enter the text inside the image numerically",
                    CaptchaGeneratorLanguage = Language.English,
                    CaptchaGeneratorDisplayMode = DisplayMode.NumberToWord)]
        public async Task<IActionResult> Login(UserLoginViewModel input, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(input);


            //get user
            var userQueryResult = await _memberService.GetBaseUserByUserNameAsync(input.Email);
            if (!userQueryResult.IsSuccess)
                return View(userQueryResult, input);


            //validate user password
            var validateQueryResult = await _memberService.ValidateUserPasswordAsync(input.Email, input.Password);
            if (!validateQueryResult.IsSuccess || !validateQueryResult.Value)
                return View(userQueryResult, input);

            var user = _mapper.Map<AuthenticatedUser>(userQueryResult.Value);
            await _cookieAuthenticationService.SignInAsync(user, isPersistent: true);

            return RedirectToLocal(returnUrl);
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Please enter the text inside the image numerically",
                    CaptchaGeneratorLanguage = Language.English,
                    CaptchaGeneratorDisplayMode = DisplayMode.NumberToWord)]
        public async Task<IActionResult> Register(UserInputDto input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var cmdResult = await _memberService.CreateAsync(input);
            return View(cmdResult, input);
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _cookieAuthenticationService.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }


        #endregion

        #region Private Methods


        /// <summary>
        /// 
        /// </summary>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(actionName: nameof(DashboardController.Index), controllerName: "Dashboard", routeValues: new { Area = "admin" });
        }



        #endregion


    }
}
