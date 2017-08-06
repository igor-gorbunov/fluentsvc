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
            SvcInstaller = new ServiceInstaller()
            {
                Description = "Service wrapping facility for Windows OS programs.",
                DisplayName = "Service Wrapper",
                ServiceName = "ServiceWrapper",
                StartType = ServiceStartMode.Automatic
            };

            SvcProcessInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem
            };

            Installers.Add(SvcInstaller);
            Installers.Add(SvcProcessInstaller);
        }

        public override void Install(System.Collections.IDictionary StateSaver)
        {
            if (Context.Parameters.ContainsKey("ServiceName"))
                SvcInstaller.ServiceName = Context.Parameters["ServiceName"];

            if (Context.Parameters.ContainsKey("DisplayName"))
                SvcInstaller.DisplayName = Context.Parameters["DisplayName"];

            if (Context.Parameters.ContainsKey("Description"))
                SvcInstaller.Description = Context.Parameters["Description"];

            base.Install(StateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary SavedState)
        {
            if (Context.Parameters.ContainsKey("ServiceName"))
                SvcInstaller.ServiceName = Context.Parameters["ServiceName"];

            if (Context.Parameters.ContainsKey("DisplayName"))
                SvcInstaller.DisplayName = Context.Parameters["DisplayName"];

            if (Context.Parameters.ContainsKey("Description"))
                SvcInstaller.Description = Context.Parameters["Description"];

            base.Uninstall(SavedState);
        }

        private ServiceInstaller SvcInstaller;
        private ServiceProcessInstaller SvcProcessInstaller;
    }
}
