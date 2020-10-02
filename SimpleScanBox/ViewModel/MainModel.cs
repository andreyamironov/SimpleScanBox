using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;

namespace SimpleScanBox.ViewModel
{
    public class MainModel : SimpleScan.Model.BasedNotifyModel
    {
        const string LocalDateTimeFormat = "dd.MM.yyyy HH:mm:ss:fff";
        const string Default_RegexFindErrorChars = @"[А-Яа-я]";
        const int Default_MinimumLenghtSsCc = 8;

        public readonly string DirCurrentFile;
        public readonly string DirArchiveFile;
        public readonly string DirTempFile;
        public readonly string CurrentFileName;

        int LastId { get; set; }
      
        private bool IsFillData { get; set; } 

        string regexFindErrorChars = Default_RegexFindErrorChars;
        public string RegexFindErrorChars
        {
            get
            {
                return regexFindErrorChars;
            }
            set
            {
                regexFindErrorChars = value;
                try
                {
                    Properties.Settings.Default.RegexFindErrorChars = regexFindErrorChars;
                    Properties.Settings.Default.Save();
                }
                catch{}
                OnPropertyChanged(nameof(RegexFindErrorChars));
            }
        }

        int minLenght = Default_MinimumLenghtSsCc;
        public int MinimumLenghtSsCc
        {
            get
            {
                return minLenght;
            }
            set
            {
                if (value < 1) minLenght = 1;
                else minLenght = value;

                try
                {
                    Properties.Settings.Default.MinimumLenghtSsCc = minLenght;
                    Properties.Settings.Default.Save();
                }
                catch { }

                OnPropertyChanged(nameof(MinimumLenghtSsCc));
            }
        }

        string pallet;
        public string Pallet
        {
            get
            {
                if (string.IsNullOrWhiteSpace(pallet)) return "---";
                return pallet;
            }
            set
            {
                pallet = value;
                Carton = null;
                OnPropertyChanged(nameof(Pallet));
            }
        }
        
        string box;
        public string Carton
        {
            get
            {
                if (string.IsNullOrWhiteSpace(box)) return "---";
                return box;
            }
            set
            {
                box = value;
                OnPropertyChanged(nameof(Carton));
            }
        }

        ObservableCollection<SimpleScan.Model.Row> listSsCc;
        SimpleScan.Model.Row selectedRow;

