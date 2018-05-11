using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GeoFleetService {
    public partial class MainService : ServiceBase {
        private System.Timers.Timer timerDevices;
        private System.Timers.Timer timerDailyHistory;
        private All all;

        public MainService() {
            InitializeComponent();
            this.all = new All();
        }


        private void runDailyHistorySync(object sender, System.Timers.ElapsedEventArgs e) {

        }

        protected override void OnStart(string[] args) {
            if (this.all.getDevicesTimer() > 0) {
                this.timerDevices = new System.Timers.Timer(this.all.getDevicesTimer());
                this.timerDevices.AutoReset = true;
                this.timerDevices.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) => {
                    this.all.SyncDevices();
                });
                this.timerDevices.Start();
            }

            if (this.all.getDailyHistoryTimer() > 0) {
                this.timerDailyHistory = new System.Timers.Timer(this.all.getDailyHistoryTimer());
                this.timerDailyHistory.AutoReset = true;
                this.timerDailyHistory.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) => {
                    this.all.SyncDailyHistory();
                });
                this.timerDailyHistory.Start();
            }

        }

        protected override void OnStop() {
            if (this.timerDevices != null) {
                this.timerDevices.Stop();
                this.timerDevices = null;
            }
            if (this.timerDailyHistory != null) {
                this.timerDailyHistory.Stop();
                this.timerDailyHistory = null;
            }            
            this.all.end();
            this.all = null;
        }
    }
}
