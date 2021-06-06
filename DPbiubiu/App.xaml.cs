using biubiu.Domain.biuMessageBox;
using ShowMeTheXAML;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebApiClient;

namespace biubiu
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public EventWaitHandle ProgramStarted { get; set; }

        public App()
        {
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("程序异常." + Environment.NewLine + e.Exception.Message);
            //Shutdown(1);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "DPbiubiu", out bool createNew);

            DirectoryInfo dir = new DirectoryInfo("C:/");
            foreach (var item in dir.GetFileSystemInfos())
            {
                if (item.Extension.Equals(".purl"))
                {
                    Config.ServerURL = "http://"+Path.GetFileNameWithoutExtension(item.Name)+ ":8080/weigh/";
                    break;
                }
            }

            HttpApi.Register<IApi>().ConfigureHttpApiConfig(c =>
            {
                c.HttpHost = new Uri(Config.ServerURL);
                c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
                //c.HttpClient.Timeout = TimeSpan.FromSeconds(20);
            });

            if (!createNew)
            {
                MessageBox.Show("应用已经开启!");
                App.Current.Shutdown();
                Environment.Exit(0);
            }
            XamlDisplay.Init();
            base.OnStartup(e);

            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
    }
}
