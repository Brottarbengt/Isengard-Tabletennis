using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class PlayerDTO
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly? Birthday { get; set; }

        public string FullName { get; set; } = string.Empty;
        public int? BirthYear => Birthday.HasValue && Birthday != DateOnly.MinValue
        ? Birthday.Value.Year
        : null;
    }
}
