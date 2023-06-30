using DocumentFormat.OpenXml.InkML;
using NPOI.OpenXmlFormats.Dml.Diagram;
using SharedFunctionsLibrary.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry;

namespace VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
public static class ETGFactSymmetryConfigMapper
{


    internal static List<ETGSummaryConfig> getETGSummaryConfig(IEnumerable<ETGFactSymmetryViewModel> esmList)
    {
        var targetList = esmList.Select(x => new ETGSummaryConfig()
        {
            ETG_Base_Class = x.ETG_Base_Class,
            ETG_Description = x.ETG_Description,
            Premium_Specialty = x.Premium_Specialty,
            Never_Map = (x.Never_Mapped == true ? 'Y' : 'N'),
            Never_Map_Previous = (x.Never_Mapped_Previous == true ? 'Y' : 'N'),
            Previous_Rx_NRx = x.RX_NRXPrevious,
            Current_Rx_NRx = x.RX_NRX,
            Previous_LOB = x.LOBPrevious,
            Current_LOB = x.LOB,
            PC_Current_Treatment_Indicator = x.PC_Treatment_Indicator,
            PC_Previous_Treatment_Indicator = x.PC_Treatment_Indicator_Previous,
            //PC_Episode_Cnt = x.PC_Episode_Count,
            //PC_Tot_Cost = x.PC_Total_Cost,
            //PC_Avg_Cost = x.PC_Average_Cost,
            //PC_CV = x.PC_Coefficients_of_Variation,
            PC_Spec_Episode_Cnt = x.PC_Spec_Episode_Count,
            PC_Spec_Episode_Distribution = Math.Round(x.PC_Spec_Episode_Distribution,4),
            PC_Spec_Perc_of_Episodes = Math.Round(x.PC_Spec_Percent_of_Episodes,4),
            PC_Spec_Tot_Cost = Math.Round(x.PC_Spec_Total_Cost,2),
            PC_Spec_Avg_Cost = Math.Round(x.PC_Spec_Average_Cost,2),
            PC_Spec_Normalized_Pricing = Math.Round(x.PC_Normalized_Pricing_Total_Cost,2),
            PC_Spec_CV = Math.Round(x.PC_Spec_CV, 4),
            PC_Prev_Attribution = x.PC_Attribution_Previous,
            PC_Current_Attribution = x.PC_Attribution,
            PC_Change_Comments = x.PC_Change_Comments,
            EC_Current_Treatment_Indicator = x.EC_Treatment_Indicator,
            EC_Previous_Treatment_Indicator = x.EC_Treatment_Indicator_Previous,
            //EC_Episode_Count = x.EC_Episode_Count,
            //EC_Tot_Cost = x.EC_Total_Cost,
            //EC_Avg_Cost = x.EC_Average_Cost,
            //EC_CV = x.EC_Coefficients_of_Variation,
            EC_Spec_Episode_Cnt = x.EC_Spec_Episode_Count,
            EC_Spec_Episode_Distribution = Math.Round(x.EC_Spec_Episode_Distribution, 4),
            EC_Spec_Perc_of_Episodes = Math.Round(x.EC_Spec_Percent_of_Episodes, 4),
            EC_Spec_Tot_Cost = Math.Round(x.EC_Spec_Total_Cost, 2),
            EC_Spec_Avg_Cost = Math.Round(x.EC_Spec_Average_Cost, 2),
            EC_Spec_Normalized_Pricing = Math.Round(x.EC_Normalized_Pricing_Total_Cost, 2),
            EC_Spec_CV = Math.Round(x.EC_Spec_CV, 4),
            EC_Previous_Mapping = x.EC_Mapping_Previous,
            EC_Current_Mapping = x.EC_Mapping,
            EC_Change_Comments = x.EC_Change_Comments,
            //Previous_Pt_Centric_Mapping = x.Patient_Centric_Mapping_Previous,
            //Current_Pt_Centric_Mapping = x.Patient_Centric_Mapping,
            //Pt_Centric_Change_Comments = x.Patient_Centric_Change_Comments,
            Measure_Status = x.PC_Measure_Status


        }).ToList();


        return targetList;

    }



    internal static List<ETGEpisodeCostConfig> getETGEpisodeCostConfig(IEnumerable<ETGFactSymmetryViewModel> esmList)
    {
        var targetList = esmList.Select(x => new ETGEpisodeCostConfig()
        {
            ETG_Base_Class = x.ETG_Base_Class,
            ETG_Description = x.ETG_Description,
            Premium_Specialty = x.Premium_Specialty,
            Current_Rx_NRx = x.RX_NRX,
            Current_LOB = x.LOB,
            EC_Current_Treatment_Indicator = x.EC_Treatment_Indicator,
            EC_Episode_Count = x.EC_Episode_Count,
            EC_Tot_Cost = x.EC_Total_Cost,
            EC_Avg_Cost = x.EC_Average_Cost,
            EC_CV = x.EC_Coefficients_of_Variation,
            EC_Spec_Episode_Cnt = x.EC_Spec_Episode_Count,
            EC_Spec_Episode_Distribution = x.EC_Spec_Episode_Distribution,
            EC_Spec_Perc_of_Episodes = x.EC_Spec_Percent_of_Episodes,
            EC_Spec_Tot_Cost = x.EC_Spec_Total_Cost,
            EC_Spec_Avg_Cost = x.EC_Spec_Average_Cost,
            EC_Spec_CV = x.EC_Spec_CV,
            EC_Current_Mapping = x.EC_Mapping,
            EC_Change_Comments = x.EC_Change_Comments,

        }).ToList();


        return targetList;

    }


}
