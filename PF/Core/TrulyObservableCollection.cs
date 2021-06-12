﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PF.Core
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (T item in pItems)
                Add(item);
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (object item in e.NewItems)
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;

            if (e.OldItems != null)
                foreach (object item in e.OldItems)
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args =
                new(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));

            OnCollectionChanged(args);
        }
    }
}
