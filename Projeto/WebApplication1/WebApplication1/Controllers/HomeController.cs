using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            //Vai buscar todos os posts e todos os users
            var posts = await _context.Post.ToListAsync();
            var users = _userManager.Users.ToList();

            //Ordena os posts por data de cração
            posts.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));

            /*Dependendo da quantidade de posts existente cria uma lista diferente, para nao dar erro na home page*/
            if(posts.Count == 0)
            {
                var modell = new CombinedViewModelUsersPosts
                {

                    Posts = posts,
                    Users = users,
                    LoggedInUserId = _userManager.GetUserId(User)   // get the id of the logged in user
                };

                return View(modell);
            }

            if(posts.Count == 1)
            {
                var post1 = posts[0];
                posts = new List<Post> { post1 };
            }
            else if (posts.Count == 2)
            {
                var post1 = posts[0];
                var post2 = posts[1];
                posts = new List<Post> { post1, post2 };
            }
            else if (posts.Count == 3)
            {
                var post1 = posts[0];
                var post2 = posts[1];
                var post3 = posts[2];
                posts = new List<Post> { post1, post2, post3 };
            }
            else
            {
                var post1 = posts[0];
                var post2 = posts[1];
                var post3 = posts[2];
                posts = new List<Post> { post1, post2, post3 };
            }

            var model = new CombinedViewModelUsersPosts
            {

                Posts = posts,
                Users = users,
                LoggedInUserId = _userManager.GetUserId(User)   // get the id of the logged in user

            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
