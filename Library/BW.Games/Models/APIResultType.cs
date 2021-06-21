using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    public enum APIResultType
    {
        Success,
        [Description("用户不存在")] 
        NOUSER,
        [Description("用户名不符合规则")] 
        BADNAME,
        [Description("密码不符合规则")] 
        BADPASSWORD,
        [Description("用户名已经存在")] 
        EXISTSUSER,
        [Description("金额错误")] 
        BADMONEY,
        [Description("订单号错误")] 
        BANORDER,
        [Description("订单号已经存在")] 
        EXISTSORDER,
        [Description("订单号不存在")] 
        NOORDER,
        [Description("未指定转账动作")] 
        TRANSFER_NO_ACTION,
        [Description("IP未授权")] 
        IP,
        [Description("用户已锁定")] 
        USERLOCK,
        [Description("余额不足")] 
        NOBALANCE,
        [Description("平台额度不足")] 
        NOCREDIT,
        [Description("密钥错误")] 
        Authorization,
        [Description("错误")] 
        Faild,
        [Description("未配置域名")] 
        DOMAIN,
        [Description("内容错误")] 
        CONTENT,
        [Description("签名错误")] 
        Sign,
        [Description("不支持的操作")] 
        NOSUPPORT,
        [Description("超时请求")] 
        TIMEOUT,
        [Description("状态错误")] 
        STATUS,
        [Description("配置错误")] 
        CONFIGERROR,
        [Description("查询日期错误")] 
        DATEEROOR,
        [Description("订单号不存在")] 
        ORDER_NOTFOUND,
        [Description("错误的类型")] 
        TYPE_ERROR,
        [Description("系统维护中")] 
        MAINTENANCE,
        [Description("订单处理中")] 
        PROCCESSING,
        [Description("订单失败")]
        ORDER_FAILD,
        BUSY,
        Exception
    }
}
