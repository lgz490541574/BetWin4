# 贝盈API接口
## 游戏支持

代码|游戏|类型
--|--|--
AGLive|AG视讯|真人视讯
AVIA|泛亚电竞|电子竞技
Pinnacle|平博|体育
IMSport|IM体育|体育
IMOne|电竞牛|电子竞技
BBIN|波音|真人视讯
XJ188|小金188|体育
KY|开元棋牌|棋牌

## API接口
### 会员注册
> 地址：/v1/account/register
#### 提交数据
```
{
	"UserName":"test",
	"Password":"test"
}
```

### 会员登录
> 地址：/v1/account/login

#### 提交数据
```
{
	"UserName":"test",
	"Game":"AGLive"
}
```

## 统一错误代码
#### 错误格式
```
{
	"Code":"错误代码",
	"Message":"错误信息"
}
```
代码|说明
--|--
NOUSER|用户不存在
BADNAME|用户名不符合规则
BADPASSWORD|密码不符合规则
EXISTSUSER|用户名已经存在
BADMONEY|金额错误
BANORDER|订单号错误
EXISTSORDER|订单号已经存在
NOORDER|订单号不存在
TRANSFER_NO_ACTION|未指定转账动作
IP|IP未授权
USERLOCK|用户已锁定
NOBALANCE|余额不足
NOCREDIT|平台额度不足
Authorization|密钥错误
Faild|通用错误
DOMAIN|未配置域名
CONTENT|内容错误
Sign|签名错误
NOSUPPORT|不支持的操作
TIMEOUT|超时请求
STATUS|状态错误
CONFIGERROR|配置错误
DATEEROOR|查询日期错误
ORDER_NOTFOUND|订单号不存在
TYPE_ERROR|错误的类型
MAINTENANCE|系统维护中
PROCCESSING|订单处理中
ORDER_FAILD|订单失败
BUSY|系统繁忙
Exception|系统异常
<!--stackedit_data:
eyJoaXN0b3J5IjpbLTE4Nzc3ODg3MTEsMTA1MDU2MTIzNF19
-->