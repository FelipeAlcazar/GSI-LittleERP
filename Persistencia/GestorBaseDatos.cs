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
                FechaEmision TEXT,
                Total REAL,
                UsuarioId INTEGER,
                FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
            );";
                ExecuteNonQuery(createFacturaTable);

                // Create Gasto table
                string createGastoTable = @"
            CREATE TABLE Gasto (
                id INTEGER PRIMARY KEY,
                cantidad REAL,
                descripcion TEXT,
                fecha TEXT,
                UsuarioId INTEGER,
                FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
            );";
                ExecuteNonQuery(createGastoTable);

                // Create Ingreso table
                string createIngresoTable = @"
            CREATE TABLE Ingreso (
                id INTEGER PRIMARY KEY,
                cantidad REAL,
                descripcion TEXT,
                fecha TEXT,
                UsuarioId INTEGER,
                FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
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

        public ObservableCollection<Gasto> GetGastosFromDatabase(int userId)
        {
            ObservableCollection<Gasto> gastos = new ObservableCollection<Gasto>();

            try
            {
                Debug.WriteLine("Opening connection...");
                connection.Open();

                string consulta = "SELECT * FROM Gasto WHERE UsuarioId = @UserId";
                Debug.WriteLine($"Executing query: {consulta}");
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@UserId", userId);

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
        public bool VerifyLogin(string email, string password)
        {
            bool isAuthenticated = false;

            try
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Usuario WHERE CorreoElectronico = @Email AND Contraseña = @Password";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    isAuthenticated = (count > 0);
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verifying login: {ex.Message}");
            }

            return isAuthenticated;
        }

        public int GetUserId(string email, string password)
        {
            int userId = -1;

            try
            {
                connection.Open();

                string query = "SELECT Id FROM Usuario WHERE CorreoElectronico = @Email AND Contraseña = @Password";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user ID: {ex.Message}");
            }
            Debug.WriteLine($"Got this user: {userId}");
            return userId;
        }

        public void RegisterUser(string nombre, string apellido, string correo, string contraseña)
        {
            try
            {
                connection.Open();

                string consulta = "INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, EsAdmin) VALUES (@Nombre, @Apellido, @Correo, @Contraseña, 0)";
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    comando.Parameters.AddWithValue("@Apellido", apellido);
                    comando.Parameters.AddWithValue("@Correo", correo);
                    comando.Parameters.AddWithValue("@Contraseña", contraseña);

                    comando.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error registering user: {ex.Message}");
            }
        }
    }
}
