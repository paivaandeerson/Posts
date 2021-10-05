using System.Collections.Generic;
using System.Linq;

namespace BatchImport.v2
{
    public class RefactoredService
    {
        public List<string> Errors { get; set; } = new List<string>();

        public void Validate(ContainerDto container)
        {
            //hashset to irregular keys
            var mastersHashSet = container.Courses.AsEnumerable<DomainModel>()
                   .Concat(container.Teachers)
                   .Concat(container.Campus)
                   .ToHashSet();

            //allDataInCsv 2100, 350 masters, classes 1780 * 2
            foreach (var @class in container.Classes)
            {
                // Making sure that the Topology is valid
                var lazyErrors = @class.Validate(mastersHashSet);
                Errors.AddRange(lazyErrors);                
            }
        }
    }
}