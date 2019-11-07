﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SeleniumDriver.DriverHelpers;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace XPathCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DriverHelper driver;
        public TextBox TbXPath => tbXPath;
        public TextBox TbXPathResponse => tbXPathResponse;
        public ListView LvFoundElements => lvFoundElements;
        private Highlight highlight;
        private List<IWebElement> foundElementsList;
        public TextBox TbAppPath => tbAppPath;
        public TextBox TbAppName => tbAppName;
        public CheckBox CbStartApp => cbStartApp;
        public CheckBox CbOverlay => cbOverlay;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            tbXPath.Text = "//MenuItem[@Name=\"File\"]";
        }

        private void btnFindElementsButton_Click(object sender, RoutedEventArgs e)
        {
            var userXPath = Regex.Unescape(tbXPath.Text);
            TbXPathResponse.Text = "";
            var appWindow = driver.GetAppWindow();
            highlight.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);

            try
            {
                lvFoundElements.Items.Clear();
                var foundElements = driver.FindElements(By.XPath(userXPath), 5);
                foundElementsList = foundElements;
                tbXPathResponse.Text = "";

                foreach (var element in foundElements)
                {
                    lvFoundElements.Items.Add("Text: " + element.Text + " \t " + "TagName: " + element.TagName);

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                tbXPathResponse.Text = "";
                tbXPathResponse.Text = exception.Message;
            }
        }

        private void listView_Click(object sender, MouseButtonEventArgs e)
        {
            var appWindow = driver.GetAppWindow();
            highlight.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
            var index = lvFoundElements.SelectedIndex;
            highlight.DrawRect(foundElementsList[index].Location.X, foundElementsList[index].Location.Y, foundElementsList[index].Size.Width, foundElementsList[index].Size.Height);
        }

        private void btnFindApp_Click(object sender, RoutedEventArgs e)
        {
            tbXPathResponse.Text = "";
            btnFindApp.Content = "Finding App...";
            try
            {
                btnFindApp.Background = Brushes.Red;
                highlight?.Close();
                driver?.Dispose();
                driver = new DriverHelper(tbAppName.Text, tbAppPath.Text, cbStartApp.IsChecked.Value);
                var appWindow = driver.GetAppWindow();
                highlight = new Highlight(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
                btnFindApp.Background = Brushes.Green;
                btnFindApp.Content = "App Found!";
                cbOverlay.IsChecked = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                tbXPathResponse.Text = "";
                tbXPathResponse.Text = exception.Message;
                btnFindApp.Content = "App Not Found!";
                btnFindApp.Background = Brushes.Red;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            highlight?.Close();
            driver?.Dispose();
        }

        private void cbOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (cbOverlay.IsChecked.GetValueOrDefault())
            {
                var appWindow = driver.GetAppWindow();
                highlight?.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
                highlight?.Show();
            }
            else
            {
                highlight?.Hide();
            }
        }
    }
}







