using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.views.setting.print;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace biubiu.view_model.print
{
    public class PrintPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// tabControl 的SelectedIndex
        /// </summary>
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        private BillConfig _stockEnter = Config.STOCK_ENTER_CONFIG;
        public BillConfig StockEnter
        {
            get { return _stockEnter; }
            set
            {
                _stockEnter = value;
                NotifyPropertyChanged("StockEnter");
            }
        }

        private BillConfig _stockExit = Config.STOCK_EXIT_CONFIG;
        public BillConfig StockExit
        {
            get { return _stockExit; }
            set
            {
                _stockExit = value;
                NotifyPropertyChanged("StockExit");
            }
        }

        private BillConfig _shipEnter = Config.SHIP_ENTER_CONFIG;
        public BillConfig ShipEnter
        {
            get { return _shipEnter; }
            set
            {
                _shipEnter = value;
                NotifyPropertyChanged("ShipEnter");
            }
        }

        private BillConfig _shipExit = Config.SHIP_EXIT_CONFIG;
        public BillConfig ShipExit
        {
            get { return _shipExit; }
            set
            {
                _shipExit = value;
                NotifyPropertyChanged("ShipExit");
            }
        }

        /// <summary>
        /// 当前tabControl使用的BillConfig
        /// </summary>
        private BillConfig _currentBillConfig;
        public BillConfig CurrentBillConfig
        {
            get { return _currentBillConfig; }
            set
            {
                _currentBillConfig = value;
                NotifyPropertyChanged("CurrentBillConfig");
            }
        }

        /// <summary>
        /// 底部弹窗
        /// </summary>
        /*
        private SnackbarViewModel _messageBar = new SnackbarViewModel();
        public SnackbarViewModel MessageBar
        {
            get { return _messageBar; }
            set
            {
                _messageBar = value;
                NotifyPropertyChanged("MessageBar");
            }
        }
        */

        /// <summary>
        /// 本机打印机集合
        /// </summary>
        private ObservableCollection<string> _printerItems;
        public ObservableCollection<string> PrinterItems
        {
            get { return _printerItems; }
            set
            {
                _printerItems = value;
                NotifyPropertyChanged("PrinterItems");
            }
        }

        /// <summary>
        /// 标准规格
        /// </summary>
        private ObservableCollection<BillConfig> _standardBillItems;
        public ObservableCollection<BillConfig> StandardBillItems
        {
            get { return _standardBillItems; }
            set
            {
                _standardBillItems = value;
                NotifyPropertyChanged("StandardBillItems");
            }
        }

        /// <summary>
        /// 特殊规格
        /// </summary>
        private ObservableCollection<BillConfig> _specialBillItems;
        public ObservableCollection<BillConfig> SpecialBillItems
        {
            get { return _specialBillItems; }
            set
            {
                _specialBillItems = value;
                NotifyPropertyChanged("SpecialBillItems");
            }
        }

        /// <summary>
        /// 热敏打印机
        /// </summary>
        private ObservableCollection<BillConfig> _thermalBillItems;
        public ObservableCollection<BillConfig> ThermalBillItems
        {
            get { return _thermalBillItems; }
            set
            {
                _thermalBillItems = value;
                NotifyPropertyChanged("ThermalBillItems");
            }
        }

        /// <summary>
        /// 其他
        /// </summary>
        private ObservableCollection<BillConfig> _otherBillItems;
        public ObservableCollection<BillConfig> OtherBillItems
        {
            get { return _otherBillItems; }
            set
            {
                _otherBillItems = value;
                NotifyPropertyChanged("OtherBillItems");
            }
        }

        private ObservableCollection<TemplateItems> _tItems;
        public ObservableCollection<TemplateItems> TItems
        {
            get { return _tItems; }
            set
            {
                _tItems = value;
                NotifyPropertyChanged("TItems");
            }
        }

        private BillConfig _selectedBill;
        public BillConfig SelectedBill
        {
            get { return _selectedBill; }
            set
            {
                _selectedBill = value;
                NotifyPropertyChanged("SelectedBill");
            }
        }

        private string _title = "进料进厂票据设置";
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private readonly List<string> _paths = new List<string> {@"./template/Stock/Enter/",@"./template/Stock/Exit/", @"./template/Ship/Enter/", @"./template/Ship/Exit/" };

        public ICommand SaveCommand => new AnotherCommandImplementation(ExecuteSaveCommand);
        public ICommand RunPreviewBillTemplateDialogCommand => new AnotherCommandImplementation(ExecuteRunPreviewBillTemplateDialogCommand);

        private void ExecuteSaveCommand(object o)
        {
            if (CurrentBillConfig.IsAppoint && CurrentBillConfig.ApponitPrinterName.Equals(""))
            {
                BiuMessageBoxWindows.BiuShow("尚未选择要指定的打印机!",image:BiuMessageBoxImage.Warning);
                return;
            }
            try
            {
                var section = "";
                switch (SelectedTabIndex)
                {
                    case 0:
                        section = "StockEnter";
                        break;
                    case 1:
                        section = "StockExit";
                        break;
                    case 2:
                        section = "ShipEnter";
                        break;
                    case 3:
                        section = "ShipExit";
                        break;
                }
                CurrentBillConfig.WriteToFile(new INIClass(Config.BILL_CONFIG_PATH), section);
                SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("保存失败，原因：" + er.Message,image:BiuMessageBoxImage.Error);
            }
        }
        private void ExecuteRunPreviewBillTemplateDialogCommand(object o)
        {
            var win = new PreviewBillTemplateWindow(CurrentBillConfig);
            win.ShowDialog();
        }

        public PrintPageViewModel()
        {
            CurrentBillConfig = StockEnter;
            GetPrinterItems();
            GetBillConfigItems();
        }

        private void GetPrinterItems()
        {
            try
            {
                var printers = PrinterSettings.InstalledPrinters;
                var items = new List<string>();
                foreach (var item in printers)
                {
                    var name = item.ToString();
                    items.Add(name);
                }
                items.Add("");
                PrinterItems = new ObservableCollection<string>(items);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
            }
        }

        public void GetBillConfigItems()
        {
            var path = _paths[SelectedTabIndex];
            Task.Run(() =>
            {
                var templateItems = new ObservableCollection<TemplateItems>();
                foreach (var folder in Directory.GetDirectories(path))
                {
                    var list = new ObservableCollection<BillConfig>();
                    foreach (var file in Directory.GetFiles(folder, "*.rdlc"))
                    {
                        list.Add(new BillConfig { TemplateName = Path.GetFileName(file).Replace(".rdlc", ""), TemplatePath = file });
                    }
                    templateItems.Add(new TemplateItems
                    {
                        //FolderName = folder.Replace("./template/", ""),
                        FolderName = folder.Replace(path, ""),
                        BillItems = list
                    });
                }
                TItems = templateItems;
            });
        }

        /// <summary>
        /// NotifyPropertyChanged事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "SelectedTabIndex":
                    switch (SelectedTabIndex)
                    {
                        case 0:
                            CurrentBillConfig = StockEnter;
                            Title = "进料进厂票据设置";
                            break;
                        case 1:
                            CurrentBillConfig = StockExit;
                            Title = "进料出厂票据设置";
                            break;
                        case 2:
                            CurrentBillConfig = ShipEnter;
                            Title = "出料预装车票据设置";
                            break;
                        case 3:
                            CurrentBillConfig = ShipExit;
                            Title = "出料出厂票据设置";
                            break;
                    }
                    GetPrinterItems();
                    GetBillConfigItems();
                    break;
                case "SelectedBill":
                    CurrentBillConfig.TemplateName = SelectedBill.TemplateName;
                    CurrentBillConfig.TemplatePath = SelectedBill.TemplatePath;
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TemplateItems : INotifyPropertyChanged
    {
        private string _folderName;
        public string FolderName
        {
            get { return _folderName; }
            set
            {
                _folderName = value;
                NotifyPropertyChanged("FolderName");
            }
        }

        private ObservableCollection<BillConfig> _billItems;
        public ObservableCollection<BillConfig> BillItems
        {
            get { return _billItems; }
            set
            {
                _billItems = value;
                NotifyPropertyChanged("BillItems");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
