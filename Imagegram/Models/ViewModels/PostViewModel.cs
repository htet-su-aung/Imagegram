using System;
using System.Collections.Generic;

namespace Imagegram.Models.ViewModels
{
    public class PostViewModel
    {
        public PostViewModel()
        {
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CommentCount { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
