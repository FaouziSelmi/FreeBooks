using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly RoleManager<IdentityRole> _RoleManager;

        public AccountsController(RoleManager<IdentityRole> roleManager)
        {
            _RoleManager = roleManager;
        }
        public IActionResult Roles()
        {
            var Model = new RolesViewModel 
            {
             NewRole = new NewRole(),
             Roles= _RoleManager.Roles.OrderBy(x=>x.Name).ToList()
            };
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task <IActionResult> Roles(RolesViewModel Model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Id=Model.NewRole.RoleId,
                    Name=Model.NewRole.RoleName
                };
                //Create
                if(role.Id==null)
                {
                    role.Id = Guid.NewGuid().ToString();
                    var result=  await _RoleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        //Succeeded
                    }
                    else
                    { // not Succeeded 
                    }
                }
                //Update
                else 
                {
                }
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
