using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TaskSchedulerClient.CryptographyMethods;
using TaskSchedulerClient.Models;

namespace TaskSchedulerClient.Controllers
{
    public class UserController : Controller
    {
        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;
        private readonly Cryptography _cryptography;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly User user;
        private ICollection<Assignment> assignments;

        public ICollection<Assignment> Assignments
        {
            get { return assignments; }
            set { assignments = value; }
        }

        public UserController(IConfiguration configuration,
            Cryptography cryptography, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _cryptography = cryptography;
            _httpContextAccessor = httpContextAccessor;
            HttpClient client = ConnectToApi();
            Assignments = GetAllAssignment(client);
            user = GetUser(client);
        }

        #endregion

        #region *** Connect To API ***
        private HttpClient ConnectToApi()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.
                    AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Session.GetString("token"));

            return client;
        }
        #endregion

        #region *** GetUser ***
        private User GetUser(HttpClient client)
        {

            client.DefaultRequestHeaders.Add("userPublicKey",
                _configuration["PublicKey"]);

            var response = client.GetAsync(_configuration["ConnectionAPI:Path"] +
                "/api/User/GetUser").Result;

            var result = response.Content.ReadAsStringAsync().Result;

            User user = JsonConvert.
                DeserializeObject<User>(result);
            return user;

        }
        #endregion

        #region *** Get All Assignment ***
        private List<Assignment> GetAllAssignment(HttpClient client)
        {
            var response = client.GetAsync(_configuration["ConnectionAPI:Path"] +
                "/api/Assignment/GetAllAssignments").Result;

            var result = response.Content.
                ReadAsStringAsync().Result;

            List<Assignment> assignments = JsonConvert.
                DeserializeObject<List<Assignment>>(result);
            return assignments;
        }
        #endregion

        #region *** Action CRUD User ***
        public IActionResult PersonalAccount()
        {
            _cryptography.RSA_Decrypt_IUser(user);

            IndexModel viewModel = new()
            {
                Assignments = assignments,
                Users = user,
            };
            return View(viewModel);
        }

        #region *** Update ***

        public IActionResult UpdateUser(User User, int id)
        {
            _cryptography.RSA_Decrypt_IUser(user);
            User = user;
            return View(User);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateData(user);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            return RedirectToAction("PersonalAccount");
        }

        private async Task UpdateData(User user)
        {
            HttpClient client = ConnectToApi();
            CreateUpdateUserObj(user);
            await PutAsync(user, client);
        }

        private static object CreateUpdateUserObj(User user)
        {
            User entityObject = new()
            {
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPassword = user.UserPassword,
            };
            return entityObject;
        }

        private async Task PutAsync(User user, HttpClient client)
        {
            await client.PutAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                "/api/User/UpdateUser", _cryptography.RSAEncryptIUser(user, _configuration["APIkey"]));
        }
        #endregion

        #region *** Delete ***

        [HttpGet]
        public async Task<IActionResult> DeleteUser()
        {
            HttpClient client = ConnectToApi();
            await DeleteUserAsync(client);

            return Redirect("~/Home/Index");
        }

        private async Task DeleteUserAsync(HttpClient client)
        {
            await client.DeleteAsync(_configuration["ConnectionAPI:Path"] +
                $"/api/User/DeleteUser");
        }

        #endregion

        #endregion

        #region *** LogOut ***

        [HttpGet]
        public IActionResult LogOut()
        {
            _httpContextAccessor.HttpContext.Session.Remove("token");
            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}
