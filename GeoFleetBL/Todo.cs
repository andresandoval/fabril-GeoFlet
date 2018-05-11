using GeoFleetBL.GeoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;
using System.Threading;


namespace GeoFleetBL {
    public class Todo {

        private Boolean manual = false;
        private ApiHelper api;

        private String dbConnection;

        private String token;
        private String emp;
        private String imei;

        private Double devicesTimer;
        private Double dailyHistoryTimer;
        private DateTime startDailyHistorySyncDate;

        private devicesTableAdapter dta;
        private dailyHistoryTableAdapter dhta;
        private DevicesSyncLogTableAdapter dslta;
        private LogHelper log;

        private String serviceName;
        private readonly AutoResetEvent mWaitForThread = new AutoResetEvent(false);

        public Todo() {

            try {
                this.dbConnection = System.Configuration.ConfigurationManager.ConnectionStrings["GeoFleetService.Properties.Settings.geoFleetConnectionString"].ConnectionString;


                this.token = System.Configuration.ConfigurationManager.AppSettings["apiToken"].ToString();
                this.emp = System.Configuration.ConfigurationManager.AppSettings["enterpriceName"].ToString();
                this.imei = System.Configuration.ConfigurationManager.AppSettings["deviceImei"].ToString();

                this.devicesTimer = Double.Parse(System.Configuration.ConfigurationManager.AppSettings["devicesTimer"].ToString());
                this.dailyHistoryTimer = Double.Parse(System.Configuration.ConfigurationManager.AppSettings["dailyHistoryTimer"].ToString());
                this.startDailyHistorySyncDate = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["startDailyHistorySyncDate"].ToString());

                this.api = new ApiHelper(this.token);

                this.dta = new devicesTableAdapter();
                this.dhta = new dailyHistoryTableAdapter();
                this.dslta = new DevicesSyncLogTableAdapter();
                this.dta.Connection.ConnectionString = this.dbConnection;
                this.dhta.Connection.ConnectionString = this.dbConnection;
                this.dslta.Connection.ConnectionString = this.dbConnection;

                this.log = new LogHelper(System.Configuration.ConfigurationManager.AppSettings["serviceName"].ToString());

                this.serviceName = System.Configuration.ConfigurationManager.AppSettings["serviceName"].ToString();
                this.log.write("Servicio iniciado exitosamente");

            } catch (Exception ex) {
                this.devicesTimer = 0;
                this.dailyHistoryTimer = 0;
                this.log = new LogHelper("Error");
                this.log.write("Ocurrio un error al iniciar el servicio : " + ex.ToString());
            }
        }

        public Todo(String __conn, String __token, String __emp, String __imei, Double __devicesTimer, Double __dailyHistoryTimer, String __startDailyHistorySyncDate, String __serviceName) {
            try {
                this.dbConnection = __conn;

                this.token = __token;
                this.emp = __emp;
                this.imei = __imei;

                this.devicesTimer = __devicesTimer;
                this.dailyHistoryTimer = __dailyHistoryTimer;
                this.startDailyHistorySyncDate = DateTime.Parse(__startDailyHistorySyncDate);

                this.api = new ApiHelper(this.token);

                this.dta = new devicesTableAdapter();
                this.dhta = new dailyHistoryTableAdapter();
                this.dslta = new DevicesSyncLogTableAdapter();
                this.dta.Connection.ConnectionString = this.dbConnection;
                this.dhta.Connection.ConnectionString = this.dbConnection;

                this.dslta.Connection.ConnectionString = this.dbConnection;

                this.log = new LogHelper(__serviceName);
                this.serviceName = __serviceName;
                this.log.write("Configuracion cargada exitosamente iniciado exitosamente");
                this.log.write("Iniciando sincronizacion manual");
                this.manual = true;

            } catch (Exception ex) {
                this.devicesTimer = 0;
                this.dailyHistoryTimer = 0;
                this.log = new LogHelper("Error");
                this.log.write("Ocurrio un error al cargar la informacion : " + ex.ToString());
            }
        }

        public Double getDevicesTimer() {
            return this.devicesTimer;
        }

        public Double getDailyHistoryTimer() {
            return this.dailyHistoryTimer;
        }

        public void SyncDevices() {
            var api = new ApiHelper(this.token);
            this.log.write("Sincronizacion de vehiculos", true);

            try {
                var request = api.GetdeviceInfo();
                this.log.write("Recuperados " + request.Length + " registros");
                if (request != null) {
                    if (!string.IsNullOrWhiteSpace(emp)) {
                        request = request.Where(o => o.Empresa.Equals(emp)).ToArray();
                        this.log.write("Filtrados " + request.Length + " registros por empresa: " + emp);
                    }
                }

                int count = 0;

                if (request.Length > 0) {
                    for (int i = 0; i < request.Length; i++) {
                        deviceInfo vehiculo = request[i];
                        DataTable dt = this.dta.GetDeviceById(vehiculo.imei);

                        if (dt.Rows.Count == 0) {
                            this.dta.Insert(vehiculo.imei,
                              vehiculo.alias,
                              vehiculo.placa,
                              vehiculo.ramv_cpn,
                              vehiculo.chasis,
                              vehiculo.motor,
                              vehiculo.Tipo_Combustible,
                              vehiculo.Año,
                              vehiculo.marca,
                              vehiculo.modelo,
                              vehiculo.color1,
                              vehiculo.color2,
                              vehiculo.Tipo_Conductor,
                              vehiculo.Propietario,
                              vehiculo.Empresa,
                              vehiculo.Tipo_Cobertura,
                              vehiculo.Tipo_Carroceria_1,
                              vehiculo.Tipo_Carroceria_2,
                              vehiculo.Transporte,
                              vehiculo.Unidades,
                              vehiculo.Capacidad,
                              vehiculo.Cilindraje,
                              vehiculo.Unidad_consumo_combustible,
                              vehiculo.Ultima_Actualizacion);

                            count++;
                        }
                        this.log.write("Insertados  " + count + " registros");
                    }
                }
            } catch (Exception ex) {
                this.log.write("Ocurrio un error: " + ex.ToString());
            } finally {
                this.log.write("Sincronizacion de vehiculos", true, true);
            }

        }

