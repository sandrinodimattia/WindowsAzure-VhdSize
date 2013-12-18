using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.VhdSize
{
    public static class PageBlobExtensions
    {
        /// <summary>
        /// Based on this script: http://gallery.technet.microsoft.com/scriptcenter/Get-Billable-Size-of-32175802
        /// </summary>
        /// <returns></returns>
        public static long GetActualDiskSize(this CloudPageBlob pageBlob)
        {
            pageBlob.FetchAttributes();
            return 124 + pageBlob.Name.Length * 2 + pageBlob.Metadata.Sum(m => m.Key.Length + m.Value.Length + 3) + pageBlob.GetPageRanges().Sum(r => 12 + (r.EndOffset - r.StartOffset));
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public static string GetFormattedDiskSize(long size)
        {
            var sb = new StringBuilder(11);
            StrFormatByteSize(size, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}