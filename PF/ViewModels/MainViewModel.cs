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
            WayPoints1 = new ObservableCollection<WayPoint>();

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

        private ObservableCollection<WayPoint> _wayPoints1;

        public ObservableCollection<WayPoint> WayPoints1
        {
            get => _wayPoints1;
            set
            {
                _wayPoints1 = value;
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

            SetFromPoint(position);
        }

        private ICommand _mouseMoveCommand;
        public ICommand MouseMoveCommand => _mouseMoveCommand ??= new RelayCommand(MouseMove);

        private void MouseMove(object commandParameter)
        {
            MouseEventArgs e = (MouseEventArgs)commandParameter;

            object source = e.Source;

            if (source is not Canvas canvas) return;

            Point position = e.GetPosition(canvas);

            SetToPoint(position);
        }

        private void SetToPoint(Point position)
        {
            if (WayPoints.Count == 1 && WayPoints1.Count == 0)
            {
                WayPoint wayPoint = new()
                {
                    Position = position
                };

                WayPoints1.Add(wayPoint);
            }

            if (WayPoints.Count == 1)
            {
                var from = WayPoints.FirstOrDefault();
                var to = position;

                FindPath(from.Position, to);
            }
        }

        private void SetFromPoint(Point position)
        {
            WayPoints.Clear();

            WayPoint wayPoint = new()
            {
                Position = position
            };

            WayPoints.Add(wayPoint);
        }

        private async void FindPath(Point from, Point to)
        {
            List<Area> areas = new(Areas);

            List<Point> wayPointsPosition = AStarAlgorithm.FindPath(areas, from, to);

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

            Way way = new ();

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

            int width = 150;
            int height = 150;

            Areas.Clear();

            for (int i = 0; i < 48; i++)
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

                startX += 150;

                if (startX >= 1400)
                {
                    startX = 0;
                    startY += 150;
                }

                Areas.Add(area);
            }
        }
    }
}
