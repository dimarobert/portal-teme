using PortalTeme.Data.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.Data.Models {
    public class CourseAssistant {

        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public string AssistantId { get; set; }
        public User Assistant { get; set; }
    }
}
