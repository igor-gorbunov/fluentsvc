using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

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
            _ProcessStartInfo = new ProcessStartInfo()
            {
                FileName = "C:\\Program Files\\nginx\\nginx.exe",
                Arguments = "-p \"C:/Program Files/nginx\""
            };

            _ChildProcess = Process.Start(_ProcessStartInfo);

            _ProcessStopInfo = new ProcessStartInfo()
            {
                FileName = "C:\\Program Files\\nginx\\nginx.exe",
                Arguments = "-s stop -p \"C:/Program Files/nginx\""

            };
        }

        protected override void OnStop()
        {
            if (null != _ProcessStopInfo)
                Process.Start(_ProcessStopInfo);
        }

        private ProcessStartInfo _ProcessStartInfo;
        private ProcessStartInfo _ProcessStopInfo;
        private Process _ChildProcess;
    }

    [RunInstaller(true)]
    public partial class WrapperInstaller : Installer
    {
        public WrapperInstaller() : base()
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

            BeforeInstall += new InstallEventHandler(BeforeInstallEventHandler);
            BeforeUninstall += new InstallEventHandler(BeforeUninstallEventHandler);

            AfterInstall += new InstallEventHandler(AfterInstallEventHandler);

            Installers.Add(SvcInstaller);
            Installers.Add(SvcProcessInstaller);
        }

        private void BeforeInstallEventHandler(object sender, InstallEventArgs e)
        {
            if (sender is Installer)
            {
                if (!Context.Parameters.ContainsKey("ServiceName"))
                    throw new InstallException("Required argument missing: ServiceName");

                if (!Context.Parameters.ContainsKey("StartCommand"))
                    throw new InstallException("Required argument missing: StartCommand");

                SvcInstaller.ServiceName = Context.Parameters["ServiceName"];

                if (Context.Parameters.ContainsKey("DisplayName"))
                    SvcInstaller.DisplayName = Context.Parameters["DisplayName"];

                if (Context.Parameters.ContainsKey("Description"))
                    SvcInstaller.Description = Context.Parameters["Description"];

                StringBuilder ImagePath = new StringBuilder(Context.Parameters["AssemblyPath"]);
                ImagePath.Append(" --service=");
                ImagePath.Append(Context.Parameters["ServiceName"]);
                Context.Parameters["AssemblyPath"] = ImagePath.ToString();
            }
        }

        private void BeforeUninstallEventHandler(object sender, InstallEventArgs e)
        {
            if (sender is Installer)
            {
                if (Context.Parameters.ContainsKey("ServiceName"))
                    SvcInstaller.ServiceName = Context.Parameters["ServiceName"];
            }
        }

        private void AfterInstallEventHandler(object sender, InstallEventArgs e)
        {
            if (sender is Installer)
            {
                string ServiceKeyPath = @"SYSTEM\CurrentControlSet\Services\" + SvcInstaller.ServiceName;
                RegistryKey ServiceRegistryPath = Registry.LocalMachine.OpenSubKey(ServiceKeyPath, true);
                RegistryKey SubKey = ServiceRegistryPath.CreateSubKey(@"Parameters");

                if (Context.Parameters.ContainsKey("StartCommand"))
                    SubKey.SetValue(@"StartCommand", Context.Parameters["StartCommand"]);

                if (Context.Parameters.ContainsKey("StartArguments"))
                    SubKey.SetValue(@"StartArguments", Context.Parameters["StartArguments"]);

                if (Context.Parameters.ContainsKey("StopCommand"))
                    SubKey.SetValue(@"StopCommand", Context.Parameters["StopCommand"]);

                if (Context.Parameters.ContainsKey("StopArguments"))
                    SubKey.SetValue(@"StopArguments", Context.Parameters["StopArguments"]);
            }
        }

        private ServiceInstaller SvcInstaller;
        private ServiceProcessInstaller SvcProcessInstaller;
    }
}
