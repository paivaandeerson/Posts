using System;
using System.Collections.Generic;

namespace BatchImport
{
    //Stores all records read
    public struct ContainerDto
    {
        const string ClassType = "CLASS";
        const string CourseType = "COURSE";
        const string TeacherType = "TEACHER";
        const string CampusType = "CAMPUS";
        const string CsvSeparator = ",";
        const string CourseSeparator = "-";
        const string ValueSeparator = "+";

        internal ICollection<ClassDomainModel> Classes { get; set; }
        internal ICollection<TeacherDomainModel> Teachers { get; set; }
        internal ICollection<CourseDomainModel> Courses { get; set; }
        internal ICollection<CampusDomainModel> Campus { get; set; }

        public static ContainerDto ContainerFactory(IEnumerable<string> coursesCsv)
        {
            var container = new ContainerDto
            {
                Campus = new HashSet<CampusDomainModel>(),
                Classes = new HashSet<ClassDomainModel>(),
                Teachers = new HashSet<TeacherDomainModel>(),
                Courses = new List<CourseDomainModel>()
            };
            
            var firstLine = true;

            foreach (var line in coursesCsv)
            {
                if (firstLine || line is "")
                {
                    firstLine = false;
                    continue;
                }

                var currentValue = line.Split(CsvSeparator);

                switch (currentValue[0].ToUpperInvariant())
                {
                    case TeacherType:
                        {
                            var teacher = new TeacherDomainModel
                            {
                                SocialSecurityCode = currentValue[1].Trim(),
                                Name = currentValue[2].Trim()
                            };
                            if (!container.Teachers.Contains(teacher))
                                container.Teachers.Add(teacher);
                        }
                        break;

                    case CampusType:
                        {
                            var campus = new CampusDomainModel
                            {
                                CampusCode = currentValue[1].Trim(),
                                Name = currentValue[2].Trim()
                            };
                            if (!container.Campus.Contains(campus))
                                container.Campus.Add(campus);
                        }
                        break;

                    case ClassType:
                        {
                            var courseAndTeacher = currentValue[1].Trim().Split(ValueSeparator);
                            var @class = new ClassDomainModel
                            {
                                CourseCode = courseAndTeacher[0].Trim(),
                                TeacherSocialSecurityCode = courseAndTeacher[1].Trim(),
                                Name = currentValue[2].Trim(),
                                StartTime = currentValue[3].Trim(),
                            };
                            if (!container.Classes.Contains(@class))
                                container.Classes.Add(@class);
                        }
                        break;

                    case CourseType:
                        {
                            var campusAndCourse = currentValue[1].Trim();
                            var course = new CourseDomainModel
                            {
                                CourseCode = campusAndCourse,
                                CampusCode = campusAndCourse.Split(CourseSeparator)[1].Trim(),
                                Name = currentValue[2].Trim(),
                                StartDate = DateTime.Parse(currentValue[3].Trim())
                            };

                            container.Courses.Add(course);
                        }
                        break;

                    default:
                        continue;
                }
            }

            return container;
        }
    }
}