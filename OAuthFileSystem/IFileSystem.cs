using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuthFileSystem.OAuth.Model;

namespace OAuthFileSystem.OAuth
{
    /// <summary>
    /// 文件系统接口
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// 文件系统根目录
        /// </summary>
        string RootDir { get; }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">本地文件路径</param>
        /// <param name="desDirPath">网格路径</param>
        /// <param name="uploadProgressFunc">上传文件进度回调函数</param>
        /// <returns></returns>
        Task<bool> PushFile(string fileName, string desDirPath,Action<ProgressChangedEventArgs> uploadProgressFunc = null);


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="desDirPath">网格路径</param>
        /// <param name="fileName">本地目标位置</param>
        /// <param name="downloadProgressFunc">下载文件进度回调函数</param>
        /// <returns>是否下载成功</returns>
        Task<bool> PullFile(string desDirPath, string fileName, Action<ProgressChangedEventArgs> downloadProgressFunc = null);
       

        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteFile(string filePath);
        Task<Model.FileItemInfo> MakeDir(string dirPath);
        /// <summary>
        /// 获取当前用户空间配额信息。
        /// </summary>
        /// <returns></returns>
        Task<Model.Quota> QueryQuotaAsync();
        /// <summary>
        /// 获取目录下的文件列表，包含文件和文件夹。
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>目录下的文件和文件夹</returns>
        Task<List<Model.FileItemInfo>> QueryDirFileList(string path = null);
        /// <summary>
        /// 按文件名搜索文件（不支持查找目录）。
        /// </summary>
        /// <param name="path">查找目录</param>
        /// <param name="keyword">关键词</param>
        /// <param name="isRecall">是否递归。“0”表示不递归“1”表示递归 缺省为“0”</param>
        /// <returns>符合条件的文件列表</returns>
        Task<List<Model.FileItemInfo>> Search(string keyword,string path = null,bool isRecall = false);
    }
}
