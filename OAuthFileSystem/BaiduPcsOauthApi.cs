using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace OAuthFileSystem.OAuth
{
    /// <summary>
    /// 百度PCS OAuth API
    /// </summary>
    public class BaiduPcsFileSystemApi : OAuthFileSystemAPI, IOAuth , IFileSystem
    {
        private static readonly string _baseApiUri = "https://pcs.baidu.com/rest/2.0/pcs/";
        /// <summary>
        /// 百度PCS OAuth API 构造函数
        /// </summary>
        /// <param name="name">应用程序名称</param>
        /// <param name="key">应用程序 API KEY</param>
        public BaiduPcsFileSystemApi(string name,string key)
           : base(name,key) {
               this.RootDir = string.Format("/apps/{0}/",name);
        }

        /// <summary>
        /// 登录URL地址
        /// </summary>
        public override string LOGINURL{
            get {
                string random = new Random((int)DateTime.Now.Ticks).Next().ToString();
                return string.Format("http://openapi.baidu.com/oauth/2.0/authorize?response_type=token&client_id={0}&redirect_uri=oob&scope=basic%20netdisk&forcelogin=true&random={1}", this.APIKEY, random);
            }
        }
        /// <summary>
        /// OAuth基地址
        /// </summary>
        public override string BaseAPIUrl{  get{ return _baseApiUri; } }
        /// <summary>
        /// FileSystem网络文件系统根目录
        /// </summary>
        public string RootDir { private set; get; }
     
        public async Task<dynamic> Invoke(string model,string method, Dictionary<string, string> parameters = null)
        {
            dynamic obj = null;
            String responseJson = string.Empty;
            string paramListString = string.Empty;
            if (parameters != null)
            {
               paramListString = String.Join("&", 
                    parameters.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));
            }

            string invokeUrl = string.Format("{0}{1}?method={2}&{3}&access_token={4}", BaseAPIUrl, model, method,paramListString,this.AccessToken);

            using(WebResponse response = await HttpWebResponseUtility.CreateGetAsyncHttpResponse(invokeUrl))
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseJson = await reader.ReadToEndAsync();
                    }
                }
            }

            if (!string.IsNullOrEmpty(responseJson))
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

                obj = serializer.Deserialize(responseJson, typeof(object));
            }
            return obj;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public async Task<bool> PushFile(string fileName, string desDirPath, Action<ProgressChangedEventArgs> uploadProgressFunc = null)
        {
            string invokeUrl = string.Format("{0}{1}?method={2}&access_token={3}&path={4}", BaseAPIUrl, "file", "upload", this.AccessToken, desDirPath);
            using(FileStream fs = new FileStream(fileName,FileMode.Open))
            {
                UploadFile file = new UploadFile(){ Name="file", Filename = Path.GetFileName(fileName) , ContentType="text/plain", Stream =  fs};
                string html = UploadFileExtension.UploadFiles(invokeUrl, file, null);
            }
            return true;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public async Task<bool> PullFile(string desDirPath, string fileName,Action<ProgressChangedEventArgs> downloadProgressFunc = null)
        {
            string invokeUrl = string.Format("{0}{1}?method={2}&access_token={3}&path={4}", BaseAPIUrl, "file", "download", this.AccessToken,desDirPath);
            try
            {
                using (var wc = new WebClient())
                {
                    DownloadProgressChangedEventHandler downloadProgressChangedEventHandler = null;
                    if(downloadProgressFunc!=null)
                    {
                        downloadProgressChangedEventHandler = (s, e) => 
                                downloadProgressFunc(new ProgressChangedEventArgs(e.ProgressPercentage,e.UserState)) ;
                        wc.DownloadProgressChanged += downloadProgressChangedEventHandler;
                    }

                    await wc.DownloadFileTaskAsync(new Uri(invokeUrl), fileName);

                    if(downloadProgressFunc!=null)
                        wc.DownloadProgressChanged -= downloadProgressChangedEventHandler;
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 删除文件或目录 
        /// </summary>
        public async Task<bool> DeleteFile(string filePath)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["path"] = filePath;
            try{
                dynamic query = await this.Invoke("file", "delete", dic);
            }
            catch { return false; }

            return true;
        }
        /// <summary>
        /// 新建文件夹
        /// </summary>
        public async Task<Model.FileItemInfo> MakeDir(string dirPath)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["path"] = dirPath;
            dynamic query = null;
            try
            {
                query = await this.Invoke("file", "mkdir", dic);
            }
            catch { return null; }
            var fileItemInfo = new Model.FileItemInfo();
            fileItemInfo.fs_id = query.fs_id;
            fileItemInfo.ctime = query.ctime;
            fileItemInfo.mtime = query.mtime;
            fileItemInfo.path = query.path;

            return fileItemInfo;
        }

        /// <summary>
        /// 查询指定文件系统配额
        /// </summary>
        public async Task<Model.Quota> QueryQuotaAsync()
        {
            dynamic query = null;
            try
            {
                query = await this.Invoke("quota", "info");
            }
            catch { return null; }
            return new Model.Quota() { quota = query.quota , used = query.used };
        }

        /// <summary>
        /// 查询指定文件夹下的文件或文件夹
        /// </summary>
        public async Task<List<Model.FileItemInfo>> QueryDirFileList(string path = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (path == null) {
                dic["path"] = this.RootDir;
            }
            else
            {
                dic["path"] = path;
            }
            dic["by"] = "time"; //time,name,size
            dic["order"] = "asc"; // asc , desc
            //dic["order"] = "n1-n2";
            dynamic query = null;
            try
            {
                query = await this.Invoke("file", "list", dic);
            }
            catch {
                return null;
            }
            int count = query.list.Count;
            List<Model.FileItemInfo> fileInfoList = new List<Model.FileItemInfo>();
            for (int i = 0; i < count; i++ )
            {
                var fileItemInfo = new Model.FileItemInfo();
                dynamic queryInfo = query.list[i];
                fileItemInfo.fs_id = queryInfo.fs_id;
                fileItemInfo.ctime = queryInfo.ctime;
                fileItemInfo.mtime = queryInfo.mtime;
                fileItemInfo.path = queryInfo.path;
                fileItemInfo.size = queryInfo.size;
                fileItemInfo.md5 = queryInfo.md5;
                fileItemInfo.isdir = Convert.ToBoolean(queryInfo.isdir);

                fileInfoList.Add(fileItemInfo);
            }



            return fileInfoList;

        }

    

        /// <summary>
        /// 搜索指定规则文件
        /// </summary>
        public async Task<List<Model.FileItemInfo>> Search(string keyword, string path = null, bool isRecall = false)
        {
            if(string.IsNullOrEmpty(keyword))
                throw new NotSupportedException("查找关键字不能为空!");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (path == null)
                dic["path"] = this.RootDir;
            else
                dic["path"] = path;
            dic["wd"] = keyword;
            dic["re"] = isRecall ? "1" : "0";
            dynamic query = null;
            try
            {
                query = await this.Invoke("file", "search", dic);
            }
            catch {
                return null;
            }
            int count = query.list.Count;
            List<Model.FileItemInfo> fileInfoList = new List<Model.FileItemInfo>();
            for (int i = 0; i < count; i++)
            {
                var fileItemInfo = new Model.FileItemInfo();
                dynamic queryInfo = query.list[i];
                fileItemInfo.fs_id = queryInfo.fs_id;
                fileItemInfo.ctime = queryInfo.ctime;
                fileItemInfo.mtime = queryInfo.mtime;
                fileItemInfo.path = queryInfo.path;
                fileItemInfo.size = queryInfo.size;
                fileItemInfo.md5 = queryInfo.md5;
                fileItemInfo.isdir = Convert.ToBoolean(queryInfo.isdir);

                fileInfoList.Add(fileItemInfo);
            }


            return fileInfoList;
        }
    }
}
