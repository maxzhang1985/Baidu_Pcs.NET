using OAuthFileSystem.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApplication4.OAuth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            IOAuth oath = new BaiduPcsFileSystemApi("RoslynEditor", "EbTOgWgI62ouQVpGSIw0Tzu8");
         
            if (oath.ShowOAuthUI())
            {
                var fs = oath.AsFileSystem();
                //var quota = await fs.QueryQuotaAsync();
                var fileList = await fs.QueryDirFileList();

                var List = await fs.Search(keyword:"kms", isRecall: true);


                await fs.PushFile(@"e:\wpfdemo.txt", fs.RootDir + "demo.txt");

                //var info = fs.PullFile(List[0].path, @"e:\a.rar", (ev) => {

                //    this.Dispatcher.BeginInvoke(new Action(()=>{

                //        this.progress.Value = ev.ProgressPercentage;
                    
                //    }), null);
                
                //});

            }



        }
    }
}
