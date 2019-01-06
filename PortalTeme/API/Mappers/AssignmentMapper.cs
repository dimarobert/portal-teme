using PortalTeme.API.Models.Assignments;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;

namespace PortalTeme.API.Mappers {

    public interface IAssignmentMapper {
        AssignmentDTO MapAssignment(Assignment assignment);
        Assignment MapAssignmentDTO(AssignmentDTO assignment);
    }

    public class AssignmentMapper : IAssignmentMapper {
        private readonly ICourseMapper courseMapper;

        public AssignmentMapper(ICourseMapper courseMapper) {
            this.courseMapper = courseMapper;
        }

        public AssignmentDTO MapAssignment(Assignment assignment) {
            return new AssignmentDTO {
                Id = assignment.Id,
                Course = courseMapper.MapCourseEdit(assignment.Course),
                Name = assignment.Name,
                Description = assignment.Description,

                DateAdded = assignment.DateAdded,
                StartDate = assignment.StartDate,
                LastUpdated = assignment.LastUpdated,
                EndDate = assignment.EndDate,
            };
        }

        public Assignment MapAssignmentDTO(AssignmentDTO assignment) {
            return new Assignment {
                Id = assignment.Id ?? Guid.Empty,
                Course = new Course {
                    Id = assignment.Course.Id ?? throw new Exception("No course was set for the assignment.")
                },
                Name = assignment.Name,
                Description = assignment.Description,

                DateAdded = assignment.DateAdded,
                StartDate = assignment.StartDate,
                LastUpdated = assignment.LastUpdated,
                EndDate = assignment.EndDate
            };
        }
    }
}
