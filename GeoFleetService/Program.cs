using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;

namespace GeoFleetService
{
    static class Program
    {



        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main(string[] args)
        {

            if (System.Environment.UserInteractive) {
                if (args.Count() == 1) {
                    if (args[0] == "-install")
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });

                    if (args[0] == "-uninstall")
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
                return;
            }


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
