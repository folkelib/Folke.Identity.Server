using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Folke.Identity.Server.Views
{
    public class ChangeUserNameView
    {
        [Required]
        public string Username { get; set; }
    }
}
