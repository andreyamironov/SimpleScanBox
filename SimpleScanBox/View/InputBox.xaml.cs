using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SimpleScanBox.View
{
    /// <summary>
    /// Логика взаимодействия для InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        SimpleScan.Core.TypeScan TypeScan = SimpleScan.Core.TypeScan.None;
        public InputBox()
        {
            InitializeComponent();
        }
        public InputBox(SimpleScan.Core.TypeScan _typeScan) : this()
        {
            this.TypeScan = _typeScan;
            SetHeader(_typeScan);
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                this.Close();
            }
        }
        private void SetHeader(SimpleScan.Core.TypeScan _typeScan)
        {
            if (_typeScan == SimpleScan.Core.TypeScan.Pallet)
            {
                this.Title = "Паллет";
            }
            else if (_typeScan == SimpleScan.Core.TypeScan.Carton)
            {
                this.Title = "Короб";

            }
            this.Header.Text = this.Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, Input);
        }
    }
}
