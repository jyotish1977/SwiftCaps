using System.IO;

namespace Xamariners.Functions.Core.Helpers
{
    public static class MemoryStreamHelpers
    {
        public static string ReadToEnd(Stream stream)
        {
            if (stream is MemoryStream)
            {
                if (stream.Position != 0)
                {
                    stream.Position = 0;
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
