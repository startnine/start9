using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Start9.NodeControl;
using System.Windows;
using System.Diagnostics;

namespace Start9.NodeControl
{
    public class NodeControlPageViewModel : AbstractModelBase
    {
        public ModuleNetworkViewModel network = null;
        private Double contentScale = 1;

        private Double contentOffsetX = 0;

        
        private Double contentOffsetY = 0;

        private Double contentWidth = 1000;

        private Double contentHeight = 1000;

        private Double contentViewportWidth = 0;

        private Double contentViewportHeight = 0;


        public ModuleNetworkViewModel Network
        {
            get
            {
                return network;
            }
            set
            {
                network = value;

                OnPropertyChanged("Network");
            }
        }
        public Double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;

                OnPropertyChanged("ContentScale");
            }
        }
        public Double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;

                OnPropertyChanged("ContentOffsetX");
            }
        }
        public Double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;

                OnPropertyChanged("ContentOffsetY");
            }
        }
        public Double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value;

                OnPropertyChanged("ContentWidth");
            }
        }
        public Double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value;

                OnPropertyChanged("ContentHeight");
            }
        }
        public Double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;

                OnPropertyChanged("ContentViewportWidth");
            }
        }
        public Double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;

                OnPropertyChanged("ContentViewportHeight");
            }
        }
        public MessagePathViewModel ConnectionDragStarted(EntryViewModel draggedOutConnector, Point curDragPoint)
        {
            var connection = new MessagePathViewModel();

            if (draggedOutConnector.Type == EntryType.Message)
            {
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }
            this.Network.Connections.Add(connection);

            return connection;
        }
        public void QueryConnnectionFeedback(EntryViewModel draggedOutConnector, EntryViewModel draggedOverConnector, out Object feedbackIndicator, out Boolean connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                connectionOk = false;
            }
            else
            {
                var sourceConnector = draggedOutConnector;
                var destConnector = draggedOverConnector;
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
                                 sourceConnector.Type != destConnector.Type;
            }

            feedbackIndicator = null;
        }
        public void ConnectionDragging(Point curDragPoint, MessagePathViewModel connection)
        {
            if (connection.DestConnector == null)
            {
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }
        public void ConnectionDragCompleted(MessagePathViewModel newConnection, EntryViewModel connectorDraggedOut, EntryViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                this.Network.Connections.Remove(newConnection);
                return;
            }
            var connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
                                connectorDraggedOut.Type != connectorDraggedOver.Type;

            if (!connectionOk)
            {
                this.Network.Connections.Remove(newConnection);
                return;
            }
            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                this.Network.Connections.Remove(existingConnection);
            }
            if (newConnection.DestConnector == null)
            {
                newConnection.DestConnector = connectorDraggedOver;
            }
            else
            {
                newConnection.SourceConnector = connectorDraggedOver;
            }
        }
        public MessagePathViewModel FindConnection(EntryViewModel connector1, EntryViewModel connector2)
        {
            Trace.Assert(connector1.Type != connector2.Type);
            var sourceConnector = connector1.Type == EntryType.Message ? connector1 : connector2;
            var destConnector = connector1.Type == EntryType.Message ? connector2 : connector1;

            foreach (var connection in sourceConnector.AttachedConnections)
            {
                if (connection.DestConnector == destConnector)
                {
                    return connection;
                }
            }

            return null;
        }
        public void DeleteSelectedNodes()
        {
            var nodesCopy = this.Network.Nodes.ToArray();
            foreach (var node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    DeleteNode(node);
                }
            }
        }
        public void DeleteNode(ModuleViewModel node)
        {
            this.Network.Connections.RemoveRange(node.AttachedConnections);
            this.Network.Nodes.Remove(node);
        }

        public ModuleViewModel CreateNode(Module m, Point nodeLocation, Boolean centerNode)
        {
            if (Network == null)
            {
                Network = new ModuleNetworkViewModel();
            }

            var node = new ModuleViewModel(m)
            {
                X = nodeLocation.X,
                Y = nodeLocation.Y,
            };


            if (centerNode)
            {
                EventHandler<EventArgs> sizeChangedEventHandler = null;
                sizeChangedEventHandler =
                    delegate(Object sender, EventArgs e)
                    {
                        node.X -= node.Size.Width / 2;
                        node.Y -= node.Size.Height / 2;
                        node.SizeChanged -= sizeChangedEventHandler;
                    };
                node.SizeChanged += sizeChangedEventHandler;
            }
            this.Network.Nodes.Add(node);

            return node;
        }
        public void DeleteConnection(MessagePathViewModel connection)
        {
            this.Network.Connections.Remove(connection);
        }
    }
}
