using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PF.Core;

namespace PF.Models
{
    public class AreaPoint : ObservableObject
    {
        public AreaPoint()
        {
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

        private ICommand _onDragCompletedCommand;

        public ICommand OnDragCompletedCommand => _onDragCompletedCommand
            ??= new RelayCommand(OnDragCompleted);

        private ICommand _onDragCommand;

        public ICommand OnDragCommand => _onDragCommand
            ??= new RelayCommand(OnDrag);

        private void OnDrag(object commandParameter)
        {
            DragDeltaEventArgs e = (DragDeltaEventArgs)commandParameter;

            AreaPoint areaPoint = (AreaPoint)((FrameworkElement)e.Source).DataContext;

            double x = areaPoint.Position.X + e.HorizontalChange;
            double y = areaPoint.Position.Y + e.VerticalChange;

            areaPoint.Position = new Point(x, y);
        }

        private async void OnDragCompleted(object commandParameter)
        {
            //await _areaPointInteractor.Update(this);
        }
    }
}
