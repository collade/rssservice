using System.ServiceProcess;

namespace RssService.Application
{
    partial class RssWindowsService : ServiceBase
    {
        public RssWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Bootstrapper.Initialize();
        }

        protected override void OnStop()
        {
            
        }
    }
}
