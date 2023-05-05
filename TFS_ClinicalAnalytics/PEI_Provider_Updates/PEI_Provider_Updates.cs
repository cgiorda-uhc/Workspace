using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEI_Provider_Updates
{
    class PEI_Provider_Updates
    {
        static void Main(string[] args)
        {
            generateSQLScripts();
        }


        private static void generateSQLScripts()
        {

            // EXEC xp_cmdshell  'sqlcmd -S LH7U05CB3210C2B -d  PEIPortalDB -i C:\pei2_provider_updates\1.1.1_fid1_provider_master.sql'
            string strConnectionString = ConfigurationManager.AppSettings["UHN"];
            string strScriptPath = ConfigurationManager.AppSettings["ScriptPath"];


            StreamWriter swProviderMasterScript = null;

            string strSQL = "SELECT MPIN, TIN, NPI,LastName, FirstName, Street, City, State, ZipCd, Phone_nbr, Fax_nbr, CASE Group_Individual WHEN 'ALLIED HEALTH PROF GROUP' THEN 'Allied Health' WHEN 'PROVIDER GROUP' THEN 'Provider Group' WHEN 'PHYSICIAN' THEN 'Physician' ELSE NULL END as ProviderType ,Prov_Specialty FROM ( SELECT MPIN, TaxID as TIN,LastName, FirstName, Street, City, State, ZipCd, PrimAdrInd as Prim_Address_Ind, AdrType, max(Phone) as Phone_nbr, max(Fax) as Fax_nbr, addressid,Group_Individual,NPI,Prov_Specialty FROM ( SELECT Distinct P.MPIN, MT.TaxID,P.LastName, P.FirstName, P.ProvStatus, p.CommercialInd, a.Street, a.City, a.State, a.ZipCd, l.PrimAdrInd, l.AdrType, PRO.AreaCd, PRO.PhoneNbr, PRO.PhoneType, PRO.PrimPhoneInd, case when PRO.PhoneType in ('P','C') then PRO.AreaCd+' '+PRO.PhoneNbr end as Phone, case when PRO.PhoneType = 'F' then PRO.AreaCd+' '+PRO.PhoneNbr end as Fax, a.addressid, CASE WHEN p.provtype='P' THEN 'PHYSICIAN' ELSE ot.longdesc END as Group_Individual, NPI,s.LongDesc as Prov_Specialty FROM dbo.PROVIDER as P inner join dbo.SPECIALTY_TYPES as s on s.SpecTypeCd=p.PrimSpec left join dbo.PROV_MPIN_TAXID MT ON P.MPIN=MT.MPIN left join dbo.mpin_location l ON l.MPIN=MT.MPIN AND l.TaxID=MT.TaxID left join dbo.prov_address a ON l.AddressID=a.AddressID left join dbo.PROV_LOC_PHONE PRO ON l.MPIN=PRO.MPIN AND l.TaxIDType=PRO.TaxIDType AND l.AdrType=PRO.AdrType AND l.AddressID=PRO.AddressID left join dbo.Org_Types ot on p.orgtypecd = ot.orgtypecd left join (select MPIN,min(NatlProvID) as NPI from dbo.NPI group by MPIN) as npi on npi.MPIN=p.MPIN WHERE PrimAdrInd = 'P' and ( (p.provtype = 'P' and p.provdegree in ('MD','DO','DPM','DC')) or (p.provtype = 'O' and p.OrgTypeCd in('033','050')) ) ) as b GROUP BY MPIN, TaxID,LastName, FirstName,Street, City, State, ZipCd, PrimAdrInd, AdrType, addressid,Group_Individual,NPI,Prov_Specialty ) t";




            string strMPIN;
            string strTaxID;
            string strNPI;
            string strLastName;
            string strFirstName;
            string strStreet;
            string strCity;
            string strState;
            string strZipCd;
            string strPhone_nbr;
            string strFull_Name;
            string strProviderType;
            string strProvSpecialty;

            int intCnt = 0;
            int intFileCnt = 1;

            StringBuilder sbSQLMain = new StringBuilder();
            StringBuilder sbSQLScript = new StringBuilder();

            DataTable dtMain = null;



            dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
            sbSQLMain.Remove(0, sbSQLMain.Length);


            if (dtMain.Rows.Count > 0)
            {
                Console.WriteLine("INDIVIDUAL PROVIDER MASTER START");
                swProviderMasterScript = new StreamWriter(strScriptPath + "1.1.1_fid" + intFileCnt + "_provider_master.sql", false);

                foreach (DataRow dr in dtMain.Rows)
                {

                    strMPIN = dr["MPIN"].ToString();
                    strTaxID = (string.IsNullOrEmpty(dr["TIN"].ToString()) ? "NULL" : dr["TIN"].ToString().Trim());
                    strNPI = (string.IsNullOrEmpty(dr["NPI"].ToString()) ? "NULL" : dr["NPI"].ToString().Trim());
                    strLastName = (string.IsNullOrEmpty(dr["LastName"].ToString()) ? "NULL" : dr["LastName"].ToString().Trim().Replace("'", "''"));
                    strFirstName = (string.IsNullOrEmpty(dr["FirstName"].ToString()) ? "NULL" : dr["FirstName"].ToString().Trim().Replace("'", "''"));

                    strStreet = (string.IsNullOrEmpty(dr["Street"].ToString()) ? "NULL" : "'" + dr["Street"].ToString().Trim().Replace("'", "''") + "'");
                    strCity = (string.IsNullOrEmpty(dr["City"].ToString()) ? "NULL" : "'" + dr["City"].ToString().Trim().Replace("'", "''") + "'");
                    strState = (string.IsNullOrEmpty(dr["State"].ToString()) ? "NULL" : "'" + dr["State"].ToString().Trim().Replace("'", "''") + "'");
                    strZipCd = (string.IsNullOrEmpty(dr["ZipCd"].ToString()) ? "NULL" : "'" + dr["ZipCd"].ToString().Trim().Replace("'", "''") + "'");

                    strPhone_nbr = (string.IsNullOrEmpty(dr["Phone_nbr"].ToString()) ? "NULL" : "'" + dr["Phone_nbr"].ToString().Trim().Replace("'", "''") + "'");


                    strFull_Name = ((strFirstName != "NULL" ? strFirstName.Trim() : "") + " " + strLastName.Trim()).Trim();


                    strProviderType = (string.IsNullOrEmpty(dr["ProviderType"].ToString()) ? "NULL" : "'" + dr["ProviderType"].ToString().Trim().Replace("'", "''") + "'");
                    strProvSpecialty = (string.IsNullOrEmpty(dr["Prov_Specialty"].ToString()) ? "NULL" : "'" + dr["Prov_Specialty"].ToString().Trim().Replace("'", "''") + "'");

                    sbSQLScript.Append("IF EXISTS(SELECT 1 FROM PEI2_org_prov_master WHERE mpin = " + strMPIN + ") ");

                    sbSQLScript.Append("UPDATE PEI2_org_prov_master  SET ");
                    sbSQLScript.Append("full_name = '" + strFull_Name + "', ");
                    sbSQLScript.Append("first_name = '" + strFirstName + "', ");
                    sbSQLScript.Append("last_name = '" + strLastName + "', ");
                    sbSQLScript.Append("organization_type_id = (SELECT organization_type_id FROM PEI2_organization_type WHERE organization_type = " + strProviderType + "), ");
                    sbSQLScript.Append("specialty_id = (SELECT specialty_id FROM PEI2_specialty WHERE specialty_name = " + strProvSpecialty + "), ");

                    if (strTaxID != "NULL")
                        sbSQLScript.Append("tin = " + strTaxID + ", ");

                    if (strNPI != "NULL")
                        sbSQLScript.Append("npi = " + strNPI + ", ");

                    if (strStreet != "NULL")
                        sbSQLScript.Append("street = " + strStreet + ", ");


                    if (strCity != "NULL")
                        sbSQLScript.Append("city = " + strCity + ", ");

                    if (strState != "NULL")
                        sbSQLScript.Append("state = " + strState + ", ");

                    if (strZipCd != "NULL")
                        sbSQLScript.Append("zip_code = " + strZipCd + ", ");

                    if (strPhone_nbr != "NULL")
                        sbSQLScript.Append("phone = " + strPhone_nbr + ", ");


                    sbSQLScript.Append("updated_by_username = 'cgiorda', ");
                    sbSQLScript.Append("update_date = getDate(), ");
                    sbSQLScript.Append("update_data_source = 'UHN', ");
                    sbSQLScript.Append("is_highlight = 1 ");
                    sbSQLScript.Append("WHERE mpin = " + strMPIN + " ");
                    sbSQLScript.Append("and ISNULL(update_data_source,'') <> 'PEI2' ");
                    sbSQLScript.Append("; ");

                    sbSQLScript.Append("ELSE ");

                    sbSQLScript.Append("INSERT INTO PEI2_org_prov_master ");
                    sbSQLScript.Append("(mpin, tin, npi, full_name,original_full_name,first_name,last_name,  street,  city,state,zip_code,phone,added_by_username,insert_date,insert_data_source, is_flagged, is_archived, is_highlight, organization_type_id, specialty_id) ");
                    sbSQLScript.Append("VALUES ");
                    sbSQLScript.Append("(" + strMPIN + "," + strTaxID + ", " + strNPI + ", '" + strFull_Name + "', '" + strFull_Name + "', '" + strFirstName + "', '" + strLastName + "', " + strStreet + " ,  " + strCity + " , " + strState + " , " + strZipCd + ", " + strPhone_nbr + " , 'cgiorda', getDate(), 'UHN', 0, 0, 1, (SELECT organization_type_id FROM PEI2_organization_type WHERE organization_type = "+ strProviderType + "), (SELECT specialty_id FROM PEI2_specialty WHERE specialty_name = " + strProvSpecialty + ") ) ");
                    sbSQLScript.Append(";");


                    swProviderMasterScript.WriteLine(sbSQLScript.ToString());
                    sbSQLScript.Remove(0, sbSQLScript.Length);

                    intCnt++;
                    Console.WriteLine("Row " + intCnt + " of " + dtMain.Rows.Count + " : INDIVIDUAL PROVIDER MASTER INSERT, MPIN = " + strMPIN);

                    if (intCnt % 50 == 0)
                    {

                        swProviderMasterScript.WriteLine(" GO ");
                        swProviderMasterScript.Flush();
                    }


                    if (intCnt % 300000 == 0)
                    {

                        swProviderMasterScript.Flush();
                        swProviderMasterScript.Close();
                        swProviderMasterScript = null;

                        intFileCnt++;
                        swProviderMasterScript = new StreamWriter(strScriptPath + "1.1.1_fid"+ intFileCnt + "_provider_master.sql", false);

                    }



                }
                swProviderMasterScript.Flush();
                swProviderMasterScript.Close();
            }

        }

    }
}
