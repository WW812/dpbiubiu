using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.role;
using biubiu.model.user;
using biubiu.view_model.role;
using biubiu.view_model.user;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using WebApiClient;

namespace biubiu.views.setting.role
{
    /// <summary>
    /// RolePage.xaml 的交互逻辑
    /// </summary>
    public partial class RolePage : UserControl
    {
        //public ObservableCollection<Role> ObservableRoles;
        public List<Role> RolesListViewItemsSource = new List<Role>();
        public List<PermissionViewModel> PermissionListViewItemsSource = new List<PermissionViewModel>();
        public UserGridViewModel uGVM;
        public RolePage()
        {
            InitializeComponent();
            RolesListView.ItemsSource = RolesListViewItemsSource;
            PermissionListView.ItemsSource = PermissionListViewItemsSource;
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        public async void GetRoles()
        {
            try
            {
                var Result = await ModelHelper.ApiClient.GetRolesAsync();
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    RolesListViewItemsSource.Clear();
                    RolesListViewItemsSource.AddRange(Result.Data);
                    RolesListView.Items.Refresh();
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        #region 设置角色ListView
        /// <summary>
        /// 设置角色ListView
        /// </summary>
        /// <param name="LRoles"></param>
        /*
        void SetRolesListView(List<Role> LRoles)
        {
            LRoles.ForEach(delegate (Role role)
            {
                MaterialDesignThemes.Wpf.PackIcon EditPI = new MaterialDesignThemes.Wpf.PackIcon();
                EditPI.Kind = MaterialDesignThemes.Wpf.PackIconKind.TableEdit;

                Button EditBtn = new Button();
                EditBtn.Background = null;
                EditBtn.BorderBrush = null;
                EditBtn.Padding = new Thickness(0, 0, 0, 0);
                EditBtn.Height = 20;
                EditBtn.HorizontalAlignment = HorizontalAlignment.Right;
                EditBtn.Width = 18;
                EditBtn.Content = EditPI;

                MaterialDesignThemes.Wpf.PackIcon DeletePI = new MaterialDesignThemes.Wpf.PackIcon();
                DeletePI.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteForever;

                Button DeleteBtn = new Button();
                DeleteBtn.Background = null;
                DeleteBtn.BorderBrush = null;
                DeleteBtn.Padding = new Thickness(0, 0, 0, 0);
                DeleteBtn.Height = 20;
                DeleteBtn.HorizontalAlignment = HorizontalAlignment.Right;
                DeleteBtn.Width = 18;
                DeleteBtn.Content = DeletePI;

                TextBlock RoleName = new TextBlock();
                RoleName.Width = 156;
                RoleName.HorizontalAlignment = HorizontalAlignment.Right;
                RoleName.Text = role.Name;

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Children.Add(RoleName);
                sp.Children.Add(EditBtn);
                sp.Children.Add(DeleteBtn);

                ListViewItem LVItem = new ListViewItem();
                LVItem.Height = 36;
                LVItem.Padding = new Thickness(8, 8, 8, 8);
                LVItem.Content = sp;

                RolesListView.Items.Add(LVItem);
            });
        }
        */
        #endregion

        public async void GetUsers(Role RP)
        {
            /*
            var RoleParam = RP;
            try
            {
                var Result = await ModelHelper.ApiClient.GetUserAsync(RoleParam);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    uGVM = new UserGridViewModel(Result.Data);
                    UserDataGrid.ItemsSource = uGVM.UserItems;
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetUserAsync,
                RP,
                delegate (DataInfo<List<User>> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        uGVM = new UserGridViewModel(result.Data);
                        UserDataGrid.ItemsSource = uGVM.UserItems;
                    }));
                });
        }

        public async void DeleteUser(string userID)
        {
            /*
            var UserParam = new User { ID = userID };
            try
            {
                var Result = await ModelHelper.ApiClient.DeleteUserAsync(UserParam);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    GetUsers(RolesListView.SelectedItem as Role);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.DeleteUserAsync,
                new { ID = userID },
                delegate (DataInfo<User> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        GetUsers(RolesListView.SelectedItem as Role);
                    }));
                });
        }

        private void RolesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(RolesListView.SelectedItem is Role selectedRole)) return;

            GetUsers(selectedRole);
            GetPermissionByRole(selectedRole);
            PermissionListView.IsEnabled = true;
            PermissionListViewTitleTextBlock.Text = selectedRole.Name;
        }

        /// <summary>
        /// 获取全部权限
        /// </summary>
        public async void GetPermissionItems()
        {
            try
            {
                var Result = await ModelHelper.ApiClient.GetPermissionsAsync();
                var pList = new List<PermissionViewModel>();
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    PermissionListViewItemsSource.Clear();
                    Result.Data.ForEach(delegate (Permission permission)
                    {
                        PermissionListViewItemsSource.Add(permission.ToPermissionViewModel());
                    });
                    PermissionListView.Items.Refresh();
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 根据角色获取权限
        /// </summary>
        /// <param name="role"></param>
        public async void GetPermissionByRole(Role role)
        {
            if (role == null)
            {
                return;
            }
            try
            {
                var Result = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetPermissionByRole, role);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    for (var j = 0; j < PermissionListViewItemsSource.Count; j++)
                    {
                        PermissionListViewItemsSource[j].IsSelected = false;
                        for (var i = 0; i < Result.Data.Count; i++)
                        {
                            if (Result.Data[i].ID == PermissionListViewItemsSource[j].ID)
                            {
                                PermissionListViewItemsSource[j].IsSelected = true;
                                continue;
                            }
                        }
                    }
                    PermissionListView.Items.Refresh();
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 编辑用户按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowEditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserViewModel uVM = btn.CommandParameter as UserViewModel;
            RolePageViewModel rVM = DataContext as RolePageViewModel;
            rVM.RunEditUserDialogCommand.Execute(uVM);
        }

        /// <summary>
        /// 删除用户按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserViewModel uVM = btn.CommandParameter as UserViewModel;
            DeleteUser(uVM.ID);
        }

        /// <summary>
        /// 权限修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRoleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 删除角色按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var role = (sender as Button).CommandParameter as Role;
            if (BiuMessageBoxResult.OK.Equals(BiuMessageBoxWindows.BiuShow("是否删除角色: " + role.Name, BiuMessageBoxButton.OKCancel, BiuMessageBoxImage.Question)))
            {
                DeleteRoleFunction(role);
            }
        }

        /// <summary>
        /// 删除角色函数
        /// </summary>
        private async void DeleteRoleFunction(Role role)
        {
            /*
            try
            {
                var result = await ModelHelper.ApiClient.DeleteRoleAsync(role);
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    GetRoles();
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.DeleteRoleAsync,
                role,
                delegate (DataInfo<object> result)
                {
                    GetRoles();
                });
        }

        /// <summary>
        /// 界面刷新函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new RolePageViewModel(this);
            GetRoles();
            GetPermissionItems();
            PermissionListViewTitleTextBlock.Text = "当前未选择角色";
        }

        private void SubmitPermissionBtn_Click(object sender, RoutedEventArgs e)
        {
            SetPermission();
        }

        private async void SetPermission()
        {
            var permissionIDs = new List<string>();
            var selectRole = (RolesListView.SelectedItem as Role);
            PermissionListViewItemsSource.ForEach(delegate (PermissionViewModel permissionViewModel)
            {
                if (permissionViewModel.IsSelected)
                    permissionIDs.Add(permissionViewModel.ID);
            });
            /*
            var param = new
            {
                role = selectRole.ID,
                ids = permissionIDs
            };

            try
            {
                var result = await ModelHelper.ApiClient.SetPermissionByRole(param);
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    GetPermissionByRole(selectRole);
                    SnackbarViewModel.GetInstance().PoupMessageAsync("添加成功!");
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.SetPermissionByRole,
                new
                {
                    role = selectRole.ID,
                    ids = permissionIDs
                },
                delegate(DataInfo<object> param) {
                    GetPermissionByRole(selectRole);
                    SnackbarViewModel.GetInstance().PoupMessageAsync("添加成功!");
                });
        }
    }
}
