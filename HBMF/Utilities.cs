using System.IO;
using System.Reflection;

namespace HBMF.Utilities
{
    public class Utils
    {
        public static byte[] GetResource(Assembly assembly, string name)
        {
            MemoryStream memoryStream = new();
            Stream stream = assembly.GetManifestResourceStream(name);
            stream.CopyTo(memoryStream);
            stream.Close();
            byte[] data = memoryStream.ToArray();
            memoryStream.Close();
            return data;
        }
    }
}
