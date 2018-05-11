using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using GeoFleetBL;
using System.Timers;


using System.Reflection;
using System.IO;

namespace GeoFleetService {

    public partial class MainService : ServiceBase {

        private System.Timers.Timer timerDevices;
        private System.Timers.Timer timerDailyHistory;
        private Todo todo;              

        public MainService() {
            InitializeComponent();
            todo = new Todo();
        }

        private void OnDevicesTimer(object source, ElapsedEventArgs e) {
            this.todo.SyncDevices();
        }
        private void OnDailyHistoryTimer(object source, ElapsedEventArgs e) {
            this.todo.SyncDevices();
        }


        protected override void OnStart(string[] args) {
            if (this.todo.getDevicesTimer() > 0) {
                this.timerDevices = new System.Timers.Timer(this.todo.getDevicesTimer());
                this.timerDevices.AutoReset = true;
                this.timerDevices.Elapsed += new ElapsedEventHandler(OnDevicesTimer);
                this.timerDevices.Start();
            }

            if (this.todo.getDailyHistoryTimer() > 0) {
                this.timerDailyHistory = new System.Timers.Timer(this.todo.getDailyHistoryTimer());
                this.timerDailyHistory.AutoReset = true;
                this.timerDailyHistory.Elapsed += new ElapsedEventHandler(OnDailyHistoryTimer);
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
            this.todo.end();
            this.todo = null;
        }
    }
}
