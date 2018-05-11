using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeoFleetBL {
    class LogHelper {

        private String logName;

        private String getNow() {
            return String.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }

        public LogHelper(String fileName) {
            this.logName = String.Format("{0}\\{1}.exe.log", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),fileName);
        }


        private bool checkFileExists() {
            FileStream fs = null;
            Byte[] info = null;
            try {
                if (!File.Exists(this.logName)) {
                    fs = File.Create(this.logName);
                    info = new UTF8Encoding(true).GetBytes(String.Format("{0} : Generacion de registro de  eventos para Descarga GoeFleet... \n\n", this.getNow()));
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                return true;
            } catch (Exception ex) { }
            return false;
        }

        public void write(String msg, bool title = false, bool end = false) {
            StreamWriter lw = null;
            try {
                if (this.checkFileExists()) {
                    lw = new StreamWriter(this.logName, true, Encoding.UTF8);
                    if (title) {
                        lw.WriteLine(String.Format("{0}: ====> <{1}> <====", this.getNow(), msg.ToUpper()));
                    } else if (end) {
                        lw.WriteLine(String.Format("{0}: ====> </{1}> <====\n\n\n\n", this.getNow(), msg.ToUpper()));
                    } else {
                        lw.WriteLine(String.Format("{0}: {1}", this.getNow(), msg));
                    }
                }
            } finally {
                if (lw != null) {
                    lw.Close();
                }
            }
        }
    }
}
