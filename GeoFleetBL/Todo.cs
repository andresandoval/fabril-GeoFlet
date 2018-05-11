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

        public void SyncDailyHistory_old() {
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

                Parallel.ForEach(__imeis, __imei => {

                    //Generar lista de fechas por sincronizar por cada IMEI
                    var __dhta1 = new dailyHistoryTableAdapter();
                    __dhta1.Connection.ConnectionString = this.dbConnection;
                    __tmpDbSyncFrom = __dhta1.GetMaxSyncDateByImei(__imei);

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


                    Parallel.ForEach(__tmpSyncDates, f => {

                        var request = api.GetdailyHistory(__imei, f.Year, f.Month, f.Day, "-5", "es");

                        this.log.write("Recuperado  " + request.Count + " registros de " + f.ToShortDateString() + " para IMEI " + __imei);

                        if (request != null && request.Count > 0) {
                            __tmpCount = 0;
                            foreach (var item in request) {
                                item.imei = __imei;
                                var __dhta2 = new dailyHistoryTableAdapter();
                                __dhta2.Connection.ConnectionString = this.dbConnection;
                                DataTable dt = __dhta2.GetHistoryById(item.uniqueId);

                                //SI no existe, se ingresa
                                if (dt.Rows.Count == 0) {
                                    __dhta2.Insert(
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
                    });
                });
            } catch (Exception ex) {
                this.log.write("Ocurrio un error: " + ex.ToString());
            } finally {
                this.log.write("Sincronizacion del historial", true, true);
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

            List<String> __sql = new List<String>();
            String __tmpSql = "";
            int __currentCount = 0;
            int __maxRows = 1000;
            int __totalRows = 0;
            Boolean __startOfStack = true;

            try {

                foreach (String __imei in __imeis) {

                    //Generar lista de fechas por sincronizar por cada IMEI
                    var __dhta1 = new dailyHistoryTableAdapter();
                    __dhta1.Connection.ConnectionString = this.dbConnection;
                    __tmpDbSyncFrom = __dhta1.GetMaxSyncDateByImei(__imei);

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

                    } while (__tmpDate <= __syncTo);


                    foreach (DateTime f in __tmpSyncDates) {

                        this.log.write("Iniciando descarga de registros del " + f.ToShortDateString() + " para el IMEI " + __imei);
                        var request = api.GetdailyHistory(__imei, f.Year, f.Month, f.Day, "-5", "es");
                        this.log.write("Descargados  " + request.Count + " registros del " + f.ToShortDateString() + " para el IMEI: " + __imei);

                        if (request != null && request.Count > 0) {

                            foreach (var item in request) {
                                item.imei = __imei;

                                if (__currentCount >= __maxRows) {
                                    __sql.Add(__tmpSql);
                                    __startOfStack = true;
                                    __currentCount = 0;
                                }

                                if (__startOfStack) {
                                    __tmpSql = "";
                                    __startOfStack = false;
                                } else {
                                    __tmpSql += ", ";
                                }

                                __tmpSql += String.Format("('{0}', '{1}', '{2}', '{3}', {4}, {5}, '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, '{15}', '{16}')",
                                    item.imei,
                                    item.date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                    item.time.TimeOfDay.ToString(),
                                    item.uniqueId,
                                    item.latitude.ToString(CultureInfo.InvariantCulture),
                                    item.longitude.ToString(CultureInfo.InvariantCulture),
                                    item.address,
                                    item.speed.ToString(CultureInfo.InvariantCulture),
                                    item.mileage.ToString(CultureInfo.InvariantCulture),
                                    item.temperature_1.HasValue ? item.temperature_1.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                    item.temperature_2.HasValue ? item.temperature_2.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                    item.temperature_3.HasValue ? item.temperature_3.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                    item.temperature_4.HasValue ? item.temperature_4.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                    item.internal_battery_level,
                                    item.is_exception ? "1" : "0",
                                    item.alert_message,
                                    item.zone);

                                __currentCount++;
                                __totalRows++;
                            }
                        }
                    }
                }
                if (__tmpSql.Length > 0) {
                    __sql.Add(__tmpSql);
                }
                String __headSql = "insert into tmpDailyHistory(imei, date, time, uniqueId, latitude, longitude, address, speed, mileage, temperature_1, temperature_2, temperature_3, temperature_4, internal_battery_level, is_exception, alert_message, zone) values ";
                this.log.write("Recoleccion completa: " + __totalRows + " registros, iniciando escritura..");
                using (SqlConnection connection = new SqlConnection(this.dbConnection)) {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select 1", connection);
                    foreach (String s in __sql) {
                        command.CommandText = __headSql + s;
                        command.ExecuteNonQuery();
                    }
                    this.log.write("Escritura completa completa, normalizando datos..");
                    command.CommandText = "insert into dailyHistory (imei, date, time, uniqueId, latitude, longitude, address, speed, mileage, temperature_1, temperature_2, temperature_3, temperature_4, internal_battery_level, is_exception, alert_message, zone) select t1.imei, t1.date, t1.time, t1.uniqueId, t1.latitude, t1.longitude, t1.address, t1.speed, t1.mileage, t1.temperature_1, t1.temperature_2, t1.temperature_3, t1.temperature_4, t1.internal_battery_level, t1.is_exception, t1.alert_message, t1.zone from (select *, ROW_NUMBER() OVER(PARTITION BY uniqueId ORDER BY dailyHistoryId DESC) row_count from tmpdailyHistory) t1 where NOT EXISTS(SELECT 1 FROM dailyHistory t2 WHERE t2.uniqueId = t1.uniqueId) and row_count = 1";
                    command.ExecuteNonQuery();
                    this.log.write("Normalizacion completa, iniciando limpieza...");
                    command.CommandText = "truncate table tmpDailyHistory";
                    command.ExecuteNonQuery();
                    connection.Close();
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