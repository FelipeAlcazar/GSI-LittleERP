using System;
using LittleERP.Persistencia;

namespace LittleERP.Dominio
{
    public class Usuario
    {
        private static GestorBaseDatos gestorUsuario= new GestorBaseDatos();

        public int id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contraseña { get; set; }
        public bool EsAdmin { get; set; }

        public Usuario(int Id, string nombre, string apellido, string correoElectronico, string contraseña, bool esAdmin)
        {
            id = Id;
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correoElectronico;
            Contraseña = contraseña;
            EsAdmin = esAdmin;
        }
        // Method to authenticate a user
        public static Usuario VerifyLogin(string email, string password)
        {
            return gestorUsuario.VerifyLogin(email, password);
        }

        // Method to update user information
        public void ActualizarUsuario()
        {
            gestorUsuario.ActualizarUsuario(this);
        }

        // Method to register a new user
        public static void RegisterUsuario(string nombre, string apellido, string correo, string contraseña)
        {
            gestorUsuario.RegisterUser(nombre, apellido, correo, contraseña);
        }

        // Method to get user ID by email and password
        public static int GetUserId(string email, string password)
        {
            return gestorUsuario.GetUserId(email, password);
        }
    }
}
