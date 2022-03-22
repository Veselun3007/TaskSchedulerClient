using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskSchedulerClient.Models;
using TaskSchedulerClient.CryptographyMethods;

namespace TaskSchedulerClient.Controllers
{
    /// <summary>
    /// Контролер, для роботи з авторизацією та реєстрацією
    /// </summary>

    public class AuthController : Controller
    {

        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;
        private readonly Cryptography _cryptography;

        public AuthController(IConfiguration configuration, 
            Cryptography cryptography)
        {
            _configuration = configuration;
            _cryptography = cryptography;
        }

        #endregion

        private async Task GetAPIPublicKey()
        {
            using var client = new HttpClient();
            var response = client.GetAsync(_configuration["ConnectionAPI:Path"] +
                    "/api/User/GetPublicKey").Result;
            _configuration["APIkey"] = await response.Content.ReadAsStringAsync();
        }


        #region *** Login ***

       [HttpGet]
        public async Task<IActionResult> Login()
        {
            await GetAPIPublicKey();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                CreteLoginUserObj(loginModel);
                return View(loginModel);
            }
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = await LogIn(loginModel, client);

                Dictionary<string, string> dictionaryResult = ExtractToken(response);
                _configuration["JWTtoken"] = dictionaryResult["token"];

                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View(loginModel);
            }
        }

        private async Task<HttpResponseMessage> LogIn(LoginModel loginModel, HttpClient client)
        {
            return (await client.PostAsJsonAsync(
                _configuration["ConnectionAPI:Path"] + "/api/Auth/Auth", 
                (object)_cryptography.RSA_Encrypt_IUser(loginModel, _configuration["APIkey"])));
        }

        private static Dictionary<string, string> ExtractToken(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject
                <Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
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
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                CreteRegisterUserObj(registerModel);
                return View(registerModel);
            }
            try
            {
                using var client = new HttpClient();
                await UserRegister(registerModel, client);
                GetToken(registerModel, client);

                return RedirectToAction("Index", "User");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View(registerModel);
            }
        }

        private async Task UserRegister(RegisterModel registerModel, HttpClient client)
        {
            await client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] 
                + "/api/User/Post", registerModel);
        }

        /// <summary>
        /// Метод, що отримує токен користувача 
        /// при реєстації для подальшої роботи.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <param name="client"></param>
        /// <returns>Повертає токен</returns>
        private void GetToken(RegisterModel registerModel, HttpClient client)
        {
            LoginModel loginModel = CreatAuthUser(registerModel);

            var response = client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                "/api/Auth/Auth", loginModel).Result;

            Dictionary<string, string> dictionaryResult = JsonConvert.DeserializeObject
              <Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);

            _configuration["JWTtoken"] = dictionaryResult["token"];
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
