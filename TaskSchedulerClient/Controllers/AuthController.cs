using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskSchedulerClient.Models;
using TaskSchedulerClient.CryptographyMethods;
using Microsoft.AspNetCore.Mvc;
using TaskSchedulerClient.ErrorHandling;
using TaskShedulerDesktopClient.Data.Errors;

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
        public ErrorInfo ErrorInfo { get; set; } = new ErrorInfo();

        public AuthController(IConfiguration configuration, 
            Cryptography cryptography)
        {
            _configuration = configuration;
            _cryptography = cryptography;
        }

        #endregion

        #region *** Get public API key
        private async Task GetAPIPublicKey()
        {
            using var client = new HttpClient();
            var response = client.GetAsync(_configuration["ConnectionAPI:Path"] +
                    "/api/User/GetPublicKey").Result;
            _configuration["APIkey"] = await response.Content.ReadAsStringAsync();
        }
        #endregion

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
                _cryptography.RSA_KeyGenerate();
                HttpResponseMessage response = await LogIn(loginModel, client);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                Dictionary<string, string> dictionaryResult = ExtractToken(response);

                _configuration["JWTtoken"] = dictionaryResult["token"];
                return RedirectToAction("Index", "Assignment");
            }
            catch (ServerException ex)
            {
                ErrorInfo.SetServerErrors(ErrorInfo, ex.ResponseMessage);
                ModelState.AddModelError("UserName", ErrorInfo.ServerError);
                ModelState.AddModelError("UserPassword", ErrorInfo.ServerError);
            }
            return View(loginModel);
        }

        private async Task<HttpResponseMessage> LogIn(LoginModel loginModel, HttpClient client)
        {
            return await client.PostAsJsonAsync( 
                _configuration["ConnectionAPI:Path"] + "/api/Auth/Auth",
                _cryptography.RSAEncryptIUser(loginModel, _configuration["APIkey"]));
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
                _cryptography.RSA_KeyGenerate();
                await UserRegister(registerModel, client);
                await AuthorizationUser(registerModel, client);
                return RedirectToAction("Index", "Assignment");
            }
            catch (ServerException ex)
            {
                ErrorInfo.SetServerErrors(ErrorInfo, ex.ResponseMessage);
                ModelState.AddModelError("UserName", ErrorInfo.ServerError);
                ModelState.AddModelError("UserEmail", ErrorInfo.ServerError);
            }
            return View(registerModel);
        }

        private async Task AuthorizationUser(RegisterModel registerModel, HttpClient client)
        {
            LoginModel loginModel = CreatAuthUser(registerModel);
            HttpResponseMessage response = await SingUp(client, loginModel);
            Dictionary<string, string> dictionaryResult = ExtractToken(response);
            _configuration["JWTtoken"] = dictionaryResult["token"];
        }

        private async Task UserRegister(RegisterModel registerModel, HttpClient client)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                _configuration["ConnectionAPI:Path"] + "/api/User/CreateUser", 
                _cryptography.RSAEncryptIUser(registerModel, _configuration["APIkey"]));
            if (!response.IsSuccessStatusCode) throw new ServerException(response);
        }
        private async Task<HttpResponseMessage> SingUp(HttpClient client, LoginModel loginModel)
        {
            return await client.PostAsJsonAsync(
                  _configuration["ConnectionAPI:Path"] + "/api/Auth/Auth", loginModel);
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
                UserPassword = registerModel.UserPassword,
            };
            return entityObject;
        }

        #endregion

        /// <summary>
        /// Метод, що витягує токен з HTTP відповіді
        /// </summary>
        /// <param name="response">HttpResponseMessage</param>
        /// <returns>Словник значень отримані з HTTP відповіді</returns>
        private static Dictionary<string, string> ExtractToken(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject
                <Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
        }

    }
}
