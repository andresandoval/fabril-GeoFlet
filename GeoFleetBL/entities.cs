using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFleetBL
{

  public class dailyHistory
  {
    //public int dailyHistoryId { get; set; }
    public string imei { get; set; }
    public DateTime date { get; set; }
    public DateTime time { get; set; }
    public string uniqueId
    {
      get
      {
        return imei + (new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second)).ToString("yyyyMMddHHmmss");
      }
    }
    public decimal latitude { get; set; }
    public decimal longitude { get; set; }
    public string address { get; set; }
    public decimal speed { get; set; }
    public decimal mileage { get; set; }
    public decimal? temperature_1 { get; set; }
    public decimal? temperature_2 { get; set; }
    public decimal? temperature_3 { get; set; }
    public decimal? temperature_4 { get; set; }
    public int internal_battery_level { get; set; }
    public bool is_exception { get; set; }
    public string alert_message { get; set; }
    public string zone { get; set; }

  }

  public class dailyHistoryJson
  {
    [JsonProperty(PropertyName = "0")]
    public List<dailyHistory> rows;
  }

  public class deviceInfo
  {
    public string imei { get; set; }
    public string alias { get; set; }
    public string placa { get; set; }
    public string ramv_cpn { get; set; }
    public string chasis { get; set; }
    public string motor { get; set; }
    public string Tipo_Combustible { get; set; }
    public int @Año { get; set; }
    public string marca { get; set; }
    public string modelo { get; set; }
    public string color1 { get; set; }
    public string color2 { get; set; }
    public string Tipo_Conductor { get; set; }
    public string Propietario { get; set; }
    public string Empresa { get; set; }
    public string Tipo_Cobertura { get; set; }
    public string Tipo_Carroceria_1 { get; set; }
    public string Tipo_Carroceria_2 { get; set; }
    public string Transporte { get; set; }
    public string Unidades { get; set; }
    public int Capacidad { get; set; }
    public decimal Cilindraje { get; set; }
    public string Unidad_consumo_combustible { get; set; }
    public DateTime Ultima_Actualizacion { get; set; }
  }

  public class deviceInfoJson
  {
    [JsonProperty(PropertyName = "0")]
    public deviceInfo[] rows;
  }

}
