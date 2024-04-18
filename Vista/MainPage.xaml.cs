using LittleERP.Persistencia;
using LittleERP.Dominio;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace LittleERP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Debug.WriteLine(DateTime.Now);
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

            Usuario user = Usuario.VerifyLogin(email, password);

            if (user != null)
            {
                Frame.Navigate(typeof(HomePage), user);
            }
            else
            {
                // Show a message for invalid credentials
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
            // Check if any of the fields is empty
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtApellido.Text) ||
                string.IsNullOrEmpty(txtCorreo.Text) ||
                string.IsNullOrEmpty(txtContra.Password))
            {
                // Show a message indicating that one of the fields is empty
                MessageDialog emptyFieldDialog = new MessageDialog("⚠️ Por favor, completa todos los campos antes de registrarte.", "Campos Vacíos");
                await emptyFieldDialog.ShowAsync();
            }
            else
            {
                // Show a confirmation message
                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que deseas registrarte?", "Confirmar Registro");
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    string nombre = txtName.Text;
                    string apellido = txtApellido.Text;
                    string correo = txtCorreo.Text;
                    string contraseña = txtContra.Password;

                    Usuario.RegisterUsuario(nombre, apellido, correo, contraseña);

                    // Show a success message with a check mark
                    MessageDialog successDialog = new MessageDialog("¡Registro exitoso! ✔️", "Éxito");
                    await successDialog.ShowAsync();

                    // Go back to the previous page
                    GoBack_Click(sender, e);
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Show the confirmation message
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
