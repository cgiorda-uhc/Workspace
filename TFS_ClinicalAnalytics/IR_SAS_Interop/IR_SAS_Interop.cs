using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


namespace IR_SAS_Interop
{
    class IR_SAS_Interop
    {
        static void Main(string[] args)
        {


            string strHost = "sasgrid.uhc.com";
            int intPort = 8564; 
            string strUserId = "cgiorda";
            string strPassword = "BooWooDooFoo2023!!";
            string strClassIdentifier = "0217E202-B560-11DB-AD91-001083FF6836";
            string strServerName = "SAS_CSHARP_TEST";

            SASObjectManager.ObjectKeeper objKeeper = new SASObjectManager.ObjectKeeper();
            SASObjectManager.ObjectFactoryMulti2 objFactory = new SASObjectManager.ObjectFactoryMulti2();
            SASObjectManager.ServerDef objServerDef = new SASObjectManager.ServerDef();
            SAS.Workspace objSAS = null;
            SAS.Libref objLibRef = null;

            OleDbDataReader oleDr = null;
            try
            {
                objServerDef.MachineDNSName = strHost;
                objServerDef.Port = intPort;
                objServerDef.Protocol = SASObjectManager.Protocols.ProtocolBridge;
                objServerDef.BridgeEncryptionAlgorithm = "SASProprietary";
                objServerDef.BridgeSecurityPackage = "Negotiate";
                objServerDef.ClassIdentifier = strClassIdentifier;


                dynamic omi = objFactory.CreateObjectByServer(strServerName, true, objServerDef, strUserId, strPassword);
                objFactory.SetRepository(omi);
                IEnumerable<SASObjectManager.ServerDef> serverDefs = objFactory.ServerDefs.Cast<SASObjectManager.ServerDef>();
                SASObjectManager.IServerDef workSpaceServerDef = default(SASObjectManager.IServerDef);

                foreach (SASObjectManager.ServerDef serverDef in serverDefs)
                {
                    if (serverDef.Name == "SASEG - Workspace Server")
                    {
                        workSpaceServerDef = serverDef;
                        break;
                    }
                }

                objSAS = (SAS.Workspace)objFactory.CreateObjectByServer(strServerName, true, (SASObjectManager.ServerDef)workSpaceServerDef, strUserId, strPassword);
                objKeeper.AddObject(1, "WorkspaceObject", objSAS);
                //objSAS.LanguageService.Submit("?????");
                //"libname ir_phase /optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34; proc sql; select * from ir_phase.table_name;";
                //objLibRef = objSAS.DataService.AssignLibref("ir_phase",null, "/optum/uhs/01datafs/phi/projects/analytics/pbp/" + "Ph34", null);

                objLibRef = objSAS.DataService.AssignLibref("ir_sas", string.Empty, "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph14", string.Empty);

                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = "provider=sas.iomprovider.1; SAS Workspace ID=" + objSAS.UniqueIdentifier;
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("select a.* from ir_sas.PBP_RAD_MPIN as a;", conn);
                cmd.CommandTimeout = 9999999;
                oleDr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                int rowCnt = 0;
                while (oleDr.Read())
                {
                    rowCnt++;
                    for (int colIndex = 0; colIndex < oleDr.FieldCount; colIndex++)
                    {
                        Console.WriteLine("Row #"+ rowCnt +": " + oleDr.GetName(colIndex) + " = " + oleDr.GetValue(colIndex));
                    }

                    if (rowCnt == 30)
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (objSAS != null)
                {
                    objKeeper.RemoveObject(objSAS);
                    objSAS.Close();

                }
                objKeeper = null;

                if(oleDr != null)
                {
                    oleDr.Close();
                    oleDr.Dispose();

                }
                oleDr = null;

                Console.WriteLine("DONE!!!");
                Console.Read();

            }


        }
    }
}
