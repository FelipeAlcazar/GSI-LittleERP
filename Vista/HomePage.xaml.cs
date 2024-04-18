using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Popups;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Windows.Storage;
using Syncfusion.Drawing;
using Syncfusion.Licensing;
using Syncfusion.Pdf.Grid;
using System.Threading.Tasks;
using LittleERP.Vista;
using LittleERP.Dominio;


// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LittleERP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private Usuario user;

        public HomePage()
        {
            this.InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCe0x3Qnxbf1x0ZFNMYltbQX9PMyBoS35RckVnWHhednZdRmBYVEJy");
            CarruselPestañas.SelectionChanged += CarruselPestañas_SelectionChanged;
        }

        private void CarruselPestañas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateUserInfo();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && e.Parameter is Usuario)
            {
                user = (Usuario)e.Parameter;
                Debug.WriteLine($"User ID: {user.id}");
                LoadGastos();
                LoadIngresos();
                PopulateUserInfo();
            }
            else
            {
                // Handle invalid parameter
            }
        }

        private void PopulateUserInfo()
        {
            if (user != null)
            {
                txtNombre.Text = user.Nombre;
                txtApellido.Text = user.Apellido;
                txtCorreoElectronico.Text = user.CorreoElectronico;
            }
        }

        private void LoadGastos()
        {
            Gasto gastoAux = new Gasto();
            ObservableCollection<Gasto> gastos = gastoAux.GetGastosFromDatabase(user.id);
            gvGastos.ItemsSource = gastos;
        }

        private async void LogoutButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var confirmDialog = new MessageDialog("¿Estás seguro de que quieres cerrar sesión?", "Cerrar sesión");

            confirmDialog.Commands.Add(new UICommand("Sí", (command) =>
            {
                Frame.Navigate(typeof(MainPage));
            }));

            confirmDialog.Commands.Add(new UICommand("No", (command) =>
            {
                // No hacer nada si el usuario elige "No"
            }));

            await confirmDialog.ShowAsync();
        }

        private async void bntAddGasto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarTransaccionDialog();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Aquí puedes manejar la lógica para guardar el ingreso con la cantidad y descripción proporcionadas
                string cantidad = dialog.Cantidad;
                string descripcion = dialog.Descripcion;
                //Printea la cantidad y la descripción
                Debug.WriteLine($"Cantidad: {cantidad}, Descripción: {descripcion}");

                // Verificar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(cantidad) || string.IsNullOrWhiteSpace(descripcion))
                {
                    // Mostrar un mensaje indicando que los campos no pueden estar vacíos
                    MessageDialog emptyFieldsDialog = new MessageDialog("⚠️ Por favor, asegúrate de completar todos los campos.", "Campos Vacíos");
                    await emptyFieldsDialog.ShowAsync();
                    return;
                }

                // Verificar si la cantidad es un número
                if (!double.TryParse(cantidad, out _))
                {
                    // Mostrar un mensaje indicando que la cantidad debe ser un número
                    MessageDialog invalidQuantityDialog = new MessageDialog("⚠️ La cantidad debe ser un número.", "Cantidad Inválida");
                    await invalidQuantityDialog.ShowAsync();
                    return;
                }

                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que quieres agregar este gasto?", "Confirmar Agregar Ingreso");

                // Agregar botones
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    // Crear un nuevo objeto Ingreso
                    Gasto nuevoGasto = new Gasto
                    {
                        cantidad = Double.Parse(cantidad),
                        descripcion = descripcion,
                        fecha = DateTime.Now,
                        idUser = user.id
                    };

                    // Agregar el nuevo ingreso a la base de datos
                    nuevoGasto.AgregarGasto();

                    // Actualizar los datos de los ingresos
                    LoadGastos();
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Mostrar el cuadro de diálogo de confirmación
                await confirmDialog.ShowAsync();


            }
        }

        private async void btnRemoveGasto_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el gasto seleccionado
            Gasto selectedGasto = gvGastos.SelectedItem as Gasto;

            if (selectedGasto != null)
            {
                // Pedir confirmación
                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que quieres eliminar este gasto?", "Confirmar Eliminación");

                // Agregar botones
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    // Eliminar el gasto seleccionado de la base de datos
                    Gasto gastoAux = new Gasto();
                    gastoAux.BorrarGasto(selectedGasto.id);

                    // Actualizar los datos de los gastos
                    LoadGastos();
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Mostrar el cuadro de diálogo de confirmación
                await confirmDialog.ShowAsync();
            }
            else
            {
                // Mostrar mensaje indicando que no se ha seleccionado ningún gasto
                MessageDialog noSelectionDialog = new MessageDialog("⚠️ Por favor, selecciona un gasto para eliminar.", "Ningún Gasto Seleccionado");
                await noSelectionDialog.ShowAsync();
            }
        }

        private async void GenerarInformePDFGasto_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user if they are sure about generating the PDF
            var confirmDialog = new MessageDialog("¿Está seguro de generar el informe en formato PDF?", "Confirmación");

            // Add response options
            confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();

                // Set document information
                document.DocumentInformation.Title = "Informe de Gastos";

                // Add a new page to the document
                PdfPage page = document.Pages.Add();

                // Create PDF graphics
                PdfGraphics graphics = page.Graphics;

                // Define font and text format for the title
                PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 24, PdfFontStyle.Bold);
                PdfBrush titleBrush = PdfBrushes.Black;

                // Draw the title centered
                SizeF titleSize = titleFont.MeasureString("Informe de Gastos");
                float titleX = (page.Size.Width - titleSize.Width) / 2;
                graphics.DrawString("Informe de Gastos", titleFont, titleBrush, new PointF(titleX, 50));

                // Get gastos data from gvGastos
                ObservableCollection<Gasto> gastos = gvGastos.ItemsSource as ObservableCollection<Gasto>;

                // Create a PDF table
                PdfGrid pdfGrid = new PdfGrid();
                pdfGrid.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                // Add columns to the table
                pdfGrid.Columns.Add(4);

                // Add header row to the table
                PdfGridRow headerRow = pdfGrid.Headers.Add(1)[0];
                headerRow.Cells[0].Value = "ID";
                headerRow.Cells[1].Value = "Descripción";
                headerRow.Cells[2].Value = "Cantidad";
                headerRow.Cells[3].Value = "Fecha";

                double total = 0; // Initialize total

                // Add data rows to the table
                foreach (var gasto in gastos)
                {
                    PdfGridRow row = pdfGrid.Rows.Add();
                    row.Cells[0].Value = gasto.id.ToString();
                    row.Cells[1].Value = gasto.descripcion;
                    row.Cells[2].Value = gasto.cantidad.ToString();
                    row.Cells[3].Value = gasto.fecha.ToString();

                    // Sum to total
                    total += gasto.cantidad;
                }

                // Add row to show the total
                PdfGridRow totalRow = pdfGrid.Rows.Add();
                totalRow.Cells[1].Value = "Total";
                totalRow.Cells[2].Value = total.ToString();
                totalRow.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);

                // Draw the table centered on the page
                PointF gridPosition = new PointF((page.Size.Width - pdfGrid.Columns.Count * 100) / 2, 150);
                pdfGrid.Draw(page, gridPosition);

                // Get the Assets folder in the app folder
                StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");

                // Get the "logo.png" file within the Assets folder
                StorageFile logoFile = await assetsFolder.GetFileAsync("logo.png");

                // Add the logo to the PDF
                using (Stream logoStream = await logoFile.OpenStreamForReadAsync())
                {
                    PdfImage logoImage = PdfImage.FromStream(logoStream);
                    // Adjust logo size
                    float logoWidth = 200; // Change logo width as per preference
                    float logoHeight = logoImage.Height * logoWidth / logoImage.Width;
                    // Center the logo
                    float logoX = (page.Size.Width - logoWidth) / 2;
                    float logoY = page.Size.Height - logoHeight - 50;
                    graphics.DrawImage(logoImage, new RectangleF(logoX, logoY, logoWidth, logoHeight));
                }

                // Generate a unique filename with the current date and time
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string pdfFileName = $"InformeGastos_{timestamp}.pdf";

                // Save the PDF document in the user's documents folder with the unique filename
                string pdfFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, pdfFileName);
                using (Stream stream = File.OpenWrite(pdfFilePath))
                {
                    document.Save(stream);
                }

                // Show success message
                var successDialog = new MessageDialog($"Informe PDF generado correctamente! ✔️\nEl archivo se encuentra en: {pdfFilePath}", "Éxito");

                // Add a button to open the folder
                successDialog.Commands.Add(new UICommand("Abrir carpeta", async (cmd) =>
                {
                    await Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
                }));

                await successDialog.ShowAsync();
            }));

            // Add option to cancel
            confirmDialog.Commands.Add(new UICommand("Cancelar"));

            // Show the confirmation dialog
            await confirmDialog.ShowAsync();
        }

        private void LoadIngresos()
        {
            Ingreso ingresoAux = new Ingreso();
            Collection<Ingreso> ingresos = ingresoAux.GetIngresosFromDatabase(user.id);
            gvIngresos.ItemsSource = ingresos;
        }

        private async void bntAddIngreso_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarTransaccionDialog();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Aquí puedes manejar la lógica para guardar el ingreso con la cantidad y descripción proporcionadas
                string cantidad = dialog.Cantidad;
                string descripcion = dialog.Descripcion;
                //Printea la cantidad y la descripción
                Debug.WriteLine($"Cantidad: {cantidad}, Descripción: {descripcion}");

                // Verificar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(cantidad) || string.IsNullOrWhiteSpace(descripcion))
                {
                    // Mostrar un mensaje indicando que los campos no pueden estar vacíos
                    MessageDialog emptyFieldsDialog = new MessageDialog("⚠️ Por favor, asegúrate de completar todos los campos.", "Campos Vacíos");
                    await emptyFieldsDialog.ShowAsync();
                    return;
                }

                // Verificar si la cantidad es un número
                if (!double.TryParse(cantidad, out _))
                {
                    // Mostrar un mensaje indicando que la cantidad debe ser un número
                    MessageDialog invalidQuantityDialog = new MessageDialog("⚠️ La cantidad debe ser un número.", "Cantidad Inválida");
                    await invalidQuantityDialog.ShowAsync();
                    return;
                }

                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que quieres agregar este ingreso?", "Confirmar Agregar Ingreso");

                // Agregar botones
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    // Crear un nuevo objeto Ingreso
                    Ingreso nuevoIngreso = new Ingreso
                    {
                        cantidad = Double.Parse(cantidad),
                        descripcion = descripcion,
                        fecha = DateTime.Now,
                        idUser = user.id
                    };

                    // Agregar el nuevo ingreso a la base de datos
                    nuevoIngreso.AgregarIngreso();

                    // Actualizar los datos de los ingresos
                    LoadIngresos();
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Mostrar el cuadro de diálogo de confirmación
                await confirmDialog.ShowAsync();


            }
        }

        private async void btnRemoveIngreso_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el ingreso seleccionado
            Ingreso selectedIngreso = gvIngresos.SelectedItem as Ingreso;

            if (selectedIngreso != null)
            {
                // Pedir confirmación
                MessageDialog confirmDialog = new MessageDialog("¿Estás seguro de que quieres eliminar este ingreso?", "Confirmar Eliminación");

                // Agregar botones
                confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
                {
                    // Eliminar el ingreso seleccionado de la base de datos
                    Ingreso ingresoAux = new Ingreso();
                    ingresoAux.BorrarIngreso(selectedIngreso.id);

                    // Actualizar los datos de los ingresos
                    LoadIngresos();
                }));
                confirmDialog.Commands.Add(new UICommand("No"));

                // Mostrar el cuadro de diálogo de confirmación
                await confirmDialog.ShowAsync();
            }
            else
            {
                // Mostrar mensaje indicando que no se ha seleccionado ningún ingreso
                MessageDialog noSelectionDialog = new MessageDialog("⚠️ Por favor, selecciona un ingreso para eliminar.", "Ningún Ingreso Seleccionado");
                await noSelectionDialog.ShowAsync();
            }
        }
        private async void GenerarInformePDFIngreso_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user if they are sure about generating the PDF
            var confirmDialog = new MessageDialog("¿Está seguro de generar el informe en formato PDF?", "Confirmación");

            // Add response options
            confirmDialog.Commands.Add(new UICommand("Sí", async (command) =>
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();

                // Set document information
                document.DocumentInformation.Title = "Informe de Ingresos";

                // Add a new page to the document
                PdfPage page = document.Pages.Add();

                // Create PDF graphics
                PdfGraphics graphics = page.Graphics;

                // Define font and text format for the title
                PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 24, PdfFontStyle.Bold);
                PdfBrush titleBrush = PdfBrushes.Black;

                // Draw the title centered
                SizeF titleSize = titleFont.MeasureString("Informe de Ingresos");
                float titleX = (page.Size.Width - titleSize.Width) / 2;
                graphics.DrawString("Informe de Ingresos", titleFont, titleBrush, new PointF(titleX, 50));

                // Get ingresos data from gvIngresos
                ObservableCollection<Ingreso> ingresos = gvIngresos.ItemsSource as ObservableCollection<Ingreso>;

                // Create a PDF table
                PdfGrid pdfGrid = new PdfGrid();
                pdfGrid.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                // Add columns to the table
                pdfGrid.Columns.Add(4);

                // Add header row to the table
                PdfGridRow headerRow = pdfGrid.Headers.Add(1)[0];
                headerRow.Cells[0].Value = "ID";
                headerRow.Cells[1].Value = "Descripción";
                headerRow.Cells[2].Value = "Cantidad";
                headerRow.Cells[3].Value = "Fecha";

                double total = 0; // Initialize total

                // Add data rows to the table
                foreach (var ingreso in ingresos)
                {
                    PdfGridRow row = pdfGrid.Rows.Add();
                    row.Cells[0].Value = ingreso.id.ToString();
                    row.Cells[1].Value = ingreso.descripcion;
                    row.Cells[2].Value = ingreso.cantidad.ToString();
                    row.Cells[3].Value = ingreso.fecha.ToString();

                    // Sum to total
                    total += ingreso.cantidad;
                }

                // Add row to show the total
                PdfGridRow totalRow = pdfGrid.Rows.Add();
                totalRow.Cells[1].Value = "Total";
                totalRow.Cells[2].Value = total.ToString();
                totalRow.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);

                // Draw the table centered on the page
                PointF gridPosition = new PointF((page.Size.Width - pdfGrid.Columns.Count * 100) / 2, 150);
                pdfGrid.Draw(page, gridPosition);

                // Get the Assets folder in the app folder
                StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");

                // Get the "logo.png" file within the Assets folder
                StorageFile logoFile = await assetsFolder.GetFileAsync("logo.png");

                // Add the logo to the PDF
                using (Stream logoStream = await logoFile.OpenStreamForReadAsync())
                {
                    PdfImage logoImage = PdfImage.FromStream(logoStream);
                    // Adjust logo size
                    float logoWidth = 200; // Change logo width as per preference
                    float logoHeight = logoImage.Height * logoWidth / logoImage.Width;
                    // Center the logo
                    float logoX = (page.Size.Width - logoWidth) / 2;
                    float logoY = page.Size.Height - logoHeight - 50;
                    graphics.DrawImage(logoImage, new RectangleF(logoX, logoY, logoWidth, logoHeight));
                }

                // Generate a unique filename with the current date and time
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string pdfFileName = $"InformeIngresos_{timestamp}.pdf";

                // Save the PDF document in the user's documents folder with the unique filename
                string pdfFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, pdfFileName);
                using (Stream stream = File.OpenWrite(pdfFilePath))
                {
                    document.Save(stream);
                }

                // Show success message
                var successDialog = new MessageDialog($"Informe PDF generado correctamente! ✔️\nEl archivo se encuentra en: {pdfFilePath}", "Éxito");

                // Add a button to open the folder
                successDialog.Commands.Add(new UICommand("Abrir carpeta", async (cmd) =>
                {
                    await Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
                }));

                await successDialog.ShowAsync();
            }));

            // Add option to cancel
            confirmDialog.Commands.Add(new UICommand("Cancelar"));

            // Show the confirmation dialog
            await confirmDialog.ShowAsync();
        }

        private async void ActualizarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Check if any changes are made
            if (user != null && (user.Nombre != txtNombre.Text || user.Apellido != txtApellido.Text || user.CorreoElectronico != txtCorreoElectronico.Text || txtContraseña.Password != "" || txtConfirmarContraseña.Password != ""))
            {
                // Check if passwords match
                if (txtContraseña.Password != txtConfirmarContraseña.Password)
                {
                    // Show error message if passwords don't match
                    MessageDialog errorDialog = new MessageDialog("⚠️ Las contraseñas no coinciden.", "Error");
                    await errorDialog.ShowAsync();
                    return;
                }

                // Check if nombre, apellido, and correo electronico are empty
                if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) || string.IsNullOrEmpty(txtCorreoElectronico.Text))
                {
                    // Show error message if any of these fields are empty
                    MessageDialog errorDialog = new MessageDialog("⚠️ El nombre, apellido y correo electrónico son campos obligatorios.", "Error");
                    await errorDialog.ShowAsync();
                    return;
                }

                // Confirmation dialog
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirmar cambios",
                    Content = "¿Estás seguro de que deseas actualizar la información del usuario? La sesión se cerrará",
                    PrimaryButtonText = "Sí",
                    CloseButtonText = "No"
                };

                ContentDialogResult result = await confirmDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Update user information
                    user.Nombre = txtNombre.Text;
                    user.Apellido = txtApellido.Text;
                    user.CorreoElectronico = txtCorreoElectronico.Text;

                    // Update password if provided
                    if (!string.IsNullOrEmpty(txtContraseña.Password))
                    {
                        user.Contraseña = txtContraseña.Password;
                    }

                    // Update user information in the database
                    try
                    {
                        user.ActualizarUsuario();
                        // Show success message
                        MessageDialog successDialog = new MessageDialog("La información del usuario ha sido actualizada correctamente! ✔️", "Actualización Exitosa");
                        await successDialog.ShowAsync();
                        // Navigate to main page (logout)
                        Frame.Navigate(typeof(MainPage));
                    }
                    catch (Exception ex)
                    {
                        // Show error message if updating user information fails
                        MessageDialog errorDialog = new MessageDialog($"⚠️ Error al actualizar la información del usuario: {ex.Message}", "Error");
                        await errorDialog.ShowAsync();
                    }
                }
            }
            else
            {
                // Show message if no changes are made
                MessageDialog noChangesDialog = new MessageDialog("⚠️ No se han realizado cambios en la información del usuario.", "Sin cambios");
                await noChangesDialog.ShowAsync();
            }
        }

    }
}
