using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace RestApiClient
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      string cs = System.Configuration.ConfigurationManager.ConnectionStrings["RestClient.Properties.Settings.geoFleetConnectionString"].ConnectionString;

      this.devicesTableAdapter.Connection.ConnectionString = cs;
      this.dailyHistoryTableAdapter.Connection.ConnectionString = cs;

      // TODO: esta línea de código carga datos en la tabla 'apiDataset.dailyHistory' Puede moverla o quitarla según sea necesario.
      this.dailyHistoryTableAdapter.Fill(this.apiDataset.dailyHistory);
      // TODO: esta línea de código carga datos en la tabla 'apiDataset.devices' Puede moverla o quitarla según sea necesario.
      this.devicesTableAdapter.Fill(this.apiDataset.devices);

    }

    private void callButton_Click(object sender, EventArgs e)
    {
      var api = new ApiHelper(tokenText.Text);

      Cursor.Current = Cursors.WaitCursor;

      try
      {
        if (tabControl1.SelectedTab.Equals(devicesTab))
        {
          var request = api.GetdeviceInfo();
          int numRecords = 0;

          if (request != null)
          {
            if (string.IsNullOrWhiteSpace(empresaText.Text))
            {
              devicesGrid.DataSource = request;
              numRecords = request.Length;
            }
            else
            {
              var req = request.Where(o => o.Empresa.Equals(empresaText.Text)).ToArray();
              devicesGrid.DataSource = req;
              numRecords = req.Length;
            }
          }

          records1Label.Text = string.Format("{0} registros encontrados", numRecords);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void import1Button_Click(object sender, EventArgs e)
    {
      int inserted = 0;

      Cursor.Current = Cursors.WaitCursor;

      try
      {
        for (int i = 0; i < devicesGrid.RowCount; i++)
        {
          DataGridViewRow rowInGrid = devicesGrid.Rows[i];
          deviceInfo vehiculo = (deviceInfo)rowInGrid.DataBoundItem;
          var dt = devicesTableAdapter.GetDeviceById(vehiculo.imei);

          // Verifica existencia del registro en la BD
          // Si no existe, lo inserta
          if (dt.Rows.Count == 0)
          {
            devicesTableAdapter.Insert(vehiculo.imei,
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

            inserted++;
          }
        }

        if (inserted > 0)
        {
          apiDataset.AcceptChanges();
          MessageBox.Show(string.Format("{0} filas insertadas en la BD", inserted));
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void import2Button_Click(object sender, EventArgs e)
    {
      int inserted = 0;

      Cursor.Current = Cursors.WaitCursor;

      try
      {
        for (int i = 0; i < historyGrid.RowCount; i++)
        {
          DataGridViewRow rowInGrid = historyGrid.Rows[i];
          dailyHistory history = (dailyHistory)rowInGrid.DataBoundItem;
          var dt = dailyHistoryTableAdapter.GetHistoryById(history.uniqueId);

          // Verifica existencia del registro en la BD
          // Si no existe, lo inserta
          if (dt.Rows.Count == 0)
          {
            dailyHistoryTableAdapter.Insert(
              history.imei,
              history.date,
              history.time.TimeOfDay,
              history.uniqueId,
              history.latitude,
              history.longitude,
              history.address,
              history.speed,
              history.mileage,
              history.temperature_1,
              history.temperature_2,
              history.temperature_3,
              history.temperature_4,
              history.internal_battery_level,
              history.is_exception,
              history.alert_message,
              history.zone);

            inserted++;
          }
        }

        if (inserted > 0)
        {
          apiDataset.AcceptChanges();
          MessageBox.Show(string.Format("{0} filas insertadas en la BD", inserted));
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void call2Button_Click(object sender, EventArgs e)
    {
      var api = new ApiHelper(tokenText.Text);

      Cursor.Current = Cursors.WaitCursor;

      try
      {
        DateTime fecha = desdeDate.Value;
        string imei = imeiText.Text;
        var request = api.GetdailyHistory(imei, fecha.Year, fecha.Month, fecha.Day, "-5", "es");
        int numRecords = 0;

        if (request != null)
        {
          foreach (var item in request)
          {
            item.imei = imei;
          }

          historyGrid.DataSource = request;
          numRecords = request.Count;
        }
        records2Label.Text = string.Format("{0} registros encontrados", numRecords);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void call3Button_Click(object sender, EventArgs e)
    {
      DateTime desde = desdeDate.Value;
      DateTime hasta = hastaDate.Value;
      List<DateTime> fechas = new List<DateTime>();
      DateTime fecha = desde.Date;

      // Crear lista de fechas a procesar
      do
      {
        fechas.Add(fecha);
        fecha = fecha.AddDays(1).Date;
      } while (fecha < hasta);

      var api = new ApiHelper(tokenText.Text);

      Cursor.Current = Cursors.WaitCursor;

      // Procesar
      int numRecords = 0;
      string imei = imeiText.Text;
      List<dailyHistory> history = new List<dailyHistory>();

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      try
      {
        foreach (var f in fechas)
        {
          var request = api.GetdailyHistory(imei, f.Year, f.Month, f.Day, "-5", "es");

          if (request != null && request.Count > 0)
          {
            foreach (var item in request)
            {
              item.imei = imei;

              var dt = dailyHistoryTableAdapter.GetHistoryById(item.uniqueId);

              if (dt.Rows.Count == 0)
              {
                dailyHistoryTableAdapter.Insert(
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
              }

              //history.AddRange(request);

              numRecords += request.Count;
            }
          }

          apiDataset.AcceptChanges();
        }

        stopwatch.Stop();
        //historyGrid.DataSource = history;

        records2Label.Text = string.Format("{0} registros encontrados. Tiempo utilizado: {1}", numRecords, stopwatch.Elapsed);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
        stopwatch.Stop();
      }
    }

    private void call4Button_Click(object sender, EventArgs e)
    {
      if (devicesGrid.RowCount > 0)
      {
        DateTime desde = desdeDate.Value;
        DateTime hasta = hastaDate.Value;
        List<DateTime> fechas = new List<DateTime>();
        DateTime fecha = desde.Date;

        // Crear lista de fechas a procesar
        do
        {
          fechas.Add(fecha);
          fecha = fecha.AddDays(1).Date;
        } while (fecha < hasta);

        // Crear lista de vehículos a procesar
        List<string> vehiculos = new List<string>();

        for (int i = 0; i < devicesGrid.RowCount; i++)
        {
          vehiculos.Add((string)devicesGrid.Rows[i].Cells[0].Value);
        }


        var api = new ApiHelper(tokenText.Text);

        Cursor.Current = Cursors.WaitCursor;

        // Procesar
        int numRecords = 0;
        List<dailyHistory> history = new List<dailyHistory>();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
          int countVehiculos = 0;
          foreach (var imei in vehiculos)
          {
            foreach (var f in fechas)
            {
              var request = api.GetdailyHistory(imei, f.Year, f.Month, f.Day, "-5", "es");

              if (request != null && request.Count > 0)
              {
                foreach (var item in request)
                {
                  item.imei = imei;
                }

                history.AddRange(request);

                numRecords += request.Count;
              }
            }

            countVehiculos++;

            //if (countVehiculos == 2)
            //{
            //  break;  
            //}
          }
          stopwatch.Stop();
          historyGrid.DataSource = history;

          records2Label.Text = string.Format("{0} registros encontrados. Tiempo utilizado: {1}", numRecords, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
          Cursor.Current = Cursors.Default;
          stopwatch.Stop();
        }

      }
    }





  }
}
