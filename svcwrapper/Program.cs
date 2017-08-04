using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace SvcWrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceWrapper Service = new ServiceWrapper();
            ServiceBase.Run(Service);
        }
    }

    public partial class ServiceWrapper : ServiceBase
    {
        public ServiceWrapper()
        {
            ServiceName = "ServiceWrapper";
        }

        protected override void OnStart(string[] args)
        {
            _ChildProcess = Process.Start("C:\\Program Files\\nginx\\nginx.exe", "-p \"C:/Program Files/nginx\"");
        }

        protected override void OnStop()
        {
            Process.Start("C:\\Program Files\\nginx\\nginx.exe", "-s stop -p \"C:/Program Files/nginx\"");
        }

        private Process _ChildProcess;
    }

    [RunInstaller(true)]
    public partial class WrapperInstaller : Installer
    {
        public WrapperInstaller()
        {
            SvcInstaller = new ServiceInstaller();

            SvcInstaller.Description = "Nginx HTTP/HTTPS server.";
            SvcInstaller.DisplayName = "Nginx";
            SvcInstaller.ServiceName = "ServiceWrapper";
            SvcInstaller.StartType = ServiceStartMode.Automatic;

            SvcProcessInstaller = new ServiceProcessInstaller();
            SvcProcessInstaller.Account = ServiceAccount.LocalSystem;

            Installers.Add(SvcInstaller);
            Installers.Add(SvcProcessInstaller);
        }

        private ServiceInstaller SvcInstaller;
        private ServiceProcessInstaller SvcProcessInstaller;
    }
}
