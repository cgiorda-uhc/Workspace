using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;

namespace VCPortal_WPF.Components;
public class YesNoHyperlink : Hyperlink
{
    public string Question { get; set; }
    public bool CheckSaves { get; set; }

    protected override void OnClick()
    {
        if (string.IsNullOrWhiteSpace(Question))
        {
            base.OnClick();
            return;
        }

        bool showPrompt = true;
        if (CheckSaves)
        {
            int cnt = (SharedETGSymmObjects.ETGFactSymmetry_Tracking_List != null ? SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Count : 0);
            cnt += (SharedChemoObjects.ChemotherapyPX_Tracking_List != null ? SharedChemoObjects.ChemotherapyPX_Tracking_List.Count : 0);
            if (cnt <= 0)
            {
                showPrompt = false;
            }
        }

        if (showPrompt)
        {
            var messageBoxResult = MessageBox.Show(Question, "Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
                base.OnClick();
        }

    }
}

