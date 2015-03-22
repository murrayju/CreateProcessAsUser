using murrayju.ProcessExtensions;
using System.ServiceProcess;

namespace demo
{
    public partial class DemoService : ServiceBase
    {
        public DemoService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
        }

        protected override void OnStop()
        {
        }
    }
}
