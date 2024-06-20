using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.ViewModels
{
    public class EditUserViewModel
    {
        public IEnumerable<Microsoft.AspNetCore.Identity.IdentityRole> Roles { get; set; }  //lista de Roles

        public string LoggedInUserId { get; set; } // Guarda o ID do utilizador Logado

        public string UserRole { get; set; }    //Role do utilizador Logado

        //Dados do utilizador logado
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }

    }
}
