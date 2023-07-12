using System;
using System.Collections.Generic;
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

using PSPDFKit.Document;
using PSPDFKit.Pdf;
using PSPDFKit.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RandomAccessStreamDataProviderExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            ViewModel.Initialize(PDFView);
            Unloaded += DataProvider_Unloaded;
        }

        private void DataProvider_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ViewUnloaded();
        }

        public DataProviderViewModel ViewModel { get; } = new DataProviderViewModel();
    }
}
