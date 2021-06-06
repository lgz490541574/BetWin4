/**

 @Name：全局配置
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License：LPPL（layui付费产品协议）
    
 */

layui.define(['laytpl', 'layer', 'element', 'util'], function (exports) {
    exports('setter', {
        container: 'LAY_app' //容器ID
        , base: layui.cache.base //记录layuiAdmin文件夹所在路径
        , views: layui.cache.base + 'views/' //视图所在目录
        , entry: 'index' //默认视图文件名
        , engine: '.html' //视图文件后缀名
        , pageTabs: true //是否开启页面选项卡功能。单页面专业版不推荐开启

        , name: 'ES 3.0'
        , tableName: 'layuiAdmin' //本地存储表名
        , MOD_NAME: 'admin' //模块事件名

        , debug: true //是否开启调试模式。如开启，接口异常时会抛出异常 URL 等信息

        , interceptor: false //是否开启未登入拦截

        //自定义请求字段
        , request: {
            tokenName: "Token" //自动携带 token 的字段名。可设置 false 不携带。
        }

        //自定义响应字段
        , response: {
            statusName: 'success' //数据状态的字段名称
            , statusCode: {
                ok: 1 //数据状态一切正常的状态码
                , faild: 0
                , logout: function (res) {
                    return !res.success && res.info && (res.info.ErrorType === "Login" || res.info.ErrorType === "Authorization");
                }//登录状态失效的状态码
            }
            , msgName: 'msg' //状态信息的字段名称
            , dataName: 'info' //数据详情的字段名称
        }

        //独立页面路由，可随意添加（无需写参数）
        , indPage: [
            '/user/login' //登入页
            ,'/market/esport/trader'    // 电竞操盘页面
            ,'/diag/anchor/bet'
            ,'/anchor/trade'//主播操盘页面
        ]

        //扩展的第三方模块
        , extend: [
            'echarts', //echarts 核心包
            'echartsTheme' //echarts 主题
        ]

        //主题配置
        , theme: {
            //配色方案，如果用户未设置主题，第一个将作为默认
            color: [{
                main: '#20222A' //主题色
                , selected: '#009688' //选中色
                , alias: 'default' //默认别名
            }]
        }
    });
});
