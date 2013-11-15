using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthFileSystem.OAuth.Model
{
    public class FileItemInfo
    {
        public Int64 fs_id { set; get; }
        public string path { set; get; }
        public int ctime { set; get; }
        public int mtime { set; get; }

        public string md5 { set; get; }
        public Int64 size { set; get; }

        public bool isdir { set; get; }
    }
}
