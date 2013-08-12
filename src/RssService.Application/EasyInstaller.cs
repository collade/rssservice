namespace RssService.Application
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class EasyInstaller : Installer
    {
        private readonly ServiceProcessInstaller _serviceProcess;
        private readonly ServiceInstaller _serviceInstaller;

        public EasyInstaller()
        {
            InitializeComponent();

            _serviceProcess = new ServiceProcessInstaller { Account = ServiceAccount.NetworkService };
            _serviceInstaller = new ServiceInstaller
            {
                ServiceName = "ColladeRssService",
                DisplayName = "Collade Rss Service",
                Description = "Busy waiting for rss updares and saving them to db.",
                StartType = ServiceStartMode.Automatic
            };
            Installers.Add(_serviceProcess);
            Installers.Add(_serviceInstaller);
        }
    }
}
