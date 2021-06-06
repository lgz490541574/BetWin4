/**

 @Name：layuiAdmin 公共业务
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License：LPPL
    
 */
layui.define(function (exports) {
    var $ = layui.$,
        layer = layui.layer,
        laytpl = layui.laytpl,
        setter = layui.setter,
        view = layui.view,
        form = layui.form,
        admin = layui.admin;

    // 退出
    admin.events.logout = function () {
        //执行退出接口
        admin.req({
            url: '/manage/account/Logout',
            success: function (res) {
                if (res.success) {
                    //清空本地记录的 token，并跳转到登入页
                    admin.exit();
                }
            }
        });
    };

    // 修改头像
    admin.events.avatar = function () {
        var avatar = function () {
            SP.Avatar({
                url: form.config.upload.url,
                callback: function (res) {
                    if (res.code !== 0) {
                        layer.msg(res.msg, {
                            icon: 2
                        });
                        return;
                    }
                    layer.msg("头像修改成功", {
                        icon: 1
                    });
                    document.getElementById("layout-face").src = res.data.src;
                    admin.req({
                        url: "/manage/account/SaveFace",
                        data: res.data
                    });
                }
            });
        };

        if (window["SP"] && SP.Avatar) {
            avatar();
        } else {
            $.ajaxSetup({
                cache: true
            });
            $.getScript("//studio.racdn.com/js/sp/avatar.js", avatar);
        }
    };


    //对外暴露的接口
    exports('common', {});
});