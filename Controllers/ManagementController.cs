using Culinaria.Areas.Identity.Data;
using Culinaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Culinaria.Controllers
{
    public class ManagementController : Controller
    {
        private readonly SignInManager<CulinariaUser> _signInManager;
        private readonly UserManager<CulinariaUser> _userManager;

        public ManagementController(UserManager<CulinariaUser> userManager, SignInManager<CulinariaUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível encontrar o utilizador com o ID '{_userManager.GetUserId(User)}'.");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfile(CulinariaUser user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            currentUser.Name = user.Name;
            await _userManager.UpdateAsync(currentUser);
            await _signInManager.RefreshSignInAsync(currentUser);
            ViewBag.StatusMessage = "O seu perfil foi alterado com sucesso!";
            return View("Index", currentUser);
        }

        public IActionResult Password()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível encontrar o utilizador com o ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Password");
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewBag.StatusMessage = "A sua Palavra-Chave foi alterada com sucesso!";

            return View("Password");
        }

        public async Task<IActionResult> MyRecipes()
        {
            List<Recipe> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44328/api/recipes?ownerId=" + _userManager.GetUserId(User));
                HttpResponseMessage response = null;
                response = await client.GetAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    list = await response.Content.ReadAsAsync<List<Recipe>>();
                }
                else
                {
                    return BadRequest();
                }

            }
            return View(list);
        }
    }
}
