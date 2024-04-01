using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleERP.Persistencia
{
    public class GestorBaseDatos
    {
        private MySqlConnection connection;
        string databaseFilePath = @"C:\path\to\your\databasefile.mysql";
        string usuario = "root";
        string contraseña = "pass";

        public GestorBaseDatos(string servidor, string usuario, string contraseña, string baseDeDatos)
        {
            string cadenaConexion = $"server=localhost;user={usuario};password={contraseña};database={databaseFilePath}";
            connection = new MySqlConnection(cadenaConexion);
        }

        public ObservableCollection<Gasto> ObtenerGastos()
        {
            ObservableCollection<Gasto> gastos = new ObservableCollection<Gasto>();

            try
            {
                connection.Open();

                string consulta = "SELECT * FROM gastos";
                MySqlCommand comando = new MySqlCommand(consulta, connection);

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        gastos.Add(new Gasto
                        {
                            id = lector.GetInt32(0),
                            cantidad = lector.GetDouble(1),
                            descripcion = lector.GetString(2),
                            fecha = lector.GetDateTime(3)
                        });
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                // Manejar excepciones
            }

            return gastos;
        }

        public void AgregarGasto(double monto, string descripcion, DateTime fecha)
        {
            try
            {
                connection.Open();

                string consulta = "INSERT INTO gastos (monto, descripcion, fecha) VALUES (@monto, @descripcion, @fecha)";
                MySqlCommand comando = new MySqlCommand(consulta, connection);
                comando.Parameters.AddWithValue("@monto", monto);
                comando.Parameters.AddWithValue("@descripcion", descripcion);
                comando.Parameters.AddWithValue("@fecha", fecha);

                comando.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Manejar excepciones
            }
        }
    }
}