        string input;
        public string Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
            }
        }
        
        string lastInput;
        public string LastInput
        {
            get
            {
                return lastInput;
            }
            set
            {
                lastInput = value;
                OnPropertyChanged(nameof(LastInput));
            }
        }
        
        public ObservableCollection<SimpleScan.Model.Row> ListSsCc
        {
            get
            {
                return listSsCc;
            }
            set
            {
                listSsCc = value;
                OnPropertyChanged(nameof(ListSsCc));
            }
        }
        public SimpleScan.Model.Row SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }

        int statusBarCount;
        public int StatusBarCount
        {
            get
            {
                return statusBarCount;
            }
            set
            {
                statusBarCount = value;
                OnPropertyChanged(nameof(StatusBarCount));
            }
        }

        public MainModel()
        {
            DirCurrentFile      = Directory.GetCurrentDirectory() + @"\data";
            DirArchiveFile      = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SimpleScanDataArchive";
            DirTempFile         = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SimpleScanDataTemp";
            CurrentFileName     = System.IO.Path.Combine(DirCurrentFile, "scan.txt");

            IsFillData = false;

            try
            {
                RegexFindErrorChars = Default_RegexFindErrorChars;
                MinimumLenghtSsCc = Properties.Settings.Default.MinimumLenghtSsCc;
            }
            catch
            {
                RegexFindErrorChars = Default_RegexFindErrorChars;
                MinimumLenghtSsCc = Default_MinimumLenghtSsCc;
            }

            listSsCc = new ObservableCollection<SimpleScan.Model.Row>();
            listSsCc.CollectionChanged += ListSsCc_CollectionChanged;
        }

        private void ListSsCc_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var obj = sender as ObservableCollection<SimpleScan.Model.Row>;
            this.StatusBarCount = obj.Count();
        }
        public void Add(string input)
        {
            if (!Directory.Exists(DirCurrentFile))
            {
                Directory.CreateDirectory(DirCurrentFile);
            }
            if (!string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    if (input.Length < minLenght) throw new Exception($"Минимальная длинна шк: {minLenght}");

                    Regex regex = new Regex(regexFindErrorChars);
                    MatchCollection matches = regex.Matches(input);
                    if (matches.Count > 0)
                    {
                        throw new Exception($"Недопустимые символы: {RegexFindErrorChars}");
                    }

                    Validate_FirstScan(CurrentFileName);

                    if (this.listSsCc == null) this.ListSsCc = new ObservableCollection<SimpleScan.Model.Row>();

                    //---------------------------------------------------
                    SimpleScan.Model.Row addRow = new SimpleScan.Model.Row()
                    {
                        Id = LastId + 1,
                        Pallet = Pallet,
                        Carton = Carton,
                        SsCc = input,
                        Date = DateTime.Now
                    };
                    this.ListSsCc.Add(addRow);
                    this.SelectedRow = addRow;
                    this.LastInput = input;

                    string record = RowToString(addRow);
                    File.AppendAllLines(CurrentFileName, new List<string>() { record });
                    IsFillData = true;

                    this.Input = string.Empty;
                    LastId++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);                  
                }
                finally
                {
                    this.Input = string.Empty;
                }
            }
        }
        string RowToString(SimpleScan.Model.Row _row)
        {
            return $"{_row.Id}\t{_row.Pallet}\t{_row.Carton}\t{_row.SsCc}\t{_row.Date.ToString(LocalDateTimeFormat)}";
        }

        public void RemoveSelectedRowAndRewriteFile()
        {
            if(selectedRow != null)
            {
                this.ListSsCc.Remove(SelectedRow);
                StringBuilder sb = new StringBuilder();
                foreach(var r in this.listSsCc)
                {
                    sb.AppendLine(RowToString(r));
                }

                File.WriteAllText(CurrentFileName, sb.ToString());
                FillData(CurrentFileName);
            }
        }
        public void FillData(string _fileName)
        {
            if (File.Exists(_fileName))
            {
                Input = null;
                ListSsCc.Clear();
                IsFillData = false;

                var file = File.ReadAllLines(_fileName);
                if (this.listSsCc == null) this.ListSsCc = new ObservableCollection<SimpleScan.Model.Row>();

                SimpleScan.Model.Row addRow = null;
                foreach (var row in file)
                {
                    string[] r = row.Split('\t');

                    addRow = new SimpleScan.Model.Row()
                    {
                        Id = int.Parse(r[0]),
                        Pallet = r[1],
                        Carton = r[2],
                        SsCc = r[3],
                        Date = DateTime.ParseExact(r[4], LocalDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)
                       
                    };
                    this.ListSsCc.Add(addRow);

                }
                LastId = this.listSsCc.OrderBy(i => i.Id).Select(i => i.Id).Last();
                this.SelectedRow = addRow;
            }
            else
            {
                throw new Exception("Текущий файл отсутствует.\nНачинайте просто сканировать");
            }
        }
        private void Validate_FirstScan(string _fileName)
        {
            if (!IsFillData && File.Exists(CurrentFileName))
            {
                FillData(CurrentFileName);
                IsFillData = true;
            }
        }

        public void ViewCopyCurrentFile(string _fileName)
        {
            if (!Directory.Exists(DirTempFile)) Directory.CreateDirectory(DirTempFile);
            if(File.Exists(_fileName))
            {
                string copyPath = Path.Combine(DirTempFile, $"copy_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff")}.txt");
                File.Copy(_fileName, copyPath);
                System.Diagnostics.Process.Start(copyPath);
            }
        }
        public void NewSessionScan(string _fileName)
        {
            if (!Directory.Exists(DirArchiveFile)) Directory.CreateDirectory(DirArchiveFile);
            if (File.Exists(_fileName))
            {
                string copyPath = Path.Combine(DirArchiveFile, $"archive_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff")}.txt");
                File.Move(_fileName, copyPath);

                Pallet = Carton = Input = null;
                ListSsCc.Clear();
                IsFillData = false;

            }
        }

        public void SetConstraintsByDefault()
        {
            this.MinimumLenghtSsCc      = Default_MinimumLenghtSsCc;
            this.RegexFindErrorChars    = Default_RegexFindErrorChars;
        }

        

       
        public void OpenCurrentDirectory()
        {
            if (System.IO.Directory.Exists(this.DirCurrentFile))
                System.Diagnostics.Process.Start(this.DirCurrentFile);
        }
        public void OpenArchiveDirectory()
        {
            if (System.IO.Directory.Exists(this.DirArchiveFile))
                System.Diagnostics.Process.Start(this.DirArchiveFile);
        }
        public void OpenTempDirectory()
        {
            if (System.IO.Directory.Exists(this.DirTempFile))
                System.Diagnostics.Process.Start(this.DirTempFile);
        }

        public void OpenListSsCcToExcel()
        {
            if (!Directory.Exists(DirTempFile)) Directory.CreateDirectory(DirTempFile);
            string fName = System.IO.Path.Combine(DirTempFile, $"export_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff")}.xlsx");
            byte[] fByte = SimpleScan.Dal.Excel.ToExcel_Quick<SimpleScan.Model.Row>(this.listSsCc);
            File.WriteAllBytes(fName, fByte);
            System.Diagnostics.Process.Start(fName);
        }
    }
}
