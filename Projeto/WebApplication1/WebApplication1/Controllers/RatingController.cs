using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebApplication1.Controllers
{
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [TempData]
        public string Message { get; set; }

        public RatingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId, int ratingValue)
        {
            //Se o valor inserido estiver fora de 1-5 então mostra erro
            if (ratingValue < 1 || ratingValue > 5)
            {
                return BadRequest("Invalid rating value.");
            }

            //Vai sucar os dados do user lgoado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Verifica se já existe uma avaliação do utilizador para o post
            var existingRating = await _context.Rating
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == user.Id);

            //Se existir já uma avaliação deste post feita por este user apenas atualiza aquela avaliação
            if (existingRating != null)
            {
                // Atualiza a avaliação existente
                existingRating.RatingValue = ratingValue;
                existingRating.UpdatedAt = DateTime.Now;
                _context.Update(existingRating);
            }
            else //Senão cria uma nova avaliação
            {
                var rating = new Rating
                {
                    PostId = postId,
                    UserId = user.Id,
                    RatingValue = ratingValue,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Add(rating);
            }

            await _context.SaveChangesAsync();
            Message = "Post rated successfully!";
            return RedirectToAction("Details", "Posts", new { id = postId });
        }
    }
}


