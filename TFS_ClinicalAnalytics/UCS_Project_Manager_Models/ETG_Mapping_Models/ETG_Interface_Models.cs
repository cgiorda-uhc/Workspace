using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Project_Manager
{
    public class ETG_Interface_Models
    {
        public ETG_Interface_Models()
        {
            loadItems();
        }

        public List<string> lobOptionsArr;
        public List<string> treatmentIndicatorOptionsArr;
        public List<string> attributionOptionsArr;
        public List<string> treatmentIndicatorECOptionsArr;
        public List<string> mappingOptionsArr;
        public List<string> patientCentricMappingOptionsArr;
        public List<string> measureStatusOptionsArr;


        private void loadItems()
        {
            lobOptionsArr = new List<string>();
            lobOptionsArr.Add("Not Selected");
            lobOptionsArr.Add("All");
            lobOptionsArr.Add("Commercial + Medicare");
            lobOptionsArr.Add("Commercial + Medicaid");
            lobOptionsArr.Add("Medicare + Medicaid");
            lobOptionsArr.Add("Commercial Only");
            lobOptionsArr.Add("Medicare Only");
            lobOptionsArr.Add("Medicaid Only");


            treatmentIndicatorOptionsArr = new List<string>();
            treatmentIndicatorOptionsArr.Add("Not Selected");
            treatmentIndicatorOptionsArr.Add("All");
            treatmentIndicatorOptionsArr.Add("0");

            attributionOptionsArr = new List<string>();
            //attributionOptionsArr.Add("Not Selected");
            attributionOptionsArr.Add("Not Mapped");
            attributionOptionsArr.Add("Always Attributed");
            attributionOptionsArr.Add("If Involved");

            treatmentIndicatorECOptionsArr = new List<string>();
            treatmentIndicatorECOptionsArr.Add("Not Selected");
            treatmentIndicatorECOptionsArr.Add("0");
            treatmentIndicatorECOptionsArr.Add("0 & 1");

            mappingOptionsArr = new List<string>();
            //mappingOptionsArr.Add("Not Selected");
            mappingOptionsArr.Add("Mapped");
            mappingOptionsArr.Add("Not Mapped");

            patientCentricMappingOptionsArr = new List<string>();
            //patientCentricMappingOptionsArr.Add("Not Selected");
            patientCentricMappingOptionsArr.Add("Yes");
            patientCentricMappingOptionsArr.Add("No");

            measureStatusOptionsArr = new List<string>();
            measureStatusOptionsArr.Add("Added");
            measureStatusOptionsArr.Add("Inconsistent Mapping");
            measureStatusOptionsArr.Add("No Change");
            measureStatusOptionsArr.Add("Removed");


        }

    }



}
