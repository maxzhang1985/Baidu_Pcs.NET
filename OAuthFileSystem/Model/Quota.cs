using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthFileSystem.OAuth.Model
{
    public class Quota
    {
        public Int64 quota { set; get; }
        public Int64 used { set; get; }
    }
}
