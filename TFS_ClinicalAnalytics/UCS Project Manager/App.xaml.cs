using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //EXCEPTIONS
        //https://stackoverflow.com/questions/1472498/wpf-global-exception-handler/1472562#1472562

        protected override void OnStartup(StartupEventArgs e)
        {
            // Global exception handling  
            AppDomain.CurrentDomain.UnhandledException += (s,ex) =>
    LogUnhandledException((Exception)ex.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Application.Current.DispatcherUnhandledException += (s, ex) =>
                LogUnhandledException(ex.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, ex) =>
                LogUnhandledException(ex.Exception, "TaskScheduler.UnobservedTaskException");


            base.OnStartup(e);

            if (File.Exists(GlobalState.strVersionPath))
            {
                File.Delete(GlobalState.strVersionPath);
            }

            //throw new Exception("Oh nooooooo!!!");
        }


        bool blError = false;
        private void LogUnhandledException(Exception exception, string @event)
        {
  

            //MessageBox.Show(exception.Message + Environment.NewLine + @event, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);

            if(blError == false)
            {
                var message = (exception.InnerException != null ? exception.InnerException.ToString() : exception.ToString());
                MessageBox.Show(message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                blError = true;
            }

            this.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            this.Shutdown();
            return;





            //_log.Exception(exception)
            //    .Data("Event", @event)
            //    .Fatal("Unhandled exception");

            //// wait until the logmanager has written the entry
            //_log.LogManager.FlushEntriesAsOf(DateTimeOffset.Now.AddSeconds(1));
        }

        void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string errorMessage = string.Format("An application error occurred.\nPlease check whether your data is correct and repeat the action. If this error occurs again there seems to be a more serious malfunction in the application, and you better close it.\n\nError: {0}\n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)",

            e.Exception.Message + (e.Exception.InnerException != null ? "\n" +
            e.Exception.InnerException.Message : null));
            MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);


            //if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            //{
            //    if (MessageBox.Show("WARNING: The application will close. Any changes will not be saved!\nDo you really want to close it?", "Close the application!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            //    {
            //        Application.Current.Shutdown();
            //    }
            //}

        }

    }

}
