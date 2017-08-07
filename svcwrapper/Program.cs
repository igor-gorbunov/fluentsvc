using Microsoft.Win32;
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
        static void Main(string[] args)
        {
            string servicename = null;

            foreach (string s in args)
            {
                if (s.StartsWith("--service="))
                    servicename = s.Remove(0, "--service=".Length);
            }

            if (null == servicename)
                return;

            ServiceWrapper Service = new ServiceWrapper(servicename);
            ServiceBase.Run(Service);
        }
    }

    public partial class ServiceWrapper : ServiceBase
    {
        public ServiceWrapper(string servicename) : base()
        {
            ServiceName = servicename;
        }

        protected override void OnStart(string[] args)
        {

            string ServiceKeyPath = @"SYSTEM\CurrentControlSet\Services\" + ServiceName + @"\Parameters";
            RegistryKey RegistrySubKey = Registry.LocalMachine.OpenSubKey(ServiceKeyPath, true);

            string StartCommand = null;
            if (RegistrySubKey.GetValue("StartCommand") is string)
            {
                StartCommand = (string)RegistrySubKey.GetValue("StartCommand");
                if (!StartCommand.StartsWith("\""))
                    StartCommand = string.Format("\"{0}\"", StartCommand);
            }

            string StartArguments = null;
            if (RegistrySubKey.GetValue("StartArguments") is string)
                StartArguments = (string)RegistrySubKey.GetValue("StartArguments");

            string StopCommand = null;
            if (RegistrySubKey.GetValue("StopCommand") is string)
            {
                StopCommand = (string)RegistrySubKey.GetValue("StopCommand");
                if (!StopCommand.StartsWith("\""))
                    StopCommand = string.Format("\"{0}\"", StopCommand);
            }

            string StopArguments = null;
            if (RegistrySubKey.GetValue("StopArguments") is string)
                StopArguments = (string)RegistrySubKey.GetValue("StopArguments");

            _ProcessStartInfo = new ProcessStartInfo()
            {
                FileName = StartCommand,
                Arguments = StartArguments
            };

            _ChildProcess = Process.Start(_ProcessStartInfo);

            _ProcessStopInfo = new ProcessStartInfo()
            {
                FileName = StopCommand,
                Arguments = StopArguments

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

                string ImagePath;
                if (Context.Parameters["AssemblyPath"].StartsWith("\""))
                    ImagePath = string.Format("{0} --service={1}", Context.Parameters["AssemblyPath"], Context.Parameters["ServiceName"]);
                else
                    ImagePath = string.Format("\"{0}\" --service={1}", Context.Parameters["AssemblyPath"], Context.Parameters["ServiceName"]);
                Context.Parameters["AssemblyPath"] = ImagePath;
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
                if (!Context.Parameters.ContainsKey("ServiceName"))
                    throw new InstallException("Required argument missing: ServiceName");

                string ServiceKeyPath = @"SYSTEM\CurrentControlSet\Services\" + SvcInstaller.ServiceName;
                RegistryKey ServiceRegistryPath = Registry.LocalMachine.OpenSubKey(ServiceKeyPath, true);
                RegistryKey SubKey = ServiceRegistryPath.CreateSubKey("Parameters");

                if (Context.Parameters.ContainsKey("StartCommand"))
                    SubKey.SetValue("StartCommand", Context.Parameters["StartCommand"]);

                if (Context.Parameters.ContainsKey("StartArguments"))
                    SubKey.SetValue("StartArguments", Context.Parameters["StartArguments"]);

                if (Context.Parameters.ContainsKey("StopCommand"))
                    SubKey.SetValue("StopCommand", Context.Parameters["StopCommand"]);

                if (Context.Parameters.ContainsKey("StopArguments"))
                    SubKey.SetValue("StopArguments", Context.Parameters["StopArguments"]);
            }
        }

        private ServiceInstaller SvcInstaller;
        private ServiceProcessInstaller SvcProcessInstaller;
    }
}
