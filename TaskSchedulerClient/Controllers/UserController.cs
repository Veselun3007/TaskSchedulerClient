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
        private readonly User user;
        private ICollection<Assignment> assignments;

        public ICollection<Assignment> Assignments
        {
            get { return assignments; }
            set { assignments = value; }
        }

        public UserController(IConfiguration configuration,
            Cryptography cryptography)
        {
            _configuration = configuration;
            _cryptography = cryptography;
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
                    AuthenticationHeaderValue("Bearer", _configuration["JWTtoken"]);

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

        public IActionResult PersonaAccount()
        {
            _cryptography.RSA_Decrypt_IUser(user);

            IndexModel viewModel = new()
            {
                Assignments = assignments,
                Users = user,
            };
            return View(viewModel);
        }

        #region *** Action CRUD User ***


        #region *** Update ***

        public async Task<IActionResult> UpdateUserObj(UserEditModel model)
        {
            try
            {
                HttpClient client = ConnectToApi();

                var user = GetUser(client);

                UpdateUserObject(user, model);
                await PutAsync(model, client);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        private static void UpdateUserObject(User user,
            UserEditModel model)
        {
            user.UserName = model.UserName;
            user.UserEmail = model.UserEmail;
            user.UserPassword = model.UserPassword;
        }

        private async Task PutAsync(UserEditModel user, HttpClient client)
        {
            await client.PutAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                "/api/User/UpdateUser", user);
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
    }
}
