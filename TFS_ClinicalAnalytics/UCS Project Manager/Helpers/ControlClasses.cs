using System.Windows.Controls;
using System;

namespace UCS_Project_Manager
{
    //ADDS SORTED EVENT FOR DG - NOT USED YET ;-)
    public class ValueEventArgs<T> : EventArgs
    {
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; set; }

    }

    public class DataGridExt : DataGrid
    {
        public event EventHandler<ValueEventArgs<DataGridColumn>> Sorted;

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            base.OnSorting(eventArgs);

            if (Sorted == null) return;
            var column = eventArgs.Column;
            Sorted(this, new ValueEventArgs<DataGridColumn>(column));
        }
    }
}
