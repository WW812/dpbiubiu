using biubiu.Domain;
using biubiu.model;
using biubiu.model.bill;
using biubiu.model.customer.ship_customer;
using biubiu.model.customer.stock_customer;
using biubiu.model.FactoryService;
using biubiu.model.goods.stock_goods;
using biubiu.model.paytype;
using biubiu.model.role;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
using biubiu.model.stock_order;
using biubiu.model.system;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WebApiClient;
using WebApiClient.Attributes;

/// <summary>
/// Http请求
/// </summary>
namespace biubiu
{
    /// <summary>
    /// user模块请求
    /// </summary>
    public interface IApi : IHttpApi
    {
        #region 用户模块(user)请求
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "login")]
        ITask<DataInfo<User>> LoginAsync([JsonContent] object param, CancellationToken toke);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "logout")]
        ITask<DataInfo<User>> LogoutAsync();

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getRoles")]
        ITask<DataInfo<List<Role>>> GetRolesAsync();

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "addRole")]
        ITask<DataInfo<Role>> NewRoleAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "deleteRole")]
        ITask<DataInfo<Object>> DeleteRoleAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "updateRole")]
        ITask<DataInfo<Role>> EditRoleAsync([JsonContent] Role role, CancellationToken toke);


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="role">角色Id</param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getUser")]
        ITask<DataInfo<List<User>>> GetUserAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getUser")]
        ITask<DataInfo<List<User>>> GetAllUserAsync();


        /// <summary>
        /// 为角色配置权限
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "addPermission")]
        ITask<DataInfo<List<Permission>>> AllotPermissionToRoleAsync();


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "addUser")]
        ITask<DataInfo<User>> NewUserAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "deleteUser")]
        ITask<DataInfo<User>> DeleteUserAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "updateUser")]
        ITask<DataInfo<User>> EditUserAsync([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 获取左侧菜单(获取权限)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getMenu")]
        ITask<DataInfo<List<Permission>>> GetPermissionMenuAsync();


        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getAllPermissions")]
        ITask<DataInfo<List<Permission>>> GetPermissionsAsync();


        /// <summary>
        /// 根据角色获得权限
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "getRolePermission")]
        ITask<DataInfo<List<Permission>>> GetPermissionByRole([JsonContent] object param, CancellationToken toke);


        /// <summary>
        /// 根据角色分配权限
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "addRolePermission")]
        ITask<DataInfo<Object>> SetPermissionByRole([JsonContent] object param, CancellationToken toke);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"> password:新密码 , oldPassword:老密码</param>
        /// <returns></returns>
        [HttpPost(Config.UserURL + "updatePassword")]
        ITask<DataInfo<object>> ChangePasswordAsync([JsonContent] object param, CancellationToken toke);

        #endregion


        #region 销售模块 Order
        /// <summary>
        /// 添加料品
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "addGoods")]
        ITask<DataInfo<ShipGoods>> CreateGoodsAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取出料料品全部
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getGoods")]
        ITask<DataInfo<List<ShipGoods>>> GetGoodsAsync();

        /// <summary>
        /// 获取出料料品(出料过磅界面使用)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getGoods")]
        ITask<DataInfo<List<ShipGoods>>> GetGoodsAsync([JsonContent] object goods, CancellationToken token);

        /// <summary>
        /// 根据料品ID获取料品价格
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getGoodsById")]
        ITask<DataInfo<ShipGoods>> GetGoodsByIdAsync([JsonContent] object goods, CancellationToken token);

        /// <summary>
        /// 根据客户获取料品价格
        /// </summary>
        [HttpPost(Config.ShipCustomerURL + "getGoodsPriceByCustomer")]
        ITask<DataInfo<ShipCustomerGoodsPrice>> GetShipCustomerGoodsPriceSingleAsync([JsonContent] object shipCustomerGoodsPrice, CancellationToken token);

        /// <summary>
        /// 修改料品启用禁用状态
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "updateGoodsValid")]
        ITask<DataInfo<ShipGoods>> ChangeGoodsValidAsync([JsonContent] object goods, CancellationToken token);

        /// <summary>
        /// 修改单个料品价格
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "updateGoodsPrice")]
        ITask<DataInfo<ShipGoods>> ChangeSingleGoodsAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改料品价格
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "updateAllGoodsPrice")]
        ITask<DataInfo<Object>> ChangeGoodsPriceAsync([JsonContent] object change, CancellationToken token);


        /// <summary>
        /// 创建进厂出料单子
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "createEnterOrder")]
        ITask<DataInfo<ShipOrder>> CreateEnterShipOrderAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 创建出厂出料单子
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "createExitOrder")]
        ITask<DataInfo<ShipOrder>> CreateExitShipOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 删除已结账出料单据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "deleteOrder")]
        ITask<DataInfo<Object>> DeleteExitShipOrderAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 获取出料单据(过磅界面使用)
        /// </summary>
        /// <param name="param">status: -1获取空车，0获取进厂，1获取出场</param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getOrder")]
        ITask<DataInfo<List<ShipOrder>>> GetShipOrdersAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 批量修改客户单据
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "editOrderforCus")]
        ITask<DataInfo<object>> ChangeOrderForCus([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "addCustomer")]
        ITask<DataInfo<ShipCustomer>> CreateShipCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "deleteCustomer")]
        ITask<DataInfo<object>> DeleteShipCustomerAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 获取客户(出料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomer")]
        ITask<DataInfo<List<ShipCustomer>>> GetShipCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取客户(出料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomerById")]
        ITask<DataInfo<ShipCustomer>> GetShipCustomerByIDAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取客户车辆(出料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomerCar")]
        ITask<DataInfo<List<ShipCustomerCar>>> GetShipCustomerCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 添加客户车辆(出料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "addCustomerCar")]
        ITask<DataInfo<ShipCustomerCar>> CreateShipCustomerCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 删除客户车辆(出料)
        /// </summary>
        /// <param name="shipCustomerCar"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "deleteCustomerCar")]
        ITask<DataInfo<ShipCustomerCar>> DeleteShipCustomerCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取预付款流水
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomerMoneyList")]
        ITask<DataInfo<List<ShipCustomerMoney>>> GetShipCustomerStatement([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 导出预付款流水
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.ShipCustomerURL + "exportCusMoneyDetailReport")]
        ITask<Stream> ExportShipCustomerStatement(object param);

        /// <summary>
        /// 获取出料客户的出料订单
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getOrderByCus")]
        ITask<DataInfo<List<ShipOrder>>> GetShipOrderByShipCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取客户料品价格
        /// </summary>
        /// <param name="shipCustomerGoodsPrice"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomerGoodsPrice")]
        ITask<DataInfo<List<ShipCustomerGoodsPrice>>> GetShipCustomerGoodsPriceAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 设置客户料品优惠额度
        /// </summary>
        /// <param name="param">list<></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "addCustomerPrice")]
        ITask<DataInfo<Object>> SetShipCustomerPriceAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改出料进厂单据
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "editEnterOrder")]
        ITask<DataInfo<ShipOrder>> ChangeEnterShipOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改出料出厂单据
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "editExitOrder")]
        ITask<DataInfo<ShipOrder>> ChangeExitShipOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 出料单据管理
        /// 获取单据
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getAllOrder")]
        ITask<DataInfo<List<ShipOrder>>> GetAllShipOrdersAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 导出出料单据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.ShipOrderURL + "exportOrderByTime")]
        ITask<Stream> ExportShipOrder(object param);

        /// <summary>
        /// 导出进料单据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.StockOrderURL + "exportOrderByTime")]
        ITask<Stream> ExportStockOrder(object param);

        /// <summary>
        /// 添加出料客户预付款
        /// </summary>
        /// <param name="shipCustomerMoney"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "addCustomerMoney")]
        ITask<DataInfo<ShipCustomerMoney>> SetShipCustomerMoneyAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 获取客户预付款
        /// </summary>
        /// <param name="shipCustomerMoney"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getCustomerMoney")]
        ITask<DataInfo<List<ShipCustomerMoney>>> GetShipCustomerMoneyAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改出料客户信息
        /// </summary>
        /// <param name="shipCustomer"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "updateCustomer")]
        ITask<DataInfo<ShipCustomer>> ChangeShipCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 出料单补单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "appendOrder")]
        ITask<DataInfo<ShipOrder>> MendShipOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取进料交账统计
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getTodayOrderDetails")]
        ITask<DataInfo<BillDetailsModel>> GetReferShipBillDetails();

        /// <summary>
        /// 出料交账
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "addTodayOrder")]
        ITask<DataInfo<Object>> ReferShipBill();

        /// <summary>
        /// 获取出料交账单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getTodayOrderList")]
        ITask<DataInfo<List<BillModel>>> GetShipBillsAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据ID获取单个交账单详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getTodayOrderById")]
        ITask<DataInfo<BillModel>> GetShipBillsByIDAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据交账单ID 获取单据列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipOrderURL + "getOrderByTodayOrderId")]
        ITask<DataInfo<List<ShipOrder>>> GetShipOrderByBillIDAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取出料客户详单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.ShipCustomerURL + "exportCusOrderDetailReport")]
        ITask<Stream> ExportShipCusOrder(object param);

        /// <summary>
        /// 作废客户预付款
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "deleteCustomerMoney")]
        ITask<DataInfo<object>> VoidCustomerMoney([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取客户结算详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "settleDetail")]
        ITask<DataInfo<ShipCustomerSettleDetail>> GetSettleShipCustomerAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 结算客户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "settleAccounts")]
        ITask<DataInfo<object>> SettleShipCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.ShipCustomerURL + "getSettle")]
        ITask<DataInfo<List<ShipCustomerSettle>>> GetShipCustomerSettleAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 导出客户结算详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.ShipCustomerURL + "exportCusSettleDetailReport")]
        ITask<Stream> ExportSettleAsync(object param);

        #endregion


        #region 采购模块(进料)
        /// <summary>
        /// 添加料品
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "addGoods")]
        ITask<DataInfo<StockGoods>> CreateStockGoodsAsync([JsonContent] object goods, CancellationToken token);


        /// <summary>
        /// 获取进料料品全部
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getGoods")]
        ITask<DataInfo<List<StockGoods>>> GetStockGoodsAsync();

        /// <summary>
        /// 获取进料料品(进料过磅界面使用)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getGoods")]
        ITask<DataInfo<List<StockGoods>>> GetStockGoodsAsync([JsonContent] object goods, CancellationToken token);


        /// <summary>
        /// 修改料品启用禁用状态
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "updateGoodsValid")]
        ITask<DataInfo<StockGoods>> ChangeStockGoodsValidAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "addCustomer")]
        ITask<DataInfo<StockCustomer>> CreateStockCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "deleteCustomer")]
        ITask<DataInfo<object>> DeleteStockCustomerAsync([JsonContent] object param,CancellationToken token);

        /// <summary>
        /// 获取客户(进料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "getCustomer")]
        ITask<DataInfo<List<StockCustomer>>> GetStockCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取客户料品价格
        /// </summary>
        /// <param name="shipCustomerGoodsPrice"></param>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "getCustomerGoodsPrice")]
        ITask<DataInfo<List<StockCustomerGoodsPrice>>> GetStockCustomerGoodsPriceAsync([JsonContent] object stockCustomerGoodsPrice, CancellationToken token);


        /// <summary>
        /// 获取客户车辆(进料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "getCustomerCar")]
        ITask<DataInfo<List<StockCustomerCar>>> GetStockCustomerCarAsync([JsonContent] object shipCustomerCar, CancellationToken token);


        /// <summary>
        /// 添加客户车辆(进料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "addCustomerCar")]
        ITask<DataInfo<StockCustomerCar>> CreateStockCustomerCarAsync([JsonContent] object shipCustomerCar, CancellationToken token);


        /// <summary>
        /// 删除客户车辆(进料)
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "deleteCustomerCar")]
        ITask<DataInfo<StockCustomerCar>> DeleteStockCustomerCarAsync([JsonContent] object shipCustomerCar, CancellationToken token);


        /// <summary>
        /// 获取出料客户的出料订单
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getOrderByCus")]
        ITask<DataInfo<List<StockOrder>>> GetStockOrderByStockCustomerAsync([JsonContent] object stockOrder, CancellationToken token);


        /// <summary>
        /// 修改客户料品价格
        /// </summary>
        /// <param name="param">list<></param>
        /// <returns></returns>
        [HttpPost(Config.StockCustomerURL + "addCustomerPrice")]
        ITask<DataInfo<Object>> SetStockCustomerPriceAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改客户信息
        /// </summary>
        [HttpPost(Config.StockCustomerURL + "updateCustomer")]
        ITask<DataInfo<StockCustomer>> ChangeStockCustomerAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据料品ID获取料品价格
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getGoodsById")]
        ITask<DataInfo<StockGoods>> GetStockGoodsByIdAsync([JsonContent] object goods, CancellationToken token);

        /// <summary>
        /// 根据客户获取料品价格
        /// </summary>
        [HttpPost(Config.StockCustomerURL + "getGoodsPriceByCustomer")]
        ITask<DataInfo<StockCustomerGoodsPrice>> GetStockCustomerGoodsPriceSingleAsync([JsonContent] object stockCustomerGoodsPrice, CancellationToken token);

        /// <summary>
        /// 创建进厂出料单子
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "createEnterOrder")]
        ITask<DataInfo<StockOrder>> CreateEnterStockOrderAsync([JsonContent] object stockOrder, CancellationToken token);


        /// <summary>
        /// 创建出厂出料单子
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "createExitOrder")]
        ITask<DataInfo<StockOrder>> CreateExitStockOrderAsync([JsonContent] object shipOrder, CancellationToken token);

        /// <summary>
        /// 删除已结账进料单据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "deleteOrder")]
        ITask<DataInfo<object>> DeleteExitStockOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取进料单据(过磅界面使用)
        /// </summary>
        /// <param name="param">status: -1获取空车，0获取进厂，1获取出场</param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getOrder")]
        ITask<DataInfo<List<StockOrder>>> GetStockOrdersAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 出料单据管理
        /// 获取单据
        /// </summary>
        /// <param name="stockOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getAllOrder")]
        ITask<DataInfo<List<StockOrder>>> GetAllStockOrdersAsync([JsonContent] object stockOrder, CancellationToken token);

        /// <summary>
        /// 修改进料进厂单据
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "editEnterOrder")]
        ITask<DataInfo<StockOrder>> ChangeEnterStockOrderAsync([JsonContent] object order, CancellationToken token);

        /// <summary>
        /// 修改进料出厂单据
        /// </summary>
        /// <param name="shipOrder"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "editExitOrder")]
        ITask<DataInfo<StockOrder>> ChangeExitStockOrderAsync([JsonContent] object order, CancellationToken token);

        /// <summary>
        /// 修改单个进料料品价格
        /// </summary>
        [HttpPost(Config.StockOrderURL + "updateGoodsPrice")]
        ITask<DataInfo<StockGoods>> ChangeStockGoodsPriceAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "paidOrder")]
        ITask<DataInfo<StockOrder>> PaymentStockOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 进料单补单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "appendOrder")]
        ITask<DataInfo<StockOrder>> MendStockOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取进料交账统计
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getTodayOrderDetails")]
        ITask<DataInfo<BillDetailsModel>> GetReferStockBillDetails();

        /// <summary>
        /// 进料交账
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "addTodayOrder")]
        ITask<DataInfo<object>> ReferStockBill();

        /// <summary>
        /// 获取进料交账单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getTodayOrderList")]
        ITask<DataInfo<List<BillModel>>> GetStockBillsAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据ID获取单个交账单详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getTodayOrderById")]
        ITask<DataInfo<BillModel>> GetStockBillsByIDAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 根据交账单ID 获取单据列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.StockOrderURL + "getOrderByTodayOrderId")]
        ITask<DataInfo<List<StockOrder>>> GetStockOrderByBillIDAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 批量修改单据，根据交账单
        /// </summary>
        [HttpPost(Config.StockOrderURL + "editOrders")]
        ITask<DataInfo<object>> EditOrdersByBill([JsonContent] object param, CancellationToken token);

        #endregion

        #region 设置模块

        /// <summary>
        /// 设置系统设置
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost(Config.SystemSettingURL + "updateConfig")]
        ITask<DataInfo<SystemModel>> SetSystemSetting([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取系统设置
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.SystemSettingURL + "getConfig")]
        ITask<DataInfo<SystemModel>> GetSystemSetting();

        [HttpPost(Config.SystemSettingURL + "enable")]
        ITask<DataInfo<SystemModel>> SetAdjustEnabled([JsonContent] object param,CancellationToken token);
        #endregion

        #region 七牛云相关接口

        /// <summary>
        /// 获取七牛云上传凭证
        /// </summary>
        /// <param name="param"> object 下面包含 id => 订单ID</param>
        /// <returns></returns>
        [HttpPost("common/qiniu/getToken")]
        ITask<DataInfo<String>> GetQiniuUploadToken([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取图片地址
        /// </summary>
        /// <param name="param"> object 下面包含 id => 订单ID</param>
        /// <returns></returns>
        [HttpPost("common/qiniu/getImg")]
        ITask<DataInfo<List<String>>> GetPictureURL([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取更新地址
        /// </summary>
        /// <returns></returns>
        [HttpPost("common/updateUrl")]
        ITask<DataInfo<String>> GetUpdateURL();

        #endregion


        #region 财务模块

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "addAccount")]
        ITask<DataInfo<PayType>> AddPayTypeAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取账户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "getAccount")]
        ITask<DataInfo<List<PayType>>> GetPayTypeAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改账户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "updateAccount")]
        ITask<DataInfo<PayType>> ChangePayTypeAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取承兑
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "getCustomerHonour")]
        ITask<DataInfo<List<ShipCustomerMoney>>> GetAcceptsAsync([JsonContent] object param,CancellationToken token);

        /// <summary>
        /// 修改承兑
        /// </summary>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "updateCustomerHonour")]
        ITask<DataInfo<object>> EditAcceptAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据时间获取出料报表
        /// </summary>
        [HttpPost(Config.FinanceURL + "getSalesOrderReportByTime")]
        ITask<DataInfo<BillDetailsModel>> GetShipReportAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 根据时间获取进料报表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FinanceURL + "getStockOrderReportByTime")]
        ITask<DataInfo<BillDetailsModel>> GetStockReprotAsync([JsonContent] object param, CancellationToken token);

        #endregion


        #region 厂务模块

        /// <summary>
        /// 获取无人值守车队集合
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "getStoneTeam")]
        ITask<DataInfo<List<StoneCarGroupModel>>> GetStoneCarGroupAsync();


        /// <summary>
        /// 添加无人值守车队
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "addStoneTeam")]
        ITask<DataInfo<StoneCarGroupModel>> AddStoneCarGroupAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改无人值守车队
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "changeStoneTeam")]
        ITask<DataInfo<StoneCarGroupModel>> EditStoneCarGroupAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 删除无人值守车队
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "deleteStoneTeam")]
        ITask<DataInfo<StoneCarGroupModel>> DeleteStoneCarGroupAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 获取无人值守车辆集合
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "getStoneCar")]
        ITask<DataInfo<List<StoneCarModel>>> GetStoneCarAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 添加无人值守车辆
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "addStoneCar")]
        ITask<DataInfo<StoneCarModel>> AddStoneCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 修改无人值守车辆
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "changeStoneCar")]
        ITask<DataInfo<StoneCarModel>> EditStoneCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 无人值守车辆离队
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "deleteStoneCar")]
        ITask<DataInfo<StoneCarModel>> LeaveStoneCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 无人值守车辆换队
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "convertTeam")]
        ITask<DataInfo<StoneCarModel>> ExchangeStoneCarAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 获取无人值守单据
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "getStoneOrderByTime")]
        ITask<DataInfo<List<StoneOrderModel>>> GetStoneOrderAsync([JsonContent] object param, CancellationToken token);


        /// <summary>
        /// 删除无人值守单据
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "deleteStoneOrder")]
        ITask<DataInfo<StoneOrderModel>> DeleteStoneOrderAsync([JsonContent] object param, CancellationToken token);

        /// <summary>
        /// 导出无人值守单据
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.FactoryURL + "exportOrderByTime")]
        ITask<Stream> ExportStoneOrderAsync(object param);


        /// <summary>
        /// 导出无人值守报表详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Timeout(60000)]
        [HttpGet(Config.FactoryURL + "exportOrderDetailByTime")]
        ITask<Stream> ExportStoneOrderDetailAsync(object param);


        /// <summary>
        /// 获取统计
        /// </summary>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost(Config.FactoryURL + "getOrderTotal")]
        ITask<DataInfo<StoneTotalModel>> GetStoneTotalAsync();

        #endregion
    }

}
