using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Util.Biz.Payments.Core;
using Util.Biz.Payments.Properties;
using Util.Biz.Payments.Wechatpay.Abstractions;
using Util.Biz.Payments.Wechatpay.Configs;
using Util.Biz.Payments.Wechatpay.Results;
using Util.Helpers;
using Util.Validations;

namespace Util.Biz.Payments.Wechatpay.Services {
    /// <summary>
    /// 微信支付通知服务
    /// </summary>
    public class WechatpayNotifyService : IWechatpayNotifyService {
        /// <summary>
        /// 配置提供器
        /// </summary>
        private readonly IWechatpayConfigProvider _configProvider;
        /// <summary>
        /// 微信支付结果
        /// </summary>
        private WechatpayResult _result;
        /// <summary>
        /// 是否已加载
        /// </summary>
        private bool _isLoad;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 初始化微信支付通知服务
        /// </summary>
        /// <param name="configProvider">配置提供器</param>
        /// <param name="httpContextAccessor"></param>
        public WechatpayNotifyService(IWechatpayConfigProvider configProvider,
            IHttpContextAccessor httpContextAccessor) {
            configProvider.CheckNull(nameof(configProvider));
            _configProvider = configProvider;
            _isLoad = false;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetRequestString()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            using (var sr = new StreamReader(request.Body, Encoding.UTF8))
            {
                var body = await sr.ReadToEndAsync();
                return body;
            }

        }

        /// <summary>
        /// 获取参数集合
        /// </summary>
        public async Task<IDictionary<string, string>> GetParamsAsync() {
            await Init();
            return _result.GetParams();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="name">参数名</param>
        public async Task<T>  GetParamAsync<T>(string name) {
            return Util.Helpers.Convert.To<T>(await GetParamAsync(name));
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="name">参数名</param>
        public async Task<string> GetParamAsync(string name) {
            await Init();
            return _result.GetParam(name);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private async Task Init() {
            if (_isLoad)
                return;
            await InitResult();
            _isLoad = true;
        }

        /// <summary>
        /// 初始化支付结果
        /// </summary>
        private async Task InitResult() {
            string requestString = await GetRequestString();
            _result = new WechatpayResult(_configProvider, requestString);
        }

        /// <summary>
        /// 验证
        /// </summary>
        public async Task<ValidationResultCollection> ValidateAsync() {
            await Init();
            if (Money <= 0)
                return new ValidationResultCollection(PayResource.InvalidMoney);
            return await _result.ValidateAsync();
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        public string Success() {
            return Return(WechatpayConst.Success, WechatpayConst.Ok);
        }

        /// <summary>
        /// 返回消息
        /// </summary>
        private string Return(string code, string message) {
            var xml = new Xml();
            xml.AddCDataNode(code, WechatpayConst.ReturnCode);
            xml.AddCDataNode(message, WechatpayConst.ReturnMessage);
            return xml.ToString();
        }

        /// <summary>
        /// 返回失败消息
        /// </summary>
        public string Fail() {
            return Return(WechatpayConst.Fail, WechatpayConst.Fail);
        }

        public string GetParam(string name)
        {
            return GetParamAsync(name).GetAwaiter().GetResult();
        }


        public IDictionary<string, string> GetParams()
        {
            return GetParamsAsync().GetAwaiter().GetResult();
        }

        ///// <summary>
        ///// 商户订单号
        ///// </summary>
        //public async Task<string> GetOrderId()
        //{
        //    return await GetParamAsync(WechatpayConst.OutTradeNo);
        //}

        ///// <summary>
        ///// 支付订单号
        ///// </summary>
        //public async Task<string> GetTradeNo()
        //{
        //    return await GetParamAsync(WechatpayConst.TransactionId); 
        //}


        ///// <summary>
        ///// 支付金额
        ///// </summary>
        //public async Task<decimal> GetMoney()
        //{ 
        //    return (await GetParamAsync(WechatpayConst.TotalFee)).ToDecimal() / 100M;
        //}

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OrderId => GetParamAsync(WechatpayConst.OutTradeNo).GetAwaiter().GetResult();

        /// <summary>
        /// 支付订单号
        /// </summary>
        public string TradeNo => GetParamAsync(WechatpayConst.TransactionId).GetAwaiter().GetResult();

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Money => GetParamAsync(WechatpayConst.TotalFee).GetAwaiter().GetResult().ToDecimal() / 100M;
    }
}
