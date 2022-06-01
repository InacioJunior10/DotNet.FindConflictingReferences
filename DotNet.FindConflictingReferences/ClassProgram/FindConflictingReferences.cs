using System.Reflection;
using System.Text;

namespace DotNet.FindConflictingReferences.ClassProgram
{
    public class FindConflictingReferences
    {        
        string fileLog = "FindConflictingReferences";

        public bool FindConflicting(string pathBin, string pathLog)
        {
            try
            {
                var assemblies = GetAllAssemblies(pathBin);

                var references = GetReferencesFromAllAssemblies(assemblies);

                var groupsOfConflicts = FindReferencesWithTheSameShortNameButDiffererntFullNames(references);

                StringBuilder textLog = new StringBuilder();

                foreach (var group in groupsOfConflicts)
                {
                    textLog.AppendLine($"Possible conflicts for {group.Key}:");

                    foreach (var reference in group)
                    {                        
                        textLog.AppendLine($"{reference.Assembly.Name.PadRight(25)} references {reference.ReferencedAssembly.FullName}");
                    }

                    textLog.AppendLine(Environment.NewLine);
                }

                if (!Directory.Exists(pathLog))
                    Directory.CreateDirectory(pathLog);

                File.WriteAllText($@"{pathLog}\{fileLog}_{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss").Replace(":", "-").Replace(" ", "_")}.log", textLog.ToString());
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        private IEnumerable<IGrouping<string, Reference>> FindReferencesWithTheSameShortNameButDiffererntFullNames(List<Reference> references)
        {
            return from reference in references
                   group reference by reference.ReferencedAssembly.Name
                       into referenceGroup
                   where referenceGroup.ToList().Select(reference => reference.ReferencedAssembly.FullName).Distinct().Count() > 1
                   select referenceGroup;
        }

        private List<Reference> GetReferencesFromAllAssemblies(List<Assembly> assemblies)
        {
            var references = new List<Reference>();
            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    references.Add(new Reference
                    {
                        Assembly = assembly.GetName(),
                        ReferencedAssembly = referencedAssembly
                    });
                }
            }
            return references;
        }

        private List<Assembly> GetAllAssemblies(string path)
        {
            var files = new List<FileInfo>();
            var directoryToSearch = new DirectoryInfo(path);
            files.AddRange(directoryToSearch.GetFiles("*.dll", SearchOption.AllDirectories));
            files.AddRange(directoryToSearch.GetFiles("*.exe", SearchOption.AllDirectories));
            return files.ConvertAll(file => Assembly.LoadFile(file.FullName));
        }

        private class Reference
        {
            public AssemblyName Assembly { get; set; }
            public AssemblyName ReferencedAssembly { get; set; }
        }
    }
}
