using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Reports
{
    public class UserView
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$)", ErrorMessage = "Minium 6 Character & Must Contain Capital Letter, Small Letter, Numeric & Special Char")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Region { get; set; }
    }
}
