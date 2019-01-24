using PortalTeme.API.Models.Assignments;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortalTeme.API.Mappers {

    public interface IAssignmentMapper {
        AssignmentDTO MapAssignment(Assignment assignment);
        StudentAssignmentDTO MapStudentAssignment(Assignment assignment, User user);

        Assignment MapAssignmentEditDTO(AssignmentEditDTO assignment);
    }

    public class AssignmentMapper : IAssignmentMapper {
        private readonly ICourseMapper courseMapper;
        private readonly ITaskMapper taskMapper;

        public AssignmentMapper(ICourseMapper courseMapper, ITaskMapper taskMapper) {
            this.courseMapper = courseMapper;
            this.taskMapper = taskMapper;
        }

        public AssignmentDTO MapAssignment(Assignment assignment) {
            var dto = new AssignmentDTO();
            MapAssignmentInternal(dto, assignment);
            return dto;
        }

        public StudentAssignmentDTO MapStudentAssignment(Assignment assignment, User user) {
            var dto = new StudentAssignmentDTO();
            MapAssignmentInternal(dto, assignment);

            var assignedTask = assignment.AssignmentTasks.FirstOrDefault(t => t.StudentsAssigned.Any(sa => sa.StudentId == user.Id));
            if (!(assignedTask is null))
                dto.AssignedTask = taskMapper.MapTask(assignedTask);

            return dto;
        }

        private void MapAssignmentInternal(AssignmentDTO dto, Assignment assignment) {
            dto.Id = assignment.Id;
            dto.Course = courseMapper.MapCourseEdit(assignment.Course);
            dto.Name = assignment.Name;
            dto.Slug = assignment.Slug;

            dto.Type = assignment.Type;
            dto.NumberOfDuplicates = assignment.NumberOfDuplicates;

            dto.Description = assignment.Description;

            dto.DateAdded = assignment.DateAdded;
            dto.StartDate = assignment.StartDate;
            dto.LastUpdated = assignment.LastUpdated;
            dto.EndDate = assignment.EndDate;

            dto.Tasks = assignment.AssignmentTasks.Select(task => taskMapper.MapTask(task)).ToList();
        }

        public Assignment MapAssignmentEditDTO(AssignmentEditDTO assignment) {
            return new Assignment {
                Id = assignment.Id ?? Guid.Empty,
                Course = new Course {
                    Id = assignment.Course.Id
                },
                Name = assignment.Name,

                Type = assignment.Type,
                NumberOfDuplicates = assignment.NumberOfDuplicates ?? 1,

                Description = assignment.Description,

                DateAdded = assignment.DateAdded,
                StartDate = assignment.StartDate,
                LastUpdated = assignment.LastUpdated,
                EndDate = assignment.EndDate
            };
        }

    }
}
