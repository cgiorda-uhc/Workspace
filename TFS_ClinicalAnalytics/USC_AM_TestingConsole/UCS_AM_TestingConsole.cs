using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS_AM_ProcessManager;
using System.Timers;

namespace USC_AM_TestingConsole
{
    class UCS_AM_TestingConsole
    {

        private static int eventId = 1;
        private static EventLog eventLog = null;
        private static Timer timer = new System.Timers.Timer();
        static void Main(string[] args)
        {
            eventId = 1;

            //SETUP TIMER
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval =5000; // = 60 seconds 1000 ms => 1 second 
            timer.Enabled = true;
            timer.Start();

            //KEEP RUNNING
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;

        }

        private static void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            
            try
            {
                timer.Enabled = false; //PAUSE TIMER DURING EXECUTION BUT KEEP TICKS
                //timer.Stop();
                //blIsRunningGLOBAL = true;
                eventId = ProcessManager.ManageProcesses(eventLog, eventId);
            }
            catch(Exception ex)
            {
                string str = ex.ToString();
            }
            finally
            {
                timer.Enabled = true;
                //timer.Start();
                //blIsRunningGLOBAL = false;
            }

            return;

        }
    }
}
