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

            Installers.Add(SvcInstaller);
            Installers.Add(SvcProcessInstaller);
        }

        private void BeforeInstallEventHandler(object sender, InstallEventArgs e)
        {
            if (sender is Installer)
            {
                if (!Context.Parameters.ContainsKey("StartCommand"))
                    throw new InstallException("Required argument missing: StartCommand");

                StringBuilder ImagePath = new StringBuilder(Context.Parameters["AssemblyPath"]);

                ImagePath.Append(" --startcmd=\"");
                ImagePath.Append(Context.Parameters["StartCommand"]);
                ImagePath.Append("\"");

                if (Context.Parameters.ContainsKey("StartArguments"))
                {
                    ImagePath.Append(" --startargs=\"");
                    ImagePath.Append(Context.Parameters["StartArguments"]);
                    ImagePath.Append("\"");
                }

                if (Context.Parameters.ContainsKey("StopCommand"))
                {
                    ImagePath.Append(" --stopcmd=\"");
                    ImagePath.Append(Context.Parameters["StopCommand"]);
                    ImagePath.Append("\"");
                }

                if (Context.Parameters.ContainsKey("StopArguments"))
                {
                    ImagePath.Append(" --stopargs=\"");
                    ImagePath.Append(Context.Parameters["StopArguments"]);
                    ImagePath.Append("\"");
                }

                Context.Parameters["AssemblyPath"] = ImagePath.ToString();

                if (Context.Parameters.ContainsKey("ServiceName"))
                    SvcInstaller.ServiceName = Context.Parameters["ServiceName"];

                if (Context.Parameters.ContainsKey("DisplayName"))
                    SvcInstaller.DisplayName = Context.Parameters["DisplayName"];

                if (Context.Parameters.ContainsKey("Description"))
                    SvcInstaller.Description = Context.Parameters["Description"];
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

        private ServiceInstaller SvcInstaller;
        private ServiceProcessInstaller SvcProcessInstaller;
    }
}
