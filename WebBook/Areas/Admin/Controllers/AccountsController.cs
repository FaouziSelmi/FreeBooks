using Domin.Entity;
using Infrastructure.Data;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FreeBookDbContext _context;

        public AccountsController(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, FreeBookDbContext context)
        {
            _RoleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Roles()
        {
            var Model = new RolesViewModel 
            {
             NewRole = new NewRole(),
             Roles= _RoleManager.Roles.OrderBy(x=>x.Name).ToList()
            };
            return View(Model);
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
                        HttpContext.Session.SetString("msgType", "success");
                        HttpContext.Session.SetString("title", "تم الحفظ");
                        HttpContext.Session.SetString("msg", "تم حفظ مجموعة المستخدم بنجاح");
                     
                    }
                    else
                    { // not Succeeded 
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", "لم يتم الحفظ  ");
                        HttpContext.Session.SetString("msg", "خطأ لم يتم الحفظ مجموعة المستخدم");
                    }
                }
                //Update
                else 
                {
                    var RoleUpdate = await _RoleManager.FindByIdAsync(role.Id);
                    RoleUpdate.Id = Model.NewRole.RoleId;
                    RoleUpdate.Name = Model.NewRole.RoleName;
                    var Result = await _RoleManager.UpdateAsync(RoleUpdate);
                    if (Result.Succeeded)
                    {
                        // Succeeded
                        HttpContext.Session.SetString("msgType", "success");
                        HttpContext.Session.SetString("title", Resource.ResourceWeb.lbUpdate);
                        HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbUpdateMsgRole);
                    }
                    else
                    {
                        // Not Successeded
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.ResourceWeb.lbNotUpdate);
                        HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotUpdateMsgRole);
                    }
                }
            }
            // return View();
            return RedirectToAction("Roles");
        }
        public async Task<IActionResult> DeleteRole(string  Id)
        {
            var role = _RoleManager.Roles.FirstOrDefault(x => x.Id == Id);
         if ((await  _RoleManager.DeleteAsync(role)).Succeeded)
            {
                return RedirectToAction(nameof(Roles));
            }
            return RedirectToAction("Roles");
        }
        public IActionResult Registers()
        {
            var model = new RegisterViewModel
            {
                NewRegister = new NewRegister(),
                Roles = _RoleManager.Roles.OrderBy(x => x.Name).ToList(),
                // Users= _userManager.Users.OrderBy(x=>x.Name).ToList()
                Users = _context.VwUsers.OrderBy(x => x.Role).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registers(RegisterViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var file = HttpContext.Request.Form.Files;
                if (file.Count() > 0)
                {
                    string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                    var fileStream = new FileStream(Path.Combine(@"wwwroot/", Helper.PathSaveImageuser, ImageName), FileMode.Create);
                    file[0].CopyTo(fileStream);
                    model.NewRegister.ImageUser = ImageName;
                }
                else
                {
                    model.NewRegister.ImageUser = model.NewRegister.ImageUser;
                }
                var user = new ApplicationUser
                {
                    Id = model.NewRegister.Id,
                    Name = model.NewRegister.Name,
                    UserName = model.NewRegister.Email,
                    Email = model.NewRegister.Email,
                    ActiveUser = model.NewRegister.ActiveUser,
                    ImageUser = model.NewRegister.ImageUser
                };
                if (user.Id == null)
                {
                    //Create
                    user.Id = Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user, model.NewRegister.Password);// password crypté
                    if (result.Succeeded)
                    {
                        //Succsseded
                        var Role = await _userManager.AddToRoleAsync(user, model.NewRegister.RoleName);
                        if (Role.Succeeded)
                        {  //Succsseded add role to user
                            HttpContext.Session.SetString("msgType", "success");
                            HttpContext.Session.SetString("title", Resource.ResourceWeb.lbSave);
                            HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotSavedMsgUserRole);
                        }
                        else
                        {// Not Succsseded to add role to user
                            HttpContext.Session.SetString("msgType", "error");
                            HttpContext.Session.SetString("title", Resource.ResourceWeb.lbNotSaved);
                            HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotSavedMsgUser);
                        }
                    }
                    else 
                    {
                        //Not Succsseded to add user
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.ResourceWeb.lbNotSaved);
                        HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotSavedMsgUser);
                    }
                }
                else
                { //Update
                    var userUpdate = await _userManager.FindByIdAsync(user.Id);
                    userUpdate.Id = model.NewRegister.Id;
                    userUpdate.Name = model.NewRegister.Name;
                    userUpdate.UserName = model.NewRegister.Email;
                    userUpdate.Email = model.NewRegister.Email;
                    userUpdate.ActiveUser = model.NewRegister.ActiveUser;
                    userUpdate.ImageUser = model.NewRegister.ImageUser;
                    var result = await _userManager.UpdateAsync(userUpdate);
                    if (result.Succeeded)
                    {
                        var oldRole = await _userManager.GetRolesAsync(userUpdate);
                        await _userManager.RemoveFromRolesAsync(userUpdate, oldRole);
                        var AddRole = await _userManager.AddToRoleAsync(userUpdate, model.NewRegister.RoleName);
                        if (AddRole.Succeeded)
                        {
                            HttpContext.Session.SetString("msgType", "success");
                            HttpContext.Session.SetString("title", Resource.ResourceWeb.lbUpdate);
                            HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbUpdateMsgUserRole);
                        }
                        else
                        {
                            HttpContext.Session.SetString("msgType", "error");
                            HttpContext.Session.SetString("title", Resource.ResourceWeb.lbNotUpdate);
                            HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotUpdateMsgUserRole);
                        }
                    }
                    else
                    {
                        //Not Succsseded to update user
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.ResourceWeb.lbNotSaved);
                        HttpContext.Session.SetString("msg", Resource.ResourceWeb.lbNotSavedMsgUser);
                    }
                }

            }

            // return View();
            return RedirectToAction("Registers");
        }

        public async Task<IActionResult> DeleteUser(string userId)
        {
            var User = _userManager.Users.FirstOrDefault(x => x.Id == userId);

            if (User.ImageUser != null && User.ImageUser != Guid.Empty.ToString())
            {
                var PathImage = Path.Combine(@"wwwroot/", Helper.PathImageUser, User.ImageUser);
                if (System.IO.File.Exists(PathImage))
                    System.IO.File.Delete(PathImage);
            }
            if ((await _userManager.DeleteAsync(User)).Succeeded)
            {
                return RedirectToAction("Registers", "Accounts");
            }
            return RedirectToAction("Registers", "Accounts");
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
