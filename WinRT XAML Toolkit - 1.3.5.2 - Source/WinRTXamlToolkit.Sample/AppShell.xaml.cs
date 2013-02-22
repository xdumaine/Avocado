﻿using WinRTXamlToolkit.Sample.Views;
using Windows.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Sample
{
    public sealed partial class AppShell : UserControl
    {
        public AppShell()
        {
            this.InitializeComponent();
            this.RootFrame.Navigate(typeof (MainPage));
        }
    }
}
