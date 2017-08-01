using System;
using System.ServiceProcess;
using System.Management;

namespace SvcList
{
    static class Program
    {
        static int Main(string[] args)
        {
            ServiceController[] Services;
            Services = ServiceController.GetServices();

            Console.WriteLine("Services running on the local computer:");
            foreach (ServiceController NextService in Services)
            {
                Console.WriteLine();
                Console.WriteLine("    Service:    {0}", NextService.ServiceName);
                Console.WriteLine("       Display name:    {0}", NextService.DisplayName);
                Console.WriteLine("       Status:          {0}", NextService.Status.ToString());

                ManagementObject WmiService = new ManagementObject("Win32_Service.Name='" + NextService.ServiceName + "'");
                WmiService.Get();
                Console.WriteLine("       Start name:      {0}", WmiService["StartName"]);
                Console.WriteLine("       Description:     {0}", WmiService["Description"]);
            }

            return 0;
        }
    }
}
