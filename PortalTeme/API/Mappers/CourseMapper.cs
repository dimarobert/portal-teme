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
