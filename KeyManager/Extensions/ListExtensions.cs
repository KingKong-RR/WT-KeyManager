using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeyManager.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IList<T> list)
        {
            foreach (var item in list)
                observableCollection.Add(item);
        }
    }
}
