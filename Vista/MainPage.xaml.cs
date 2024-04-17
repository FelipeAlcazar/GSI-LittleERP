using LittleERP.Persistencia;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using System;

namespace LittleERP
{
    public sealed partial class MainPage : Page
    {
        GestorBaseDatos gestorBaseDatos;
        public MainPage()
        {
            InitializeComponent();
            gestorBaseDatos=new GestorBaseDatos();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            imgLogo.Visibility = Visibility.Collapsed;
            // Show the login fields
            LoginGrid.Visibility = Visibility.Visible;

            // Hide the original buttons
            OriginalButtonsPanel.Visibility = Visibility.Collapsed;
        }


        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            bool isAuthenticated = gestorBaseDatos.VerifyLogin(email, password);
            int userId = gestorBaseDatos.GetUserId(email, password);

            if (isAuthenticated)
            {
                Frame.Navigate(typeof(HomePage), userId);
            }
            else
            {
                // Mostrar un mensaje de credenciales inválidas
                MessageDialog invalidCredentialsDialog = new MessageDialog("⚠️ Las credenciales ingresadas son inválidas. Por favor, inténtalo de nuevo.", "Credenciales Inválidas");
                await invalidCredentialsDialog.ShowAsync();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            imgLogo.Visibility = Visibility.Collapsed;
            // Show the registration fields
            RegisterGrid.Visibility = Visibility.Visible;

            // Hide the original buttons
            OriginalButtonsPanel.Visibility = Visibility.Collapsed;
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si alguno de los campos está vacío
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtApellido.Text) ||
                string.IsNullOrEmpty(txtCorreo.Text) ||
                string.IsNullOrEmpty(txtContra.Password))
            {
                // Mostrar un mensaje indicando que uno de los campos está vacío
                MessageDialog emptyFieldDialog = new MessageDialog("⚠️ Por favor, completa todos los campos antes de registrarte.", "Campos Vacíos");
                await emptyFieldDialog.ShowAsync();
            }
            else
            {
                // Mostrar un mensaje de confirmación
                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que deseas registrarte?", "Confirmar Registro");
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    string nombre = txtName.Text;
                    string apellido = txtApellido.Text;
                    string correo = txtCorreo.Text;
                    string contraseña = txtContra.Password;

                    gestorBaseDatos.RegisterUser(nombre, apellido, correo, contraseña);

                    // Mostrar un mensaje de éxito con una marca de verificación
                    MessageDialog successDialog = new MessageDialog("¡Registro exitoso! ✔️", "Éxito");
                    await successDialog.ShowAsync();

                    // Regresar a la página anterior
                    GoBack_Click(sender, e);
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Mostrar el mensaje de confirmación
                await confirmDialog.ShowAsync();
            }
        }



        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegisterGrid.Visibility = Visibility.Collapsed;
            imgLogo.Visibility = Visibility.Visible;
            // Hide the registration fields
            RegisterGrid.Visibility = Visibility.Collapsed;

            // Show the original buttons
            OriginalButtonsPanel.Visibility = Visibility.Visible;
        }


    }
}
