namespace GeoFleetService {
    partial class ProjectInstaller {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent() {
            this.mainServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.GeoFleetServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // mainServiceProcessInstaller
            // 
            this.mainServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.mainServiceProcessInstaller.Password = null;
            this.mainServiceProcessInstaller.Username = null;
            // 
            // GeoFleetServiceInstaller
            // 
            this.GeoFleetServiceInstaller.Description = "Sincronizacion de GeoFleet con el origen de datos local";
            this.GeoFleetServiceInstaller.DisplayName = "GeoFleet Sync";
            this.GeoFleetServiceInstaller.ServiceName = "GeoFleetDownloadService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.mainServiceProcessInstaller,
            this.GeoFleetServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller mainServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller GeoFleetServiceInstaller;
    }
}