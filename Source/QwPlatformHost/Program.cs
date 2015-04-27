using System.Text;
using QwVirtualFileSystem;

namespace QwPlatformHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var runtime = new VFSRuntime())
            {
                // 暂时在我本机上
                var folder = runtime.Query(@"embedded://QwEmbedded:C:\Users\HarryCU\Documents\Visual Studio 2013\Projects\QwEmbedded\QwEmbedded\bin\Debug\QwEmbedded.dll");

                var file = folder.SearchFile("VFS:/Views/Admin/Login.cshtml");

                using (var reader = file.OpenRead())
                {
                    byte[] buffer;
                    reader.Read( 0, reader.Length, out buffer);
                    var content = Encoding.UTF8.GetString(buffer);
                }
            }
        }
    }
}
