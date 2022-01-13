namespace SCCM.WindowsService.PowerShellExecucao
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
            this.PowerShellExecucaoInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PowerShellExecucaoServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PowerShellExecucaoInstaller
            // 
            this.PowerShellExecucaoInstaller.Password = null;
            this.PowerShellExecucaoInstaller.Username = null;
            // 
            // PowerShellExecucaoServiceInstaller
            // 
            this.PowerShellExecucaoServiceInstaller.Description = "Serviço responsável pela execução de scripts SCCM/PowerShell";
            this.PowerShellExecucaoServiceInstaller.DisplayName = "SCCM PowerShellAgent";
            this.PowerShellExecucaoServiceInstaller.ServiceName = "SCCM PowerShellAgent";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PowerShellExecucaoInstaller,
            this.PowerShellExecucaoServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PowerShellExecucaoInstaller;
        private System.ServiceProcess.ServiceInstaller PowerShellExecucaoServiceInstaller;
    }
}