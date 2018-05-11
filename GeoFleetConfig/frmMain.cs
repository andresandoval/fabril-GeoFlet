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

namespace GeoFleetConfig {
    public partial class frmMain : Form {

        private Configuration serviceConfig;


        public frmMain() {
            InitializeComponent();
        }

        private System.Timers.Timer checkServiceStatus;
        private ServiceController mainService;// = new ServiceController(SERVICENAME);


        private String getServiceStatus() {
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
                    ExeConfigurationFileMap serviceExeConfig = new ExeConfigurationFileMap();
                    serviceExeConfig.ExeConfigFilename = serviceConfigFile;
                    this.serviceConfig = ConfigurationManager.OpenMappedExeConfiguration(serviceExeConfig, ConfigurationUserLevel.None);

                    this.txtToken.Text = this.serviceConfig.AppSettings.Settings["apiToken"].Value;

                    this.txtEnterpriceName.Text = this.serviceConfig.AppSettings.Settings["enterpriceName"].Value;

                    string __logFileName = this.serviceConfig.AppSettings.Settings["logFileName"].Value;
                    if (String.IsNullOrEmpty(__logFileName) || String.IsNullOrWhiteSpace(__logFileName)) {
                        __logFileName = Path.Combine(Path.GetDirectoryName(serviceConfigFile), this.serviceConfig.AppSettings.Settings["serviceName"].Value + ".exe.log");
                    }
                    if (File.Exists((__logFileName))) {
                        this.linkLog.Tag = __logFileName;
                        this.linkLog.Text = "Abrir archivo de log";
                    } else {
                        this.linkLog.Tag = null;
                        this.linkLog.Text = "No hay un archivo de log";
                    }

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

                    this.Text += " : " + this.serviceConfig.AppSettings.Settings["serviceDisplayName"].Value;
                    this.trayNotifyIcon.BalloonTipTitle = this.Text;

                    string __serviceDescription = this.serviceConfig.AppSettings.Settings["serviceDescription"].Value;

                    if (this.checkServiceStatus == null) {
                        this.checkServiceStatus = new System.Timers.Timer(1500D);
                        this.checkServiceStatus.AutoReset = true;
                        this.mainService = new ServiceController(this.serviceConfig.AppSettings.Settings["serviceName"].Value);
                        this.checkServiceStatus.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) => {
                            this.mainService.Refresh();
                            this.lblServiceStatus.Text = "Estado del servicio: " + this.getServiceStatus();
                            this.trayNotifyIcon.BalloonTipText = __serviceDescription + "\n\n" + this.lblServiceStatus.Text;
                        });
                        this.checkServiceStatus.Start();
                    }
                    this.BringToFront();
                    this.Activate();

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
                    if (this.txtVehicleImei.TextLength <= 0) {
                        MessageBox.Show("El campo 'IMEI' es obligatorio..");
                        return;
                    }

                    if (MessageBox.Show("¿Esta seguro que desea actualizar la configuracion del servicio de sincronizacion?, si elige continuar, se guardaran los cambios realizados y se procedera a reiniciar el servicio para su efecto inmediato, ", "Confirmar actualizacion", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                        return;
                    }

                    this.serviceConfig.AppSettings.Settings["apiToken"].Value = this.txtToken.Text;
                    this.serviceConfig.AppSettings.Settings["enterpriceName"].Value = this.txtEnterpriceName.Text;
                    this.serviceConfig.AppSettings.Settings["logFileName"].Value = this.linkLog.Tag.ToString();

                    double __vehicleTimer = (Double.Parse(this.cbxVehicleDay.Text) * 1000 * 60 * 60 * 24) + (Double.Parse(this.cbxVehicleHour.Text) * 1000 * 60 * 60) + (Double.Parse(this.cbxVehicleMin.Text) * 1000 * 60);
                    double __historyTimer = (Double.Parse(this.cbxHistoryDay.Text) * 1000 * 60 * 60 * 24) + (Double.Parse(this.cbxHistoryHour.Text) * 1000 * 60 * 60) + (Double.Parse(this.cbxHistoryMin.Text) * 1000 * 60);
                    this.serviceConfig.AppSettings.Settings["devicesTimer"].Value = __vehicleTimer.ToString();
                    this.serviceConfig.AppSettings.Settings["dailyHistoryTimer"].Value = __historyTimer.ToString();

                    this.serviceConfig.AppSettings.Settings["deviceImei"].Value = this.txtVehicleImei.Text;
                    this.serviceConfig.AppSettings.Settings["startDailyHistorySyncDate"].Value = this.dateHistoryStartSyncDate.Value.ToShortDateString();

                    this.serviceConfig.Save();
                    this.mainService.Refresh();
                    if (this.mainService.CanStop) {
                        this.mainService.Stop();
                    }
                    this.mainService.Start();
                    MessageBox.Show("Proceso completado exitosamente, puede revisar el archivo LOG para mas detalles..");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error: cargando configuracion");
            } finally {
                Cursor.Current = Cursors.Default;
            }

        }


        private void frmMain_Load(object sender, EventArgs e) {
            this.DownloadConfiguration();
        }

                private void linkLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (this.linkLog.Tag != null) {
                System.Diagnostics.Process.Start(this.linkLog.Tag.ToString());
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

    }
}
