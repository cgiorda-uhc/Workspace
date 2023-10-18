using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace VCPortal_WPF.Components;
public class SelectAllCheckComboBox : CheckComboBox
{
    protected override void UpdateText()
    {
        //Do not display the “Select All” in the TextBox.
        var selectedItemsList = this.SelectedItems.Cast<object>().Select(x => this.GetItemDisplayValue(x)).Where(x => !x.Equals("--All--"));

        var newValue = String.Join(this.Delimiter, selectedItemsList);
        if (String.IsNullOrEmpty(this.Text) || !this.Text.Equals(newValue))
        {
            this.SetCurrentValue(CheckComboBox.TextProperty, newValue);
        }

        //this.SelectAll();
    }
}



