﻿// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;

namespace TestInjectorSample.WpfApplication45
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go_OnClick(object sender, RoutedEventArgs e)
        {
            this.HeadingLabel.Text = this.HeadingTextBox.Text;
        }
    }
}
