using System;
using System.Collections.Generic;
using System.IO; // Adicionado para trabalhar com o sistema de arquivos
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Adicionado para trabalhar com IFormFile
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /*TEMP DATA VARIABLE*/
        [TempData]        
        public string Message { get; set; }

        public PostsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString, int? searchMonth, int? searchYear)
        {
            //Vai buscar todos os posts e todos os users
            var posts = from p in _context.Post
                        select p;

            var users = _userManager.Users.ToList();

            //--FILTOS--
            //1- Filtra por titulo
            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Title.Contains(searchString));
            }

            //2- Filtra por mês
            if (searchMonth.HasValue)
            {
                posts = posts.Where(s => s.CreatedAt.Month == searchMonth.Value);
            }

            //3- Filtra por ano
            if (searchYear.HasValue)
            {
                posts = posts.Where(s => s.CreatedAt.Year == searchYear.Value);
            }

            //Combina tudo num model para enviar para a Lista de Posts
            var model = new CombinedViewModelUsersPosts
            {
                Posts = posts,
                Users = users,
                LoggedInUserId = _userManager.GetUserId(User)

            };

            return View(model);
        }


        // GET: MyPosts
        [Authorize]

        public async Task<IActionResult> MyPosts()
        {
            //Vai buscar todos os posts
            var posts = from p in _context.Post
                        select p;

            //Vai buscar todos os users
            var users = _userManager.Users.ToList();

            //Vai buscar os dados do user logado
            var user = await _userManager.GetUserAsync(User);

            //Posts passa a ser apenas os posts do utilizador logado
            posts = posts.Where(s => s.UserId == user.Id);

            //Combina tudo e envia para a view MyPosts
            var model = new CombinedViewModelUsersPosts
            {
                Posts = posts,
                Users = users,
                LoggedInUserId = _userManager.GetUserId(User)
            };

            return View(model);
        }

        // GET: Posts/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtém o post pelo id que vem pela rota
            var post = await _context.Post.FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Obtém todos os utilizadores
            var users = _userManager.Users.ToList();

            // Obtém todas as notas
            var ratings = _context.Rating.ToList();

            //faz a média das avaliações deste post
            double averageRating = 0;
            int totalRatings = 0;
            int userRating = 0;
            foreach (var rating in ratings)
            {
                if (rating.PostId == id)
                {
                    averageRating += rating.RatingValue;
                    totalRatings++;
                }
                if(rating.PostId == id && rating.UserId == _userManager.GetUserId(User))
                {
                    userRating = rating.RatingValue;
                }
            }

            if (totalRatings > 0)
            {
                averageRating = averageRating / totalRatings;
            }

            // Combina tudo e envia para a view
            var model = new CombinedViewModelUsersPosts
            {
                Posts = new List<Post> { post }, // Apenas o post específico
                Users = users,
                LoggedInUserId = _userManager.GetUserId(User),
                AverageRating = averageRating,
                UserRating = userRating
            };

            return View(model);
        }


        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,UserId,CreatedAt,IsRestricted")] Post post, IFormFile image)
        {
            if (ModelState.IsValid)
            {

                //Vai buscar os dados do utilizador logado
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }

                // Se o utilizador logado não for admin, o post não é restrito
                if (!User.IsInRole("Admin"))
                {
                    post.IsRestricted = false;
                }

                post.UserId = user.Id;
                post.CreatedAt = DateTime.Now;

                // Verifica se uma imagem foi fornecida
                if (image != null && image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/posts", fileName);

                    // Copia a imagem para o diretório
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    post.Image = "/images/posts/" + fileName;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                Message= "Post created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,IsRestricted")] Post post, IFormFile ?image) //Only accepts Title, Content and Image
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica se o post que veio pela rota existe
                    var existingPost = await _context.Post.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    // Atualiza os valores do post para os novos valores que o utilizador inseriu
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.IsRestricted = post.IsRestricted;

                    // Verifica se a imagem foi alterada
                    if(image == null && existingPost.Image != null)
                    {
                        existingPost.Image = existingPost.Image;
                    }

                    // Verifica se a imagem foi fornecida e guarda-a na pasta de imagens
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/posts/", fileName);

                        // Copia a nova imagem para o diretório
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        existingPost.Image = "/images/posts/" + fileName;
                    }

                    _context.Entry(existingPost).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Message = "Post edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
                await _context.SaveChangesAsync();
            }
            Message = "Post deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
