using System;
using GalaSoft.MvvmLight.Messaging;


namespace UCS_Project_Manager
{
    public class AppMessages
    {
        public static class ProjectChangeTracking
        {
            public static void Send(bool argument)
            {
                Messenger.Default.Send<bool>(argument);
            }


            //CHRIS ADDED
            public static void Send(string argument)
            {
                Messenger.Default.Send<string>(argument);
            }

            ////CHRIS ADDED
            public static void Send(ETG_Fact_Symmetry_Update_Tracker argument)
            {
                Messenger.Default.Send<ETG_Fact_Symmetry_Update_Tracker>(argument);
            }


            public static void Register(object recipient, Action<bool> action)
            {
                Messenger.Default.Register(recipient, action);
            }

            //CHRIS ADDED
            public static void Register(object recipient, Action<string> action)
            {
                Messenger.Default.Register(recipient, action);
            }


            ////CHRIS ADDED
            public static void Register(object recipient, Action<ETG_Fact_Symmetry_Update_Tracker> action)
            {
                Messenger.Default.Register(recipient, action);
            }
        }
    }

  

}
