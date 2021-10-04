using System;
namespace Imagegram.Models.ViewModels
{
    public class CommentViewModel
    {
        public CommentViewModel()
        {
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
