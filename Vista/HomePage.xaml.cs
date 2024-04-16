﻿using LittleERP.Persistencia;
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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LittleERP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private GestorBaseDatos gestorBaseDatos;
        private int userId;

        public HomePage()
        {
            this.InitializeComponent();
            gestorBaseDatos = new GestorBaseDatos();
            LoadGastos();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && e.Parameter is int)
            {
                userId = (int)e.Parameter;
                Debug.WriteLine($"User ID: {userId}");
                gestorBaseDatos = new GestorBaseDatos();
                LoadGastos();
            }
            else
            {
                // Handle invalid parameter
            }
        }

        private void LoadGastos()
        {
            ObservableCollection<Gasto> gastos = gestorBaseDatos.GetGastosFromDatabase(userId);
            gvGastos.ItemsSource = gastos;
        }

        private void GenerarInformePDF_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
