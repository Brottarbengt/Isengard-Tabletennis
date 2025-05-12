using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters!")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "First name cannot start with a space!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters!")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "First name cannot start with a space!")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="Must choose a gender!")]
        public Gender Gender { get; set; }
        [Required]
        public DateOnly Birthday { get; set; }

        public virtual ICollection<PlayerMatch> PlayerMatches { get; set; }
    }
}
