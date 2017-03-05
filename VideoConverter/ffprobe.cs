using System.IO;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFprobe zu interagieren.
    /// </summary>
    internal class ffprobe
    {
        /// <summary>
        /// Überprüft ob FFprobe vorhanden ist.
        /// </summary>
        public static bool bExists()
        {
            return File.Exists( "ffprobe.exe" );
        }
    }
}