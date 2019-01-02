using PortalTeme.API.Models;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Mappers {

    public interface ICourseMapper {
        CourseDefinition MapDefinitionDTO(CourseDefinitionDTO dto, AcademicYear year);
        CourseDefinitionDTO MapDefinition(CourseDefinition model);

        AcademicYear MapYearDTO(AcademicYearDTO dto);
        AcademicYearDTO MapYear(AcademicYear model);

        StudyDomainDTO MapStudyDomain(StudyDomain domain);
        StudyDomain MapStudyDomainDTO(StudyDomainDTO dto);

        GroupDTO MapGroup(Group group);
        Group MapGroupDTO(GroupDTO dto, StudyDomain domain, AcademicYear year);
    }

    public class CourseMapper : ICourseMapper {
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
