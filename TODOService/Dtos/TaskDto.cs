﻿using System.ComponentModel.DataAnnotations;

namespace TODOService.Dtos
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string TaskTitle { get; set; } = null!;
        [Required]
        public string TaskDescription { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
