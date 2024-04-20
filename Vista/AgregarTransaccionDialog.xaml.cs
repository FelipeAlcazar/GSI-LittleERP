using System.Diagnostics;
using Windows.UI.Xaml.Controls;

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
