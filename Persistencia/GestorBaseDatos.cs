using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
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
            Debug.WriteLine($"Local folder: {localFolder.Path}");

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

                // Create Usuario table
                string createUsuarioTable = @"
            CREATE TABLE IF NOT EXISTS Usuario (
                Id INTEGER PRIMARY KEY,
                Nombre TEXT,
                Apellido TEXT,
                CorreoElectronico TEXT,
                Contraseña TEXT,
                EsAdmin INTEGER
            );";
                ExecuteNonQuery(createUsuarioTable);

                // Create Factura table
                string createFacturaTable = @"
            CREATE TABLE IF NOT EXISTS Factura (
                Id INTEGER PRIMARY KEY,
                FechaEmision TEXT,
                Total REAL,
                UsuarioId INTEGER,
                FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
            );";
                ExecuteNonQuery(createFacturaTable);

                // Create Gasto table
                string createGastoTable = @"
            CREATE TABLE IF NOT EXISTS Gasto (
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
            CREATE TABLE IF NOT EXISTS Ingreso (
                id INTEGER PRIMARY KEY,
                cantidad REAL,
                descripcion TEXT,
                fecha TEXT,
                UsuarioId INTEGER,
                FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
            );";
                ExecuteNonQuery(createIngresoTable);

                // Admin User 1
                string insertAdminUser1 = @"
            INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, EsAdmin)
            VALUES ('Admin1', 'Lastname1', 'admin1@example.com', 'admin', 1);";
                ExecuteNonQuery(insertAdminUser1);

                // Admin User 2
                string insertAdminUser2 = @"
            INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, EsAdmin)
            VALUES ('Admin2', 'Lastname2', 'admin2@example.com', 'admin', 1);";
                ExecuteNonQuery(insertAdminUser2);

                // Non-admin User
                string insertNonAdminUser = @"
            INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, EsAdmin)
            VALUES ('User', 'Lastname', 'user@example.com', 'admin', 0);";
                ExecuteNonQuery(insertNonAdminUser);

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
                                fecha = DateTime.ParseExact(lector["fecha"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
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

                string consulta = "INSERT INTO Gasto (cantidad, descripcion, fecha, UsuarioId) VALUES (@cantidad, @descripcion, @fecha, @usuarioId)";
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@cantidad", gasto.cantidad);
                    comando.Parameters.AddWithValue("@descripcion", gasto.descripcion);
                    comando.Parameters.AddWithValue("@fecha", gasto.fecha.ToString("dd/MM/yyyy"));
                    comando.Parameters.AddWithValue("@usuarioId", gasto.idUser);

                    comando.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void BorrarGasto(int gastoId)
        {
            try
            {
                connection.Open();

                string consulta = "DELETE FROM Gasto WHERE id = @gastoId";
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@gastoId", gastoId);
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

        public Collection<Ingreso> GetIngresosFromDatabase(int userId)
        {
            Collection<Ingreso> ingresos = new ObservableCollection<Ingreso>();

            try
            {
                Debug.WriteLine("Opening connection...");
                connection.Open();

                string consulta = "SELECT * FROM Ingreso WHERE UsuarioId = @UserId";
                Debug.WriteLine($"Executing query: {consulta}");
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@UserId", userId);

                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Debug.WriteLine($"Leer ingreso: {lector["fecha"].ToString()}");
                            ingresos.Add(new Ingreso
                            {
                                id = Convert.ToInt32(lector["id"]),
                                cantidad = Convert.ToDouble(lector["cantidad"]),
                                descripcion = Convert.ToString(lector["descripcion"]),
                                fecha = DateTime.ParseExact(lector["fecha"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
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

            return ingresos;
        }

        public void AgregarIngreso(Ingreso ingreso)
        {
            try
            {
                connection.Open();

                string consulta = "INSERT INTO Ingreso (cantidad, descripcion, fecha, UsuarioId) VALUES (@cantidad, @descripcion, @fecha, @usuarioId)";
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@cantidad", ingreso.cantidad);
                    comando.Parameters.AddWithValue("@descripcion", ingreso.descripcion);
                    Debug.WriteLine($"Fecha: {ingreso.fecha.ToString("dd/MM/yyyy")}");
                    comando.Parameters.AddWithValue("@fecha", ingreso.fecha.ToString("dd/MM/yyyy"));
                    comando.Parameters.AddWithValue("@usuarioId", ingreso.idUser);

                    comando.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void BorrarIngreso(int ingresoId)
        {
            try
            {
                connection.Open();

                string consulta = "DELETE FROM Ingreso WHERE id = @ingresoId";
                using (SQLiteCommand comando = new SQLiteCommand(consulta, connection))
                {
                    comando.Parameters.AddWithValue("@ingresoId", ingresoId);
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
