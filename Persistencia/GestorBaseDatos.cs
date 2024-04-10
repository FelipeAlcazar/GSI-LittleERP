using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SQLitePCL;

namespace LittleERP.Persistencia
{
    public class GestorBaseDatos
    {
        private SqliteConnection connection;
        string databaseFileName = "littleerp.db";

        public GestorBaseDatos()
        {
            // Initialize SQLite provider
            Batteries.Init();

            string connectionString = $"Data Source={databaseFileName}";
            connection = new SqliteConnection(connectionString);
        }

        public ObservableCollection<Gasto> ObtenerGastos()
        {
            ObservableCollection<Gasto> gastos = new ObservableCollection<Gasto>();

            try
            {
                connection.Open();

                string consulta = "SELECT * FROM Gasto";
                using (SqliteCommand comando = new SqliteCommand(consulta, connection))
                {
                    using (SqliteDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            gastos.Add(new Gasto
                            {
                                id = lector.GetInt32(0),
                                cantidad = lector.GetDouble(1),
                                descripcion = lector.GetString(2),
                                fecha = DateTime.Parse(lector.GetString(3))
                            });
                        }
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return gastos;
        }

        public void AgregarGasto(Gasto gasto)
        {
            try
            {
                connection.Open();

                string consulta = "INSERT INTO Gasto (cantidad, descripcion, fecha) VALUES (@cantidad, @descripcion, @fecha)";
                using (SqliteCommand comando = new SqliteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@cantidad", gasto.cantidad);
                    comando.Parameters.AddWithValue("@descripcion", gasto.descripcion);
                    comando.Parameters.AddWithValue("@fecha", gasto.fecha.ToString("dd-MM-yyyy"));

                    comando.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
