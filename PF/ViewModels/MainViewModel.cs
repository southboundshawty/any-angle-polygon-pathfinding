using PF.Core;
using PF.Models;
using PF.PathFinding.AStar;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Ways = new ObservableCollection<Way>();

            Areas = new ObservableCollection<Area>();

            WayPoints = new ObservableCollection<WayPoint>();

            AreasCollection = CollectionViewSource.GetDefaultView(Areas);
        }

        private ObservableCollection<WayPoint> _wayPoints;

        public ObservableCollection<WayPoint> WayPoints
        {
            get => _wayPoints;
            set
            {
                _wayPoints = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Way> _ways;

        public ObservableCollection<Way> Ways
        {
            get => _ways;
            set
            {
                _ways = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Area> _areas;

        public ObservableCollection<Area> Areas
        {
            get => _areas;
            init
            {
                _areas = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _areasCollection;

        public ICollectionView AreasCollection
        {
            get => _areasCollection;
            set
            {
                _areasCollection = value;
                OnPropertyChanged();
            }
        }

        private WayPoint _from;

        public WayPoint From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged();
            }
        }

        private WayPoint _to;

        public WayPoint To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged();
            }
        }

        private ICommand _setFromPointCommand;
        public ICommand SetFromPointCommand => _setFromPointCommand ??= new RelayCommand(SetFromPoint);

        private void SetFromPoint(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs)commandParameter;

            object source = e.Source;

            if (source is not Canvas canvas) return;

            Point position = e.GetPosition(canvas);

            SetWayPoint(position);
        }

        private ICommand _setPointCommand;
        public ICommand SetToPointCommand => _setPointCommand ??= new RelayCommand(SetToPoint);

        private void SetToPoint(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs)commandParameter;

            object source = e.Source;

            if (source is not Canvas canvas) return;

            Point position = e.GetPosition(canvas);

            SetWayPoint(position);
        }

        private void SetWayPoint(Point position)
        {
            if(WayPoints.Count == 2)
                WayPoints.Clear();

            WayPoint wayPoint = new()
            {
                Position = position
            };

            WayPoints.Add(wayPoint);

            if (WayPoints.Count == 1)
                From = wayPoint;

            if (WayPoints.Count == 2)
            {
                To = wayPoint;
                FindPath();
            }
        }

        private void FindPath()
        {
            AStarAlgorithm aStar = new();

            List<Area> areas = new(Areas);
            Point from = From.Position;
            Point to = To.Position;

            List<Point> wayPointsPosition = aStar.FindPath(areas, from, to);

            if (wayPointsPosition is null)
            {
                MessageBox.Show("Нет пути");

                return;
            }

            List<WayPoint> wayPoints = new();

            foreach (Point waPointPosition in wayPointsPosition)
            {
                WayPoint wayPoint = new()
                {
                    Position = waPointPosition
                };

                wayPoints.Add(wayPoint);
            }

            Ways.Clear();
            Way way = new Way();

            foreach (WayPoint wayPoint in wayPoints)
            {
                way.WayPoints.Add(wayPoint);
            }

            Ways.Add(way);
        }

        private ICommand _generateRandomAreasCommand;
        public ICommand GenerateRandomAreasCommand => _generateRandomAreasCommand ??= new RelayCommand(GenerateRandomAreas);

        private void GenerateRandomAreas(object commandParameter)
        {
            Random random = new(DateTime.Now.Millisecond);

            int startX = 192;
            int startY = 0;

            int width = 200;
            int height = 200;

            Areas.Clear();

            for (int i = 0; i < 19; i++)
            {
                Area area = new()
                {
                    IsClosed = true
                };

                int pointsCount = random.Next(3, 5);

                for (int j = 0; j < pointsCount; j++)
                {
                    int x = random.Next(startX, startX + width);
                    int y = random.Next(startY, startY + height);

                    ShapePoint shapePoint = new()
                    {
                        Position = new Point(x, y)
                    };

                    area.ShapePoints.Add(shapePoint);
                }

                startX += 200;

                if (startX >= 1200)
                {
                    startX = 0;
                    startY += 200;
                }

                Areas.Add(area);
            }
        }
    }
}
