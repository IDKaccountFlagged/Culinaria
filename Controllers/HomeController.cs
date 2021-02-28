using Culinaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Culinaria.Areas.Identity.Data;

namespace Culinaria.Controllers
{
    public class HomeController : Controller
    {
        private readonly Uri baseAdress = new Uri("https://localhost:44328/api/recipes");

        private readonly UserManager<CulinariaUser> _userManager;

        public HomeController(UserManager<CulinariaUser> userManager)
        {
            _userManager = userManager;

        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync(string? searchBar, int? n)
        {
            if (n == 0)
            {
                ViewBag.StatusMessage = "Receita criada com sucesso!";
            }

            List<Recipe> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = this.baseAdress;
                HttpResponseMessage response = null;
                if (!String.IsNullOrEmpty(searchBar))
                {
                    response = await client.GetAsync(client.BaseAddress + "?search=" + searchBar);
                } else
                {
                    response = await client.GetAsync(client.BaseAddress);
                }
          
                if (response.IsSuccessStatusCode)
                {
                    list = await response.Content.ReadAsAsync<List<Recipe>>();
                }
                else
                {
                    return BadRequest();
                }

            }


            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAdmin = IsAdmin().Result;
            }

            return View(list);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Recipe(int id, int? n)
        {
            if (n == 0)
            {
                ViewBag.StatusMessage = "Receita editada com sucesso!";
            }
            Recipe recipe = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = this.baseAdress;
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var responseDoc = response.Content.ReadAsStringAsync().Result;
                    recipe = JsonConvert.DeserializeObject<Recipe>(responseDoc);
                }
                else
                {
                    return BadRequest();
                }
            }

            if (recipe == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAdmin = IsAdmin().Result;
            }

            return View(recipe);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsOwnerAsync(id))
            {
                return Forbid();
            }
            using (var client = new HttpClient())
                {
                    client.BaseAddress = this.baseAdress;
                    HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress + "/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    return View();

                }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await IsOwnerAsync(id))
            {
                return Forbid();
            }

            Recipe recipe = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = this.baseAdress;
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var responseDoc = response.Content.ReadAsStringAsync().Result;
                    recipe = JsonConvert.DeserializeObject<Recipe>(responseDoc);
                }
            }

            var user = await _userManager.GetUserAsync(User);
            if (recipe == null)
            {
                return NotFound();
            }

            

            return View(recipe);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task<bool> IsOwnerAsync(int recipeId)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = await _userManager.IsInRoleAsync(user, "Admin");
            string ownerId = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = this.baseAdress;
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "/" + recipeId);
                if (response.IsSuccessStatusCode)
                {
                    var responseDoc = response.Content.ReadAsStringAsync().Result;
                    ownerId = JsonConvert.DeserializeObject<Recipe>(responseDoc).OwnerId;
                }
            }

            return user.Id == ownerId | role;
        }

        private async Task<bool> IsAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}
