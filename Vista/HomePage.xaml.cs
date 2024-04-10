using LittleERP.Persistencia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LittleERP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private GestorBaseDatos gestorBaseDatos;
        public HomePage()
        {
            this.InitializeComponent();
            gestorBaseDatos = new GestorBaseDatos();

            // Insert a sample Gasto
            double monto = 100.00; // Example amount
            string descripcion = "Ejemplo de gasto"; // Example description
            DateTime fecha = DateTime.Now; // Current date and time
            // Create a Gasto object
            Gasto gasto = new Gasto
            {
                cantidad = monto,
                descripcion = descripcion,
                fecha = fecha
            };

            // Add the Gasto to the database
            gestorBaseDatos.AgregarGasto(gasto);
            LoadGastos();
        }

        private void LoadGastos()
        {
            ObservableCollection<Gasto> gastos = gestorBaseDatos.ObtenerGastos();
            gvGastos.ItemsSource = gastos;
        }

        private void GenerarInformePDF_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
