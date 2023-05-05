using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCS_Project_Manager;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace UCS_Project_Manager_Services
{
    public interface IETG_Fact_Symmetry_Repository
    {
        List<ETG_Fact_Symmetry_Model> GetETGFactSymmetry();
        List<ETG_Fact_Symmetry_Model> GetETGFactSymmetry(string strData_Period);
        List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetry(string strCurrent_Data_Period, string strPrevious_Data_Period);
        //List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetrySQL(string strCurrent_Data_Period, string strPrevious_Data_Period);

        List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetrySQL(string strCurrent_Symmetry_Version, string strPrevious_Symmetry_Version);
        Task<List<ETG_Fact_Symmetry_Model>> GetETGFactSymmetryAsync();
        Task<ETG_Fact_Symmetry_Model> GetETGFactSymmetryAsync(Int64 id);
        Task<ETG_Fact_Symmetry_Interface_Model> AddETGFactSymmetryAsync(ETG_Fact_Symmetry_Interface_Model ETG_Fact_Symmetry);
        Task<ETG_Fact_Symmetry_Interface_Model> UpdateETGFactSymmetryAsync(ETG_Fact_Symmetry_Interface_Model ETG_Fact_Symmetry);

        List<ETG_Fact_Symmetry_Update_Tracker> GetETGFactSymmetryUpdatesSQL(string strCurrent_Data_Period, string strPrevious_Data_Period);



        List<ETG_Fact_Symmetry_PateintCentric> GetETGFactSymmetryPATIENT_CENTRIC_CONFIGSQL();
        List<ETG_Fact_Symmetry_Config_Model> GetETGFactSymmetryPOP_EPISODE_CONFIGSQL();

        List<ETG_Fact_Symmetry_RxNrxConfig_Model> GetETGFactSymmetryRX_NRX_CONFIGSQL();

        Task UpdateETGFactSymmetrySQLAsync(List<ETG_Fact_Symmetry_Update_Tracker> amc, string strUser);

        Task DeleteETGFactSymmetryModelAsync(Int64 id);
        List<ETG_Data_Date> GetDataDates();


        List<ETG_Symmetry_Verion> GetSymmetryVersion();

        /////////////////////////////////////////////////////////////////////////////////////////

        List<ETG_Dim_Premium_Spec_Master_Model> GetETGDimPremiumSpec();
        Task<List<ETG_Dim_Premium_Spec_Master_Model>> GetETGDimPremiumSpecAsync();
        Task<ETG_Dim_Premium_Spec_Master_Model> GetETGDimPremiumSpecAsync(Int64 id);
        Task<ETG_Dim_Premium_Spec_Master_Model> AddETGDimPremiumSpecAsync(ETG_Dim_Premium_Spec_Master_Model Premium_Spec_Master);
        Task<ETG_Dim_Premium_Spec_Master_Model> UpdateETGDimPremiumSpecAsync(ETG_Dim_Premium_Spec_Master_Model Premium_Spec_Master);
        Task DeleteETGDimPremiumSpecModelAsync(Int64 id);


        /////////////////////////////////////////////////////////////////////////////////////////

        List<ETG_Dim_Master_Model> GetETGDimMaster();
        Task<List<ETG_Dim_Master_Model>> GetETGDimMasterAsync();
        Task<ETG_Dim_Master_Model> GetETGDimMasterAsync(string id);
        Task<ETG_Dim_Master_Model> AddETGDimMasterAsync(ETG_Dim_Master_Model ETG_Dim_Master);
        Task<ETG_Dim_Master_Model> UpdateETGDimMasterAsync(ETG_Dim_Master_Model ETG_Dim_Master);
        Task DeleteETGDimMasterModelAsync(string id);



        ///////////////////////////////////////////////////////////////////////////////////////////

        //List<ETG_Dim_LOB_Model> GetETGDimLOB();
        //Task<List<ETG_Dim_LOB_Model>> GetETGDimLOBAsync();
        //Task<ETG_Dim_LOB_Model> GetETGDimLOBAsync(Int16 id);
        //Task<ETG_Dim_LOB_Model> AddETGDimLOBAsync(ETG_Dim_LOB_Model ETG_Dim_LOB);
        //Task<ETG_Dim_LOB_Model> UpdateETGDimLOBAsync(ETG_Dim_LOB_Model ETG_Dim_LOB);
        //Task DeleteETGDimLOBModelAsync(Int16 id);



    }


    public class ETG_Fact_Symmetry_Repository : IETG_Fact_Symmetry_Repository
    {
        private _EFMainContext _context = new _EFMainContext();

        public ETG_Fact_Symmetry_Repository()
        {


            //string strPWPath = ConfigurationManager.AppSettings["PWPath"];
            //string strPW = Base64Decode(File.ReadAllText(strPWPath));

            //_context.Database.Connection.ConnectionString = _context.Database.Connection.ConnectionString.Replace("{$PW}", strPW);
            //_context.Database.Connection.Open();
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //CHRIS ADDED NON ASYNC
        public List<ETG_Fact_Symmetry_Model> GetETGFactSymmetry()
        {

            //var a = _context.ETG_Fact_Symmetry.Where(c => c.ETG_Fact_Symmetry_id == 1033 || c.ETG_Fact_Symmetry_id == 6406 || c.ETG_Fact_Symmetry_id == 5 || c.ETG_Fact_Symmetry_id == 26).ToList();
            var a = _context.ETG_Fact_Symmetry.ToList();
            return a;
        }

        public List<ETG_Fact_Symmetry_Model> GetETGFactSymmetry(string strData_Period)
        {

            var query = (from t in _context.ETG_Fact_Symmetry
                         where t.Data_Period == strData_Period
                         select
                         new
                         {
                             //CASE STATEMENTS HERE
                             ETG_Fact_Symmetry_id = t.ETG_Fact_Symmetry_id,
                             ETG_Base_Class = t.ETG_Base_Class,
                             ETG_Dim_Master_Model = t.ETG_Dim_Master_Model,
                             Premium_Specialty_id = t.Premium_Specialty_id,
                             ETG_Dim_Premium_Spec_Master_Model = t.ETG_Dim_Premium_Spec_Master_Model,
                             Pop_Cost_Treatment_Indicator = t.Pop_Cost_Treatment_Indicator,
                             Attribution = t.Attribution,
                             Pop_Cost_Change_Comments = t.Pop_Cost_Change_Comments,
                             Episode_Cost_Treatment_Indicator = t.Episode_Cost_Treatment_Indicator,
                             Mapping = t.Mapping,
                             Episode_Cost_Change_Comments = t.Episode_Cost_Change_Comments,
                             Patient_Centric_Mapping = t.Patient_Centric_Mapping,
                             Data_Date = t.Data_Date,
                             Data_Period = t.Data_Period,
                             has_Commercial = t.has_Commercial,
                             has_Medicare = t.has_Medicare,
                             has_Medicaid = t.has_Medicaid

                         }).ToList().Select(x => new ETG_Fact_Symmetry_Model //MAP ANONYMOUS MODEL BACK TO ITSELF
                         {
                             ETG_Fact_Symmetry_id = x.ETG_Fact_Symmetry_id,
                             ETG_Base_Class = x.ETG_Base_Class,
                             ETG_Dim_Master_Model = x.ETG_Dim_Master_Model,
                             Premium_Specialty_id = x.Premium_Specialty_id,
                             ETG_Dim_Premium_Spec_Master_Model = x.ETG_Dim_Premium_Spec_Master_Model,
                             Pop_Cost_Treatment_Indicator = x.Pop_Cost_Treatment_Indicator,
                             Attribution = x.Attribution,
                             Pop_Cost_Change_Comments = x.Pop_Cost_Change_Comments,
                             Episode_Cost_Treatment_Indicator = x.Episode_Cost_Treatment_Indicator,
                             Mapping = x.Mapping,
                             Episode_Cost_Change_Comments = x.Episode_Cost_Change_Comments,
                             Patient_Centric_Mapping = x.Patient_Centric_Mapping,
                             Data_Date = x.Data_Date,
                             Data_Period = x.Data_Period,
                             has_Commercial = x.has_Commercial,
                             has_Medicare = x.has_Medicare,
                             has_Medicaid = x.has_Medicaid

                         });

            return query.ToList();

        }

        public List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetry(string strCurrent_Data_Period, string strPrevious_Data_Period)
        {


            //var current = _context.ETG_Fact_Symmetry.Where(c => c.Data_Period == strCurrent_Data_Period);
            //var previous = _context.ETG_Fact_Symmetry.Where(p => p.Data_Period == strPrevious_Data_Period);

            //var queryFinal = (from c in current
            //                  from p in previous
            //                  where c.ETG_Base_Class == p.ETG_Base_Class && c.Premium_Specialty_id == p.Premium_Specialty_id
            //                  select
            //                   new ETG_Fact_Symmetry_Interface_Model
            //                   {

            //                       ETG_Fact_Symmetry_id = c.ETG_Fact_Symmetry_id,
            //                       ETG_Base_Class = c.ETG_Base_Class,
            //                       //ETG_Dim_Master_Model = c.ETG_Dim_Master_Model,
            //                       Premium_Specialty_id = c.Premium_Specialty_id,
            //                       //ETG_Dim_Premium_Spec_Master_Model = c.ETG_Dim_Premium_Spec_Master_Model,
            //                       //Pop_Cost_Treatment_Indicator = c.Pop_Cost_Treatment_Indicator,
            //                       //Attribution = c.Attribution,
            //                       Pop_Cost_Change_Comments = c.Pop_Cost_Change_Comments,
            //                       //Episode_Cost_Treatment_Indicator = c.Episode_Cost_Treatment_Indicator,
            //                       //Current_Mapping = c.Current_Mapping,
            //                       Episode_Cost_Change_Comments = c.Episode_Cost_Change_Comments,
            //                       //Current_Patient_Centric_Mapping = c.Current_Patient_Centric_Mapping,
            //                       LOBCurrentString = c.LOBString,
            //                       LOBPreviousString = p.LOBString


            //                   }).ToList();


            return null;


        }

        //public List<ETGFactSymmetryFilters> ETGFactSymmetryFilters;
        //public List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetrySQL(string strCurrent_Data_Period, string strPrevious_Data_Period)
        //{

        //    StringBuilder sbSQL = new StringBuilder();

        //    sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_Main_Interface v WHERE v.Curent_Data_Period = " + strCurrent_Data_Period + " AND v.Previous_Data_Period = " + strPrevious_Data_Period + " ");
        //    sbSQL.Append(" ORDER BY Premium_Specialty, ETG_Description ");

        //    var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_Interface_Model>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_Interface_Model>();

        //    return list;

        //}

        public List<ETG_Fact_Symmetry_Interface_Model> GetETGFactSymmetrySQL(string strCurrent_Symmetry_Version, string strPrevious_Symmetry_Version)
        {

            StringBuilder sbSQL = new StringBuilder();



            sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_Main_Interface v ORDER BY Premium_Specialty, ETG_Description ");


            //sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_Main_Interface v WHERE v.Current_Symmetry_Version = " + strCurrent_Symmetry_Version + " AND v.Previous_Symmetry_Version = " + strPrevious_Symmetry_Version + " ");
            //sbSQL.Append(" ORDER BY Premium_Specialty, ETG_Description ");

            //sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_Main_Interface v WHERE v.Current_Symmetry_Version = " + strCurrent_Symmetry_Version + " AND (v.Previous_Symmetry_Version = " + strPrevious_Symmetry_Version + " OR v.Previous_Symmetry_Version = " + strCurrent_Symmetry_Version + "  )");
            //sbSQL.Append(" ORDER BY Premium_Specialty, ETG_Description ");



            var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_Interface_Model>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_Interface_Model>();

            return list;

        }


        public List<ETG_Fact_Symmetry_Update_Tracker> GetETGFactSymmetryUpdatesSQL(string strCurrent_Data_Period, string strPrevious_Data_Period)
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT[ETG_Fact_Symmetry_id] ,[Current_Patient_Centric_Mapping] ,[Previous_Patient_Centric_Mapping] ,[Current_Mapping] ,[Previous_Mapping] ,[Current_Episode_Cost_Treatment_Indicator] ,[Previous_Episode_Cost_Treatment_Indicator] ,[Current_Attribution] ,[Previous_Attribution] ,[Pop_Cost_Current_Treatment_Indicator] ,[Pop_Cost_Previous_Treatment_Indicator] ,[LOBCurrentString] ,[LOBPreviousString] ,[Pop_Cost_Change_Comments] ,[Episode_Cost_Change_Comments] ,[username] ,[Patient_Centric_Change_Comments] FROM [CSG].[ETG_Fact_Symmetry_Update_Tracker] WHERE [ETG_Fact_Symmetry_id] in (SELECT[ETG_Fact_Symmetry_id] FROM [CSG].VW_ETG_Symmetry_Main_Interface v); ");


            //sbSQL.Append("SELECT[ETG_Fact_Symmetry_id] ,[Current_Patient_Centric_Mapping] ,[Previous_Patient_Centric_Mapping] ,[Current_Mapping] ,[Previous_Mapping] ,[Current_Episode_Cost_Treatment_Indicator] ,[Previous_Episode_Cost_Treatment_Indicator] ,[Current_Attribution] ,[Previous_Attribution] ,[Pop_Cost_Current_Treatment_Indicator] ,[Pop_Cost_Previous_Treatment_Indicator] ,[LOBCurrentString] ,[LOBPreviousString] ,[Pop_Cost_Change_Comments] ,[Episode_Cost_Change_Comments] ,[username] ,[Patient_Centric_Change_Comments] FROM [CSG].[ETG_Fact_Symmetry_Update_Tracker] WHERE[ETG_Fact_Symmetry_id] in (SELECT[ETG_Fact_Symmetry_id] FROM [CSG].VW_ETG_Symmetry_Main_Interface v WHERE v.Curent_Data_Period = " + strCurrent_Data_Period + " AND v.Previous_Data_Period = " + strPrevious_Data_Period + "); ");

            //sbSQL.Append("SELECT[ETG_Fact_Symmetry_id] ,[Current_Patient_Centric_Mapping] ,[Previous_Patient_Centric_Mapping] ,[Current_Mapping] ,[Previous_Mapping] ,[Current_Episode_Cost_Treatment_Indicator] ,[Previous_Episode_Cost_Treatment_Indicator] ,[Current_Attribution] ,[Previous_Attribution] ,[Pop_Cost_Current_Treatment_Indicator] ,[Pop_Cost_Previous_Treatment_Indicator] ,[LOBCurrentString] ,[LOBPreviousString] ,[Pop_Cost_Change_Comments] ,[Episode_Cost_Change_Comments] ,[username] ,[Patient_Centric_Change_Comments] FROM [CSG].[ETG_Fact_Symmetry_Update_Tracker] WHERE[ETG_Fact_Symmetry_id] in (SELECT[ETG_Fact_Symmetry_id] FROM [CSG].VW_ETG_Symmetry_Main_Interface v WHERE v.Curent_Data_Period = " + strCurrent_Data_Period + " AND (v.Previous_Data_Period = " + strPrevious_Data_Period + "  OR v.Previous_Data_Period = " + strCurrent_Data_Period + ")); ");

            var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_Update_Tracker>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_Update_Tracker>();


            return list;

        }

        public List<ETG_Fact_Symmetry_PateintCentric> GetETGFactSymmetryPATIENT_CENTRIC_CONFIGSQL()
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_PATIENT_CENTRIC_CONFIG v");
            sbSQL.Append(" ORDER BY  v.[Base_ETG],v.[Premium_Specialty] ");

            var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_PateintCentric>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_PateintCentric>();

            return list;

        }



        public List<ETG_Fact_Symmetry_Config_Model> GetETGFactSymmetryPOP_EPISODE_CONFIGSQL()
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_POP_EPISODE_CONFIG v");
            sbSQL.Append(" ORDER BY   v.[Base_ETG],v.[Premium_Specialty]");

            var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_Config_Model>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_Config_Model>();

            return list;

        }

        public List<ETG_Fact_Symmetry_RxNrxConfig_Model> GetETGFactSymmetryRX_NRX_CONFIGSQL()
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT * FROM CSG.VW_ETG_Symmetry_RX_NRX_CONFIG v");
            sbSQL.Append(" ORDER BY   v.[Base_ETG],v.[Premium_Specialty]");

            var list = _context.Database.SqlQuery<ETG_Fact_Symmetry_RxNrxConfig_Model>(sbSQL.ToString()).ToList<ETG_Fact_Symmetry_RxNrxConfig_Model>();

            return list;

        }


        public async Task UpdateETGFactSymmetrySQLAsync(List<ETG_Fact_Symmetry_Update_Tracker> amc, string strUser)
        {
            StringBuilder sbSQL = new StringBuilder();

            foreach (ETG_Fact_Symmetry_Update_Tracker ac in amc)
            {
                sbSQL.Append("INSERT INTO [CSG].[ETG_Fact_Symmetry_Update_Tracker] ([ETG_Fact_Symmetry_id] ,[Current_Patient_Centric_Mapping] ,[Previous_Patient_Centric_Mapping] ,[Current_Mapping] ,[Previous_Mapping] ,[Current_Mapping_Original] ,[Previous_Mapping_Original] ,[Current_Episode_Cost_Treatment_Indicator] ,[Previous_Episode_Cost_Treatment_Indicator] ,[Current_Attribution] ,[Previous_Attribution] ,[Pop_Cost_Current_Treatment_Indicator] ,[Pop_Cost_Previous_Treatment_Indicator] ,[LOBCurrentString] ,[LOBPreviousString] ,[Pop_Cost_Change_Comments] ,[Episode_Cost_Change_Comments] ,[Patient_Centric_Change_Comments], [username] ,[update_date]) VALUES (" + ac.ETG_Fact_Symmetry_id + ", '" + ac.Current_Patient_Centric_Mapping + "', '" + ac.Previous_Patient_Centric_Mapping + "', '" + ac.Current_Mapping + "', '" + ac.Previous_Mapping + "', '" + ac.Current_Mapping_Original + "', '" + ac.Previous_Mapping_Original + "', '" + ac.Current_Episode_Cost_Treatment_Indicator + "', '" + ac.Previous_Episode_Cost_Treatment_Indicator + "', '" + ac.Current_Attribution + "', '" + ac.Previous_Attribution + "', '" + ac.Pop_Cost_Current_Treatment_Indicator + "', '" + ac.Pop_Cost_Previous_Treatment_Indicator + "', '" + ac.LOBCurrentString + "', '" + ac.LOBPreviousString + "', " + (ac.Pop_Cost_Change_Comments == null ? "NULL" : "'" + ac.Pop_Cost_Change_Comments.Replace("'", "''") + "'") + ", " + (ac.Episode_Cost_Change_Comments == null ? "NULL" : "'" + ac.Episode_Cost_Change_Comments.Replace("'", "''") + "'") + ", " + (ac.Patient_Centric_Change_Comments == null ? "NULL" : "'" + ac.Patient_Centric_Change_Comments.Replace("'", "''") + "'") + ", '" + strUser + "', getDate());");


                sbSQL.Append("UPDATE [CSG].[ETG_Fact_Symmetry] SET [has_Commercial] = " + (ac.LOBCurrentString == "Not Selected" ? "NULL" : (ac.LOBCurrentString == "All" || ac.LOBCurrentString.Contains("Commercial") ? "1" : "0")) + ",[has_Medicare] = " + (ac.LOBCurrentString == "Not Selected" ? "NULL" : (ac.LOBCurrentString == "All" || ac.LOBCurrentString.Contains("Medicare") ? "1" : "0")) + ",[has_Medicaid] =" + (ac.LOBCurrentString == "Not Selected" ? "NULL" : (ac.LOBCurrentString == "All" || ac.LOBCurrentString.Contains("Medicaid") ? "1" : "0")) + " ,[Pop_Cost_Treatment_Indicator] = " + (ac.Pop_Cost_Current_Treatment_Indicator == "Not Selected" ? "NULL" : "'" + ac.Pop_Cost_Current_Treatment_Indicator + "'") + ",[Attribution] =" + (ac.Current_Attribution == "Not Selected" ? "NULL" : "'" + ac.Current_Attribution + "'") + " ,[Episode_Cost_Treatment_Indicator] = " + (ac.Current_Episode_Cost_Treatment_Indicator == "Not Selected" ? "NULL" : "'" + ac.Current_Episode_Cost_Treatment_Indicator + "'") + ",[Mapping] = " + (ac.Current_Mapping == "Not Selected" ? "NULL" : "'" + ac.Current_Mapping + "'") + ",[Patient_Centric_Mapping] = " + (ac.Current_Patient_Centric_Mapping == "Not Selected" ? "NULL" : "'" + ac.Current_Patient_Centric_Mapping + "'") + ",[Pop_Cost_Change_Comments] = " + (ac.Pop_Cost_Change_Comments == null ? "NULL" : "'" + ac.Pop_Cost_Change_Comments.Replace("'", "''") + "'") + ",[Episode_Cost_Change_Comments] = " + (ac.Episode_Cost_Change_Comments == null ? "NULL" : "'" + ac.Episode_Cost_Change_Comments.Replace("'", "''") + "'") + ",[Patient_Centric_Change_Comments] = " + (ac.Patient_Centric_Change_Comments == null ? "NULL" : "'" + ac.Patient_Centric_Change_Comments.Replace("'", "''") + "'") + ",[update_date] = getDate() ,[username] = '" + strUser + "'  WHERE ETG_Fact_Symmetry_id = " + ac.ETG_Fact_Symmetry_id + ";");



                sbSQL.Append("UPDATE [CSG].[ETG_Fact_Symmetry] SET [Mapping] = " + (ac.Previous_Mapping == "Not Selected" ? "NULL" : "'" + ac.Previous_Mapping + "'") + " WHERE ETG_Fact_Symmetry_id = " + ac.ETG_Fact_Symmetry_id_Previous + ";");
            }


            int result = await _context.Database.ExecuteSqlCommandAsync(sbSQL.ToString());

        }



        public List<ETG_Data_Date> GetDataDates()
        {

            var query = from t in _context.ETG_Fact_Symmetry
                        select
                        new ETG_Data_Date
                        {
                            Data_Date = t.Data_Date,
                            Data_Period = t.Data_Period
                        };

            return query.Distinct().OrderByDescending(q => q.Data_Date).ToList();

        }


        public List<ETG_Symmetry_Verion> GetSymmetryVersion()
        {

            var query = from t in _context.ETG_Fact_Symmetry
                        select
                        new ETG_Symmetry_Verion
                        {
                            Symmetry_Version = t.Symmetry_Version,
                            Data_Date = t.Data_Date,
                            Data_Period = t.Data_Period
                        };

            return query.Distinct().OrderByDescending(q => q.Symmetry_Version).ToList();

        }

        public async Task<List<ETG_Fact_Symmetry_Model>> GetETGFactSymmetryAsync()
        {
            //var f = await _context.ETG_Fact_Symmetry.ToListAsync();
            return await _context.ETG_Fact_Symmetry.ToListAsync();
            //           //var list = await _context.ETG_Fact_Symmetry.IncludeFilter(x => x.ETG_Fact_Symmetry_Models
            //           //           .OrderBy(y => y.ETG_Fact_Symmetry_id)
            //           //           .Take(10)).ToListAsync();


            //var engagementKeyTopic = (from ek in _context.engagement_key_topics.Where(en => en.engagement_key_topic_id == intKeyTopicId)
            //                          join k in context.key_topics on ek.key_topic_id equals k.key_topic_id into k_t
            //                          from k in k_t.DefaultIfEmpty()
            //                          select ek).FirstOrDefault();

        }

        public Task<ETG_Fact_Symmetry_Model> GetETGFactSymmetryAsync(Int64 id)
        {
            //var list = _context.ETG_Fact_Symmetry.IncludeFilter(x => x.ETG_Fact_Symmetry_Models.Where(y => y.ETG_Fact_Symmetry_id == id)
            //                                   .OrderBy(y => y.ETG_Fact_Symmetry_id)
            //                                   .Take(10)).FirstOrDefaultAsync();

            var list = _context.ETG_Fact_Symmetry.FirstOrDefaultAsync(c => c.ETG_Fact_Symmetry_id == id);
            return list;
        }

        public async Task<ETG_Fact_Symmetry_Interface_Model> AddETGFactSymmetryAsync(ETG_Fact_Symmetry_Interface_Model ETG_Fact_Symmetry_Model)
        {
            //ADDED 6222021 IN HOPES TO SPEED UP :(
            _context.Configuration.AutoDetectChangesEnabled = false;

            // _context.ETG_Fact_Symmetry.Add(ETG_Fact_Symmetry_Model);

            //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
            //BUT THIS FIXED KEY BUG :)
            _context.ChangeTracker.DetectChanges();

            await _context.SaveChangesAsync();
            return ETG_Fact_Symmetry_Model;
        }

        public async Task<ETG_Fact_Symmetry_Interface_Model> UpdateETGFactSymmetryAsync(ETG_Fact_Symmetry_Interface_Model ETG_Fact_Symmetry_Model)
        {
            //var id = ETG_Fact_Symmetry_Model.ETG_Fact_Symmetry_Models.FirstOrDefault().ETG_Fact_Symmetry_id;
            //if (!(await _context.ETG_Fact_Symmetry.IncludeFilter(x => x.ETG_Fact_Symmetry_Models.Where(y => y.ETG_Fact_Symmetry_id == id)).AnyAsync()))
            if (!(await _context.ETG_Fact_Symmetry.AnyAsync(c => c.ETG_Fact_Symmetry_id == ETG_Fact_Symmetry_Model.ETG_Fact_Symmetry_id)))
            {
                // _context.ETG_Fact_Symmetry.Attach(ETG_Fact_Symmetry_Model);
            }

            _context.Entry(ETG_Fact_Symmetry_Model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ETG_Fact_Symmetry_Model;

        }

        public async Task DeleteETGFactSymmetryModelAsync(Int64 id)
        {
            //var ETG_Fact_Symmetry_Model = _context.ETG_Fact_Symmetry.FirstOrDefault(c => c.Id == ETG_Fact_Symmetry_ModelId);
            //var ETG_Fact_Symmetry_Model = await _context.ETG_Fact_Symmetry.IncludeFilter(x => x.ETG_Fact_Symmetry_Models.Where(y => y.ETG_Fact_Symmetry_id == id)).FirstOrDefaultAsync();
            var ETG_Fact_Symmetry_Model = await _context.ETG_Fact_Symmetry.FirstOrDefaultAsync(c => c.ETG_Fact_Symmetry_id == id);
            if (ETG_Fact_Symmetry_Model != null)
            {
                _context.ETG_Fact_Symmetry.Remove(ETG_Fact_Symmetry_Model);
            }


            await _context.SaveChangesAsync();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ////CHRIS ADDED NON ASYNC
        public List<ETG_Dim_Premium_Spec_Master_Model> GetETGDimPremiumSpec()
        {
            var a = _context.ETG_Dim_Premium_Spec_Master.OrderBy(c => c.Premium_Specialty).ToList();
            return a;
        }

        public async Task<List<ETG_Dim_Premium_Spec_Master_Model>> GetETGDimPremiumSpecAsync()
        {
            return await _context.ETG_Dim_Premium_Spec_Master.ToListAsync();

        }

        public Task<ETG_Dim_Premium_Spec_Master_Model> GetETGDimPremiumSpecAsync(Int64 id)
        {
            //var list = _context.ETG_Dim_Premium_Spec_Master.IncludeFilter(x => x.ETG_Dim_Premium_Spec_Master_Models.Where(y => y.ETG_Fact_Symmetry_id == id)
            //                                   .OrderBy(y => y.ETG_Fact_Symmetry_id)
            //                                   .Take(10)).FirstOrDefaultAsync();

            var list = _context.ETG_Dim_Premium_Spec_Master.FirstOrDefaultAsync(c => c.Premium_Specialty_id == id);
            return list;
        }

        public async Task<ETG_Dim_Premium_Spec_Master_Model> AddETGDimPremiumSpecAsync(ETG_Dim_Premium_Spec_Master_Model ETG_Dim_Premium_Spec_Master_Model)
        {
            //ADDED 6222021 IN HOPES TO SPEED UP :(
            _context.Configuration.AutoDetectChangesEnabled = false;

            _context.ETG_Dim_Premium_Spec_Master.Add(ETG_Dim_Premium_Spec_Master_Model);

            //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
            //BUT THIS FIXED KEY BUG :)
            _context.ChangeTracker.DetectChanges();

            await _context.SaveChangesAsync();
            return ETG_Dim_Premium_Spec_Master_Model;
        }

        public async Task<ETG_Dim_Premium_Spec_Master_Model> UpdateETGDimPremiumSpecAsync(ETG_Dim_Premium_Spec_Master_Model ETG_Dim_Premium_Spec_Master_Model)
        {
            //var id = ETG_Dim_Premium_Spec_Master_Model.ETG_Dim_Premium_Spec_Master_Models.FirstOrDefault().ETG_Fact_Symmetry_id;
            //if (!(await _context.ETG_Dim_Premium_Spec_Master.IncludeFilter(x => x.ETG_Dim_Premium_Spec_Master_Models.Where(y => y.ETG_Fact_Symmetry_id == id)).AnyAsync()))
            if (!(await _context.ETG_Dim_Premium_Spec_Master.AnyAsync(c => c.Premium_Specialty_id == ETG_Dim_Premium_Spec_Master_Model.Premium_Specialty_id)))
            {
                _context.ETG_Dim_Premium_Spec_Master.Attach(ETG_Dim_Premium_Spec_Master_Model);
            }

            _context.Entry(ETG_Dim_Premium_Spec_Master_Model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ETG_Dim_Premium_Spec_Master_Model;

        }

        public async Task DeleteETGDimPremiumSpecModelAsync(Int64 id)
        {
            //var ETG_Dim_Premium_Spec_Master_Model = _context.ETG_Dim_Premium_Spec_Master.FirstOrDefault(c => c.Id == ETG_Dim_Premium_Spec_Master_ModelId);
            //var ETG_Dim_Premium_Spec_Master_Model = await _context.ETG_Dim_Premium_Spec_Master.IncludeFilter(x => x.ETG_Dim_Premium_Spec_Master_Models.Where(y => y.ETG_Fact_Symmetry_id == id)).FirstOrDefaultAsync();
            var ETG_Dim_Premium_Spec_Master_Model = await _context.ETG_Dim_Premium_Spec_Master.FirstOrDefaultAsync(c => c.Premium_Specialty_id == id);
            if (ETG_Dim_Premium_Spec_Master_Model != null)
            {
                _context.ETG_Dim_Premium_Spec_Master.Remove(ETG_Dim_Premium_Spec_Master_Model);
            }


            await _context.SaveChangesAsync();
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ////CHRIS ADDED NON ASYNC
        public List<ETG_Dim_Master_Model> GetETGDimMaster()
        {
            var a = _context.ETG_Dim_Master.OrderBy(c => c.ETG_Description).ToList();
            return a;
        }

        public async Task<List<ETG_Dim_Master_Model>> GetETGDimMasterAsync()
        {
            return await _context.ETG_Dim_Master.ToListAsync();

        }

        public Task<ETG_Dim_Master_Model> GetETGDimMasterAsync(string id)
        {
            //var list = _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_Master_Models.Where(y => y.LOB_id == id)
            //                                   .OrderBy(y => y.LOB_id)
            //                                   .Take(10)).FirstOrDefaultAsync();

            var list = _context.ETG_Dim_Master.FirstOrDefaultAsync(c => c.ETG_Base_Class == id);
            return list;
        }

        public async Task<ETG_Dim_Master_Model> AddETGDimMasterAsync(ETG_Dim_Master_Model ETG_Dim_Master_Model)
        {
            //ADDED 6222021 IN HOPES TO SPEED UP :(
            _context.Configuration.AutoDetectChangesEnabled = false;

            _context.ETG_Dim_Master.Add(ETG_Dim_Master_Model);

            //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
            //BUT THIS FIXED KEY BUG :)
            _context.ChangeTracker.DetectChanges();

            await _context.SaveChangesAsync();
            return ETG_Dim_Master_Model;
        }

        public async Task<ETG_Dim_Master_Model> UpdateETGDimMasterAsync(ETG_Dim_Master_Model ETG_Dim_Master_Model)
        {
            //var id = ETG_Dim_Master_Model.ETG_Dim_Master_Models.FirstOrDefault().LOB_id;
            //if (!(await _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_Master_Models.Where(y => y.LOB_id == id)).AnyAsync()))
            if (!(await _context.ETG_Dim_Master.AnyAsync(c => c.ETG_Base_Class == ETG_Dim_Master_Model.ETG_Base_Class)))
            {
                _context.ETG_Dim_Master.Attach(ETG_Dim_Master_Model);
            }

            _context.Entry(ETG_Dim_Master_Model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ETG_Dim_Master_Model;

        }

        public async Task DeleteETGDimMasterModelAsync(string id)
        {
            //var ETG_Dim_Master_Model = _context.ETG_Dim_LOB.FirstOrDefault(c => c.Id == ETG_Dim_Master_ModelId);
            //var ETG_Dim_Master_Model = await _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_Master_Models.Where(y => y.LOB_id == id)).FirstOrDefaultAsync();
            var ETG_Dim_Master_Model = await _context.ETG_Dim_Master.FirstOrDefaultAsync(c => c.ETG_Base_Class == id);
            if (ETG_Dim_Master_Model != null)
            {
                _context.ETG_Dim_Master.Remove(ETG_Dim_Master_Model);
            }


            await _context.SaveChangesAsync();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////CHRIS ADDED NON ASYNC
        //public List<ETG_Dim_LOB_Model> GetETGDimLOB()
        //{
        //    var a = _context.ETG_Dim_LOB.ToList();
        //    return a;
        //}

        //public async Task<List<ETG_Dim_LOB_Model>> GetETGDimLOBAsync()
        //{
        //    return await _context.ETG_Dim_LOB.ToListAsync();

        //}

        //public Task<ETG_Dim_LOB_Model> GetETGDimLOBAsync(Int16 id)
        //{
        //    //var list = _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_LOB_Models.Where(y => y.LOB_id == id)
        //    //                                   .OrderBy(y => y.LOB_id)
        //    //                                   .Take(10)).FirstOrDefaultAsync();

        //    var list = _context.ETG_Dim_LOB.FirstOrDefaultAsync(c => c.LOB_id == id);
        //    return list;
        //}

        //public async Task<ETG_Dim_LOB_Model> AddETGDimLOBAsync(ETG_Dim_LOB_Model ETG_Dim_LOB_Model)
        //{
        //    //ADDED 6222021 IN HOPES TO SPEED UP :(
        //    _context.Configuration.AutoDetectChangesEnabled = false;

        //    _context.ETG_Dim_LOB.Add(ETG_Dim_LOB_Model);

        //    //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
        //    //BUT THIS FIXED KEY BUG :)
        //    _context.ChangeTracker.DetectChanges();

        //    await _context.SaveChangesAsync();
        //    return ETG_Dim_LOB_Model;
        //}

        //public async Task<ETG_Dim_LOB_Model> UpdateETGDimLOBAsync(ETG_Dim_LOB_Model ETG_Dim_LOB_Model)
        //{
        //    //var id = ETG_Dim_LOB_Model.ETG_Dim_LOB_Models.FirstOrDefault().LOB_id;
        //    //if (!(await _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_LOB_Models.Where(y => y.LOB_id == id)).AnyAsync()))
        //    if (!(await _context.ETG_Dim_LOB.AnyAsync(c => c.LOB_id == ETG_Dim_LOB_Model.LOB_id)))
        //    {
        //        _context.ETG_Dim_LOB.Attach(ETG_Dim_LOB_Model);
        //    }

        //    _context.Entry(ETG_Dim_LOB_Model).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return ETG_Dim_LOB_Model;

        //}

        //public async Task DeleteETGDimLOBModelAsync(Int16 id)
        //{
        //    //var ETG_Dim_LOB_Model = _context.ETG_Dim_LOB.FirstOrDefault(c => c.Id == ETG_Dim_LOB_ModelId);
        //    //var ETG_Dim_LOB_Model = await _context.ETG_Dim_LOB.IncludeFilter(x => x.ETG_Dim_LOB_Models.Where(y => y.LOB_id == id)).FirstOrDefaultAsync();
        //    var ETG_Dim_LOB_Model = await _context.ETG_Dim_LOB.FirstOrDefaultAsync(c => c.LOB_id == id);
        //    if (ETG_Dim_LOB_Model != null)
        //    {
        //        _context.ETG_Dim_LOB.Remove(ETG_Dim_LOB_Model);
        //    }


        //    await _context.SaveChangesAsync();
        //}

    }
}
