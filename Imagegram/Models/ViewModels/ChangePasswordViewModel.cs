using System;
namespace Imagegram.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {
        }

        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
