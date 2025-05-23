using DataAccessLayer.Enums;

namespace Tabletennis.ViewModels
{
    public class PlayerUpdateViewModel
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly? Birthday { get; set; }
        public bool IsActive { get; set; }
    }
}
