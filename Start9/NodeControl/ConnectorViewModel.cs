using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using System.Diagnostics;
using System.Windows;
using Start9.Host.Views;

namespace Start9.NodeControl
{
    public sealed class EntryViewModel : AbstractModelBase
    {
        private ImpObservableCollection<MessagePathViewModel> attachedConnections = null;

        private Point hotspot;

        public EntryViewModel(Module module, IMessageEntry messageEntry)
        {
            Module = module;
            Name = messageEntry.FriendlyName;
            Type = EntryType.Message;
        }

        public EntryViewModel(Module module, IReceiverEntry receiverEntry)
        {
            Module = module;
            Name = receiverEntry.FriendlyName;
            Type = EntryType.Receiver;
        }

        String Name { get; }

        public IMessageEntry MessageEntry => Module.MessageContract.Entries.First(e => e.FriendlyName == Name);
        public IReceiverEntry ReceiverEntry => Module.ReceiverContract.Entries.First(e => e.FriendlyName == Name);
        public Module Module { get; }

        public EntryType Type { get; internal set; }

        public Boolean IsConnected
        {
            get
            {
                foreach (var connection in AttachedConnections)
                {
                    if (connection.SourceConnector != null &&
                        connection.DestConnector != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        public Boolean IsConnectionAttached => AttachedConnections.Count > 0;

        public ImpObservableCollection<MessagePathViewModel> AttachedConnections
        {
            get
            {
                if (attachedConnections == null)
                {
                    attachedConnections = new ImpObservableCollection<MessagePathViewModel>();
                    attachedConnections.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(attachedConnections_ItemsAdded);
                    attachedConnections.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(attachedConnections_ItemsRemoved);
                }

                return attachedConnections;
            }
        }

        public ModuleViewModel ParentNode
        {
            get;
            internal set;
        }

        public Point Hotspot
        {
            get
            {
                return hotspot;
            }
            set
            {
                if (hotspot == value)
                {
                    return;
                }

                hotspot = value;

                OnHotspotUpdated();
            }
        }

        public event EventHandler<EventArgs> HotspotUpdated;


        private void attachedConnections_ItemsAdded(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (MessagePathViewModel connection in e.Items)
            {
                connection.ConnectionChanged += new EventHandler<EventArgs>(connection_ConnectionChanged);
            }

            if ((AttachedConnections.Count - e.Items.Count) == 0)
            {

                OnPropertyChanged(nameof(IsConnectionAttached));
                OnPropertyChanged(nameof(IsConnected));
            }
        }


        private void attachedConnections_ItemsRemoved(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (MessagePathViewModel connection in e.Items)
            {
                connection.ConnectionChanged -= new EventHandler<EventArgs>(connection_ConnectionChanged);
            }

            if (AttachedConnections.Count == 0)
            {
                OnPropertyChanged(nameof(IsConnectionAttached));
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private void connection_ConnectionChanged(Object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsConnectionAttached));
            OnPropertyChanged(nameof(IsConnected));
        }

        /// <summary>
        /// Called when the connector hotspot has been updated.
        /// </summary>
        private void OnHotspotUpdated()
        {
            OnPropertyChanged(nameof(Hotspot));

            HotspotUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
