using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PF.Core;

namespace PF.Models
{
    public class Area : ObservableObject
    {
        public Area()
        {
            ShapePoints = new TrulyObservableCollection<ShapePoint>();

            ShapePoints.CollectionChanged += ShapePoints_CollectionChanged;

            Id = Guid.NewGuid().ToString();
        }

        private void ShapePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Points));
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

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _isClosed;

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                _isClosed = value;
                OnPropertyChanged();
            }
        }

        private TrulyObservableCollection<ShapePoint> _shapePoints;

        public TrulyObservableCollection<ShapePoint> ShapePoints
        {
            get => _shapePoints;
            init
            {
                _shapePoints = value;
                OnPropertyChanged();
            }
        }

        public PointCollection Points => new (ShapePoints.Select(p => p.Position));

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
