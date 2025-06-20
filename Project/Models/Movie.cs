using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public int Year { get; set; }
        public string Autor { get; set; }
        public double Rating { get; set; }

        public string? Description { get; set; }

        // 🖼 Зображення у форматі байтів
        public byte[]? ImageData { get; set; }

        // MIME-тип (наприклад: image/jpeg, image/png)
        public string? ImageMimeType { get; set; }

        public string UserId {  get; set; }
        public IdentityUser User { get; set; }
    }
}
