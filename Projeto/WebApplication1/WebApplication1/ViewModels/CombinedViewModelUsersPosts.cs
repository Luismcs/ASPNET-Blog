namespace WebApplication1.ViewModels
{
    // Combina os todos os posts, todos os users, o Id do utilizador logado, e uma variável de AverageRating
    public class CombinedViewModelUsersPosts
    {
        public IEnumerable<WebApplication1.Models.Post> Posts { get; set; } //lista de Posts
        public IEnumerable<Microsoft.AspNetCore.Identity.IdentityUser> Users { get; set; }  //lista de Users

        public string LoggedInUserId { get; set; } // Guarda o ID do utilizador logado

        public double AverageRating { get; set; }  // Nota média do post

        public double UserRating { get; set; }

    }
}
