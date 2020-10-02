using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace SimpleScanBox.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для Manual.xaml
    /// </summary>
    public partial class Manual : Window
    {
        public Manual()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("pack://application:,,,/SimpleScan_Manual.xps");
            var stream = Application.GetResourceStream(uri).Stream;
            Package package = Package.Open(stream);
            PackageStore.AddPackage(uri, package);
            var xpsDoc = new XpsDocument(package, CompressionOption.Maximum, uri.AbsoluteUri);
            var fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();
            doc.Document = fixedDocumentSequence; 
            xpsDoc.Close();
        }
    }
}
