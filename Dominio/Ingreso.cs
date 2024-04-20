using LittleERP.Persistencia;
using System;
using System.Collections.ObjectModel;

namespace LittleERP
{
    public class Ingreso
    {
        private static GestorBaseDatos gestorIngresos = new GestorBaseDatos();

        public int id { get; set; }
        public double cantidad { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int idUser { get; set; }

        public Ingreso()
        {
            // Initialize the gestorIngresos instance
            gestorIngresos = new GestorBaseDatos();
        }

        public ObservableCollection<Ingreso> GetIngresosFromDatabase(int userId)
        {
            return gestorIngresos.GetIngresosFromDatabase(userId);
        }

        public void AgregarIngreso()
        {
            gestorIngresos.AgregarIngreso(this);
        }

        public void BorrarIngreso(int ingresoId)
        {
            gestorIngresos.BorrarIngreso(ingresoId);
        }
    }
}
