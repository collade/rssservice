namespace RssService.Application
{
    using System;
    using System.ServiceProcess;

    class Program
    {
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Bootstrapper.Initialize();
                Console.WriteLine("Rss Service is ready!");
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new  RssWindowsService() });
            }
        }
    }
}
