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

namespace Culinaria.Controllers
{
    public class HomeController : Controller
    {
        private Uri baseAdress = new Uri("https://localhost:44328/api/recipes");

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public async Task<IActionResult> IndexAsync(string? searchBar)
        {
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

            }
            
            return View(list);
        }


        public async Task<IActionResult> Recipe(int id)
        {
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
                    //var resp = response.Content.ReadAsStringAsync();
                    //resp.Wait();
                    //recipe = JsonConvert.DeserializeObject<Recipe>(resp.Result);
                    //recipe = await response.Content.ReadAsAsync<Recipe>();
                    var responseDoc = response.Content.ReadAsStringAsync().Result;
                    recipe = JsonConvert.DeserializeObject<Recipe>(responseDoc);
                }
            }

            if (recipe == null)
            {
                return NotFound();
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
                    //var resp = response.Content.ReadAsStringAsync();
                    //resp.Wait();
                    //recipe = JsonConvert.DeserializeObject<Recipe>(resp.Result);
                    //recipe = await response.Content.ReadAsAsync<Recipe>();
                    var responseDoc = response.Content.ReadAsStringAsync().Result;
                    recipe = JsonConvert.DeserializeObject<Recipe>(responseDoc);
                }
            }

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
    }
}
