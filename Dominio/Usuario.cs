using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleERP.Dominio
{
    internal class Usuario
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contraseña { get; set; }
        public bool EsAdmin { get; set; }

        public Usuario(string nombre, string apellido, string correoElectronico, string contraseña, bool esAdmin)
        {
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correoElectronico;
            Contraseña = contraseña;
            EsAdmin = esAdmin;
        }
    }
}
