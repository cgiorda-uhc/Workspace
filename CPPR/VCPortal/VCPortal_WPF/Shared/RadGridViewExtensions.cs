using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace VCPortal_WPF.Shared;
/// <summary>
/// Extension methods for DataGrid
/// These methods are thanks to http://blogs.msdn.com/b/vinsibal/archive/2008/11/05/wpf-datagrid-new-item-template-sample.aspx
/// https://stackoverflow.com/questions/21006888/show-popup-during-editing-of-wpf-datagrid-cell
/// </summary>
public static class RadGridViewExtensions
{
    /// <summary>
    /// Returns a DataGridCell for the given row and column
    /// </summary>
    /// <param name="grid">The DataGrid</param>
    /// <param name="row">The zero-based row index</param>
    /// <param name="column">The zero-based column index</param>
    /// <returns>The requested DataGridCell, or null if the indices are out of range</returns>
    public static DataGridCell GetCell(this RadGridView grid, Int32 row, Int32 column)
    {
        GridViewRow gridrow = grid.GetRowForItem(row);
        if (gridrow != null)
        {
            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(gridrow);

            // try to get the cell but it may possibly be virtualized
            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
            if (cell == null)
            {
                // now try to bring into view and retreive the cell
                grid.ScrollIntoView(gridrow, grid.Columns[column]);

                cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
            }

            return (cell);
        }

        return (null);
    }

    /// <summary>
    /// Gets the DataGridRow based on the given index
    /// </summary>
    /// <param name="idx">The zero-based index of the container to get</param>
    public static GridViewRow GetRow(this RadGridView dataGrid, Int32 idx)
    {
        GridViewRow row = (GridViewRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(idx);
        if (row == null)
        {
            // may be virtualized, bring into view and try again
            dataGrid.ScrollIntoView(dataGrid.Items[idx]);
            dataGrid.UpdateLayout();

            row = (GridViewRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(idx);
        }

        return (row);
    }

    private static T GetVisualChild<T>(Visual parent) where T : Visual
    {
        T child = default(T);

        Int32 numvisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (Int32 i = 0; i < numvisuals; ++i)
        {
            Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
            child = v as T;
            if (child == null)
                child = GetVisualChild<T>(v);
            else
                break;
        }

        return child;
    }
}