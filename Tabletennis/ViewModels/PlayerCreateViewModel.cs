using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tabletennis.ViewModels
{
    public class PlayerCreateViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters!")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "First name cannot start with a space!")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters!")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Last name cannot start with a space!")]
        public string LastName { get; set; } = String.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Must choose a gender!")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
