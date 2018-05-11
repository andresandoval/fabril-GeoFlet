using GeoFleetService.ApiDatasetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;


namespace GeoFleetService {
    class All {

        private ApiHelper api;

        private String dbConnection;

        private String token;// = "254896B1AD2A440A855B62B55344EC3E";
        private String emp;// = "EXTRACTORA RIO MANSO";
        private String imei;// = "867162025136011";

        private String logFileName;

        private Double devicesTimer;
        private Double dailyHistoryTimer;
        private DateTime startDailyHistorySyncDate;

        private devicesTableAdapter dta;
        private dailyHistoryTableAdapter dhta;
        private LogHelper log;

        public All() {

            try {
                this.dbConnection = System.Configuration.ConfigurationManager.ConnectionStrings["GeoFleetService.Properties.Settings.geoFleetConnectionString"].ConnectionString;

                //this.token = Properties.Settings.Default.apiToken;
                //this.emp = Properties.Settings.Default.enterpriceName;
                //this.imei = Properties.Settings.Default.deviceImei;

                //this.logFileName = Properties.Settings.Default.logFileName;

                //this.devicesTimer = Properties.Settings.Default.devicesTimer;
                //this.dailyHistoryTimer = Properties.Settings.Default.dailyHistoryTimer;
                //this.startDailyHistorySyncDate = DateTime.Parse(Properties.Settings.Default.startDailyHistorySyncDate);


                this.token = System.Configuration.ConfigurationManager.AppSettings["apiToken"].ToString();
                this.emp = System.Configuration.ConfigurationManager.AppSettings["enterpriceName"].ToString();
                this.imei = System.Configuration.ConfigurationManager.AppSettings["deviceImei"].ToString();

                this.logFileName = System.Configuration.ConfigurationManager.AppSettings["logFileName"].ToString();

                this.devicesTimer = Double.Parse(System.Configuration.ConfigurationManager.AppSettings["devicesTimer"].ToString());
                this.dailyHistoryTimer = Double.Parse(System.Configuration.ConfigurationManager.AppSettings["dailyHistoryTimer"].ToString());
                this.startDailyHistorySyncDate = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["startDailyHistorySyncDate"].ToString());

                this.api = new ApiHelper(this.token);

                this.dta = new devicesTableAdapter();
                this.dhta = new dailyHistoryTableAdapter();
                this.dta.Connection.ConnectionString = this.dbConnection;
                this.dhta.Connection.ConnectionString = this.dbConnection;

                this.log = new LogHelper(this.logFileName);
                this.log.write("Servicio iniciado exitosamente");

            } catch (Exception ex) {
                this.devicesTimer = 0;
                this.dailyHistoryTimer = 0;
                this.log = new LogHelper();
                this.log.write("Ocurrio un error al iniciar el servicio : " + ex.ToString());
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

        public void SyncDailyHistory() {
            this.log.write("Sincronizacion del historial", true);


            DateTime desde;
            Object dateFromDB = this.dhta.GetMaxSyncDateByImei(this.imei);
            if (dateFromDB == null) {
                desde = new DateTime(this.startDailyHistorySyncDate.Year, this.startDailyHistorySyncDate.Month, this.startDailyHistorySyncDate.Day);
                this.log.write("No hay datos sncronizados previamente, iniciando desde " + desde.ToLongDateString());
            } else {
                desde = (DateTime)dateFromDB;
                this.log.write("Recuperando datos desde la ultima sincronizacion del " + desde.ToLongDateString());
            }

            DateTime hasta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            List<DateTime> fechas = new List<DateTime>();
            var api = new ApiHelper(this.token);

            DateTime fecha = desde.Date;
            // Crear lista de fechas a procesar

            String tmpLog = "";
            do {
                tmpLog += fecha.ToShortDateString() + ", ";
                fechas.Add(fecha);
                fecha = fecha.AddDays(1).Date;

            } while (fecha < hasta);

            this.log.write("Intentando recuperar datos de " + tmpLog);
            int count;

            try {
                foreach (var f in fechas) {
                    var request = api.GetdailyHistory(this.imei, f.Year, f.Month, f.Day, "-5", "es");

                    this.log.write("Recuperado  " + request.Count + " registros de " + f.ToShortDateString());

                    if (request != null && request.Count > 0) {
                        count = 0;
                        foreach (var item in request) {
                            item.imei = imei;

                            DataTable dt = this.dhta.GetHistoryById(item.uniqueId);

                            //SI no existe, se ingresa
                            if (dt.Rows.Count == 0) {
                                this.dhta.Insert(
                              item.imei,
                              item.date,
                              item.time.TimeOfDay,
                              item.uniqueId,
                              item.latitude,
                              item.longitude,
                              item.address,
                              item.speed,
                              item.mileage,
                              item.temperature_1,
                              item.temperature_2,
                              item.temperature_3,
                              item.temperature_4,
                              item.internal_battery_level,
                              item.is_exception,
                              item.alert_message,
                              item.zone);
                                count++;
                            }
                        }
                        this.log.write("Insertados  " + count + " registros de " + f.ToShortDateString());
                    }
                }
            } catch (Exception ex) {
                this.log.write("Ocurrio un error: " + ex.ToString());
            } finally {
                this.log.write("Sincronizacion del historial", true, true);
            }
        }

        public void end() {
            this.log.write("Servicio detenido..");
        }
    }
}