using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utils;

namespace Start9.NodeControl
{
    public sealed class ModuleNetworkViewModel
    {
        private Lazy<ImpObservableCollection<ModuleViewModel>> nodes = new Lazy<ImpObservableCollection<ModuleViewModel>>();
        private Lazy<ImpObservableCollection<MessagePathViewModel>> connections = new Lazy<ImpObservableCollection<MessagePathViewModel>>(() =>
        {
            var col = new ImpObservableCollection<MessagePathViewModel>();
            col.ItemsRemoved += (sender, e) =>
            {
                foreach (MessagePathViewModel connection in e.Items)
                {
                    connection.SourceConnector = null;
                    connection.DestConnector = null;
                }
            };
            return col;
        });

        public ImpObservableCollection<ModuleViewModel> Nodes => nodes.Value;
        public ImpObservableCollection<MessagePathViewModel> Connections => connections.Value;
    }
}
