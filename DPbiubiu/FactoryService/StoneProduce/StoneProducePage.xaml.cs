using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.FactoryService;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.FactoryService.StoneProduce
{
    /// <summary>
    /// Interaction logic for StoneProducePage.xaml
    /// </summary>
    public partial class StoneProducePage : UserControl
    {
        private GetAllDataStatus RequestStatus = new GetAllDataStatus();

        public StoneProducePage()
        {
            InitializeComponent();
            DataContext = new StoneProduceViewModel();
        }

        #region 车队面板

        /// <summary>
        /// 车队列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_CarGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Btn_EditCarGroup.IsEnabled = ListView_CarGroup.SelectedItem != null;
            Btn_AddCar.IsEnabled = ListView_CarGroup.SelectedItem != null;
        }

        /// <summary>
        /// 添加车队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new CarGroupOperateDialog(), "RootDialog", CarGroupOperDialogCloseEvent);
        }

        /// <summary>
        /// 修改车队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_EditCarGroup_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_CarGroup.SelectedItem is null)
            {
                BiuMessageBoxWindows.BiuShow("请先选择一个车队!");
                return;
            }
            await DialogHost.Show(new CarGroupOperateDialog(ListView_CarGroup.SelectedItem as StoneCarGroupModel), "RootDialog", CarGroupOperDialogCloseEvent);
        }


        private void CarGroupOperDialogCloseEvent(object sender, DialogClosingEventArgs eventArgs)
        {
            (DataContext as StoneProduceViewModel).GetCarGroupItems();
        }


        #endregion

        #region 车辆面板
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Btn_EditCar.IsEnabled = DG_Car.SelectedItem != null;
            Btn_ChangeGroup.IsEnabled = DG_Car.SelectedItem != null;
            Btn_LeaveGroup.IsEnabled = DG_Car.SelectedItem != null;
        }

        /// <summary>
        /// 添加车辆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ListView_CarGroup.SelectedItem is null)
            {
                BiuMessageBoxWindows.BiuShow("请先选择一个车队!");
                return;
            }
            await DialogHost.Show(new CarOperateDialog(ListView_CarGroup.SelectedItem as StoneCarGroupModel), "RootDialog", CarOperDialogCloseEvent);
        }

        /// <summary>
        /// 修改车辆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_EditCar_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Car.SelectedItem is null)
            {
                BiuMessageBoxWindows.BiuShow("请先选择一个车辆");
                return;
            }
            await DialogHost.Show(new CarOperateDialog(ListView_CarGroup.SelectedItem as StoneCarGroupModel, DG_Car.SelectedItem as StoneCarModel), "RootDialog", CarOperDialogCloseEvent);
        }

        private void CarOperDialogCloseEvent(object sender, DialogClosingEventArgs eventArgs)
        {
            (DataContext as StoneProduceViewModel).GetCarItems();
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as StoneProduceViewModel).GetCarGroupItems();

            #region 获取统计
            RequestStatus.StartRequest(() =>
            {
                StoneTotalModel stm = ModelHelper.GetInstance().GetApiData(ModelHelper.ApiClient.GetStoneTotalAsync).Result.Data;
                if (stm is null) return;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Run_t1.Text = stm.MonthSize.ToString();
                    Run_t2.Text = stm.MonthWeight.ToString();
                    Run_t3.Text = stm.TotalWeight.ToString();
                });

            });
            #endregion
        }

        /// <summary>
        /// 车辆离队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LeaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Car.SelectedItem is null) return;
            var car = DG_Car.SelectedItem as StoneCarModel;
            if (DG_Car.SelectedItem != null && BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow($"确定将车辆: {car.CarID} 离队?", BiuMessageBoxButton.YesNo)))
            {
                Task.Run(() =>
                {
                    var rs = ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.LeaveStoneCarAsync,
                        new { ID = car.ID },
                        (DataInfo<StoneCarModel> success) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                (DataContext as StoneProduceViewModel).GetCarItems();
                            });
                        }).Result;
                });
            }
        }

        /// <summary>
        /// 导出单据报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportReport_Click(object sender, RoutedEventArgs e)
        {
            var content = (sender as Button).Content;
            long? startStamp = null;
            long? endStamp = null;
            if (null != StartDate.SelectedDate)
                startStamp = Common.DateTime2TimeStamp(StartDate.SelectedDate.Value.Date + StartTime.SelectedTime.Value.TimeOfDay);
            if (null != EndDate.SelectedDate)
                endStamp = Common.DateTime2TimeStamp(EndDate.SelectedDate.Value.Date + EndTime.SelectedTime.Value.TimeOfDay + TimeSpan.FromMinutes(1) - TimeSpan.FromMilliseconds(1));

            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog
            {
                Title = "选择要保存的文件路径",
                Filter = "*.xls|",
                FileName = DateTime.Now.ToString("yyyy-MM-dd HH：mm") + $" 导出无人值守{content}.xls",
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RequestStatus.StartRequest(() =>
                {
                    try
                    {
                        var fs = "统计报表".Equals(content) ? ModelHelper.ApiClient.ExportStoneOrderAsync(new
                        {
                            startTime = startStamp,
                            endTime = endStamp
                        }).GetAwaiter().GetResult() : ModelHelper.ApiClient.ExportStoneOrderDetailAsync(new
                        {
                            startTime = startStamp,
                            endTime = endStamp
                        }).GetAwaiter().GetResult();

                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);

                        // 设置当前流的位置为流的开始
                        fs.Seek(0, SeekOrigin.Begin);
                        using (FileStream fsWrite = new FileStream(ofd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            BinaryWriter bw = new BinaryWriter(fsWrite);
                            bw.Write(bytes);
                            bw.Close();
                        }
                        fs.Close();

                        BiuMessageBoxWindows.BiuShow("导出成功!");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow(er.Message);
                    }
                });
            }
        }

        /// <summary>
        /// 查询单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SearchOrder_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.SelectedDate is null && EndDate.SelectedDate is null)
            {
                BiuMessageBoxWindows.BiuShow("请先选择日期!");
                return;
            }
            StoneOrderModel som = new StoneOrderModel();
           
            if (ListView_CarGroup.SelectedItem != null) som.TeamID = (ListView_CarGroup.SelectedItem as StoneCarGroupModel).ID;
            if (DG_Car.SelectedItem != null) som.CarID = (DG_Car.SelectedItem as StoneCarModel).ID;
            (DataContext as StoneProduceViewModel).GetStoneOrderItems(som);
        }

        private void Btn_ResetSerach_Click(object sender, RoutedEventArgs e)
        {
            StartDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndDate.SelectedDate = null;
            EndTime.SelectedTime = null;
            (DataContext as StoneProduceViewModel).GetStoneOrderItems();
        }

        /// <summary>
        /// 删除车队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_CarGroup.SelectedItem is null) return;
            var carGroup = ListView_CarGroup.SelectedItem as StoneCarGroupModel;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认删除车队: " + carGroup.Name + "?", BiuMessageBoxButton.YesNo)))
            {
                Task.Run(() =>
                {
                    var rs = ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.DeleteStoneCarGroupAsync,
                        new { ID = carGroup.ID },
                        (DataInfo<StoneCarGroupModel> success) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                (DataContext as StoneProduceViewModel).GetCarGroupItems();
                            });
                        }).Result;
                });
            }
        }

        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (DG_StoneOrder.SelectedItem is null) return;
            var order = DG_StoneOrder.SelectedItem as StoneOrderModel;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认删除单据: " + order.OrderID + "?", BiuMessageBoxButton.YesNo)))
            {
                Task.Run(()=> {
                    var rs = ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.DeleteStoneOrderAsync,
                        new { ID = order.ID },
                        (DataInfo<StoneOrderModel> success) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                (DataContext as StoneProduceViewModel).GetStoneOrderItems();
                            });
                        }).Result;
                });
            }
        }
    }
}
