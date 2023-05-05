using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace VCPortal_WPF.Shared
{
    public class CustomFilteringBehavior : FilteringBehavior
    {
        public override IEnumerable<object> FindMatchingItems(string searchText, IList items, IEnumerable<object> escapedItems, string textSearchPath, TextSearchMode textSearchMode)
        {
            var result = base.FindMatchingItems(searchText, items, escapedItems, textSearchPath, textSearchMode) as IEnumerable<object>;

            if (string.IsNullOrEmpty(searchText) || !result.Any())
            {
                return ((IEnumerable<object>)items).Where(x => !escapedItems.Contains(x));
            }
            return result;
        }
    }
}
