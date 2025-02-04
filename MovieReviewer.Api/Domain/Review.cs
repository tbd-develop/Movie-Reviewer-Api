﻿using System.ComponentModel.DataAnnotations;

namespace MovieReviewer.Api.Domain
{
    public class Review : BaseEntity
    {
        public required string ReviewContent { get; set; }
        public int ReviewScore { get; set; }
        public int MovieId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDisabled { get; set; } = false;
    }
}
