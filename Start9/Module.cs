using Start9.Host.Views;
using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Start9
{
    class Module
    {
        public static ObservableCollection<Module> Modules => new ObservableCollection<Module>(AddInStore.FindAddIns(typeof(IModule), AddInPipelineRoot).Select(t => new Module(t));

        public static String AddInPipelineRoot { get; } = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), "Start9", "Pipeline");

        public Module(AddInToken token)
        {
            _token = token;
        }

        public Module(IModule module)
        {
            Instances.Add(module);
        }

        AddInToken _token;

        public ObservableCollection<IModule> Instances { get; } = new ObservableCollection<IModule>();
        public String Name => _token.Name;
        public IDictionary<AddInSegmentType, IDictionary<String, String>> QualificationData => _token.QualificationData;
        public String Description => _token.Description;
        public String Publisher => _token.Publisher;
        public Version Version
        {
            get
            {
                Version.TryParse(_token.Version, out var result);
                return result;
            }
        }
        public IEnumerable<AddInProcess> Processes => Instances.Select(i => AddInController.GetAddInController(i).AddInEnvironment.Process);

        public void KillAll()
        {
            foreach(var module in Instances)
            {
                AddInController.GetAddInController(module).Shutdown();
            }
        }

        public void Kill(IModule module)
        {
            AddInController.GetAddInController(module).Shutdown();
        }

        public IModule CreateNewInstance()
        {
            var m = _token.Activate<IModule>(new AddInProcess(), AddInSecurityLevel.FullTrust);
            m.Initialize(new Start9Host());
            Instances.Add(m);
            return m;
        }
    }
}
