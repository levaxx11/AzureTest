using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WebApplication1.Pages
{
    public class CommentsModel : PageModel
    {
        public class Comment
        {
            public string UserName { get; set; }
            public string Text { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;
        }

        private readonly string _filePath = "CommentStorage.txt";

        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string Text { get; set; }

        public List<Comment> Comments { get; set; } = new();

        public void OnGet()
        {
            LoadComments();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Text))
            {
                LoadComments();
                return Page();
            }

            var newComment = new Comment { UserName = UserName, Text = Text };
            var json = JsonSerializer.Serialize(newComment);
            System.IO.File.AppendAllLines(_filePath, new[] { json });

            return RedirectToPage();  
        }

        private void LoadComments()
        {
            if (System.IO.File.Exists(_filePath))
            {
                var lines = System.IO.File.ReadAllLines(_filePath);
                foreach (var line in lines)
                {
                    try
                    {
                        var comment = JsonSerializer.Deserialize<Comment>(line);
                        if (comment != null) Comments.Add(comment);
                    }
                    catch { }
                }
                Comments = Comments.OrderByDescending(c => c.Date).ToList();
            }
        }
    }
}
