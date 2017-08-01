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
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
