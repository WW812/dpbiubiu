using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.FactoryService;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.FactoryService.StoneProduce
{
    /// <summary>
    /// Interaction logic for CarGroupOperateDialog.xaml
    /// </summary>
    public partial class CarGroupOperateDialog : UserControl
    {
        private bool _isAdd = false;
        private StoneCarGroupModel scgm = null;

        /// <summary>
        /// 添加车队
        /// </summary>
        public CarGroupOperateDialog()
        {
            InitializeComponent();
            _isAdd = true;
            Btn_Submit.Content = "添加";
            Title.Text = "添加";
        }

        /// <summary>
        /// 修改车队
        /// </summary>
        /// <param name="o"></param>
        public CarGroupOperateDialog(StoneCarGroupModel scgm)
        {
            InitializeComponent();
            if(scgm is null)
            {
                BiuMessageBoxWindows.BiuShow("参数传输错误!");
                DialogHost.CloseDialogCommand.Execute(true, this);
            }
            this.scgm = scgm;
            _isAdd = false;
            Btn_Submit.Content = "修改";
            Title.Text = "修改";
            TB_CarGroupName.Text = scgm.Name;
            TB_SignInterval.Text = scgm.SignInterval.ToString();
        }

        private void Btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validate()) return;
                if (_isAdd) // 新增
                    AddCarGroup();
                else // 修改
                    EditCarGroup();
            }
            catch(Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        private void AddCarGroup()
        {
            var param = new StoneCarGroupModel
            {
                CompanyId = Config.COMPANY_ID,
                Name = TB_CarGroupName.Text,
                SignInterval = double.Parse(TB_SignInterval.Text)
            };
            Task.Run(()=>
            {
                try
                {
                    var rs =  ModelHelper.GetInstance().GetApiDataArg(
                            ModelHelper.ApiClient.AddStoneCarGroupAsync,
                            param,
                            (DataInfo<StoneCarGroupModel> success) => {
                                Application.Current.Dispatcher.Invoke(() => {
                                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                                });
                            }
                        ).Result;
                }
                catch(Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message);
                }
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        private void EditCarGroup()
        {
            var param = new StoneCarGroupModel
            {
                ID = scgm.ID,
                Name = TB_CarGroupName.Text,
                SignInterval = double.Parse(TB_SignInterval.Text)
            };
            Task.Run(() =>
            {
                try
                {
                    var rs = ModelHelper.GetInstance().GetApiDataArg(
                            ModelHelper.ApiClient.EditStoneCarGroupAsync,
                            param,
                            (DataInfo<StoneCarGroupModel> success) => {
                                Application.Current.Dispatcher.Invoke(() => {
                                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                                });
                            }
                        ).Result;
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message);
                }
            });
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            if (string.IsNullOrEmpty(TB_CarGroupName.Text))
            {
                BiuMessageBoxWindows.BiuShow("车队名称不可为空!");
                return false;
            }

            if(string.IsNullOrEmpty(TB_SignInterval.Text) || !RegexChecksum.IsNonnegativeReal(TB_SignInterval.Text))
            {
                BiuMessageBoxWindows.BiuShow("请输入正确的打卡间隔");
                return false;
            }

            return true;
        }
    }
}
