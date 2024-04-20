using LittleERP.Persistencia;
using System;
using System.Collections.ObjectModel;

namespace LittleERP
{
    public class Gasto
    {
        private static GestorBaseDatos gestorGastos = new GestorBaseDatos();

        public int id { get; set; }
        public double cantidad { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int idUser { get; set; }

        public Gasto()
        {
            // Initialize the gestorBaseDatos instance
            gestorGastos = new GestorBaseDatos();
        }

        public ObservableCollection<Gasto> GetGastosFromDatabase(int userId)
        {
            return gestorGastos.GetGastosFromDatabase(userId);
        }

        public void AgregarGasto()
        {
            gestorGastos.AgregarGasto(this);
        }

        public void BorrarGasto(int gastoId)
        {
            gestorGastos.BorrarGasto(gastoId);
        }
    }

}
