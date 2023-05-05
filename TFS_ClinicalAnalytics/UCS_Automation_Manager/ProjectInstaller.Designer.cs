namespace UCS_Automation_Manager
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UCS_Automation_Manager_serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.UCS_Automation_Manager_serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // UCS_Automation_Manager_serviceProcessInstaller
            // 
            this.UCS_Automation_Manager_serviceProcessInstaller.Password = null;
            this.UCS_Automation_Manager_serviceProcessInstaller.Username = null;
            // 
            // UCS_Automation_Manager_serviceInstaller
            // 
            this.UCS_Automation_Manager_serviceInstaller.Description = "Manages various UCS automated processes";
            this.UCS_Automation_Manager_serviceInstaller.DisplayName = "UCS Automation Manager Service";
            this.UCS_Automation_Manager_serviceInstaller.ServiceName = "UCS_Automation_Manager";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.UCS_Automation_Manager_serviceProcessInstaller,
            this.UCS_Automation_Manager_serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller UCS_Automation_Manager_serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller UCS_Automation_Manager_serviceInstaller;
    }
}