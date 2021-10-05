using System.Collections.Generic;
using System.Linq;

namespace BatchImport.v1
{
    public class OriginalService
    {
        public List<string> Errors { get; set; } = new List<string>();

        public void Validate(ContainerDto container)
        {
            //hashset to irregular keys
            var mastersHashSet = container.Courses.AsEnumerable<DomainModel>()
                  .Concat(container.Teachers)
                  .Concat(container.Campus)
                  .ToHashSet();

            foreach (var @class in container.Classes)
            {
                if (!@class.IsValid())
                {
                    Errors.Add($"Class { @class } is invalid");
                    continue;
                }

                // Making sure that the Topology is valid
                if (!mastersHashSet.TryGetValue(new TeacherDomainModel { SocialSecurityCode = @class.TeacherSocialSecurityCode }, out var teacherInstance) && teacherInstance.IsValid())
                    Errors.Add($"Class { @class } was discarded because of {teacherInstance}");                                

                if (!mastersHashSet.TryGetValue(new CourseDomainModel { CourseCode = @class.CourseCode, CampusCode = @class.CourseCode.Split('-')[1] }, out var courseInstance) && courseInstance.IsValid())
                    Errors.Add($"Class { @class } was discarded because of {courseInstance}");

                if (!mastersHashSet.TryGetValue(new CampusDomainModel { CampusCode = ((CourseDomainModel)courseInstance).CampusCode }, out var campusInstance) && campusInstance.IsValid())
                    Errors.Add($"Class { @class } was discarded because of {campusInstance}");
            }
        }
    }
}