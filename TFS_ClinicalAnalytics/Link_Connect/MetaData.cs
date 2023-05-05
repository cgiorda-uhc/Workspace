using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Link_Connect
{
    public class MetaData
    {
        [DataMember(Name = "fileName")]
        public string fileName { get; set; }

        [DataMember(Name = "fileDescription")]
        public string fileDescription { get; set; }

        [DataMember(Name = "createdDate")]
        public string createdDate { get; set; }

        [DataMember(Name = "createApplication")]
        public string createApplication { get; set; }

        [DataMember(Name = "expiryDate")]
        public string expiryDate { get; set; }

        [DataMember(Name = "filePath")]
        public string filePath { get; set; }

        [DataMember(Name = "tenant")]
        public string tenant { get; set; }

        [DataMember(Name = "fileType")]
        public string fileType { get; set; }

        [DataMember(Name = "category")]
        public string category { get; set; }

        [DataMember(Name = "subCategory")]
        public string subCategory { get; set; }

        [DataMember(Name = "privilege")]
        public string privilege { get; set; }

        [DataMember(Name = "corporateMpin")]
        public string corporateMpin { get; set; }

        [DataMember(Name = "tin")]
        public string tin { get; set; }
        
    }

    public class RequestContainer
    {
        [DataMember(Name = "space_id")]
        public string space_id { get; set; }

        [DataMember(Name = "external_id")]
        public string external_id { get; set; }

        [DataMember(Name = "metadata")]
        public MetaData metadata { get; set; }

    }



    public class ResponseContainer
    {
         [JsonProperty("id")]
        public string id { get; set; }


         [JsonProperty("external_id")]
        public string external_id { get; set; }


         [JsonProperty("space_id")]
        public string space_id { get; set; }


         [JsonProperty("status")]
        public string status { get; set; }

         [JsonProperty("dateCreated")]
        public string dateCreated { get; set; }

         [JsonProperty("attachments")]
        public Attachments[] attachments { get; set; }

         [JsonProperty("metadata")]
        public MetaData metadata { get; set; }

    }


    public class Attachments
    {
         [JsonProperty("id")]
        public string id { get; set; }

         [JsonProperty("index")]
        public string index { get; set; }

         [JsonProperty("status")]
        public string status { get; set; }

         [JsonProperty("space_id")]
        public string space_id { get; set; }

         [JsonProperty("file_name")]
        public string file_name { get; set; }

         [JsonProperty("file_size")]
        public string file_size { get; set; }

         [JsonProperty("content_type")]
        public string content_type { get; set; }

         [JsonProperty("date_created")]
        public string date_created { get; set; }
    }




}
