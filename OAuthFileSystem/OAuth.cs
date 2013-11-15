using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthFileSystem.OAuth
{
    public abstract class OAuthFileSystemAPI : IOAuth
    {
        public OAuthFileSystemAPI(string name,string key) 
        {
            this.APIKEY = key;
            this.APPNAME = name;
        }
        private string _accessToken;

        public string APIKEY {private set; get; }
        public string APPNAME {private set; get; }
        public string AccessToken { get { return _accessToken; } }

        public virtual string BaseAPIUrl { get { return string.Empty; } }

        public virtual string LOGINURL  { get { return string.Empty; } }



        public dynamic Invoke(string model, string method, Dictionary<string, string> parameters = null)
        {
            throw new NotImplementedException();
        }


        public bool ShowOAuthUI()
        {
            var oauthUI = new OAuthUIWindow(this.LOGINURL);
            bool result = oauthUI.ShowDialog().Value;
            if (result)
            {
                _accessToken = oauthUI.AccessToken;
            }
            return result;
        }
    }
}
