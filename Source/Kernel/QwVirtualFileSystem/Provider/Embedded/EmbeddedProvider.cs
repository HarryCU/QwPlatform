using System.IO;
using System.Reflection;
using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem.Provider.Embedded
{
    public class EmbeddedProvider : RegexSchemeProvider
    {
        public EmbeddedProvider()
            : base("embedded", "embedded://(?<AssemblyName>.*?):(?<FileName>.*)")
        {
        }

        protected override IFolder BuildFolder(string path)
        {
            using (var data = Match(path))
            {
                var assemblyName = data.GetValue("AssemblyName");
                var assemblyFile = data.GetValue("FileName");

                var assembly = Assembly.LoadFile(assemblyFile);

                Folder rootFolder = Folder.CreateRoot();
                foreach (var manifestResourceName in assembly.GetManifestResourceNames())
                {
                    var fileName = ResourceNameToFileName(assemblyName, manifestResourceName);
                    var buffer = LoadAssemblyResource(assembly, manifestResourceName);

                    rootFolder.AppendFile(fileName, buffer);
                }
                return rootFolder;
            }
        }

        private static string ResourceNameToFileName(string prefix, string resourceName)
        {
            var extension = Path.GetExtension(resourceName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(resourceName);

            if (nameWithoutExt != null)
                return string.Concat(Folder.RootName, VFSPath.PathSeparator, nameWithoutExt
                    .Replace(prefix + ".", string.Empty)
                    .Replace(".", VFSPath.PathSeparator), extension);
            return null;
        }

        private static byte[] LoadAssemblyResource(Assembly assembly, string resourceName)
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return null;
            using (var reader = new BufferedStream(stream))
            {
                var buffer = new byte[reader.Length];
                reader.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
