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
    }

    public class CourseMapper : ICourseMapper {
        public CourseDefinitionDTO MapDefinition(CourseDefinition model) {
            return new CourseDefinitionDTO {
                Id = model.Id,
                Year = model.Year.Id,
                Semester = model.Semester,
                Name = model.Name
            };
        }

        public CourseDefinition MapDefinitionDTO(CourseDefinitionDTO dto, AcademicYear year) {
            return new CourseDefinition {
                Id = dto.Id,
                Year = year,
                Semester = dto.Semester,
                Name = dto.Name
            };
        }

        public AcademicYearDTO MapYear(AcademicYear model) {
            return new AcademicYearDTO {
                Id = model.Id,
                Name = model.Name
            };
        }

        public AcademicYear MapYearDTO(AcademicYearDTO dto) {
            return new AcademicYear {
                Id = dto.Id ?? Guid.Empty,
                Name = dto.Name
            };
        }
    }
}
