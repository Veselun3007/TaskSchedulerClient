using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskSchedulerClient.Models;
using static TaskSchedulerClient.Models.SortingModel;

namespace TaskSchedulerClient.Controllers
{
    /// <summary>
    /// Контролер, для реалізації CRUD-операцій 
    /// над завданнями користувача
    /// </summary>
    public class AssignmentController : Controller
    {
        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;
        private IEnumerable<AssignmentEditModel> AssignmentEditModels { get; set; }
        private ICollection<Assignment> assignments;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly User user;

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


        public AssignmentController(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
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

        #region *** Action CRUD Assignments ***

        #region *** Detail ***

        public IActionResult Index(string searchName,
            string startDate, string endDate,
            SortingState sortOrder = SortingState.NameAsc)
        {

            IEnumerable<Assignment> newAssignments = from m in assignments
                                                     select m;
            newAssignments = SortingAssignment(sortOrder, newAssignments);
            newAssignments = FilterAssigments(startDate, endDate, searchName, newAssignments);

            IndexModel viewModel = new()
            {
                FilterViewModel = new FilterModel(startDate, endDate, searchName),
                SortViewModel = new SortingModel(sortOrder),
                Assignments = newAssignments,
                Users = user,
            };

            return View(viewModel);
        }

        #region *** Sort & Filter

        private static IEnumerable<Assignment> FilterAssigments(string startDate,
            string endDate, string searchName, IEnumerable<Assignment> assignment)
        {

            if (!String.IsNullOrEmpty(searchName))
            {
                assignment = assignment.Where(s => s.AssignmentName!
                .StartsWith(searchName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!String.IsNullOrEmpty(startDate))
            {
                assignment = assignment.Where(e => e.AssignmentTime >= DateTime.Parse(startDate));
            }
            if (!String.IsNullOrEmpty(endDate))
            {
                assignment = assignment.Where(e => e.AssignmentTime <= DateTime.Parse(endDate));
            }

            return assignment;
        }

        private static List<Assignment> SortingAssignment(
            SortingState sortOrder, IEnumerable<Assignment> assignment)
        {
            var assignments = sortOrder switch
            {
                SortingState.NameDesc => assignment.
                OrderByDescending(s => s.AssignmentName).ToList(),
                SortingState.DateAsc => assignment.
                OrderBy(s => s.AssignmentTime).ToList(),
                SortingState.DateDesc => assignment.
                OrderByDescending(s => s.AssignmentTime).ToList(),
                SortingState.StateAsc => assignment.
                OrderBy(s => s.AssignmentState).ToList(),
                SortingState.StateDesc => assignment.
                OrderByDescending(s => s.AssignmentState).ToList(),
                _ => assignment.OrderBy(s => s.AssignmentName).ToList(),
            };
            return assignments;
        }

        #endregion

        #endregion

        #region *** Create & Update ***
        public IActionResult Edit(Assignment assignment, int id)
        {
            if (id != 0)
            {
                assignment = Assignments.First(e => e.AssignmentId == id);

                return View(assignment);
            }
            else
                return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await SaveData(assignment);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            if (!ModelState.IsValid)
            {
                return View(assignment);
            }
            return RedirectToAction("Index");
        }
        private async Task SaveData(Assignment assignment)
        {
            if (assignment.AssignmentId == 0)
            {
                await AddData(assignment);
            }
            else
                await UpdateData(assignment);
        }

        #region *** Update ***
        private async Task UpdateData(Assignment assignment)
        {
            HttpClient client = ConnectToApi();
            UpdateAssignmentUserObj(assignment);
            await PutAsync(assignment, client);
        }
        private static object UpdateAssignmentUserObj(Assignment assignment)
        {
            Assignment entityObject = new()
            {
                AssignmentName = assignment.AssignmentName,
                AssignmentDescription = assignment.AssignmentDescription,
                AssignmentTime = assignment.AssignmentTime,
                AssignmentState = Convert.ToBoolean(assignment.AssignmentState),
                UserId = assignment.UserId,
            };
            return entityObject;
        }
        private async Task PutAsync(Assignment assignment, HttpClient client)
        {
            await client.PutAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                "/api/Assignment/UpdateAssignment", assignment);
        }      
        #endregion

        #region *** Create ***
        private async Task AddData(Assignment assignment)
        {
            HttpClient client = ConnectToApi();
            await CreateNewTask(assignment, client);
        }

        private async Task CreateNewTask(Assignment assignment, HttpClient client)
        {
            if (assignment.AssignmentDescription == null)
            {
                if (assignment.AssignmentTime <= DateTime.Now)
                {
                    Assignment entityObject = new()
                    {
                        AssignmentName = assignment.AssignmentName,
                        AssignmentDescription = "",
                        AssignmentTime = assignment.AssignmentTime,
                        AssignmentState = false,
                        UserId = assignment.UserId
                    };
                    await PostAsync(entityObject, client);
                }
                else
                {
                    Assignment entityObject = new()
                    {
                        AssignmentName = assignment.AssignmentName,
                        AssignmentDescription = "",
                        AssignmentTime = assignment.AssignmentTime,
                        AssignmentState = assignment.AssignmentState,
                        UserId = assignment.UserId
                    };
                    await PostAsync(entityObject, client);
                }
            }
            else
            {
                if (assignment.AssignmentTime <= DateTime.Now)
                {
                    Assignment entityObject = new()
                    {
                        AssignmentName = assignment.AssignmentName,
                        AssignmentDescription = assignment.AssignmentDescription,
                        AssignmentTime = assignment.AssignmentTime,
                        AssignmentState = false,
                        UserId = assignment.UserId
                    };
                    await PostAsync(entityObject, client);
                }
                else
                {
                    Assignment entityObject = new()
                    {
                        AssignmentName = assignment.AssignmentName,
                        AssignmentDescription = assignment.AssignmentDescription,
                        AssignmentTime = assignment.AssignmentTime,
                        AssignmentState = assignment.AssignmentState,
                        UserId = assignment.UserId
                    };
                    await PostAsync(entityObject, client);
                }
            }
        }

        private async Task PostAsync(Assignment assignment, HttpClient client)
        {
                await client.PostAsJsonAsync(_configuration["ConnectionAPI:Path"] +
                    "/api/Assignment/CreateAssignment", assignment);   
        }

        #endregion

        #endregion

        #region *** Delete ***

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            HttpClient client = ConnectToApi();
            await DeleteAsync(id, client);

            return RedirectToAction("Index");
        }
        private async Task DeleteAsync(int id, HttpClient client)
        {
            await client.DeleteAsync(_configuration["ConnectionAPI:Path"] +
                $"/api/Assignment/DeleteAssignment/{id}");
        }

        #endregion

        #endregion

        #region *** Additional Action ***

        #region *** LogOut ***

        [HttpGet]
        public IActionResult LogOut()
        {
            _httpContextAccessor.HttpContext.Session.Remove("token");
            return RedirectToAction("Login", "Auth");
        }

        #endregion

        #region *** Uptade State ***

        public async Task<IActionResult> UpdateState(AssignmentEditModel assignmentEditModel, int id)
        {
            try
            {
                HttpClient client = ConnectToApi();

                var entityObject = Assignments.First(e => e.AssignmentId == id);

                UpdateEntityObject(entityObject, assignmentEditModel);
                await PutAsync(entityObject, client);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(assignmentEditModel);
            }
            return RedirectToAction("Index");
        }

        private static void UpdateEntityObject(Assignment entityObject,
            AssignmentEditModel model)
        {
            entityObject.AssignmentName = entityObject.AssignmentName;
            entityObject.AssignmentDescription = entityObject.AssignmentDescription;
            entityObject.AssignmentTime = entityObject.AssignmentTime;
            entityObject.AssignmentState = model.AssignmentState;
            entityObject.UserId = entityObject.UserId;
        }
        #endregion

        #region *** Delete Multiple ***

        [HttpPost]
        public async Task DeleteSelected(int[] deleteList)
        {
            foreach (int id in deleteList)
            {
                var assignment = (from e in assignments
                                     where e.AssignmentId == id
                                     select e).FirstOrDefault();
                HttpClient client = ConnectToApi();
                await DeleteAsync(assignment.AssignmentId, client);
            }
        }
        #endregion

        #endregion
    }
}
