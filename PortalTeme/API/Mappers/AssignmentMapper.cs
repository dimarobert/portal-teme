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

        //StudentAssignedTaskDTO MapAssignmentEntryProjection(AssignmentEntryProjection entry);
        //StudentAssignedTask MapStudentAssignedTaskProjectionDTO(StudentAssignedTaskDTO studentAssignedTaskDto);
    }

    public class AssignmentMapper : IAssignmentMapper {
        private readonly ICourseMapper courseMapper;

        public AssignmentMapper(ICourseMapper courseMapper) {
            this.courseMapper = courseMapper;
        }

        public AssignmentDTO MapAssignment(Assignment assignment) {
            var dto = new AssignmentDTO();
            MapAssignmentInternal(dto, assignment);
            return dto;
        }

        public StudentAssignmentDTO MapStudentAssignment(Assignment assignment, User user) {
            var dto = new StudentAssignmentDTO();
            MapAssignmentInternal(dto, assignment);

            //var assignedTask = assignment.AssignmentTasks.FirstOrDefault(v => v.StudentId == user.Id);
            //if (!(assignedTask is null))
            //    dto.AssignedVariant = MapTask(assignedTask);

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

            dto.Tasks = assignment.AssignmentTasks.Select(variant => MapTask(variant)).ToList();
        }

        private AssignmentTaskDTO MapTask(AssignmentTask variant) {
            return new AssignmentTaskDTO {
                Id = variant.Id,
                Name = variant.Name,
                Description = variant.Description,
                //StudentId = variant.StudentId
            };
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

        //public StudentAssignedTaskDTO MapAssignmentEntryProjection(AssignmentEntryProjection entry) {
        //    return new StudentAssignedTaskDTO {
        //        Id = entry.Id ?? Guid.Empty,
        //        AssignmentTaskId = entry.AssignmentTaskId,
        //        CourseId = entry.CourseId,
        //        StudentId = entry.StudentId,
        //        State = entry.State,
        //        Grading = entry.Grading,
        //        Submissions = entry.Versions.Select(version => MapAssignmentEntryVersion(version)).ToList()
        //    };

        //}

        //public StudentAssignedTask MapStudentAssignedTaskProjectionDTO(StudentAssignedTaskDTO assignmentEntryDto) {
        //    return new StudentAssignedTask {
        //        //Id = assignmentEntryDto.Id ?? Guid.Empty,
        //        TaskId = assignmentEntryDto.AssignmentTaskId,
        //        StudentId = assignmentEntryDto.StudentId,
        //        Grading = assignmentEntryDto.Grading,
        //        State = assignmentEntryDto.State
        //    };
        //}


        private TaskSubmissionDTO MapAssignmentEntryVersion(TaskSubmission entryVersion) {
            return new TaskSubmissionDTO {
                Id = entryVersion.Id,
                DateAdded = entryVersion.DateAdded,
                Files = entryVersion.Files.Select(file => MapAssignmentEntryFile(file)).ToList()
            };
        }

        private AssignmentEntryFileDTO MapAssignmentEntryFile(TaskSubmissionFile file) {
            return new AssignmentEntryFileDTO {
                Id = file.Id,
                Name = file.Name,
                Description = file.Description,
                FileType = file.FileType
            };
        }
    }
}
