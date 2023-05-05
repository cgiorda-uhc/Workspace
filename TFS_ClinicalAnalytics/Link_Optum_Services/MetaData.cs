using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link_Optum_Services
{


    //documents="{'space_id':8888,'external_id':'11hello1234','metadata':{'fileName' ': 'Ganesh Test Doc Vault Functionality.pdf',  'fileDescription' : 'Test Doc Vault','createApplication' : 'FDS Test',  'expiryDate' : '2018-08-09T00:00:00.000-05:00',  'filePath' : 'EOC/FEB2017',  'tenant' : 'LINK',  'fileType' : 'Report',  'category' : 'Episodes of Care',  'subCategory' : 'Provider Report',  'privilege' : 'Patient Eligibility and Benefits',  'organization' : 'Optum Technologies',  'corporateMpin' : 'ALL',  'tin' : 'ALL',  'notificationReq' : 'Y',  'emailAlertReq' : 'N',  'testHarness' : 'true'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'outputMergedFile.pdf'}}}" -F test01 = @test1.doc - F test02=@"test2.txt" --header "Authorization: bearer $token" $url/v2/documents

    public class Documents
    {
        [JsonProperty(PropertyName = "documents")]
        public Document documents { get; set; }
    }


    public class Document
    {
        [JsonProperty(PropertyName = "space_id")]
        public int space_id { get; set; }


        [JsonProperty(PropertyName = "external_id")]
        public string external_id { get; set; }


        [JsonProperty(PropertyName = "metadata")]
        public Metadata metadata { get; set; }

    }

    public class Metadata
    {
        [JsonProperty(PropertyName = "fileName")]
        public string fileName { get; set; }

        [JsonProperty(PropertyName = "fileDescription")]
        public string fileDescription { get; set; }
    }

    
    
    


    class MetaData
    {

        public  string fileName { get; set; }
        public string fileDescription { get; set; }
        public string createdDate { get; set; }
        public string createApplication { get; set; }
        public string expiryDate { get; set; }
        public string filePath { get; set; }
        public string tenant { get; set; }
        public string claimNumber { get; set; }
        public string fileType { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public string privilege { get; set; }
        public string organization { get; set; }
        public string corporateMpin { get; set; }
        public string tin { get; set; }
        public string mpin { get; set; }
        public string memberId { get; set; }
        public string notificationReq { get; set; }
        public string emailAlertReq { get; set; }
        public string physicianName { get; set; }
        public string policyNumber { get; set; }
        public string employeeName { get; set; }
        public string dateOfService { get; set; }
        public string memberName { get; set; }
        public string providerEmailId { get; set; }
        public string frequency { get; set; }
        public string weekDay { get; set; }

    }
}
