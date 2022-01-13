namespace SCCM.WindowsService.TriggerForce
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
            this.SCCMTriggerForceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.TriggerForceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SCCMTriggerForceProcessInstaller
            // 
            this.SCCMTriggerForceProcessInstaller.Password = null;
            this.SCCMTriggerForceProcessInstaller.Username = null;
            // 
            // TriggerForceInstaller
            // 
            this.TriggerForceInstaller.Description = "Serviço responsável pela execução da Trigger - Force - do SCCM API";
            this.TriggerForceInstaller.DisplayName = "SCCM TriggerForce";
            this.TriggerForceInstaller.ServiceName = "SCCMTriggerForce";
            this.TriggerForceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SCCMTriggerForceProcessInstaller,
            this.TriggerForceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SCCMTriggerForceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller TriggerForceInstaller;
    }
}