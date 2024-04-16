using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleERP
{
    public class Ingreso
    {
        public int id { get; set; }
        public double cantidad { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int idUser { get; set; }

        public Ingreso()
        {
            fecha = DateTime.Today;
        }
    }
}
