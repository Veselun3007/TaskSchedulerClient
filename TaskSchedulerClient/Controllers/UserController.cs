using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskSchedulerAPI.Models;
using TaskSchedulerClient.Models;

namespace TaskSchedulerClient.Controllers
{
    /// <summary>
    /// Контролер, для реалізації CRUD-операцій 
    /// над завданнями користувача
    /// </summary>
    public class UserController : Controller
    {

        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;
        private ICollection<Assignment> assignments;
        private IEnumerable<AssignmentEditModel> AssignmentEditModels { get; set; }

        public ICollection<Assignment> Assignments
        {
            get { return assignments; }
            set
            {
                assignments = value;
                AssignmentEditModels = assignments
                    .Select(e => (AssignmentEditModel)e).OrderBy(e => e.AssignmentName);
            }
        }

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;

            HttpClient client = ConnectToApi();
            Assignments = GetAll(client);
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

        #region *** Get All Assignment ***
        private List<Assignment> GetAll(HttpClient client)
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

        #region *** Action CRUD ***

        [HttpGet]
        public IActionResult Index()
        {
            return View(assignments);
        }

        #region *** Create ***

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AssignmentEditModel assignment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await AddData(assignment);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                return View(assignment);
            }
            return RedirectToAction("Index");
        }

        private static object CreateAssignmentUserObj(AssignmentEditModel assignment)
        {
            AssignmentEditModel entityObject = new()
            {
                AssignmentName = assignment.AssignmentName,
                AssignmentDescription = assignment.AssignmentDescription,
                AssignmentTime = assignment.AssignmentTime,
                AssignmentState = null,
                UserId = 0
            };
            return entityObject;
        }

        private async Task AddData(AssignmentEditModel assignment)
        {
            HttpClient client = ConnectToApi();

            CreateAssignmentUserObj(assignment);
            await PostAsync(assignment, client);
        }

        private async Task PostAsync(AssignmentEditModel assignment, HttpClient client)
        {
            await client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] + 
                "/api/Assignment/CreateAssignment", assignment);
        }

        #endregion

        #region *** Update ***

        public IActionResult Update(int id)
        {
            AssignmentEditModel assignmentEdit =
                AssignmentEditModels.First(e => e.AssignmentId == id);

            return View(assignmentEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AssignmentEditModel assignment)
        {

            if (!ModelState.IsValid)
            {
                return View(assignment);
            }
            try
            {
                await UpdateData(assignment);
            }
            catch (Exception)
            {
                return View(assignment);
            }
            return RedirectToAction("Index");
        }

        private async Task UpdateData(AssignmentEditModel assignment)
        {
            HttpClient client = ConnectToApi();

            var entityObj = Assignments.
                First(e => e.AssignmentId == assignment.AssignmentId);

            UpdateAssignmentUserObj(entityObj, assignment);
            await PutAsync(assignment, client);

        }

        private async Task PutAsync(AssignmentEditModel assignment, HttpClient client)
        {
            await client.PutAsJsonAsync(_configuration["ConnectionAPI:Path"] + 
                "/api/Assignment/UpdateAssignment", assignment);
        }

        private static void UpdateAssignmentUserObj(Assignment entityObj,
            AssignmentEditModel assignment)
        {
            entityObj.AssignmentName = assignment.AssignmentName;
            entityObj.AssignmentDescription = assignment.AssignmentDescription;
            entityObj.AssignmentTime = assignment.AssignmentTime;
            entityObj.AssignmentState = assignment.AssignmentState;
            entityObj.UserId = assignment.UserId;

        }

        #endregion

        #region *** Delete ***

        public IActionResult Delete(int id)
        {

            AssignmentEditModel assignment =
               AssignmentEditModels.First(e => e.AssignmentId == id);

            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            HttpClient client = ConnectToApi();
            await DeleteAsync(id, client);

            return RedirectToAction("Index");
        }

        private async Task DeleteAsync(string id, HttpClient client)
        {
            await client.DeleteAsync(_configuration["ConnectionAPI:Path"] + 
                $"/api/Assignment/DeleteAssignment/{id}");
        }

        #endregion

        #endregion

    }
}
