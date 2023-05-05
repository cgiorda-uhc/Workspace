
using EISLogging;
using System;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace EISLogTesting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //PEI
            //AskID = UHGWM110-008381
            //CI=CI10436978
            //AppName=PEI - Physicians Engagement and Improvement

            //COMPANION
            //AskID = UHGWM110-021466
            //CI=CI100358099
            //AppName=UCS Companion Application

            //https://aide.optum.com/


            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

            string pemPath = AppDomain.CurrentDomain.BaseDirectory + @"optum.pem";
            LogData ld = new LogData
            {
                device = new Device
                {
                    hostname = Environment.MachineName
                },
                destUser = new DestUser
                {
                    uid = Environment.UserName,

                    //GRAPHANA DASHBOARDS
                    //https://github.optum.com/pages/eis/security-logs/grafana.html
                    //SCORE CARD
                    //JSON WEB TOKEN - BASE 64 encoded -TOKEN DATA
                    

                    //HOW TO TEST??? 
                    //WHAT ABOUT TOKEN VALUES????
                    //CHRIS ADDED
                    uuid = UserPrincipal.Current.EmployeeId, //string	Backend ID or UUID of which represents the uid
                    firstName = UserPrincipal.Current.DisplayName, //string	First name of user
                    lastName = UserPrincipal.Current.Surname, //string	Last name of user
                    tokenIssuer = null, //string	The issuer of the token
                    tokenCreated = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), //long	Time the token was created in the Epoch time format (1587999958) *SAMPLE1
                    tokenExpires = unixTime, //	long	Time the token expires in the Epoch time format *SAMPLE2
                    tokenHash = null //string	SHA256 hash of authorization token
                },
                logClass = LogClass.SECURITY_AUDIT,
                severity = severity.INFO,
                msg = "CreateUserSession: SUCCESS"
            };
            await EISLog.Produce(ld, pemPath);
        }




        /*FORMAL NAME  PEI - Physicians Engagement and Improvement  
   SHORT NAME  PEI2  
   ALIAS NAME  PEIPortal2;PEI - Physicians Engagement and Improvement;PEI  
   CATEGORY  Application  
   SOFTWARE TYPE  UHG Product  
 
 
Identifies whether the application was developed internally or externally
UHG Product
Vendor Product  
   SOFTWARE VENDOR    
   DELIVERY MODEL  ASP - Application Service Provider  
   DEVELOPMENT METHODOLOGY  Other  
   APPLICATION LIFECYCLE STATUS  Production  
   BUSINESS OWNED  No  
   
  ICD IMPACT  No  
   M AND A COMPANY  Not Applicable  
   INFORMATION CLASSIFICATION  Protected Information  
   DOCUMENTATION      
   ASK GLOBAL ID  UHGWM110-008381  
   CMDB REFERENCE  CI10436978  
   DR REFERENCE    
   TMDB REFERENCE  TMDB-3541290  
   eGRC REFERENCE  APPID-2558679  
   CLOUD ADOPTION LEVEL  Non-Adopters 


  FORMAL NAME  UCS Companion Application  
   SHORT NAME  UCS Companion Application  
   ALIAS NAME  UCS Companion Application  
   CATEGORY  Application  
   SOFTWARE TYPE  UHG Product  
   SOFTWARE VENDOR    
   DELIVERY MODEL  Other  
   DEVELOPMENT METHODOLOGY  Other  
   APPLICATION LIFECYCLE STATUS  Production  
   BUSINESS OWNED  Yes  
   
  ICD IMPACT  No  
   M AND A COMPANY  Not Applicable  
   INFORMATION CLASSIFICATION  Protected Information  
   DOCUMENTATION      
   ASK GLOBAL ID  UHGWM110-021466  
   CMDB REFERENCE  CI100358099  
   DR REFERENCE    
   TMDB REFERENCE  TMDB-8553476  
   eGRC REFERENCE  APPID-9456129  
   CLOUD ADOPTION LEVEL  Non-Adopters 

 */



    }
}



