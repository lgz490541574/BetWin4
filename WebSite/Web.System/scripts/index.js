if (!window["callback"]) window["callback"] = {};

layui.use(["jquery", "form", "table"], function () {
    var $ = layui.$,
        form = layui.form;

    // 获取全局参数化配置内容
    $.ajax({
        url: "config/init",
        method: "post",
        success: res => {

            GolbalSetting.enum = res.info.Enum;

            layui.config({
                base: './src/',
                version: _v
            }).use('index');

        },
        error: function (xhr) {
            layer.msg(xhr.responseText, {
                icon: 2,
                success: function () {
                    setTimeout(function () {
                        location.reload();
                    }, 50000);
                }
            });
        }
    });

    form.set({
        upload: {
            url: "/manage/Upload/LayUpload",
            kindeditor: "/manage/api/kindupload"
        }
    });
});

if (!window["UI"]) window["UI"] = new Object();

// 页面工具
! function (ns) {

    // 获取图片的绝对路径
    // style : xs(32) / sm(64) / md(128)
    ns.GetImage = function (img, style) {
        if (!img) return "//img.aviacdn.com/images/space.png";
        if (!/^http/.test(img)) {
            img = (GolbalSetting['setting'] && GolbalSetting['setting']['ImgServer']) + img;
        }
        if (style && /\.jpg|\.gif|\.png/.test(img)) {
            style = "/" + style;
        } else {
            style = "";
        }
        return img.replace(/\/\w{1,4}$/, "") + style;
    };

}(UI);

// 全局工具
! function (ns) {

    //　获取配置参数值，转化成为QueryString字符串
    ns["getSetting"] = function (elem, attribute, type) {
        if (!attribute) attribute = "data-setting";
        if (!type) type = "string";
        var setting = {};
        elem.querySelectorAll("[" + attribute + "]").forEach(t => {
            var name = t.getAttribute(attribute);
            switch (t.tagName + ":" + t.getAttribute("type")) {
                case "INPUT:checkbox":
                    setting[name] = t.checked ? 1 : 0;
                    break;
                default:
                    setting[name] = t.value;
                    break;
            }
        });
        if (type === "map") return setting;
        var query = [];
        for (var key in setting) query.push(key + "=" + encodeURIComponent(setting[key]));
        return query.join("&");
    };

    // 获取传递过来的参数转化成为JSON字符串
    ns["getParams"] = function (d) {
        var params = d.params || {};
        return JSON.stringify(params);
    };

    // 获取当前登录的管理员资料，并写入GolbalSetting
    ns.GetAdminInfo = function (d) {
        if (d.success) {
            GolbalSetting.AdminInfo = d.info;
        } else {
            delete GolbalSetting["AdminInfo"];
        }
    };

    // 商户工具
    ns["site"] = {
        // 打开商户管理
        "open": siteId => {
            layui.betwin.admin.open({
                action: "diag/site/index",
                area: "md",
                title: "商户管理",
                skin: "diag site",
                data: {
                    SiteID: siteId
                },
                done: function (res) {
                    let t = this,
                        checked = null,
                        container = t.container[0],
                        toolbarObj = container.querySelector(".diag-toolbar"),
                        contentObj = container.querySelector(".diag-content");

                    contentObj.id = "diag-site-" + new Date().getTime();
                    contentObj.style.height = (container.offsetHeight - toolbarObj.offsetHeight) + "px";

                    t.container.on("click", ".diag-toolbar [data-tab]", e => {
                        let link = e.target,
                            url = "diag/site/" + link.getAttribute("data-tab");
                        if (checked) checked.classList.remove("layui-this");
                        link.classList.add("layui-this");
                        checked = link;
                        layui.view(contentObj.id ).render(url, {
                            SiteID: siteId
                        });
                    });
                    container.querySelector(".diag-toolbar [data-tab]").click();
                }
            })
        }
    };


}(Utils);

if (!window["BW"]) window["BW"] = new Object();
if (!BW.callback) BW.callback = new Object();

(function (ns) {

    ns["bool"] = function (value) {
        return value ? "<label class='layui-text-green'>是</label>" : "<label class='layui-text-red'>否</label>";
    };

    //IP和IP地址的显示
    ns["IPAddress"] = function (ip, ipAddress) {
        var html = [];
        if (typeof ip === "object") {
            ipAddress = ip.IPAddress;
            ip = ip.IP;
        }
        if (ipAddress) {
            html.push("<p>" + ipAddress + "</p>");
        }
        if (ip) {
            html.push("<p class='layui-text-gray'>" + ip + "</p>");
        }
        return html.join("");
    };

    // 显示测试图标
    ns["Test"] = function (isTest) {
        if (!isTest) return "";
        return "<i class='am-icon-gavel layui-text-red' title='测试'></i>";
    };

    //操作人显示
    ns["Admin"] = function (adminId) {
        if (!GolbalSetting.manage.Admin[adminId]) {
            return "N/A";
        }
        if (GolbalSetting.manage.Admin[adminId]) {
            return GolbalSetting.manage.Admin[adminId];
        }
        if (type) {
            return GolbalSetting.admin[adminId].AdminName;
        }
        return "<p class='layui-text-gray'>" + GolbalSetting.admin[adminId].AdminName + "</p><p>" + GolbalSetting.admin[adminId].NickName + "</p>"
    }
    /// 设备类型
    ns["platform"] = platform => {
        if (!platform || !platform.length) return "";
        var icon = [];
        if (platform.contains("PC")) icon.push("am-icon-desktop");
        if (platform.contains("Windows")) icon.push("am-icon-windows");
        if (platform.contains("MAC")) icon.push("am-icon-safari");
        if (platform.contains("Mobile")) icon.push("am-icon-mobile");
        if (platform.contains("IOS")) icon.push("am-icon-apple");
        if (platform.contains("Android")) icon.push("am-icon-android");
        if (platform.contains("Wechat")) icon.push("am-icon-wechat");

        return icon.map(t => "<i class=\"" + t + "\"></i>").join(" ");
    };

    ns["site"] = (siteId, siteName) => {
        if (!siteName) siteName = siteId;
        return "<a class=\"diag-link site\" href=\"javascript:Utils.site.open(" + siteId + ");\">" + siteName + "<a>";
    }

})(htmlFunction);

// 扩展方法
! function () {

    // 绑定数据到Element对象上
    Element.prototype.bind = function (options) {
        var elemObj = this,
            admin = layui.admin;
        if (!admin) {
            console.error("应先引用admin模块");
            return;
        }
        admin.req({
            url: options.url,
            data: options.data || {},
            success: function (res) {
                var list = null;
                if (Array.isArray(res.info)) {
                    list = res.info;
                } else if (res.info.list && Array.isArray(res.info.list)) {
                    list = res.info.list;
                }
                if (!list) return;
                var html = list.map((item, index) => options.getItem(item, index));
                elemObj.innerHTML = html.join("");
                if (options.success) options.success.apply(elemObj, [res]);
            }
        });
    };

    // 设定或者移除checked
    Element.prototype.checked = function (checked) {
        var obj = this;
        if (checked === undefined) {
            checked = !obj.hasAttribute("checked");
        }
        if (checked) {
            obj.setAttribute("checked", true);
        } else {
            obj.removeAttribute("checked");
        }
        return checked;
    }

    // 判断数组中是否包含
    Array.prototype.any = function (action) {
        var arr = this;
        var isAny = false;
        arr.forEach(t => {
            if (isAny || action(t)) isAny = true;
        });
        return isAny;
    }
}();