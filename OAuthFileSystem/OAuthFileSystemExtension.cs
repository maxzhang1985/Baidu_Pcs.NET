using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthFileSystem.OAuth
{
    public static class OAuthFileSystemExtension
    {
        /// <summary>
        /// OAuth认证接口转换为网络文件系统接口
        /// </summary>
        /// <param name="obj">认证接口</param>
        /// <returns>网络文件系统接口</returns>
        public static IFileSystem AsFileSystem(this IOAuth obj)
        {
            return obj as IFileSystem;
        }


    }
}
