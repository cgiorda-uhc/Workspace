using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Model.V24.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using WinSCP;

namespace CCD_Data_Extract_XDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            BDPass_FileAccess();
            //parseHL7_RAW();

        }


        //cdsm_acc_partition - daily updated
        //src_batch_id + memberid1 join


        static void BDPass_FileAccess()
        {
            // Setup session options
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "apvrp59202", //dbslp0568
                UserName = "cgiorda",
                Password = "BooWooDooFoo2023!!",
                PortNumber = 22,
                SshHostKeyFingerprint = "ssh-ed25519 256 06:63:08:ca:49:a2:fd:32:c0:de:b9:a5:5d:a2:41:34"
                // ,GiveUpSecurityAndAcceptAnySshHostKey = true
            };


            //SessionOption.GiveUpSecurityAndAcceptAnySshHostKey

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);


                // EnumerationOptions enumOptions = new EnumerationOptions();
                var files = session.EnumerateRemoteFiles("/mapr/datalake/corporate/cdsm-prod/app/prd/p_inbound", "cc.txt", EnumerationOptions.None);
                foreach (var fileInfo in files)
                {
                    string s = fileInfo.Name;
                    s = fileInfo.Length.ToString();
                    s = fileInfo.FilePermissions.ToString();
                    s = fileInfo.LastWriteTime.ToString();
                }





                // Download files
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;

                TransferOperationResult transferResult;
                transferResult =
                    session.GetFiles("/mapr/datalake/corporate/cdsm-prod/app/prd/p_inbound/*", @"d:\download\", false, transferOptions);

                // Throw on any error
                transferResult.Check();

                // Print results
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                }
            }
        }








        static void parseHL7_RAW()
        {
            string messageString =  System.IO.File.ReadAllText(@"\\nasv0008\stars_suppl_data\CDX\Interfaces\CDXcADT-TO-SDR\Archive\HL7Raw\ICCTX_HL7_RAW.20200517.txt");

            

            string[] strPatients = messageString.Split(new string[] {"MSH|"}, System.StringSplitOptions.RemoveEmptyEntries);
            // instantiate a PipeParser, which handles the "traditional or default encoding"
            var ourPipeParser = new PipeParser();

            try
            {



                foreach (string s in strPatients)
                {
                    // parse the string format message into a Java message object
                    var hl7Message = ourPipeParser.Parse("MSH|" + s);


                    // Display the updated HL7 message using Pipe delimited format
                    Console.WriteLine("HL7 Pipe Delimited Message Output:");
                    Console.WriteLine(ourPipeParser.Encode(hl7Message));

                    // instantiate an XML parser that NHAPI provides
                    var ourXmlParser = new DefaultXMLParser();

                    // convert from default encoded message into XML format, and send it to standard out for display
                    Console.WriteLine("HL7 XML Formatted Message Output:");
                    string strXML = ourXmlParser.Encode(hl7Message);
                    Console.WriteLine(strXML);





                    var a01 = hl7Message as NHapi.Model.V251.Message.ADT_A01;

                    var var_ProfileIdentifier =  a01.MSH.GetMessageProfileIdentifier();
                    var var_PrincipalLanguageOfMessage = a01.MSH.PrincipalLanguageOfMessage.Identifier.Value;
                    var var_CharacterSetRepetitionsUsed = a01.MSH.CharacterSetRepetitionsUsed.ToString();
                    var var_CountryCode = a01.MSH.CountryCode.Value;
                    var var_ApplicationAcknowledgmentType = a01.MSH.ApplicationAcknowledgmentType.Value;
                    var var_AcceptAcknowledgmentType = a01.MSH.AcceptAcknowledgmentType.Value;
                    var var_ContinuationPointer = a01.MSH.ContinuationPointer.Value;
                    var var_SequenceNumber = a01.MSH.SequenceNumber.Value;
                    var var_VersionID = a01.MSH.VersionID.VersionID.Value;
                    var var_ProcessingID = a01.MSH.ProcessingID.ProcessingID.Value;
                    var var_MessageControlID = a01.MSH.MessageControlID.Value;
                    var var_MessageType = a01.MSH.MessageType.TypeName;
                    var var_Security = a01.MSH.Security.Value;
                    var var_DateTimeOfMessage = a01.MSH.DateTimeOfMessage.Time.Value;
                    var var_ReceivingFacility = a01.MSH.ReceivingFacility.NamespaceID;
                    var var_ReceivingApplication = a01.MSH.ReceivingApplication.NamespaceID;
                    var var_SendingFacility = a01.MSH.SendingFacility.NamespaceID;
                    var var_SendingApplication = a01.MSH.SendingApplication.UniversalID.Value;
                    var var_EncodingCharacters = a01.MSH.EncodingCharacters.Value;
                    var var_FieldSeparator = a01.MSH.FieldSeparator.Value;
                    var var_AlternateCharacterSetHandlingScheme = a01.MSH.AlternateCharacterSetHandlingScheme.Value;
                    var var_MessageProfileIdentifierRepetitionsUsed = a01.MSH.MessageProfileIdentifierRepetitionsUsed;
                    var var_test = a01.MSH.GetCharacterSet();



                    var var_evnMessage = a01.EVN.Message;
                    var var_ParentStructure = a01.EVN.ParentStructure;
                    //var var_envField = a01.EVN.GetField(0);
                    //var var_envFieldDescription = a01.EVN.GetFieldDescription(0);
                    //var var_envMaxCardinality = a01.EVN.GetMaxCardinality(0);
                    //var var_envStructureName = a01.EVN.GetStructureName();



                    var var_AlternatePatient = a01.PID.GetAlternatePatientIDPID();
                    var var_Citizenship = a01.PID.GetCitizenship();
                    var var_EthnicGroup = a01.PID.GetEthnicGroup();
                    var var_IdentityReliabilityCode = a01.PID.GetIdentityReliabilityCode();
                    var var_MotherSIdentifier = a01.PID.GetMotherSIdentifier();
                    var var_MotherSMaidenName = a01.PID.GetMotherSMaidenName();
                    var var_PatientAddress = a01.PID.GetPatientAddress(0);
                    var var_PatientAlias = a01.PID.GetPatientAlias();
                    var var_PatientIdentifierList = a01.PID.GetPatientIdentifierList(0);
                    var var_PatientName = a01.PID.GetPatientName(0);
                    var var_PhoneNumberBusiness = a01.PID.GetPhoneNumberBusiness();
                    var var_PhoneNumberHome = a01.PID.GetPhoneNumberHome();
                    var var_Race = a01.PID.GetRace();
                    var var_TribalCitizenship = a01.PID.GetTribalCitizenship();

                    string  st_patient_id = var_PatientIdentifierList.IDNumber.Value;
                    string str_patient_lastname = var_PatientName.FamilyName.Surname.Value;
                    string str_patient_firstname = var_PatientName.GivenName.Value;
                    string str_patient_suffix = var_PatientName.ProfessionalSuffix.Value;
                    string str_patient_streetaddress = var_PatientAddress.StreetAddress.StreetOrMailingAddress.Value;
                    string str_patient_city = var_PatientAddress.City.Value;
                    string str_patient_state = var_PatientAddress.StateOrProvince.Value;
                    string str_patient_zipcode = var_PatientAddress.ZipOrPostalCode.Value;



                    foreach (NHapi.Model.V251.Segment.DG1 dg in a01.DG1s)
                    {
                        var test111 = dg.DiagnosisCodeDG1;
                        var tt222 = dg.GetDiagnosingClinician();
                    }



                    continue;



                    //var AdministrativeSex = a01.PID.AdministrativeSex.Value;
                    //var PatientAliasRepetitionsUsed = a01.PID.PatientAliasRepetitionsUsed;
                    //var RaceRepetitionsUsed = a01.PID.RaceRepetitionsUsed;
                    //var PatientAddressRepetitionsUsed = a01.PID.PatientAddressRepetitionsUsed;
                    //var CountyCode = a01.PID.CountyCode.Value;
                    //var PhoneNumberHomeRepetitionsUsed = a01.PID.PhoneNumberHomeRepetitionsUsed;
                    //var PhoneNumberBusinessRepetitionsUsed = a01.PID.PhoneNumberBusinessRepetitionsUsed;
                    //var PrimaryLanguage = a01.PID.PrimaryLanguage.Text;
                    //var MaritalStatus = a01.PID.MaritalStatus.Text;
                    //var Religion = a01.PID.Religion.Text;
                    //var PatientAccountNumber = a01.PID.PatientAccountNumber.IDNumber.Value;
                    //var SSNNumberPatient = a01.PID.SSNNumberPatient.Value;
                    //var DriverSLicenseNumberPatient = a01.PID.DriverSLicenseNumberPatient;
                    //var MotherSIdentifierRepetitionsUsed = a01.PID.MotherSIdentifierRepetitionsUsed;
                    //var DateTimeOfBirth = a01.PID.DateTimeOfBirth.Time.Value;
                    //var EthnicGroupRepetitionsUsed = a01.PID.EthnicGroupRepetitionsUsed;
                    //var MultipleBirthIndicator = a01.PID.MultipleBirthIndicator.Value;
                    //var BirthOrder = a01.PID.BirthOrder.Value;
                    //var CitizenshipRepetitionsUsed = a01.PID.CitizenshipRepetitionsUsed;
                    //var VeteransMilitaryStatus = a01.PID.VeteransMilitaryStatus.Text.Value;
                    //var Nationality = a01.PID.Nationality.Text.Value;
                    //var PatientDeathDateAndTime = a01.PID.PatientDeathDateAndTime.Time.Value;
                    //var PatientDeathIndicator = a01.PID.PatientDeathIndicator.Value;
                    //var IdentityUnknownIndicator = a01.PID.IdentityUnknownIndicator.Value;
                    //var IdentityReliabilityCodeRepetitionsUsed = a01.PID.IdentityReliabilityCodeRepetitionsUsed;
                    //var LastUpdateDateTime = a01.PID.LastUpdateDateTime.Time.Value;
                    //var LastUpdateFacility = a01.PID.LastUpdateFacility.UniversalID.Value;
                    //var SpeciesCode = a01.PID.SpeciesCode.Text.Value;
                    //var BreedCode = a01.PID.BreedCode.Text.Value;
                    //var Strain = a01.PID.Strain.Value;
                    //var BirthPlace = a01.PID.BirthPlace.Value;
                    //var MotherSMaidenNameRepetitionsUsed = a01.PID.MotherSMaidenNameRepetitionsUsed;
                    //var PatientNameRepetitionsUsed = a01.PID.PatientNameRepetitionsUsed;
                    //var AlternatePatientIDPIDRepetitionsUsed = a01.PID.AlternatePatientIDPIDRepetitionsUsed;
                    //var ProductionClassCode = a01.PID.ProductionClassCode.Text.Value;
                    //var SetIDPID = a01.PID.SetIDPID.Value;
                    //var TribalCitizenshipRepetitionsUsed = a01.PID.TribalCitizenshipRepetitionsUsed;
                    //var PatientIdentifierListRepetitionsUsed = a01.PID.PatientIdentifierListRepetitionsUsed;
                    //var PatientID = a01.PID.PatientID.IDNumber.Value;






























                    // create a terser object instance by wrapping it around the message object
                    var terser = new Terser(hl7Message);

                    // now, let us do various operations on the message
                    var terserHelper = new OurTerserHelper(terser);



                    var test2 = terser.getSegment("PID");

          




                //https://hl7-definition.caristix.com/v2/HL7v2.5.1/Fields/PID.2
                var mshFieldSeparator = runTerserExpression("/.MSH-1-1", terserHelper);
                    var mshEncodingCharacters = runTerserExpression("/.MSH-2-1", terserHelper);
                    var mshSendingApplicationNamespaceId = runTerserExpression("/.MSH-3-1", terserHelper);
                    var mshSendingApplicationUniversalId = runTerserExpression("/.MSH-3-2", terserHelper);
                    var mshSendingApplicationUniversalIdType = runTerserExpression("/.MSH-3-3", terserHelper);
                    var mshSendingFacilityNamespaceId = runTerserExpression("/.MSH-4-1", terserHelper);
                    var mshSendingFacilityUniversalId = runTerserExpression("/.MSH-4-2", terserHelper);
                    var mshSendingFacilityUniversalIdType = runTerserExpression("/.MSH-4-3", terserHelper);
                    var mshReceivingApplicationNamespaceId = runTerserExpression("/.MSH-5-1", terserHelper);
                    var mshReceivingApplicationUniversalId = runTerserExpression("/.MSH-5-2", terserHelper);
                    var mshReceivingApplicationUniversalIdType = runTerserExpression("/.MSH-5-3", terserHelper);
                    var mshReceivingFacilityNamespaceId = runTerserExpression("/.MSH-6-1", terserHelper);
                    var mshReceivingFacilityUniversalId = runTerserExpression("/.MSH-6-2", terserHelper);
                    var mshReceivingFacilityUniversalIdType = runTerserExpression("/.MSH-6-3", terserHelper);
                    var mshDateTimeOfMessage = runTerserExpression("/.MSH-7-1", terserHelper);
                    var mshSecurity = runTerserExpression("/.MSH-8-1", terserHelper);
                    var mshMessageType = runTerserExpression("/.MSH-9-1", terserHelper);
                    var mshMessageControlID = runTerserExpression("/.MSH-10-1", terserHelper);
                    var mshProcessingID = runTerserExpression("/.MSH-11-1", terserHelper);
                    var mshVersionID = runTerserExpression("/.MSH-12-1", terserHelper);
                    var mshSequenceNumber = runTerserExpression("/.MSH-13-1", terserHelper);
                    var mshContinuationPointer = runTerserExpression("/.MSH-14-1", terserHelper);
                    var mshAcceptAcknowledgmentType = runTerserExpression("/.MSH-15-1", terserHelper);
                    var mshApplicationAcknowledgmentType = runTerserExpression("/.MSH-16-1", terserHelper);
                    var mshCountryCode = runTerserExpression("/.MSH-17-1", terserHelper);
                    var mshCharacterSet = runTerserExpression("/.MSH-18-1", terserHelper);
                    var mshPrincipalLanguageOfMessageIdentifier = runTerserExpression("/.MSH-19-1", terserHelper);
                    var mshPrincipalLanguageOfMessageText = runTerserExpression("/.MSH-19-2", terserHelper);
                    var mshPrincipalLanguageOfMessageNameOfCodingSystem = runTerserExpression("/.MSH-19-3", terserHelper);
                    var mshPrincipalLanguageOfMessageAlternateIdentifier = runTerserExpression("/.MSH-19-4", terserHelper);
                    var mshPrincipalLanguageOfMessageAlternateText = runTerserExpression("/.MSH-19-5", terserHelper);
                    var mshPrincipalLanguageOfMessageNameOfAlternateCodingSystem = runTerserExpression("/.MSH-19-6", terserHelper);
                    var mshAlternateCharacterSetHandlingScheme = runTerserExpression("/.MSH-20-1", terserHelper);
                    var mshConformanceStatementID = runTerserExpression("/.MSH-21-1", terserHelper);



                    var pidSetID_PID = runTerserExpression("/.PID-1-1", terserHelper);
                    var pidPatientIDIdNumber = runTerserExpression("/.PID-2-1", terserHelper);
                    var pidPatientIDCheckDigit = runTerserExpression("/.PID-2-2", terserHelper);
                    var pidPatientIDCheckDigitScheme = runTerserExpression("/.PID-2-3", terserHelper);
                    var pidPatientIDAssigningAuthority = runTerserExpression("/.PID-2-4", terserHelper);
                    var pidPatientIDIdentifierTypeCode = runTerserExpression("/.PID-2-5", terserHelper);
                    var pidPatientIDAssigningFacility = runTerserExpression("/.PID-2-6", terserHelper);
                    var pidPatientIDEffectiveDate = runTerserExpression("/.PID-2-7", terserHelper);
                    var pidPatientIDExpirationDate = runTerserExpression("/.PID-2-8", terserHelper);
                    var pidPatientIDAssigningJurisdiction = runTerserExpression("/.PID-2-9", terserHelper);
                    var pidPatientIDAssigningAgencyOrDepartment = runTerserExpression("/.PID-2-10", terserHelper);
                    //PID.3 - Patient Identifier List 250
                    //PID.4 - Alternate Patient ID -PID  20
                    //PID.5 - Patient Name    250
                    //PID.6 - Mother's Maiden Name	250			
                    //PID.7 - Date / Time Of Birth  26
                    //PID.8 - Administrative Sex  1
                    //PID.9 - Patient Alias   250
                    //PID.10 - Race   250


                    //int intInnerCnt = 1;
                    //int intOuterCnt = 1;
                    //string[] strElements = { "MSH", "EVN", "DG1", "PID", "PD1", "ROL", "NK1", "PV1", "PV2", "GT1" };
                    //foreach(string se in strElements)
                    //{
                    //    while(intOuterCnt < 20)
                    //    {
                    //        while (intInnerCnt < 20)
                    //        {
                    //            runTerserExpression("/." + se + "-" + intOuterCnt + "-" + intInnerCnt + "", terserHelper);
                    //            intInnerCnt++;
                    //        }
                    //        intInnerCnt = 1;
                    //        intOuterCnt++;
                    //    }

                    //    intOuterCnt = 1;
                    //}


                    //runTerserExpression("/.PID-5-2", terserHelper);
                    //runTerserExpression("/.PID-5-3", terserHelper);
                    //runTerserExpression("/.PID-5-4", terserHelper);
                    //runTerserExpression("/.PID-5-5", terserHelper);
                    //runTerserExpression("/.PID-5-6", terserHelper);
                    //runTerserExpression("/.PID-5-7", terserHelper);
                    //runTerserExpression("/.PID-5-8", terserHelper);





                   

                    ////a01.MSH.SendingApplication.UniversalID.Value = "ThisOne";
                    ////a01.MSH.ReceivingApplication.UniversalID.Value = "COHIE";
                    ////a01.PID.PatientIDExternalID.ID.Value = "123456";
                    ////a01.PV1.AttendingDoctor.FamilyName.Value = "Jones";
                    ////a01.PV1.AttendingDoctor.GivenName.Value = "Mike";
                    ////a01.PID.DateOfBirth.TimeOfAnEvent.SetShortDate(birthDate);



                    ////var setId = a03.PV1.SetIDPatientVisit.Value;
                    //var patientClass = a01.PV1.PatientClass.Value;
                    //var AssignedPatientLocation = a01.PV1.AssignedPatientLocation.PointOfCare.Value;
                    //var Admission_Type = a01.PV1.AdmissionType.Value;
                    ////var Pre_Admit_Number = a03.PV1.PreadmitNumber.ID.Value;
                    //var Prior_Patient_Location = a01.PV1.PriorPatientLocation.PointOfCare.Value;
                    ////var Attending_Doctor_Id = a03.PV1.AttendingDoctor.IDNumber.Value;
                    ////var Attending_Doctor_Name = a03.PV1.AttendingDoctor.FamilyName.Value;
                    ////var Referring_Doctor_Id = a03.PV1.ReferringDoctor.IDNumber.Value;
                    ////var Referring_Doctor_Name = a03.PV1.ReferringDoctor.FamilyName.Value;






                }





                


                //  var al =  a01.PV2

                //a01.MSH.GetCharacterSet();
                //a01.MSH.GetCharacterSet(int rep);
                //a01.MSH.GetMessageProfileIdentifier();
                //a01.MSH.GetMessageProfileIdentifier(int rep);




                //a01.MSH.
                //PV2
                //EVN
                //DG1
                //PID
                //PD1
                //ROL
                //NK1
                //PV1
                //PV2
                //GT1













               



                //runTerserExpression("MSH-6", terserHelper);
                //runTerserExpression("/.PID-5-2", terserHelper);
                //runTerserExpression("/.*ID-5-2", terserHelper);
                //runTerserExpression("/.P?D-5-2", terserHelper);
                //runTerserExpression("/.PV1-9(1)-1", terserHelper);
                //runTerserExpression("/RESPONSE/PATIENT/PID-5-1", terserHelper);
                //runTerserExpression("/RESPONSE/PATIENT/VISIT/PV1-9-3", terserHelper);
                //runTerserExpression("/RESPONSE/ORDER_OBSERVATION(0)/OBSERVATION(1)/OBX-3", terserHelper);
                //runTerserExpression("/.ORDER_OBSERVATION(0)/ORC-12-3", terserHelper);
                //runTerserExpression("/.OBSERVATION(0)/NTE-3", terserHelper);




                ////cast to ACK message to get access to ACK message data
                //var ackResponseMessage = hl7Message as ACK;
                //if (ackResponseMessage != null)
                //{
                //    //access message data and display it
                //    //note that I am using encode method at the end to convert it back to string for display
                //    var mshSegmentMessageData = ackResponseMessage.MSH;
                //    Console.WriteLine("Message Type is " + mshSegmentMessageData.MessageType.MessageType);
                //    Console.WriteLine("Message Control Id is " + mshSegmentMessageData.MessageControlID);
                //    Console.WriteLine("Message Timestamp is " + mshSegmentMessageData.DateTimeOfMessage.TimeOfAnEvent.GetAsDate());
                //    Console.WriteLine("Sending Facility is " + mshSegmentMessageData.SendingFacility.NamespaceID.Value);

                //    //update message data in MSA segment
                //    ackResponseMessage.MSA.AcknowledgementCode.Value = "AR";
                //}



                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured -> {e.StackTrace}");
                }
        }
  


            static string runTerserExpression(string terserExpression, OurTerserHelper terserHelper)
            {
                string dataRetrieved = null;
                try
                {
                    dataRetrieved = terserHelper.GetData(terserExpression);
                    Console.WriteLine($"Field 6 of MSH segment using expression '{terserExpression}' was '{dataRetrieved}'");
            
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error for '{terserExpression}': " + ex.Message);
                }

                return dataRetrieved;
            }



       



        static void parseHL7_XML()
        {



            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            int intCnt = 1;
            string folderPath = ConfigurationManager.AppSettings["XML_Path"];
            //IEnumerable<string> strXMLFiles = Directory.EnumerateFiles(folderPath, "*.xml");
            IEnumerable<string> strXMLFiles = GetFiles(folderPath, "*.xml");         //Winscp file access c#
            foreach (string file in strXMLFiles)
            //foreach (string file in Directory.EnumerateFiles(folderPath, "ATHENA_2983_EI_CLINICALENCOUNTER_6453014855_20200601233428.xml"))
            //foreach (string file in Directory.EnumerateFiles(folderPath, "QRDA_AMB_ENHF_07-01-20_02-46-30.553.xml"))
            {

                //Console.WriteLine("Searching ("+ intCnt +" of "+ strXMLFiles.Count() + ") : " + file);
                Console.WriteLine("Searching (" + intCnt + " of Recursive????) : " + file);


                XDocument xdoc = null;

                try
                {
                    xdoc = XDocument.Load(file);
                }
                catch (Exception)
                {

                    continue;
                }

                var ns = xdoc.Root.Name.Namespace;

                //var query = from t in xdoc.Descendants(ns + "patient").Elements(ns + "name")
                //            where t.Element(ns + "family").Value.ToLower() == "anderson" && t.Element(ns + "given").Value.ToLower() == "glenn"
                //            select new
                //            {
                //                // ID = t.Attribute("family").Value,
                //                given = t.Element(ns + "given").Value,
                //                family = t.Element(ns + "family").Value
                //            };

                var patient = from t in xdoc.Descendants(ns + "patient")
                              select new
                              {
                                  // ID = t.Attribute("family").Value,
                                  given = (t.Elements(ns + "name").Elements(ns + "given").Any() ? "'" + t.Elements(ns + "name").Elements(ns + "given").FirstOrDefault().Value.Replace("'", "''").Trim() + "'" : "NULL"),
                                  family = (t.Elements(ns + "name").Elements(ns + "family").Any() ? "'" + t.Elements(ns + "name").Elements(ns + "family").FirstOrDefault().Value.Replace("'", "''").Trim() + "'" : "NULL"),
                                  birthTime = (t.Elements(ns + "birthTime").Attributes("value").Any() ? "'" + t.Elements(ns + "birthTime").Attributes("value").FirstOrDefault().Value.Replace("'", "''").Trim().Substring(0, 8) + "'" : "NULL"),
                                  maritalStatusCode = (t.Elements(ns + "maritalStatusCode").Attributes("displayName").Any() ? "'" + t.Elements(ns + "maritalStatusCode").Attributes("displayName").FirstOrDefault().Value.Replace("'", "''").Trim() + "'" : "NULL"),
                                  raceCode = (t.Elements(ns + "raceCode").Attributes("displayName").Any() ? "'" + t.Elements(ns + "raceCode").Attributes("displayName").FirstOrDefault().Value.Replace("'", "''").Trim() + "'" : "NULL")

                              };





                //var names = from t in xdoc.Descendants(ns + "patient").Elements(ns + "name")
                //            select new
                //            {
                //                // ID = t.Attribute("family").Value,
                //                given = t.Element(ns + "given").Value,
                //                family = t.Element(ns + "family").Value
                //            };



                //var birthdates = from t in xdoc.Descendants(ns + "patient").Elements(ns + "birthTime")
                //             where t.Attribute("value").Value.ToLower() == "19581208"
                //             select new
                //             {
                //                 // ID = t.Attribute("family").Value,
                //                 birthTime = t.Attribute("value").Value

                //             };






                if (patient.Count() > 0)
                {
                    Console.WriteLine("Found!!!");

                    foreach (var p in patient)
                    {
                        var sql = "INSERT INTO [dbo].[CG_CCD_PROTOTYPE] ([given_name] ,[family_name] ,[birthtime],[maritalStatusCode],[raceCode] ,[hl7_filename]) VALUES( " + p.given + ", " + p.family + "," + p.birthTime + "," + p.maritalStatusCode + "," + p.raceCode + ",'" + file.Replace("'", "''").Trim() + "')";

                        DBConnection32.ExecuteMSSQL(strConnectionString, sql);

                    }



                    //Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("No Match!!!");
                }

                intCnt++;
                //var query2 = from t in xdoc.Descendants(ns + "patient").Elements(ns + "birthTime")
                //            where t.Attribute("value").Value.ToLower() == "19581208"
                //             select new
                //            {
                //                 // ID = t.Attribute("family").Value,
                //                 birthTime = t.Attribute("value").Value

                //            };

            }
        }
        




        static IEnumerable<string> GetFiles(string path, string strExtension)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path, strExtension);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }



























        static XElement FindParameter(XElement element, string type)
        {
            return element.Elements("parameter")
                          .SingleOrDefault(p => (string)p.Attribute("type") == type);
        }




    }




    public class OurTerserHelper
    {
        private readonly Terser _terser;

        public OurTerserHelper(Terser terser)
        {
            if (terser == null)
                throw new ArgumentNullException(nameof(terser),
                    "Terser object must be passed in for data retrieval operation");

            _terser = terser;
        }

        public string GetData(string terserExpression)
        {

            if (string.IsNullOrEmpty(terserExpression))
                throw new ArgumentNullException(nameof(terserExpression),
                    "Terser expression must be supplied for data retrieval operation");
            return _terser.Get(terserExpression);
        }

        public void SetData(string terserExpression, string value)
        {

            if (string.IsNullOrEmpty(terserExpression))
                throw new ArgumentNullException(nameof(terserExpression),
                    "Terser expression must be supplied for set operation");

            if (value == null) //we will let an empty string still go through
                throw new ArgumentNullException(nameof(value), "Value for set operation must be supplied");

            _terser.Set(terserExpression, value);
        }
    }





}
