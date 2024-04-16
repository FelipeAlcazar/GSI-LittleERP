using LittleERP.Persistencia;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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


        private void Login_Click(object sender, RoutedEventArgs e)
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
                // Show an error message indicating invalid credentials
                // For example:
                // MessageBox.Show("Invalid email or password. Please try again.");
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Check if any of the fields are empty
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtApellido.Text) ||
                string.IsNullOrEmpty(txtCorreo.Text) ||
                string.IsNullOrEmpty(txtContra.Password))
            {
                Debug.WriteLine($"Error registering user since one of the fields is empty");
            }
            else
            {
                string nombre = txtName.Text;
                string apellido = txtApellido.Text;
                string correo = txtCorreo.Text;
                string contraseña = txtContra.Password;

                gestorBaseDatos.RegisterUser(nombre, apellido, correo, contraseña);
                GoBack_Click(sender, e);
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
