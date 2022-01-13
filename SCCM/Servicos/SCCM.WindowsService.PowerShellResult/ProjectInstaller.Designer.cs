namespace SCCM.WindowsService.PowerShellResult
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
            this.PowerShellResultInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PowerShellResultServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PowerShellResultInstaller
            // 
            this.PowerShellResultInstaller.Password = null;
            this.PowerShellResultInstaller.Username = null;
            // 
            // PowerShellResultServiceInstaller
            // 
            this.PowerShellResultServiceInstaller.Description = "Serviço responsável pela busca de resultados do SCCMPowerShellExecucao";
            this.PowerShellResultServiceInstaller.DisplayName = "SCCM PowerShellResultAgent";
            this.PowerShellResultServiceInstaller.ServiceName = "SCCM PowerShellResultAgent";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PowerShellResultInstaller,
            this.PowerShellResultServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PowerShellResultInstaller;
        private System.ServiceProcess.ServiceInstaller PowerShellResultServiceInstaller;
    }
}