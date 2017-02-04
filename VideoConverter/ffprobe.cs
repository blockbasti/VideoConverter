using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFprobe zu interagieren.
    /// </summary>
    class ffprobe
    {
        /// <summary>
        /// Überprüft ob FFprobe vorhanden ist.
        /// </summary>  
        public bool bExists()
        {
            return File.Exists( "ffprobe.exe" );
        }
    }
}
