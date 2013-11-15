using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthFileSystem.OAuth
{
    /// <summary>
    /// OAuth认证接口
    /// </summary>
    public interface IOAuth
    {
        /// <summary>
        /// OAuth登录页面地址
        /// </summary>
        string LOGINURL {get; }
        /// <summary>
        /// OAuth认证令牌
        /// </summary>
        string AccessToken {  get; }
        /// <summary>
        /// 显示认证界面
        /// </summary>
        /// <returns></returns>
        bool ShowOAuthUI();
        /// <summary>
        /// 执行Rest API 方法
        /// </summary>
        /// <param name="model">模块名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>Json动态对象</returns>
        dynamic Invoke(string model, string method, Dictionary<string, string> parameters = null);
    }
}
