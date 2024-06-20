using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.ViewModels;

public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    [TempData]
    public string Message { get; set; }

    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    /* MÉTODO INDEX (index)*/
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        // Vai buscar a lista e retorna para a view passando a lista de users
        var users = _userManager.Users.ToList();
        return View(users);
    }

    /* MÉTODO EDIT */
    [Authorize]


    // Método para exibir o formulário de edição (edit)
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var roles = _roleManager.Roles.ToList();    //lista de roles
        var users = _userManager.Users.ToList();    //lista de users
        var user = await _userManager.FindByIdAsync(id);    //dados do utilizador logado
        var userRole = await _userManager.GetRolesAsync(user);  //role do utilizador logado

        //View model Para a página de edição do utilizador pretendido
        var model = new EditUserViewModel
        {
            Roles = roles,
            LoggedInUserId = _userManager.GetUserId(User),
            UserRole = userRole.FirstOrDefault(),
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber
        };


        if (user == null)
        {
            return NotFound();
        }


        return View(model);
    }


    // Método para processar os dados enviados pelo formulário (update)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]


    public async Task<IActionResult> Edit(string id, IdentityUser user, string roleSelected)
    {
        // Verifica se o id passado é diferente do id do user
        if (id != user.Id)
        {
            return NotFound();
        }

        // Verifica se o model é válido
        if (ModelState.IsValid)
        {
            // Procura (pelo id) o user que tem de ser editato
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Atualiza os dados do user

            //Atualiza o role do user
            var role = await _userManager.GetRolesAsync(existingUser);
            await _userManager.RemoveFromRolesAsync(existingUser, role);
            await _userManager.AddToRoleAsync(existingUser, roleSelected);

            // Atualiza os dados do user
            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            existingUser.PhoneNumber = user.PhoneNumber;

            // Atualiza o user
            var result = await _userManager.UpdateAsync(existingUser);

            // Verifica se a atualização foi bem sucedida
            if (result.Succeeded)
            {
                Message = $"User {user.UserName} edited successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(user);
    }

    /* MÉTODO DELETE */

    // Método para exibir a confirmação de exclusão
    [Authorize(Roles="Admin")]

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // Método para processar a exclusão do utilizador
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            Message = $"User {user.UserName} deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(user);
        }
    }
}
