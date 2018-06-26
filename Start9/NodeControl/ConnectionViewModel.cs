using System;
using Utils;
using System.Windows.Media;
using System.Windows;

namespace Start9.NodeControl
{
    public sealed class MessagePathViewModel : AbstractModelBase
    {
        private EntryViewModel sourceConnector = null;

        private EntryViewModel destConnector = null;

        private Point sourceConnectorHotspot;
        private Point destConnectorHotspot;

        private PointCollection points = null;

        public EntryViewModel SourceConnector
        {
            get => sourceConnector;
            set
            {
                if (sourceConnector == value)
                {
                    return;
                }

                if (sourceConnector != null)
                {
                    sourceConnector.AttachedConnections.Remove(this);
                    sourceConnector.HotspotUpdated -= new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
                }

                sourceConnector = value;

                if (sourceConnector != null)
                {
                    sourceConnector.AttachedConnections.Add(this);
                    sourceConnector.HotspotUpdated += new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
                    this.SourceConnectorHotspot = sourceConnector.Hotspot;
                }

                OnPropertyChanged(nameof(SourceConnector));
                ConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public EntryViewModel DestConnector
        {
            get => destConnector;
            set
            {
                if (destConnector == value)
                {
                    return;
                }

                if (destConnector != null)
                {
                    destConnector.AttachedConnections.Remove(this);
                    destConnector.HotspotUpdated -= new EventHandler<EventArgs>(destConnector_HotspotUpdated);
                }

                destConnector = value;

                if (destConnector != null)
                {
                    destConnector.AttachedConnections.Add(this);
                    destConnector.HotspotUpdated += new EventHandler<EventArgs>(destConnector_HotspotUpdated);
                    this.DestConnectorHotspot = destConnector.Hotspot;
                }

                OnPropertyChanged(nameof(DestConnector));
                ConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public Point SourceConnectorHotspot
        {
            get => sourceConnectorHotspot;
            set
            {
                sourceConnectorHotspot = value;

                ComputeConnectionPoints();

                OnPropertyChanged(nameof(SourceConnectorHotspot));
            }
        }

        public Point DestConnectorHotspot
        {
            get => destConnectorHotspot;
            set
            {
                destConnectorHotspot = value;

                ComputeConnectionPoints();

                OnPropertyChanged(nameof(DestConnectorHotspot));
            }
        }

        public PointCollection Points
        {
            get => points;
            set
            {
                points = value;

                OnPropertyChanged(nameof(Points));
            }
        }

        public event EventHandler<EventArgs> ConnectionChanged;

        private void sourceConnector_HotspotUpdated(Object sender, EventArgs e)
        {
            this.SourceConnectorHotspot = this.SourceConnector.Hotspot;
        }

        private void destConnector_HotspotUpdated(Object sender, EventArgs e)
        {
            this.DestConnectorHotspot = this.DestConnector.Hotspot;
        }

        private void ComputeConnectionPoints()
        {
            var computedPoints = new PointCollection();
            computedPoints.Add(this.SourceConnectorHotspot);

            var deltaX = Math.Abs(this.DestConnectorHotspot.X - this.SourceConnectorHotspot.X);
            var deltaY = Math.Abs(this.DestConnectorHotspot.Y - this.SourceConnectorHotspot.Y);
            if (deltaX > deltaY)
            {
                var midPointX = this.SourceConnectorHotspot.X + ((this.DestConnectorHotspot.X - this.SourceConnectorHotspot.X) / 2);
                computedPoints.Add(new Point(midPointX, this.SourceConnectorHotspot.Y));
                computedPoints.Add(new Point(midPointX, this.DestConnectorHotspot.Y));
            }
            else
            {
                var midPointY = this.SourceConnectorHotspot.Y + ((this.DestConnectorHotspot.Y - this.SourceConnectorHotspot.Y) / 2);
                computedPoints.Add(new Point(this.SourceConnectorHotspot.X, midPointY));
                computedPoints.Add(new Point(this.DestConnectorHotspot.X, midPointY));
            }

            computedPoints.Add(this.DestConnectorHotspot);
            computedPoints.Freeze();

            this.Points = computedPoints;
        }
    }
}
