using PF.Core;

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PF.Models
{
    public class Way : ObservableObject
    {
        public Way()
        {
            WayPoints = new TrulyObservableCollection<WayPoint>();

            WayPoints.CollectionChanged += WayPoints_CollectionChanged;

            Id = Guid.NewGuid().ToString();
        }

        private string _id;

        public string Id
        {
            get => _id;
            init
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private void WayPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Points));
        }

        private TrulyObservableCollection<WayPoint> _waysPoints;

        public TrulyObservableCollection<WayPoint> WayPoints
        {
            get => _waysPoints;
            init
            {
                _waysPoints = value;
                OnPropertyChanged();
            }
        }

        public PointCollection Points => new(WayPoints.Select(p => p.Position));

        private Point _position;

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }
    }
}
