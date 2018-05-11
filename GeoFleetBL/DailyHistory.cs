using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFleetBL {
    class DailyHistory {

        private String imei;
        private DateTime date;
        private ApiHelper api;
        private LogHelper log;
        private String dbConnection;
        private String transactionId;
        private int index;
        private int year {
            get {
                return this.date.Year;
            }
        }
        private int month {
            get {
                return this.date.Month;
            }
        }
        private int day {
            get {
                return this.date.Day;
            }
        }
        private string timeZone {
            get {
                return "-5";
            }
        }
        private string culture {
            get {
                return "es";
            }
        }
        private string myTransactionId {
            get {
                return this.transactionId + "_" + this.index.ToString();
            }
        }

        public DailyHistory(String _imei, DateTime _date, ApiHelper _api, LogHelper _log, String _dbConnection, String _transactionId, int _index) {
            this.imei = _imei;
            this.date = _date;
            this.api = _api;
            this.log = _log;
            this.dbConnection = _dbConnection;
            this.transactionId = _transactionId;
            this.index = _index;
        }

        public void sync(int _total) {

            List<String> issues = new List<String>();
            List<dailyHistory> result = null;
            SqlConnection connection = null;
            SqlCommand command;
            String __headSql = "insert into tmpDailyHistory(imei, date, time, uniqueId, latitude, longitude, address, speed, mileage, temperature_1, temperature_2, temperature_3, temperature_4, internal_battery_level, is_exception, alert_message, zone, transactionID) values ";
            List<String> __sql = new List<String>();
            String __tmpSql = "";
            int __currentCount = 0;
            int __maxRows = 1000;
            Boolean __startOfStack = true;


            //crear conexion a base de datos local
            try {
                connection = new SqlConnection(this.dbConnection);
                connection.Open();
            } catch (Exception ex) {
                this.log.write("[" + this.index + "/" + _total + "] No se pudo establecer conexion con la base de datos: " + ex.ToString());
                return;
            }
            //crear comando a base de datos local
            try {
                command = new SqlCommand("select 1", connection);
            } catch (Exception ex) {
                this.log.write("[" + this.index + "/" + _total + "] Error al generar comando de conexion: " + ex.ToString());
                return;
            }

            //descargar valores a sincronizar
            try {
                result = api.GetdailyHistory(this.imei, this.year, this.month, this.day, this.timeZone, this.culture);
            } catch (Exception ex) {
                issues.Add("Obtener datos de la web: " + ex.ToString());
            }

            //si  no hay valores, sincronizar ultima fecha de descarga y finalizar
            if (result == null || result.Count <= 0) {
                this.log.write("[" + this.index + "/" + _total + "] No hay registros para sincronizar del " + this.date.ToShortDateString() + " para el IMEI " + this.imei);
                try {
                    command.CommandText = String.Format("delete from DevicesSyncLog where imei = '{0}'", this.imei);
                    command.ExecuteNonQuery();
                    command.CommandText = String.Format("insert into DevicesSyncLog(imei, lastSyncDate) values('{0}', '{1}')", this.imei, this.date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    this.log.write("[" + this.index + "/" + _total + "] Error al actualizar registro de sincronizacion del: " + this.date.ToShortDateString() + " para el IMEI " + this.imei + " :" + ex.ToString());
                } finally {
                    if (connection != null && connection.State == ConnectionState.Closed) {
                        connection.Close();
                    }
                }
                return;
            }             
            

            foreach (var __item in result) {
                __item.imei = this.imei;

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
                try {
                    __tmpSql += String.Format("('{0}', '{1}', '{2}', '{3}', {4}, {5}, '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, '{15}', '{16}', '{17}')",
                                        __item.imei,
                                        __item.date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        __item.time.TimeOfDay.ToString(),
                                        __item.uniqueId.Replace("'", "''"),
                                        (__item.latitude == null) ? 0d.ToString(CultureInfo.InvariantCulture) : __item.latitude.ToString(CultureInfo.InvariantCulture),
                                        (__item.longitude == null) ? 0d.ToString(CultureInfo.InvariantCulture) : __item.longitude.ToString(CultureInfo.InvariantCulture),
                                        (__item.address == null) ? "" : __item.address.Replace("'", "''"),
                                        (__item.speed == null) ? 0d.ToString(CultureInfo.InvariantCulture) : __item.speed.ToString(CultureInfo.InvariantCulture),
                                        (__item.mileage == null) ? 0d.ToString(CultureInfo.InvariantCulture) : __item.mileage.ToString(CultureInfo.InvariantCulture),
                                        __item.temperature_1.HasValue ? __item.temperature_1.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                        __item.temperature_2.HasValue ? __item.temperature_2.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                        __item.temperature_3.HasValue ? __item.temperature_3.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                        __item.temperature_4.HasValue ? __item.temperature_4.Value.ToString(CultureInfo.InvariantCulture) : 0d.ToString(CultureInfo.InvariantCulture),
                                        (__item.internal_battery_level == null) ? 0 : __item.internal_battery_level,
                                        (__item.is_exception == null) ? "" : (__item.is_exception ? "1" : "0"),
                                        (__item.alert_message == null) ? "" : __item.alert_message.Replace("'", "''"),
                                        (__item.zone == null) ? "" : __item.zone.Replace("'", "''"),
                                        this.myTransactionId
                                    );

                    __currentCount++;
                } catch (Exception ex) {
                    this.log.write("[" + this.index + "/" + _total + "] Error al generar insercion SQL : " + ex.ToString());
                }
            }
            if (__tmpSql.Length > 0) {
                __sql.Add(__tmpSql);
            }

            
            
            foreach (String s in __sql) {
                try {
                    command.CommandText = __headSql + s;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    issues.Add("Ingreso individual: " + ex.ToString());
                }
            }


            try {
                command.CommandText = String.Format("insert into dailyHistory (imei, date, time, uniqueId, latitude, longitude, address, speed, mileage, temperature_1, temperature_2, temperature_3, temperature_4, internal_battery_level, is_exception, alert_message, zone) select t1.imei, t1.date, t1.time, t1.uniqueId, t1.latitude, t1.longitude, t1.address, t1.speed, t1.mileage, t1.temperature_1, t1.temperature_2, t1.temperature_3, t1.temperature_4, t1.internal_battery_level, t1.is_exception, t1.alert_message, t1.zone from (select *, ROW_NUMBER() OVER(PARTITION BY uniqueId ORDER BY dailyHistoryId DESC) row_count from tmpdailyHistory where transactionID = '{0}') t1 where NOT EXISTS(SELECT 1 FROM dailyHistory t2 WHERE t2.uniqueId = t1.uniqueId) and row_count = 1", this.myTransactionId);
                command.ExecuteNonQuery();
                command.CommandText = String.Format("delete from tmpDailyHistory where transactionID = '{0}'", this.myTransactionId);
                command.ExecuteNonQuery();
                command.CommandText = String.Format("delete from DevicesSyncLog where imei = '{0}'", this.imei);
                command.ExecuteNonQuery();
                command.CommandText = String.Format("insert into DevicesSyncLog(imei, lastSyncDate) values('{0}', '{1}')", this.imei, this.date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                issues.Add("Normalizacion, limpieza y actualizacion: " + ex.ToString());
            } finally {
                if (connection != null && connection.State == ConnectionState.Closed) {
                    connection.Close();
                }
                String iss = (issues.Count > 0) ? "Se presentaron algunos errores: \n" + String.Join(", \n", issues.ToArray()) : "";
                this.log.write("[" + this.index + "/" + _total + "] Sincronizacion del IMEI " + this.imei + " para el " + this.date.ToShortDateString() + " completada, " + result.Count + " registros procesados. " + iss);
            }


        }


    }
}
