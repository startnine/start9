using Start9.Api.Plex;
using Start9.Host.Views;
using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Start9
{
    public sealed class Module : IModuleAssembly
    {
        Module(AddInToken t)
        {
            _token = t;
        }

        readonly AddInToken _token;      
        static readonly String AddInPipelineRoot = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), "Start9", "Pipeline");
        private static Lazy<ObservableCollection<Module>> _modules = new Lazy<ObservableCollection<Module>>(() =>
        {
            var warnings = AddInStore.Rebuild(AddInPipelineRoot); //AddInStore.Update(AddInPipelineRoot);
#if DEBUG
            MessageBox.Show(null, String.Join(Environment.NewLine, warnings), "Add-In Store Warning");
#endif
            return new ObservableCollection<Module>(AddInStore.FindAddIns(typeof(IModule), AddInPipelineRoot).Select(t => new Module(t)));
        } );
        public static ObservableCollection<Module> Modules => _modules.Value;

        public String Name => _token.Name;
        public String Description => _token.Description;
        public String Publisher => _token.Publisher;
        internal AssemblyName AssemblyName => _token.AssemblyName;
        public Version Version
        {
            get
            {
                Version.TryParse(_token.Version, out var result);
                return result;
            }
        }

        public ObservableCollection<ModuleInstance> Instances { get; } = new ObservableCollection<ModuleInstance>();

        public IConfiguration SavedConfiguration => throw new NotImplementedException();

        public IConfiguration CurrentConfiguration => Instances.First().Instance.Configuration;

        public IMessageContract MessageContract => Instances.First().Instance.MessageContract;

        public IReceiverContract ReceiverContract => Instances.First().Instance.ReceiverContract;

        IList<IModuleInstance> IModuleAssembly.Instances => throw new NotImplementedException();

        public static void Kill(IModule instance) => AddInController.GetAddInController(instance).Shutdown();
        public void KillAll()
        {
            foreach (var instance in Instances)
            {
                Kill(instance.Instance);
            }
        }

        public static void UpdateInstalledModules()
        {
            AddInStore.Update(AddInPipelineRoot);
            var addins = AddInStore.FindAddIns(typeof(IModule), AddInPipelineRoot);
            foreach (var token in addins)
            {
                if (!Modules.Any(m => m._token.Equals(token)))
                {
                    Modules.Add(new Module(token));
                }
            }
        }

        public IModule Activate()   
        {
            var proc = new AddInProcess();
            var instance = _token.Activate<IModule>(proc, AddInSecurityLevel.FullTrust);
            instance.Initialize(Start9Host.Instance);
            Instances.Add(new ModuleInstance(instance, proc));
            return instance;
        }

        public static Module GetModuleForInstance(IModule module)
        {
            return Modules.First(m => AddInController.GetAddInController(module).Token.Equals(m._token));
        }

        public void Kill(IModuleInstance instance) => throw new NotImplementedException();
        void IModuleAssembly.Activate(Boolean initialize) => throw new NotImplementedException();

        private class AddInTokenEqualityComparer : IEqualityComparer<AddInToken>
        {
            public Boolean Equals( AddInToken x, AddInToken y) => x.Equals(y);
            public Int32 GetHashCode(AddInToken obj) => obj.GetHashCode();
        }
    }

    public class ModuleInstance
    {
        public ModuleInstance(IModule instance, AddInProcess proc)
        {
            Instance = instance;
            Process = proc;
            proc.ShuttingDown += (sender, e) =>
            {
                Module.GetModuleForInstance(Instance).Instances.Remove(this);
            };
        }

        public IModule Instance { get; }
        public AddInProcess Process { get; }
    }

}
