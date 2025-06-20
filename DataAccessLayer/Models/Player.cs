﻿using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly? Birthday { get; set; }
        public bool IsActive { get; set; } = true;
        public int NumberOfWins { get; set; }
        public int NumberOfLosses { get; set; }
        public decimal PlayerWinRatio { get; set; }
        public int MatchesPlayed { get; set; }

        public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
    }
}
