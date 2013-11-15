using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OAuthFileSystem.OAuth
{
    /// <summary>
    /// Interaction logic for BaiduPcsLoginWindow.xaml
    /// </summary>
    public partial class OAuthUIWindow : Window
    {

        public OAuthUIWindow(string loginUrl)
        {
            url = loginUrl;
            InitializeComponent();
         
            webbrowser.Navigate(url);
        }
        private string url;
        public string AccessToken { set; get; }
        public string ErrorMessage { set; get; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ErrorMessage = "授权取消";
            base.OnClosing(e);
        }

        private void webbrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            //this.DialogResult = true;
        }

        private void webbrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string currentUrl = webbrowser.Source.ToString();
            if (!url.Equals(currentUrl))
            {
                if (currentUrl.Contains("error"))
                {
                    ErrorMessage = "授权取消";
                    this.DialogResult = false;
                }
                else
                {
                    Match match = Regex.Match(currentUrl, "en=(?<token>[\\w\\W]*)&session_secret");
                    this.AccessToken = match.Groups["token"].Value;
                    if (!string.IsNullOrEmpty(this.AccessToken))
                    {
                        this.DialogResult = true;
                    }
                }

            }
        }


    }
}
