using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleERP.Dominio
{
    internal class Factura
    {
        public int id { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaEmision { get; set; }
        public double Total { get; set; }
        public int idUser { get; set; }
    }
}
