using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFplay zu interagieren.
    /// </summary>
    class ffplay
    {
        /// <summary>
        /// Überprüft ob FFplay vorhanden ist.
        /// </summary>        
        public bool bExists()
        {
            return File.Exists( "ffplay.exe" );
        }
    }
}
