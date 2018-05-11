using GeoFleetBL.GeoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;


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
        private LogHelper log;

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
                this.dta.Connection.ConnectionString = this.dbConnection;
                this.dhta.Connection.ConnectionString = this.dbConnection;

                this.log = new LogHelper(System.Configuration.ConfigurationManager.AppSettings["serviceName"].ToString());
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
                this.dta.Connection.ConnectionString = this.dbConnection;
                this.dhta.Connection.ConnectionString = this.dbConnection;

                this.log = new LogHelper(__serviceName);
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

        public void SyncDailyHistory() {
            this.log.write("Sincronizacion del historial", true);

            //formar lista de IMEIS a sincronizar
            List<String> __imeis = new List<String>();
            if (String.IsNullOrEmpty(this.imei) || String.IsNullOrWhiteSpace(this.imei) || this.imei == "*") {
                DataTable dtImei = this.dta.GetData();
                if (dtImei.Rows.Count <= 0) {
                    this.log.write("No hay vehiculos en BD para sincronizar el historial");
                    this.log.write("Sincronizacion del historial", true, true);
                    return;
                }
                foreach (DataRow r in dtImei.Rows) {
                    __imeis.Add(r.Field<string>("imei"));
                }
            } else {
                string[] imeiLines = this.imei.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                __imeis = imeiLines.OfType<String>().ToList();
            }
            this.log.write("Iniciando sincronizacion de los vehiculos: " + String.Join(", ", __imeis.ToArray()));

            DateTime __tmpSyncFrom;
            Object __tmpDbSyncFrom;
            List<DateTime> __tmpSyncDates;
            DateTime __syncTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime __tmpDate;

            var api = new ApiHelper(this.token);

            int __tmpCount;

            try {

                foreach (String __imei in __imeis) {

                    //Generar lista de fechas por sincronizar por cada IMEI
                    __tmpDbSyncFrom = this.dhta.GetMaxSyncDateByImei(__imei);

                    if (__tmpDbSyncFrom == null) {
                        __tmpSyncFrom = new DateTime(this.startDailyHistorySyncDate.Year, this.startDailyHistorySyncDate.Month, this.startDailyHistorySyncDate.Day);
                        this.log.write("No hay datos sncronizados previamente para " + __imei + ", iniciando desde " + __tmpSyncFrom.ToLongDateString());
                    } else {
                        __tmpSyncFrom = (DateTime)__tmpDbSyncFrom;
                        this.log.write("Recuperando datos desde la ultima sincronizacion de " + __imei + " del " + __tmpSyncFrom.ToLongDateString());
                    }

                    __tmpDate = __tmpSyncFrom;
                    __tmpSyncDates = new List<DateTime>();
                    do {
                        __tmpSyncDates.Add(__tmpDate);
                        __tmpDate = __tmpDate.AddDays(1).Date;

                    } while (__tmpDate < __syncTo);


                    foreach (DateTime f in __tmpSyncDates) {

                        var request = api.GetdailyHistory(__imei, f.Year, f.Month, f.Day, "-5", "es");

                        this.log.write("Recuperado  " + request.Count + " registros de " + f.ToShortDateString() + " para IMEI " + __imei);

                        if (request != null && request.Count > 0) {
                            __tmpCount = 0;
                            foreach (var item in request) {
                                item.imei = __imei;

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
                                    __tmpCount++;
                                }
                            }
                            this.log.write("Insertados  " + __tmpCount + " registros de " + f.ToShortDateString() + " para IMEI " + __imei);
                        }
                    }
                }
            } catch (Exception ex) {
                this.log.write("Ocurrio un error: " + ex.ToString());
            } finally {
                this.log.write("Sincronizacion del historial", true, true);
            }
        }

        public void end() {
            this.log.write(this.manual ? "Fin de sincronizacion manual" : "Servicio detenido..");
        }
    }
}