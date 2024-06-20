namespace WebApplication1.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; } = "default_user_id"; // Valor default
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Valor default
        public string ?Image { get; set; }

        public bool IsRestricted { get; set; } = false; // Valor default

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();


    }
}
