using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

namespace LittleERP.Persistencia
{
    public class GestorBaseDatos
    {
        private SQLiteConnection connection;


        public GestorBaseDatos()
        {

            // Get the local folder for the current app
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // Specify the path to the database file within the local folder
            string databaseFileName = "database.db";
            string databaseFilePath = Path.Combine(localFolder.Path, databaseFileName);

            // Construct the connection string
            string connectionString = $"Data Source={databaseFilePath};Version=3;";

            // Initialize the SQLite connection
            connection = new SQLiteConnection(connectionString);

            // Log the connection string for verification
            Debug.WriteLine($"Database connection string: {databaseFilePath}");

            // Check if the database file exists
            if (!File.Exists(databaseFilePath))
            {
                // Database file doesn't exist, create tables
                CreateTables();
            }
        }

        private void CreateTables()
        {
            try
            {
                connection.Open();

                // Create Factura table
                string createFacturaTable = @"
                    CREATE TABLE Factura (
                        Id INTEGER PRIMARY KEY,
                        UsuarioId INTEGER,
                        FechaEmision TEXT,
                        Total REAL,
                        FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
                    );";
                ExecuteNonQuery(createFacturaTable);

                // Create Gasto table
                string createGastoTable = @"
                    CREATE TABLE Gasto (
                        id INTEGER PRIMARY KEY,
                        cantidad REAL,
                        descripcion TEXT,
                        fecha TEXT
                    );";
                ExecuteNonQuery(createGastoTable);

                // Create Ingreso table
                string createIngresoTable = @"
                    CREATE TABLE Ingreso (
                        id INTEGER PRIMARY KEY,
                        cantidad REAL,
                        descripcion TEXT,
                        fecha TEXT
                    );";
                ExecuteNonQuery(createIngresoTable);

                // Create Usuario table
                string createUsuarioTable = @"
                    CREATE TABLE Usuario (
                        Id INTEGER PRIMARY KEY,
                        Nombre TEXT,
                        Apellido TEXT,
                        CorreoElectronico TEXT,
                        Contraseña TEXT,
                        EsAdmin INTEGER
                    );";
                ExecuteNonQuery(createUsuarioTable);

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating tables: {ex.Message}");
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Gasto> GetGastosFromDatabase()
        {
            ObservableCollection<Gasto> gastos = new ObservableCollection<Gasto>();

            try
            {
                Debug.WriteLine("Opening connection...");
                connection.Open();

                string consulta = "SELECT * FROM Gasto";
                Debug.WriteLine($"Executing query: {consulta}");
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            gastos.Add(new Gasto
                            {
                                id = Convert.ToInt32(lector["id"]),
                                cantidad = Convert.ToDouble(lector["cantidad"]),
                                descripcion = Convert.ToString(lector["descripcion"]),
                                fecha = DateTime.Parse(lector["fecha"].ToString())
                            });
                        }
                    }
                }

                Debug.WriteLine("Closing connection...");
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
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
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
