using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IR_SAS_SQL_Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string strILUCA_ConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
        string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
        string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

        string strSAS_Alias = ConfigurationManager.AppSettings["SAS_Alias"];
        string strSAS_Path = ConfigurationManager.AppSettings["SAS_Path"];

        public MainWindow()
        {

            InitializeComponent();

            //string strLib = "libname UHN  SQLSVR user=cgiorda password=abc123!! path=UHN_Reporting_IWA Connection=Global;run;";
            string strSQL = "select distinct TaxID ,CorpOwnerID ,CorpOwnerName FROM UHN.TAXID (obs=5) where taxid in(10617776,112223066,113135947,113528432,132710076,133351625,133425028,133497895, 133963197,133628017,161536482,330285170,330968516,352278014,510309666,460623701,471323463, 510303373,510610479,521183370,521984673,522004795,522055746,562629193,562631760,591288427, 680031320,680627403,710996388,710996392,711012364,770245054,770460670,770573991,821973466, 943378936,954651287,262656991,420634700);";
            strSQL = "proc sql OUTOBS=5;select distinct TaxID ,CorpOwnerID ,CorpOwnerName FROM UHN.TAXID;";
            strSQL = "select distinct TaxID ,CorpOwnerID ,CorpOwnerName FROM UHN.TAXID WHERE CorpOwnerName like 'freed%';";



            //IR_SAS_Connect.handle_SubmitComplete += OnSqlRowsCopied;

            txtSQL.Text = strSQL;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {

            string strSQL = txtSQL.Text;

            if (string.IsNullOrEmpty(strSQL))
                return;


            //EXECUTE
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                DataTable dtResults = IR_SAS_Connect.runPassthroughSQLCommandsDT(strSQL);
                if (dtResults != null)
                {
                    gridResults.DataContext = dtResults.DefaultView;
                }
                else
                    MessageBox.Show("NULL RESULTS: " + Environment.NewLine + strSQL);
            }
            catch(Exception ex)
            {
                MessageBox.Show("SAS SQL ERROR: " + Environment.NewLine +  ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }

        private void btnExecuteProc_Click(object sender, RoutedEventArgs e)
        {

            string strSQL = txtSQL.Text;

            if (string.IsNullOrEmpty(strSQL))
                return;


            //EXECUTE
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                IR_SAS_Connect.runProcSQLCommands(strSQL);

   
   
                MessageBox.Show(IR_SAS_Connect.strProcSQLResults);
        

    
                //MessageBox.Show("NULL RESULTS: " + Environment.NewLine + strSQL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SAS SQL ERROR: " + Environment.NewLine + ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }




        private void btnGetLog_Click(object sender, RoutedEventArgs e)
        {

            //EXECUTE
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                
                MessageBox.Show(IR_SAS_Connect.strSASConnectionLog);

                //MessageBox.Show("NULL RESULTS: " + Environment.NewLine + strSQL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SAS SQL ERROR: " + Environment.NewLine + ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


            private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IR_SAS_Connect.strSASConnectionString != null)
                IR_SAS_Connect.destroy_SAS_instance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];
                //IR_SAS_Connect.create_SAS_instance(strSAS_Alias, strSAS_Path);
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());
            }
            catch (Exception ex)
            {
                MessageBox.Show("SAS CONNECT ERROR: " + Environment.NewLine + ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }


        }

        private void btnLib_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in IR_SAS_Connect.arrLibnames)
            {
                sb.Append(s + Environment.NewLine);
            }

            MessageBox.Show(sb.ToString());
        }
    }
}
