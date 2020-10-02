using SimpleScanBox.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace SimpleScanBox
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        ViewModel.MainModel model;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                model.Add(((TextBox)sender).Text);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             model = this.Resources["model"] as ViewModel.MainModel;
            InputLanguageManager.SetInputLanguage(this.txtInput, new CultureInfo("en-US"));
            FocusManager.SetFocusedElement(this, txtInput);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListView lbx = sender as System.Windows.Controls.ListView;
            lbx.ScrollIntoView(lbx.SelectedItem);
        }

        //private void RibbonButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SimpleScan.Dal.Storage storage = new SimpleScan.Dal.Storage();
        //    storage.FillDefault();

        //    //var t = storage.GetTasks();


        //    var model = ViewModel.Manager.GetTasks(this,null,SimpleScan.Core.CurrentAction.View);
        //    ;

            
        //}
        private void btnDownloadCurrentFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.model.FillData(this.model.CurrentFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            FocusManager.SetFocusedElement(this, txtInput);
        }

        private void btnScanInNewFile_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы жействительно хотите начать сканирование в новый файл ?", "Новое сканирование", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    this.model.NewSessionScan(this.model.CurrentFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            FocusManager.SetFocusedElement(this, txtInput);
        }

        private void btnOpenCurrentFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.model.ViewCopyCurrentFile(this.model.CurrentFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            FocusManager.SetFocusedElement(this, txtInput);
        }

        private void txtMinimumLenghtSsCc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int tmp = model.MinimumLenghtSsCc;
                int.TryParse(((RibbonTextBox)sender).Text,out tmp);
                model.MinimumLenghtSsCc = tmp;
            }
        }

        private void txtRegexFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
                model.RegexFindErrorChars = ((RibbonTextBox)sender).Text;
            }
        }

        private void btnConstraintsReset_Click(object sender, RoutedEventArgs e)
        {
            model.SetConstraintsByDefault();
        }

        private void btnCurrentFileDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.model.OpenCurrentDirectory();
        }
        private void btnArchiveFileDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.model.OpenArchiveDirectory();
        }
        private void btnTempFileDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.model.OpenTempDirectory();
        }

        private void btnTypeScanPallet_Click(object sender, RoutedEventArgs e)
        {
            View.InputBox inputBox = new View.InputBox(SimpleScan.Core.TypeScan.Pallet);
            var result = inputBox.ShowDialog();
            
            if(result == true)
            {
                this.model.Pallet = inputBox.Input.Text;
            }
        }

        private void btnTypeScanCarton_Click(object sender, RoutedEventArgs e)
        {
            View.InputBox inputBox = new View.InputBox(SimpleScan.Core.TypeScan.Carton);
            var result = inputBox.ShowDialog();

            if (result == true)
            {
                this.model.Carton = inputBox.Input.Text;
            }
        }

        private void btnDeleteSelectedRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Будет произведено удаление выделенной строки и перезапись текущего файла.\nВыполнить действие ?", "Удаление строки", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    this.model.RemoveSelectedRowAndRewriteFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            FocusManager.SetFocusedElement(this, txtInput);
        }

        private void btnOpenCurrentFileEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                model.OpenListSsCcToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {


            Manual manual = new Manual();
            manual.Show();
        }
    }
}
