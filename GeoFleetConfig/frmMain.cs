using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using GeoFleetConfig.Properties;
using GeoFleetBL;
using System.Diagnostics;

namespace GeoFleetConfig {

    public enum SERVICE_OPERATIONS {
        INSTALL,
        UNINSTALL,
        RESTART,
        START,
        STOP
    }
    public partial class frmMain : Form {

        private Configuration serviceConfig;
        private ServiceController mainService;
        private String servicePath;

        public frmMain() {
            InitializeComponent();
        }

        private bool doesServiceExist() {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == this.serviceConfig.AppSettings.Settings["serviceName"].Value);
            return (ctl != null);
        }

        private String getServiceStatus() {
            if (!this.doesServiceExist()) {
                return "No Instalado";
            }
            switch (this.mainService.Status) {
                case ServiceControllerStatus.Running:
                    return "Corriendo";
                case ServiceControllerStatus.Stopped:
                    return "Detenido";
                case ServiceControllerStatus.Paused:
                    return "En pausa";
                case ServiceControllerStatus.StopPending:
                    return "Deteniendose";
                case ServiceControllerStatus.StartPending:
                    return "Iniciando";
                default:
                    return "Cambiando estado";
            }
        }

        private void findServiceConfigFIle() {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de configuracion|*.config;";
            ofd.Title = "Cargar archivo de configuracion del servicio";
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog().Equals(DialogResult.OK)) {
                try {

                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                    config.AppSettings.Settings["serviceConfigurationFile"].Value = ofd.FileName;
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                    this.DownloadConfiguration();

                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Error: actualizando configuracion local");
                    this.Dispose();
                }
            } else {
                this.Dispose();
            }
        }

        private void DownloadConfiguration() {
            Cursor.Current = Cursors.WaitCursor;
            try {
                string serviceConfigFile = System.Configuration.ConfigurationManager.AppSettings["serviceConfigurationFile"];

                if (File.Exists(serviceConfigFile)) {

                    this.servicePath = Path.GetDirectoryName(serviceConfigFile);

                    ExeConfigurationFileMap serviceExeConfig = new ExeConfigurationFileMap();
                    serviceExeConfig.ExeConfigFilename = serviceConfigFile;
                    this.serviceConfig = ConfigurationManager.OpenMappedExeConfiguration(serviceExeConfig, ConfigurationUserLevel.None);

                    this.txtToken.Text = this.serviceConfig.AppSettings.Settings["apiToken"].Value;

                    this.txtEnterpriceName.Text = this.serviceConfig.AppSettings.Settings["enterpriceName"].Value;

                    string __logFileName = this.serviceConfig.AppSettings.Settings["logFileName"].Value;
                    if (String.IsNullOrEmpty(__logFileName) || String.IsNullOrWhiteSpace(__logFileName)) {
                        __logFileName = Path.Combine(Path.GetDirectoryName(serviceConfigFile), this.serviceConfig.AppSettings.Settings["serviceName"].Value + ".exe.log");
                    }
                    this.linkLogSrv.Tag = __logFileName;

                    int __maxDays = int.Parse(System.Configuration.ConfigurationManager.AppSettings["maxExecutionDays"]);

                    Object[] __days = new Object[__maxDays + 1];
                    for (int i = 0; i < __days.Length; i++) {
                        __days[i] = i.ToString();
                    }
                    this.cbxVehicleDay.Items.Clear();
                    this.cbxVehicleDay.Items.AddRange(__days);
                    this.cbxHistoryDay.Items.Clear();
                    this.cbxHistoryDay.Items.AddRange(__days);

                    double __timerVehicle = Double.Parse(this.serviceConfig.AppSettings.Settings["devicesTimer"].Value);
                    double __timerHistory = Double.Parse(this.serviceConfig.AppSettings.Settings["dailyHistoryTimer"].Value);
                    double __x;

                    if (__timerVehicle > 0) {
                        __x = __timerVehicle / 1000;
                        double __timerVehicleSeconds = __x % 60;
                        __x /= 60;
                        double __timerVehicleMinutes = __x % 60;
                        __x /= 60;
                        double __timerVehicleHours = __x % 24;
                        __x /= 24;
                        double __timerVehicleDays = __x;

                        this.cbxVehicleDay.Text = Math.Floor(__timerVehicleDays).ToString();
                        this.cbxVehicleHour.Text = Math.Floor(__timerVehicleHours).ToString();
                        this.cbxVehicleMin.Text = Math.Floor(__timerVehicleMinutes).ToString();

                    } else {
                        this.cbxVehicleDay.Text = "0";
                        this.cbxVehicleHour.Text = "0";
                        this.cbxVehicleMin.Text = "0";
                        this.cbxVehicleDay.Enabled = false;
                        this.cbxVehicleHour.Enabled = false;
                        this.cbxVehicleMin.Enabled = false;
                        this.chkVehicleInactive.Checked = true;
                    }

                    if (__timerHistory > 0) {
                        __x = __timerHistory / 1000;
                        double __timerHistorySeconds = __x % 60;
                        __x /= 60;
                        double __timerHistoryMinutes = __x % 60;
                        __x /= 60;
                        double __timerHistoryHours = __x % 24;
                        __x /= 24;
                        double __timerHistoryDays = __x;

                        this.cbxHistoryDay.Text = Math.Floor(__timerHistoryDays).ToString();
                        this.cbxHistoryHour.Text = Math.Floor(__timerHistoryHours).ToString();
                        this.cbxHistoryMin.Text = Math.Floor(__timerHistoryMinutes).ToString();

                    } else {
                        this.cbxHistoryDay.Text = "0";
                        this.cbxHistoryHour.Text = "0";
                        this.cbxHistoryMin.Text = "0";
                        this.cbxHistoryDay.Enabled = false;
                        this.cbxHistoryHour.Enabled = false;
                        this.cbxHistoryMin.Enabled = false;
                        this.chkHistoryInactive.Checked = true;
                    }

                    this.txtVehicleImei.Text = this.serviceConfig.AppSettings.Settings["deviceImei"].Value;
                    this.dateHistoryStartSyncDate.Value = DateTime.Parse(this.serviceConfig.AppSettings.Settings["startDailyHistorySyncDate"].Value);

                    this.mainService = new ServiceController(this.serviceConfig.AppSettings.Settings["serviceName"].Value);

                    this.Text += " : " + this.serviceConfig.AppSettings.Settings["serviceDisplayName"].Value;
                    this.trayNotifyIcon.BalloonTipTitle = this.Text;
                    this.trayNotifyIcon.BalloonTipText = "Aun se esta ejecutando la aplicacion de " + this.Text;
                    this.BringToFront();
                    this.Activate();
                    this.timerServiceStatus.Enabled = true;

                } else {
                    if (MessageBox.Show("No se puede acceder a la configuracion del servicio, ¿Desea buscar el archivo de configuracion?", "Error: archivo de configuracion", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        this.findServiceConfigFIle();
                    } else {
                        this.Dispose();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error: descargando configuracion");
                this.Dispose();
            } finally {
                Cursor.Current = Cursors.Default;
            }

        }

        private void UploadConfiguration() {
            Cursor.Current = Cursors.WaitCursor;
            try {
                if (this.serviceConfig != null) {
                    if (this.txtToken.TextLength <= 0) {
                        MessageBox.Show("El campo 'Token' es obligatorio..");
                        return;
                    }

                    if (MessageBox.Show("¿Esta seguro que desea actualizar la configuracion del servicio de sincronizacion?, si elige continuar, se guardaran los cambios realizados y se procedera a reiniciar el servicio para su efecto inmediato, ", "Confirmar actualizacion", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                        return;
                    }

                    this.serviceConfig.AppSettings.Settings["apiToken"].Value = this.txtToken.Text;
                    this.serviceConfig.AppSettings.Settings["enterpriceName"].Value = this.txtEnterpriceName.Text;
                    this.serviceConfig.AppSettings.Settings["logFileName"].Value = this.linkLogSrv.Tag.ToString();

                    double __vehicleTimer = (Double.Parse(this.cbxVehicleDay.Text) * 1000 * 60 * 60 * 24) + (Double.Parse(this.cbxVehicleHour.Text) * 1000 * 60 * 60) + (Double.Parse(this.cbxVehicleMin.Text) * 1000 * 60);
                    double __historyTimer = (Double.Parse(this.cbxHistoryDay.Text) * 1000 * 60 * 60 * 24) + (Double.Parse(this.cbxHistoryHour.Text) * 1000 * 60 * 60) + (Double.Parse(this.cbxHistoryMin.Text) * 1000 * 60);
                    this.serviceConfig.AppSettings.Settings["devicesTimer"].Value = __vehicleTimer.ToString();
                    this.serviceConfig.AppSettings.Settings["dailyHistoryTimer"].Value = __historyTimer.ToString();

                    this.serviceConfig.AppSettings.Settings["deviceImei"].Value = this.txtVehicleImei.Text;
                    this.serviceConfig.AppSettings.Settings["startDailyHistorySyncDate"].Value = this.dateHistoryStartSyncDate.Value.ToShortDateString();

                    this.serviceConfig.Save();
                    this.mainService.Refresh();

                    if (this.doesServiceExist()) {

                        if (this.mainService.Status == ServiceControllerStatus.Running || this.mainService.Status == ServiceControllerStatus.Paused || this.mainService.Status == ServiceControllerStatus.StartPending) {
                            this.mainService.Stop();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Stopped);
                            this.mainService.Start();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Running);
                        } else {
                            this.mainService.Start();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Running);
                        }

                        MessageBox.Show("Proceso completado exitosamente, puede revisar el archivo LOG para mas detalles..");
                    } else {
                        MessageBox.Show("Se cambios fueron guardados, pero el servicio no esta instalado en este equipo, pulse Servicio->Instalar para instalar el servicio", "Atención!");
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error: cargando configuracion");
            } finally {
                Cursor.Current = Cursors.Default;
            }

        }

        private Todo getTodo() {
            Todo __todo = null;

            String __conn;
            String __token;
            String __emp;
            String __imei;
            Double __devicesTimer;
            Double __dailyHistoryTimer;
            String __startDailyHistorySyncDate;
            String __serviceName;

            try {
                string serviceConfigFile = System.Configuration.ConfigurationManager.AppSettings["serviceConfigurationFile"];

                if (File.Exists(serviceConfigFile)) {
                    ExeConfigurationFileMap serviceExeConfig = new ExeConfigurationFileMap();
                    serviceExeConfig.ExeConfigFilename = serviceConfigFile;
                    this.serviceConfig = ConfigurationManager.OpenMappedExeConfiguration(serviceExeConfig, ConfigurationUserLevel.None);

                    __conn = this.serviceConfig.ConnectionStrings.ConnectionStrings["GeoFleetService.Properties.Settings.geoFleetConnectionString"].ConnectionString;
                    __token = this.serviceConfig.AppSettings.Settings["apiToken"].Value;
                    __emp = this.serviceConfig.AppSettings.Settings["enterpriceName"].Value;
                    __imei = this.serviceConfig.AppSettings.Settings["deviceImei"].Value;
                    __serviceName = this.serviceConfig.AppSettings.Settings["serviceName"].Value;

                    __devicesTimer = Double.Parse(this.serviceConfig.AppSettings.Settings["devicesTimer"].Value);
                    __dailyHistoryTimer = Double.Parse(this.serviceConfig.AppSettings.Settings["dailyHistoryTimer"].Value);
                    __startDailyHistorySyncDate = this.serviceConfig.AppSettings.Settings["startDailyHistorySyncDate"].Value;

                    __todo = new Todo(__conn, __token, __emp, __imei, __devicesTimer, __dailyHistoryTimer, __startDailyHistorySyncDate, __serviceName);

                } else {
                    MessageBox.Show("No existe un archivo de configuracion", "Error: descargando configuracion");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error: descargando configuracion");
            }
            return __todo;
        }

        private void frmMain_Load(object sender, EventArgs e) {
            this.timerServiceStatus.Enabled = false;
            this.DownloadConfiguration();
        }

        private void linkLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (this.linkLogSrv.Tag != null && File.Exists(this.linkLogSrv.Tag.ToString())) {
                System.Diagnostics.Process.Start(this.linkLogSrv.Tag.ToString());
            } else {
                MessageBox.Show("No existe un archivo de log para mostrar, este se genera luego de haber iniciado el servicio", "Abrir archivo de log");
            }
        }

        private void chkVehicleInactive_CheckedChanged(object sender, EventArgs e) {
            this.cbxVehicleDay.Text = "0";
            this.cbxVehicleHour.Text = "0";
            this.cbxVehicleMin.Text = "0";
            this.cbxVehicleDay.Enabled = !this.chkVehicleInactive.Checked;
            this.cbxVehicleHour.Enabled = !this.chkVehicleInactive.Checked;
            this.cbxVehicleMin.Enabled = !this.chkVehicleInactive.Checked;
        }

        private void chkHistoryInactive_CheckedChanged(object sender, EventArgs e) {
            this.cbxHistoryDay.Text = "0";
            this.cbxHistoryHour.Text = "0";
            this.cbxHistoryMin.Text = "0";
            this.cbxHistoryDay.Enabled = !this.chkHistoryInactive.Checked;
            this.cbxHistoryHour.Enabled = !this.chkHistoryInactive.Checked;
            this.cbxHistoryMin.Enabled = !this.chkHistoryInactive.Checked;
        }

        private void btnApply_Click(object sender, EventArgs e) {
            this.UploadConfiguration();
        }

        private void frmMain_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.Hide();
                this.trayNotifyIcon.Visible = true;
                this.trayNotifyIcon.ShowBalloonTip(2000);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            if (MessageBox.Show("¿Esta seguro?", "Cerrar aplicacion", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                this.Dispose();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void trayNotifyIcon_DoubleClick(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            this.ShowIcon = true;
            this.ShowInTaskbar = true;
            this.trayNotifyIcon.Visible = false;
            this.Show();
            this.BringToFront();
            this.Activate();
        }

        private void trayNotifyIcon_MouseMove(object sender, MouseEventArgs e) {
            this.mainService.Refresh();
            this.trayNotifyIcon.Text = this.serviceConfig.AppSettings.Settings["serviceDisplayName"].Value + ": " + this.getServiceStatus();
        }

        private void btnSyncVehicles_Click(object sender, EventArgs e) {
            if (MessageBox.Show("¿Esta seguro?\n Se realizara la sincronizacion con la ultima configuracion valida", "Sincronizar vehiculos", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Todo __todo = this.getTodo();
                if (__todo == null) {
                    return;
                }
                Cursor.Current = Cursors.WaitCursor;
                try {
                    __todo.SyncDevices();
                    MessageBox.Show("Sincronizacion completa", ":)");
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Error: sincronizando vehiculos");
                } finally {
                    __todo.end();
                    __todo = null;
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void btnSyncHistory_Click(object sender, EventArgs e) {
            if (MessageBox.Show("¿Esta seguro?\n Se realizara la sincronizacion con la ultima configuracion valida", "Sincronizar historial", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Todo __todo = this.getTodo();
                if (__todo == null) {
                    return;
                }
                Cursor.Current = Cursors.WaitCursor;
                try {
                    __todo.SyncDailyHistory();
                    MessageBox.Show("Sincronizacion completa", ":)");
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Error: sincronizando historial");
                } finally {
                    __todo.end();
                    __todo = null;
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void timerServiceStatus_Tick(object sender, EventArgs e) {
            this.mainService.Refresh();
            this.lblServiceStatus.Text = "Estado del servicio: " + this.getServiceStatus();
        }

        private void linkLogMan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            String logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.serviceConfig.AppSettings.Settings["serviceName"].Value + ".exe.log");
            if (File.Exists(logFile)) {
                System.Diagnostics.Process.Start(logFile);
            } else {
                MessageBox.Show("No existe un archivo de log para mostrar, este se genera luego de realizar una sincronizacion manual", "Abrir archivo de log");
            }
        }

        private void serviceOperations(SERVICE_OPERATIONS type) {
            Cursor.Current = Cursors.WaitCursor;
            try {
                if (doesServiceExist()) {
                    if (type == SERVICE_OPERATIONS.INSTALL) {
                        MessageBox.Show("El servicio ya se encuentra instalado en el equipo!!", "Atención!");
                        return;
                    }
                    if (this.mainService.Status == ServiceControllerStatus.Running || this.mainService.Status == ServiceControllerStatus.Paused || this.mainService.Status == ServiceControllerStatus.StartPending) {
                        if (type == SERVICE_OPERATIONS.STOP) {
                            this.mainService.Stop();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Stopped);
                            MessageBox.Show("Servicio detenido correctamente!!", "Informacion!");
                            return;
                        }
                        if (type == SERVICE_OPERATIONS.RESTART) {
                            this.mainService.Stop();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Stopped);
                            this.mainService.Start();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Running);
                            MessageBox.Show("Servicio reiniciado correctamente!!", "Informacion!");
                            return;
                        }
                        if (type == SERVICE_OPERATIONS.START) {
                            MessageBox.Show("El servicio ya se encuentra en ejecucion!!", "Atencion!");
                            return;
                        }
                    } else {
                        if (type == SERVICE_OPERATIONS.STOP) {
                            MessageBox.Show("El servicio no se encuentra en ejecucion!!", "Atencion!");
                            return;
                        }
                        if (type == SERVICE_OPERATIONS.RESTART || type == SERVICE_OPERATIONS.START) {
                            this.mainService.Start();
                            this.mainService.WaitForStatus(ServiceControllerStatus.Running);
                            MessageBox.Show("Servicio iniciado correctamente!!", "Informacion!");
                            return;
                        }
                    }
                } else {
                    if (type == SERVICE_OPERATIONS.UNINSTALL) {
                        MessageBox.Show("El servicio no se encuentra instalado en el equipo, pulse Servicio->Instalar para instalar el servicio", "Atención!");
                        return;
                    }
                }

                if (MessageBox.Show("¿Esta seguro que desea " + (type == SERVICE_OPERATIONS.INSTALL ? "instalar" : "desinstalar") + " el servicio en el equipo?", "Confirmar instalacion", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                    return;
                }
                String __serviceFilePath = Path.Combine(this.servicePath, this.serviceConfig.AppSettings.Settings["serviceName"].Value + ".exe");
                if (!File.Exists(__serviceFilePath)) {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Aplicaciones|*.exe;";
                    ofd.Title = "Instalador de servicio servicio";
                    ofd.CheckFileExists = true;
                    if (ofd.ShowDialog().Equals(DialogResult.OK)) {
                        __serviceFilePath = ofd.FileName;
                    } else {
                        return;
                    }
                }
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = __serviceFilePath;
                startInfo.Arguments = (type == SERVICE_OPERATIONS.INSTALL ? "-install" : "-uninstall");
                Process.Start(startInfo);
                MessageBox.Show("Servicio " + (type == SERVICE_OPERATIONS.INSTALL ? "instalado" : "desinstalado") + " correctamente!!", "Informacion!");
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error: on operaciones sobre servicio");
            } finally {
                Cursor.Current = Cursors.Default;
            }
        }

        private void instalarToolStripMenuItem_Click(object sender, EventArgs e) {
            this.serviceOperations(SERVICE_OPERATIONS.INSTALL);
        }

        private void desinstalarToolStripMenuItem_Click(object sender, EventArgs e) {
            this.serviceOperations(SERVICE_OPERATIONS.UNINSTALL);
        }

        private void reiniciarToolStripMenuItem_Click(object sender, EventArgs e) {
            this.serviceOperations(SERVICE_OPERATIONS.RESTART);
        }

        private void iniciarToolStripMenuItem_Click(object sender, EventArgs e) {
            this.serviceOperations(SERVICE_OPERATIONS.START);
        }

        private void detenerToolStripMenuItem_Click(object sender, EventArgs e) {
            this.serviceOperations(SERVICE_OPERATIONS.STOP);
        }

    }
}
