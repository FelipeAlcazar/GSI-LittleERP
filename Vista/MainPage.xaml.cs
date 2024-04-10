﻿using MySql.Data.MySqlClient;
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

namespace LittleERP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MySqlConnection connection;
        private ObservableCollection<Gasto> expenses;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeDatabase();
        }


        private void InitializeDatabase()
        {
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Iniciar HomePage
            this.Frame.Navigate(typeof(HomePage));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}