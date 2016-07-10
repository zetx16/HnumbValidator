using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnumbValidator
{
    public static class Options
    {
        public static bool UploadToFtp { get; set; }
        public static bool DownloadFromGislab { get; set; }
        public static string[] Regions { get; set; }
        public static string DirectoryInput { get; set; }
        public static string DirectoryOutput { get; set; }

        static Options()
        {
            DownloadFromGislab = false;
            UploadToFtp = false;
            DirectoryOutput = String.Empty;
            DirectoryInput = String.Empty;
        }
    }
}
