using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LittleERP.Vista
{
    public sealed partial class AgregarTransaccionDialog : ContentDialog
    {
        public string Cantidad { get; set; }
        public string Descripcion { get; set; }

        public AgregarTransaccionDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Cantidad = txtCantidad.Text;
            Descripcion = txtDescripcion.Text;
            //Printea la cantidad y la descripción por consola
            Debug.WriteLine("Cantidad en dialog: " + Cantidad);
            Debug.WriteLine("Descripción en dialog: " + Descripcion);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // No hacer nada, simplemente cerrar el diálogo
        }
    }
}
