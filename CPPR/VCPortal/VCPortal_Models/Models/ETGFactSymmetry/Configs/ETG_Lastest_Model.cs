using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETG_Lastest_Model
{
    public string Premium_Specialty { get; set; }
      public string ETG_Base_Class { get; set; }
      public int? EC_Treatment_Indicator { get; set; }
      public int? PC_Episode_Count { get; set; }
      public double? PC_Total_Cost { get; set; }
      public double? PC_Average_Cost { get; set; }
      public double? PC_Coefficients_of_Variation { get; set; }
      public int? PC_Normalized_Pricing_Episode_Count { get; set; }
      public double? PC_Normalized_Pricing_Total_Cost { get; set; }
      public int? PC_Spec_Episode_Count { get; set; }
      public double? PC_Spec_Total_Cost { get; set; }
      public double? PC_Spec_Average_Cost { get; set; }
      public double? PC_Spec_CV { get; set; }
      public double? PC_Spec_Percent_of_Episodes { get; set; }
      public int? PC_Spec_Normalized_Pricing_Episode_Count { get; set; }
      public double? PC_Spec_Normalized_Pricing_Total_Cost { get; set; }
      public double? PC_Spec_Epsd_Volume { get; set; }
      public int? EC_Episode_Count { get; set; }
      public double? EC_Total_Cost { get; set; }
      public double? EC_Average_Cost { get; set; }
      public double? EC_Coefficients_of_Variation { get; set; }
      public int? EC_Normalized_Pricing_Episode_Count { get; set; }
      public double? EC_Normalized_Pricing_Total_Cost { get; set; }
      public int? EC_Spec_Episode_Count { get; set; }
      public double? EC_Spec_Total_Cost { get; set; }
      public double? EC_Spec_Average_Cost { get; set; }
      public double? EC_Spec_Coefficients_of_Variation { get; set; }
      public double? EC_Spec_Percent_of_Episodes { get; set; }
      public int? EC_Spec_Normalized_Pricing_Episode_Count { get; set; }
      public double? EC_Spec_Normalized_Pricing_Total_Cost { get; set; }
      public string EC_CV3 { get; set; }
      public string EC_Spec_Episode_Volume { get; set; }
      public string PD_Mapped { get; set; }
      public string PC_CV3 { get; set; }
      public string RX_NRX { get; set; }
      public string Has_RX { get; set; }
      public string Has_NRX { get; set; }
      public string RX { get; set; }
      public string NRX { get; set; }


}
