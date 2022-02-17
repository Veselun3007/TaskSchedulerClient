using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using TaskSchedulerClient.Models;
using TaskSchedulerClient.OptionsModel;

namespace TaskSchedulerClient.Controllers
{
    /// <summary>
    /// Контролер, для роботи з авторизацією та реєстрацією
    /// </summary>

    public class AuthController : Controller
    {

        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region *** Login ***

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                CreteLoginUserObj(loginModel);
                return View(loginModel);
            }
            try
            {
                using var client = new HttpClient();
                var response = client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                    "/api/Auth/Auth", loginModel).Result;

                Dictionary<string, string> dictionaryResult = JsonConvert.DeserializeObject
                    <Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);

                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View(loginModel);
            }
        }

        private static object CreteLoginUserObj(LoginModel loginModel)
        {
            LoginModel entityObject = new()
            {
                UserName = loginModel.UserName,
                UserPassword = loginModel.UserPassword
            };
            return entityObject;
        }

        #endregion

        #region *** Register ***

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                CreteRegisterUserObj(registerModel);
                return View(registerModel);
            }
            try
            { 
                using var client = new HttpClient();
                var response = client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                        "/api/User/Post", registerModel).Result;

                GetToken(registerModel, client);
                return RedirectToAction("Index", "User");
            } 
            catch 
            (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View(registerModel);
            }
        }

        /// <summary>
        /// Метод, що отримує токен користувача 
        /// при реєстації для подальшої роботи
        /// </summary>
        /// <param name="registerModel"></param>
        /// <param name="client"></param>
        /// <returns>Повертає токен</returns>
        private Dictionary<string, string> GetToken(RegisterModel registerModel, HttpClient client)
        {
            LoginModel loginModel = CreatAuthUser(registerModel);

            var response = client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                "/api/Auth/Auth", loginModel).Result;

            Dictionary<string, string> dictionaryResult = JsonConvert.DeserializeObject
              <Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);

            return dictionaryResult;
        }

        private static LoginModel CreatAuthUser(RegisterModel registerModel)
        {
            return new LoginModel
            {
                UserName = registerModel.UserName,
                UserPassword = registerModel.UserPassword
            };
        }

        private static object CreteRegisterUserObj(RegisterModel registerModel)
        {
            RegisterModel entityObject = new()
            {
                UserName = registerModel.UserName,
                UserEmail = registerModel.UserEmail,
                UserPassword = registerModel.UserPassword
            };
            return entityObject;
        }
        
        #endregion

    }
}