        private String generateTransactionId() {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Random rnd = new Random();
            int one = rnd.Next(100);
            int two = rnd.Next(1000);
            int three = rnd.Next(10000);

            String transactionID = "";

            try {
                transactionID = unixTimestamp.ToString() + one.ToString() + two.ToString() + three.ToString();
                transactionID.Replace("'", "''");
            } catch (Exception ex) {
                this.log.write("Error al generar ID de transaccion : " + ex.ToString());
                transactionID = null;
            }
            return transactionID;
        }

        private List<DailyHistory> getDailyHistoryRequests() {
            this.log.write("Creando id de transaccion...");
            String transactionId = this.generateTransactionId();
            if (transactionId == null) {
                return null;
            }
            this.log.write("Armando pila de requests.. ");
            List<DailyHistory> requests = new List<DailyHistory>();

            List<String> __imeis = new List<String>();

            Object __tmpDbSyncFrom;
            DateTime __tmpSyncFrom;
            List<DateTime> __tmpSyncDates;
            DateTime __tmpDate;

            //DateTime __syncTo = new DateTime(2017, 04, 10);
            DateTime __syncTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            int index = 1;
            if (String.IsNullOrEmpty(this.imei) || String.IsNullOrWhiteSpace(this.imei) || this.imei == "*") {
                DataTable dtImei = this.dta.GetData();
                if (dtImei.Rows.Count <= 0) {
                    this.log.write("No hay vehiculos en BD para armar la pila de requests");
                    return null;
                }
                foreach (DataRow r in dtImei.Rows) {
                    __imeis.Add(r.Field<string>("imei"));
                }
            } else {
                string[] imeiLines = this.imei.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                __imeis = imeiLines.OfType<String>().ToList();
            }


            foreach (String __imei in __imeis) {
                try {
                    __tmpDbSyncFrom = this.dslta.GetLastSyncDateById(__imei);
                    if (__tmpDbSyncFrom == null) {
                        __tmpDbSyncFrom = this.dhta.GetMaxSyncDateByImei(__imei);
                    }
                } catch (Exception ex) {
                    this.log.write("Valor por defecto asignado a " + __imei + ", motivo:" + ex.ToString());
                    __tmpDbSyncFrom = null;
                }

                if (__tmpDbSyncFrom == null) {
                    __tmpSyncFrom = new DateTime(this.startDailyHistorySyncDate.Year, this.startDailyHistorySyncDate.Month, this.startDailyHistorySyncDate.Day);
                    this.log.write("Listo para sincronizar " + __imei + " desde " + __tmpSyncFrom.ToLongDateString() + "(sin datos previos)");
                } else {
                    __tmpSyncFrom = (DateTime)__tmpDbSyncFrom;
                    this.log.write("Listo para sincronizar " + __imei + " desde " + __tmpSyncFrom.ToLongDateString());
                }

                __tmpDate = __tmpSyncFrom;
                __tmpSyncDates = new List<DateTime>();
                do {
                    __tmpSyncDates.Add(__tmpDate);
                    __tmpDate = __tmpDate.AddDays(1).Date;

                } while (__tmpDate <= __syncTo);
                foreach (DateTime syncDate in __tmpSyncDates) {
                    requests.Add(new DailyHistory(__imei, syncDate, this.api, this.log, this.dbConnection, transactionId, index));
                    index++;
                }
            }
            return requests;
        }

        public void SyncDailyHistory() {
            this.log.write("Sincronizacion del historial", true);
            List<DailyHistory> requests = this.getDailyHistoryRequests();

            if (requests == null || requests.Count <= 0) {
                this.log.write("No hay nada para sincronizar");
                this.log.write("Sincronizacion del historial", true, true);
                return;
            }
            DateTime startSync = DateTime.Now;
            int total = requests.Count;
            foreach (DailyHistory hr in requests) {
                hr.sync(total);
            }
            double minutes = DateTime.Now.Subtract(startSync).TotalMinutes;
            this.log.write("Sincronizacion del historial, tiempo = " + String.Format("{0:0.00}", minutes) + " min.", true, true);
        }
        
        public void end() {
            this.log.write(this.manual ? "Fin de sincronizacion manual" : "Servicio detenido..");
        }
    }
}