﻿using PortalTeme.API.Models;
using PortalTeme.API.Models.Courses;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Mappers {

    public interface ICourseMapper {

        AcademicYear MapYearDTO(AcademicYearDTO dto);
        AcademicYearDTO MapYear(AcademicYear model);

        StudyDomain MapStudyDomainDTO(StudyDomainDTO dto);
        StudyDomainDTO MapStudyDomain(StudyDomain domain);

        Group MapGroupDTO(GroupDTO dto, StudyDomain domain, AcademicYear year);
        GroupDTO MapGroup(Group group);

        CourseDefinition MapDefinitionDTO(CourseDefinitionDTO dto, AcademicYear year);
        CourseDefinitionDTO MapDefinition(CourseDefinition model);

        Course MapCourseEditDTO(CourseEditDTO dto);
        CourseViewDTO MapCourseView(Course course);
        CourseEditDTO MapCourseEdit(Course course);

        UserDTO MapUser(User user);
    }

    public class CourseMapper : ICourseMapper {
        public CourseViewDTO MapCourseView(Course course) {
            return new CourseViewDTO {
                Id = course.Id,
                CourseDef = MapCourseDefinitionRef(course.CourseInfo),
                Professor = MapUser(course.Professor),
                Assistants = course.Assistants.Select(assistant => MapAssistant(assistant)).ToList(),
                Groups = course.Groups.Select(group => MapGroupRef(group)).ToList(),
                Students = course.Students.Select(student => MapStudent(student)).ToList()
            };
        }

        public CourseEditDTO MapCourseEdit(Course course) {
            return new CourseEditDTO {
                Id = course.Id,
                CourseDef = MapCourseDefinitionRef(course.CourseInfo),
                Professor = MapUser(course.Professor)
            };
        }

        /// <summary>
        /// This will map one to one properties of the Course. The one to many lists must be updated separately.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Course MapCourseEditDTO(CourseEditDTO dto) {
            return new Course {
                Id = dto.Id ?? Guid.Empty,
                CourseInfo = new CourseDefinition {
                    Id = dto.CourseDef.Id
                },
                Professor = new User {
                    Id = dto.Professor.Id
                }
            };
        }

        public UserDTO MapUser(User user) {
            return new UserDTO {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        private CourseDefinitionRefDTO MapCourseDefinitionRef(CourseDefinition courseDef) {
            return new CourseDefinitionRefDTO {
                Id = courseDef.Id,
                Name = courseDef.Name
            };
        }

        private UserDTO MapAssistant(CourseAssistant assistant) {
            return new UserDTO {
                Id = assistant.AssistantId,
                FirstName = assistant.Assistant.FirstName,
                LastName = assistant.Assistant.LastName
            };
        }

        private static GroupRefDTO MapGroupRef(CourseGroup group) {
            return new GroupRefDTO {
                GroupId = group.GroupId,
                CourseId = group.CourseId,
                Name = group.Group.Name
            };
        }

        private static UserDTO MapStudent(CourseStudent student) {
            return new UserDTO {
                Id = student.StudentId,
                FirstName = student.Student.User.FirstName,
                LastName = student.Student.User.LastName
            };
        }


        public CourseDefinitionDTO MapDefinition(CourseDefinition model) {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new CourseDefinitionDTO {
                Id = model.Id,
                Year = model.Year.Id,
                Semester = model.Semester,
                Name = model.Name
            };
        }

        public CourseDefinition MapDefinitionDTO(CourseDefinitionDTO dto, AcademicYear year) {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new CourseDefinition {
                Id = dto.Id ?? Guid.Empty,
                Year = year,
                Semester = dto.Semester,
                Name = dto.Name
            };
        }

        public GroupDTO MapGroup(Group group) {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return new GroupDTO {
                Id = group.Id,
                Name = group.Name,
                Domain = group.Domain.Id,
                Year = group.Year.Id
            };
        }

        public Group MapGroupDTO(GroupDTO dto, StudyDomain domain, AcademicYear year) {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new Group {
                Id = dto.Id ?? Guid.Empty,
                Name = dto.Name,
                Domain = domain,
                Year = year
            };
        }

        public StudyDomainDTO MapStudyDomain(StudyDomain domain) {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            return new StudyDomainDTO {
                Id = domain.Id,
                Name = domain.Name
            };
        }

        public StudyDomain MapStudyDomainDTO(StudyDomainDTO dto) {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new StudyDomain {
                Id = dto.Id ?? Guid.Empty,
                Name = dto.Name
            };
        }

        public AcademicYearDTO MapYear(AcademicYear model) {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new AcademicYearDTO {
                Id = model.Id,
                Name = model.Name
            };
        }

        public AcademicYear MapYearDTO(AcademicYearDTO dto) {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new AcademicYear {
                Id = dto.Id ?? Guid.Empty,
                Name = dto.Name
            };
        }

    }
}
