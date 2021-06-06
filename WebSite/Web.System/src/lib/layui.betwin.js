layui.define(['form', 'table', 'admin', 'laytpl', 'laydate', 'view', "upload"], function (exports) {
    var $ = layui.jquery,
        form = layui.form,
        admin = layui.admin,
        laytpl = layui.laytpl,
        laydate = layui.laydate,
        table = layui.table,
        view = layui.view,
        upload = layui.upload;

    var betwin = {
        set: function (options) {

        }
    };

    table.exportFile = function (id, data, type) {
        var tab = $("#" + id);
        var box = tab.next().find(".layui-table-box");

        var thead = box.find("thead");
        var tbody = box.find("tbody");
        var tfoot = box.find("tfoot");

        var uri = "data:application/vnd.ms-excel;base64,";
        var output = null;
        switch (type) {
            case "xls":
                output = ["<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"",
                    " xmlns:x=\"urn:schemas-microsoft-com:office:excel\"",
                    " xmlns=\"http://www.w3.org/TR/REC-html40\">",
                    "<head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>",
                    "<x:Name>导出数据</x:Name>",
                    "<x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet>",
                    "</x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->",
                    "</head><body><table>",
                    "<thead>", thead.html(), "</thead>",
                    "<tbody>", tbody.html(), "</tbody>",
                    "<tfoot>", tfoot.html(), "</tfoot>",
                    "</table></body></html>"].join("");
                break;
            case "csv":
                var html = [];
                box.find("tr").each(function (index, tr) {
                    var line = [];
                    $(tr).find("td,th").each(function (num, item) {
                        line.push(item.innerText.replace(/[\n\r,]+/g, " "));
                    });
                    html.push(line.join(","));
                });
                output = html.join("\r\n");
                break;
        }
        var textType = {
            csv: 'text/csv',
            xls: 'application/vnd.ms-excel'
        }[type];
        var alink = document.createElement("a");
        alink.href = 'data:' + textType + ';charset=utf-8,\ufeff' + encodeURIComponent(output);
        console.log(alink.href);
        alink.download = '数据导出.' + type;
        document.body.appendChild(alink);
        alink.click();
        document.body.removeChild(alink);
    };

    // 表格事件
    betwin.table = {
        // 集成工具类
        tool: function (filter, toolFunction) {
            table.on("toolbar(" + filter + ")", function (e) {
                var obj = this;
                var event = e.event;

                var checkStatus = table.checkStatus(filter);

                var action = obj.getAttribute("data-action");
                var callback = obj.getAttribute("data-callback");
                switch (event) {
                    case "delete":// 批量删除
                        if (checkStatus.data.length === 0) {
                            layer.msg("没有选中要删除的记录", { icon: 2 });
                            return;
                        }
                        var idName = obj.getAttribute("data-id") || "ID";
                        var delList = [];
                        layui.each(checkStatus.data, function (index, item) {
                            if (item[idName]) delList.push(item[idName]);
                        });
                        var data = {};
                        data[idName] = delList.join(",");

                        var confirmIndex = layer.confirm(obj.getAttribute("data-msg") || "确认要删除选中的" + checkStatus.data.length + "条记录吗？", { icon: 5 }, function () {
                            layer.close(confirmIndex);
                            var loadIndex = layer.load(2, { time: 10 * 1000 });
                            admin.req({
                                url: action,
                                data: data,
                                success: function (res) {
                                    layer.close(loadIndex);
                                    layer.msg(res.msg, { icon: res.success ? 1 : 2 });
                                    if (res.success) table.reload(filter);
                                    if (BW.callback[callback]) BW.callback[callback](res);
                                }
                            });
                        });
                        break;
                    default:
                        if (toolFunction) toolFunction.bind(obj)(e);
                        break;
                }
            });
            table.on("tool(" + filter + ")", function (e) {
                var obj = this;
                var data = e.data;
                var action = obj.getAttribute("data-action");
                var callback = obj.getAttribute("data-callback");
                switch (e.event) {
                    // 删除
                    case "delete":
                        if (!action) return;
                        var deleteConfirmIndex = layer.confirm(obj.getAttribute("data-msg") || "确认要删除吗？", { icon: 5 }, function () {
                            layer.close(deleteConfirmIndex);
                            var loadIndex = layer.load(2, { time: 3000 });
                            admin.req({
                                url: action,
                                data: data,
                                success: function (res) {
                                    layer.close(loadIndex);
                                    layer.msg(res.msg, { icon: res.success ? 1 : 2 });
                                    if (res.success) e.del();
                                    if (BW.callback[callback]) BW.callback[callback](res);
                                }
                            });
                        });
                        break;
                    // 确认框
                    case "confirm":
                        if (!action) return;
                        var confirmIndex = layer.confirm(obj.getAttribute("data-msg") || "确认要操作吗？", { icon: 3 }, function () {
                            layer.close(confirmIndex);
                            var loadIndex = layer.load(2, { time: 3000 });
                            admin.req({
                                url: action,
                                data: data,
                                success: function (res) {
                                    layer.close(loadIndex);
                                    layer.msg(res.msg, { icon: res.success ? 1 : 2 });
                                    if (BW.callback[callback]) BW.callback[callback](res);
                                }
                            });
                        });
                        break;
                    // 弹出编辑框
                    case "edit":
                        if (!action) return;
                        admin.popup({
                            title: obj.getAttribute("data-title") || obj.getAttribute("title") || obj.innerText,
                            id: "edit-" + new Date().getTime(),
                            content: "正在加载...",
                            area: GolbalSetting.area[obj.getAttribute("data-area") || "md"],
                            skin: obj.getAttribute("data-skin"),
                            success: function (popup, index) {
                                data["popupId"] = index;
                                if (!data["data"]) {
                                    data["data"] = {};
                                    var attr = obj.attributes;
                                    for (var i = 0; i < attr.length; i++) {
                                        var key = attr[i].name;
                                        if (key.indexOf("data-") === 0) {
                                            data["data"][key.substr(5)] = obj.getAttribute(key);
                                        }
                                    }
                                }
                                view(this.id).render(action, data);
                            },
                            done: function (res) {
                                if (BW.callback[callback]) BW.callback[callback](res);
                            }
                        });
                        break;
                    // 输入框
                    case "prompt":
                        layer.prompt({
                            formType: 2,
                            value: obj.getAttribute("data-value") || "",
                            title: obj.getAttribute("data-title") || "",
                            area: [obj.getAttribute("data-width") || "320px", obj.getAttribute("data-height") || "32px"] //自定义文本域宽高
                        }, function (value, index, elem) {
                            data[obj.getAttribute("data-field") || "promptMessage"] = value;
                            var loadIndex = layer.load(2, { time: 3000 });
                            admin.req({
                                url: action,
                                data: data,
                                success: function (res) {
                                    layer.close(loadIndex);
                                    layer.close(index);
                                    layer.msg(res.msg, { icon: res.success ? 1 : 2 });
                                    if (BW.callback[callback]) BW.callback[callback](res);
                                }
                            });
                        });
                        break;
                    default:
                        if (toolFunction) toolFunction.bind(obj)(e);
                        break;
                }
            });
        },
        // 回调方法
        done: function (res) {
            //　当前表格对象
            var tab = this;
            var tbody = tab.elem.next().find("tbody");
            var tfoot = function () {
                var tfoot = tbody.next();
                if (tfoot.length === 0) {
                    tfoot = $("<tfoot>");
                    tbody.after(tfoot);
                }
                tfoot.empty();
                return tfoot;
            }();

            // 本页小计
            !function () {
                if (res.data.length === 0) return;
                var subdata = {};
                var subtotal = tab.cols[0].filter(function (item, index) {
                    if (item.subtotal) subdata[index] = true;
                    return item.subtotal;
                }).length > 0;
                if (!subtotal) return;
                var data = new Array();
                layui.each(tbody.find("tr"), function (trIndex, item) {
                    layui.each($(item).find("td"), function (tdIndex, td) {
                        if (!subdata[tdIndex]) return;
                        var value = td.innerText.replace(/,/g, "");
                        if (!data[tdIndex]) {
                            data[tdIndex] = value;
                        } else {
                            var number = value.match(/[0-9\.\-\+\,]+/g);
                            if (number !== null) {
                                var index = -1;
                                data[tdIndex] = data[tdIndex].replace(/([\d\.\+\-]+)/g, function (input) {
                                    index++;
                                    if (number.length < index) return input;
                                    var result = Math.round(Number(input), 4) + Math.round(Number(number[index]), 4);
                                    if (/\.\d{5,}/.test(result)) result = result.toFixed(4);
                                    return result;
                                });
                            }
                        }
                    });
                });
                var tr = $("<tr class='layui-table-subtotal'></tr>");
                layui.each(tab.cols[0], function (index, item) {
                    if (index === 0 && !subdata[0]) {
                        tr.append($("<td>小计</td>"));
                    } else {
                        if (item.subtotal) {
                            tr.append($("<td>" + htmlFunction.number(data[index]) + "</td>"));
                        } else {
                            tr.append($("<td></td>"));
                        }
                    }
                });
                tfoot.append(tr);
            }();

            // 合计
            !function () {
                if (res.count === 0) return;
                var totaldata = {};
                var total = tab.cols[0].filter(function (item, index) {
                    if (item.total) totaldata[index] = item.total;
                    return item.total;
                }).length > 0;

                if (!res.total || !total) return;

                var tr = $("<tr class='layui-table-subtotal'></tr>");
                layui.each(tab.cols[0], function (index, item) {
                    if (index === 0 && !totaldata[0]) {
                        tr.append($("<td>合计</td>"));
                    } else {
                        if (item.total) {
                            var td = $("<td></td>");
                            var templet = item.total;
                            if (/<div>/.test(templet)) {
                                td.html(laytpl(templet).render(res.total));
                            } else if (res.total[templet]) {
                                td.html(res.total[templet]);
                            }
                            tr.append(td);
                        } else {
                            tr.append($("<td></td>"));
                        }
                    }
                });
                tfoot.append(tr);
            }();

            // 排序方法
            !function () {
                var sort = null;
                layui.each(tab.cols[0], function (index, item) {
                    if (!sort && item.type === "sort") {
                        sort = item;
                    }
                });
                if (!sort) return;
                if (!window["Sortable"]) {
                    layer.msg("没有引用 Sortable.js", { icon: 2 });
                    return;
                }

                layui.each(tbody.find(".laytable-cell-sort"), function (index, item) {
                    item.setAttribute("draggable", true);
                    item.innerHTML = "<input type='hidden' class='DRAG-ID' value='" + item.innerText + "' />";
                });

                if (!tbody.data("dragevent")) {
                    tbody.on("dragstart", ".laytable-cell-sort", function (e) {
                        var tr = $(this).parents("tr");
                        e.originalEvent.dataTransfer.setData('tableDragIndex', tr.data("index"));
                    });
                    tbody.on("dragenter", "tr", function (e) {
                        $(this).addClass("drag-over");
                        e.preventDefault();
                    });
                    tbody.on("dragover", "tr", function (e) {
                        $(this).addClass("drag-over");
                        e.originalEvent.dataTransfer.dropEffect = "link";
                        e.preventDefault();
                    });
                    tbody.on("dragleave", "tr", function (e) {
                        $(this).removeClass("drag-over");
                        e.preventDefault();
                    });
                    tbody.on("drop", "tr", function (e) {
                        var target = $(this);
                        var source = tbody.find("tr[data-index=" + e.originalEvent.dataTransfer.getData('tableDragIndex') + "]");

                        tbody.find("tr.drag-over").removeClass("drag-over");
                        console.log(source, target);
                        tbody[0].insertBefore(source[0], target[0]);

                        var res = [];
                        layui.each(tbody.find(".DRAG-ID"), function (index, item) {
                            res.push(item.value);
                        });

                        switch (typeof sort.save) {
                            case "string":
                                var data = {};
                                data[sort.field] = res.join(",");
                                admin.req({
                                    url: sort.save,
                                    data: data
                                });
                                break;
                            case "function":
                                sort.save.call(tab, res);
                                break;
                        }
                    });
                    tbody.data("dragevent", 1);
                }
            }();
        },
        // 导入当页的excel数据
        output: function (tableId) {

        }
    };

    var _dataURLtoBlob = function (dataurl) {
        var arr = dataurl.split(','),
            mime = arr[0].match(/:(.*?);/)[1],
            bstr = atob(arr[1]),
            len = bstr.length,
            u8arr = new Uint8Array(len);
        while (len--) u8arr[len] = bstr.charCodeAt(len);
        return new Blob([u8arr], { type: mime });
    }

    var _fileUpload = function (file, options) {
        var formData = new FormData();
        formData.append('file', file);
        var xhr = new XMLHttpRequest();
        xhr.onload = function () {
            try {
                // 取得响应消息
                var result = JSON.parse(this.responseText);
                options.callback(result);
            } catch (err) {
                layer.msg(err.message, { icon: 2 });
                console.error("粘貼上傳失敗", err);
            }
        };
        xhr.open('POST', options.url || form.config.upload.url, true);
        xhr.send(formData);
    }

    var _pasteUpload = function (event, options) {
        var elem = this;

        if (!options.callback) {
            options.callback = function (res) {
                if (res.code !== 0) {
                    layer.msg(result.msg, { icon: 2 });
                    return;
                }
                elem.value = res.data.value;
                elem.style.backgroundImage = "url(" + res.data.src + ")";
            };
        }

        var items = (event.clipboardData && event.clipboardData.items) || [];
        var file = null;
        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                if (items[i].type.indexOf('image') !== -1) {
                    file = items[i].getAsFile();
                    break;
                }
            }
        }
        if (!file) {
            layer.msg("No Image", { icon: 2, time: 1000 });
            setTimeout(function () { elem.value = ""; }, 500);
            return;
        }

        // 图片格式转换成为jpeg之后再上传
        !function (blob) {
            var fr = new FileReader();
            fr.onload = function (e) {
                var dataURL = e.target.result;
                var img = new Image();
                img.onload = function () {
                    var canvas = document.createElement('canvas');
                    var ctx = canvas.getContext('2d');
                    canvas.width = img.width;
                    canvas.height = img.height;
                    ctx.drawImage(img, 0, 0);
                    var newDataURL = canvas.toDataURL("image/jpeg", 0.9);
                    var newBlob = _dataURLtoBlob(newDataURL);
                    var newFile = new window.File([newBlob], "screenshot.jpeg", { type: blob.type });
                    _fileUpload(newFile, options);
                    canvas = null;
                    img = null;
                };
                img.src = dataURL;
            };
            fr.readAsDataURL(blob);
        }(file);
    };

    // 对form的扩展方法
    betwin.form = {
        submit: function (filter, data, callback) {
            if (!callback) callback = {};
            if (!data) data = function (e) { return e; };
            form.on("submit(" + filter + ")", function (e) {
                var formObj = e.form;
                var postData = data.apply(formObj, [e.field]);
                if (postData === false) return false;
                var index = layer.load(2, { time: 3000 });
                var popId = $(formObj).parents("[times]").attr("times");
                var url = formObj.getAttribute("action");
                if (layui.setter.apiurl && !/^http|^\/\//.test(url)) {
                    url = layui.setter.apiurl + url;
                }
                $.ajax({
                    url: url,
                    data: postData,
                    method: "post",
                    headers: layui.data(layui.setter.tableName),
                    error: function (res) {
                        layer.close(index);
                        layer.alert(res.responseText, { title: "发生错误", btnAlign: 'c' });
                    },
                    success: function (result) {
                        layer.close(index);
                        if (callback.callback) {
                            callback.callback(result, postData, popId);
                            return;
                        }
                        layer.alert(result.msg, { icon: result.success ? 1 : 2 }, function (alertIndex) {
                            layer.close(alertIndex);
                            if (result.success) {
                                if (formObj.getAttribute("data-reset") !== null) formObj.reset();
                                if (callback.success) {
                                    if (callback.success(result, postData, popId) === false) return;
                                }
                                layer.closeAll();
                            } else {
                                if (callback.faild) {
                                    if (callback.faild(result, postData) === false) return;
                                }
                            }
                        });
                    }
                });
                return false;
            });
        },
        // 表单赋值
        val: function (action, filter, data, callback) {
            if (!callback) callback = {};
            var index = layer.load(2, { time: 3000 });
            admin.req({
                url: action,
                data: data,
                success: function (res) {
                    layer.close(index);
                    if (!res.success) {
                        if (callback.faild && callback.faild(res) === false) return;
                        layer.msg(res.msg, { icon: 2 });
                        return;
                    }
                    form.val(filter, res.info);
                    if (callback.success && callback.success(res) === false) return;
                }
            });
        },
        // 获取表单内容
        get: function (filter) {

        },
        // 搜索事件 formId:form对象的ID  tableId：表格的lay-filter
        search: function (formId, tableId, options) {
            if (!options) options = {};
            if (!options.submit) {
                options.submit = function (e) {
                    var params = {
                        where: e.field,
                        page: {
                            curr: 1 //重新从第 1 页开始
                        }
                    };
                    if (formObj.getAttribute("data-table-nopage")) {
                        params["page"] = false;
                    }
                    if (tableId) {
                        table.reload(tableId, params);
                    }
                    return false;
                };
            }

            var formObj = document.getElementById(formId);
            if (!formObj) return;
            if (!formObj.getAttribute("lay-filter")) formObj.setAttribute("lay-filter", formId);
            betwin.form.render(formId);
            var submit = $(formObj).find("[lay-submit]");
            form.on("submit(" + submit.attr("lay-filter") + ")", options.submit);
        },
        // 扩展的渲染方法
        render: function (filter) {
            var formElem = document.querySelector(".layui-form[lay-filter=" + filter + "]");
            var formObj = $(formElem);

            // 从search对象传递过来的默认值赋值
            !function () {
                layui.each(formObj.find("[data-search-value]"), function (index, item) {
                    var name = item.getAttribute("data-search-value");
                    if (!name || !layui.router().search[name]) return;
                    var value = layui.router().search[name];
                    switch (item.tagName) {
                        case "SELECT":
                            item.setAttribute("data-value", value);
                            break;
                        default:
                            item.value = value;
                            break;
                    }
                });
            }();

            // 配置枚举
            !function () {
                layui.each(formObj.find("[data-enum]"), function (index, item) {
                    var name = item.getAttribute("data-enum");
                    switch (item.getAttribute("data-type")) {
                        case "radio":
                            item.select(GolbalSetting.enum[name], null, null);
                            break;
                        default:
                            var option = $(item).find("option");    // 初始值（一般用于“全部”选项）
                            item.select(GolbalSetting.enum[name], null, null, option.text());
                            break;
                    }
                });
            }();

            // 自定义UI元素
            !function () {
                layui.each(formObj.find("[data-ui]"), function (index, item) {
                    var name = item.getAttribute("data-ui");
                    if (!UI[name]) return;
                    UI[name](item);
                });
            }();

            //　日期参数
            !function () {
                layui.each(formObj.find("[data-date]"), function (index, item) {
                    var type = item.getAttribute("data-date");
                    item.setAttribute("autocomplete", "off");
                    item.setAttribute("readonly", true);
                    laydate.render({
                        elem: item,
                        type: type
                    });
                    switch (item.getAttribute("data-date-value")) {
                        case "Today":
                        case "today":
                            var value = "";
                            switch (type) {
                                case "date":
                                    value = new Date().formatDate("yyyy-MM-dd");
                                    break;
                                case "datetime":
                                    value = new Date().formatDate("yyyy-MM-dd 00:00:00");
                                    break;
                            }
                            item.value = value;
                            break;
                    }
                });
            }();

            // 创建默认元素
            !function () {
                var dataSubmit = formObj.find("div[data-submit]");
                if (dataSubmit.length !== 1) return;
                var html = [];
                html.push("<button class='layui-btn" + (formObj.hasClass("layui-form-sm") ? " layui-btn-sm" : "") + "' lay-submit lay-filter='" + filter + "-submit'><i class='am-icon-search'></i> 搜索</button>");
                html.push("<button type='reset' class='layui-btn layui-btn-primary" + (formObj.hasClass("layui-form-sm") ? " layui-btn-sm" : "") + "'>重置</button>");

                dataSubmit.html(html.join(""));
            }();


            // 插入快捷选择天数的按钮
            !function () {
                var dataPeriod = formObj.find("*[data-period]");
                if (dataPeriod.length !== 1) return;

                var filter = dataPeriod.data("period");
                if (filter === "true") filter = "";
                if (filter) filter = filter.split(',');

                var dateObj = formObj.find("input[data-date]");
                if (dateObj.length !== 2) return;

                //本周的第一天
                var firstDayOfWeek = new Date().getFirstDayOfWeek().toShortDateString();

                //本月的第一天
                var firstDayOfMonth = new Date().getFirstDayOfMonth().toShortDateString();

                //上周第一天
                var lastWeek = new Date().addDays(-7).getFirstDayOfWeek();
                var lastWeekFirstDay = lastWeek.toShortDateString();
                var lastWeekEndDay = lastWeek.addDays(6).toShortDateString();

                //上月的第一天、最后一天
                var firstDayOfLastMonth = new Date().addMonths(-1).getFirstDayOfMonth().toShortDateString();
                var endDayOfLastMonth = new Date().getFirstDayOfMonth().addDays(-1).toShortDateString();

                var list = [
                    { type: "Today", startat: new Date().toShortDateString(), endat: new Date().toShortDateString(), name: "今天" },
                    { type: "Yesterday", startat: new Date().addDays(-1).toShortDateString(), endat: new Date().addDays(-1).toShortDateString(), name: "昨天" },
                    { type: "Week", startat: firstDayOfWeek, endat: new Date().toShortDateString(), name: "本周" },
                    { type: "LastWeek", startat: lastWeekFirstDay, endat: lastWeekEndDay, name: "上周" },
                    { type: "Month", startat: firstDayOfMonth, endat: new Date().toShortDateString(), name: "本月" },
                    { type: "LastMonth", startat: firstDayOfLastMonth, endat: endDayOfLastMonth, name: "上月" }
                ];

                var buttonClass = "layui-btn" + (formObj.hasClass("layui-form-sm") ? " layui-btn-sm" : "");
                layui.each(list, function (index, item) {
                    if (!filter || filter.contains(item.type)) {
                        dataPeriod.append($("<button type='button' class='" + buttonClass + "' lay-period data-startat='" + item.startat + "' data-endat='" + item.endat + "'>" + item.name + "</button>"));
                    }
                });

                $(dataPeriod).on("click", "[lay-period]", function () {
                    var startAt = $(this).data("startat");
                    var endAt = $(this).data("endat");
                    $(dateObj[0]).val(startAt);
                    $(dateObj[1]).val(endAt);
                    var btnSubmit = formObj.find("*[lay-submit]");
                    if (btnSubmit.length > 0) {
                        btnSubmit[0].click();
                        return false;
                    }
                });
            }();

            // 六位数的输入密码
            layui.each(formObj.find("input[type=password][lay-mode=number]"), function (index, item) {
                var pwdObj = $(item);
                var pwdLength = pwdObj.attr("lay-mode-number") || 6;
                if (pwdObj.next().hasClass(".layui-input-password")) return;
                var html = ["<div class='layui-input-password layui-input-password-" + pwdLength + "'>"];
                for (var i = 0; i < pwdLength; i++) {
                    html.push("<input type=\"text\" class=\"layui-input\" maxlength=\"1\" >");
                }
                html.push("</div>");
                var inputBox = $(html.join(""));
                pwdObj.before(inputBox);
                var pwdInput = inputBox.find("input");
                pwdInput.focus(function () {
                    this.value = "";
                    $(this).attr("type", "text");
                });
                pwdInput.keyup(function () {
                    if (this.value) {
                        $(this).attr("type", "password");
                        var next = $(this).next();
                        if (next.length !== 0) next[0].focus();
                    } else {
                        $(this).attr("type", "text");
                    }
                    var value = [];
                    layui.each(pwdInput, function (index, input) {
                        value.push(input.value);
                    });
                    pwdObj.val(value.join(""));
                });
            });

            // 关键词查询
            !function () {
                var query = formObj.find("input[data-type='query']");

                layui.each(query, function (index, item) {
                    var obj = $(item);
                    var container = obj.parent();
                    container.addClass("input-query");
                    var content = $("<div class='input-query-content'></div>");
                    container.append(content);
                    var url = item.getAttribute("data-query");
                    var queryStatus = null;
                    var selectedIndex = -1;
                    var lastValue = null;

                    content.on("click", "a", function () {
                        item.value = this.innerText;
                        item.blur();
                    });

                    item.addEventListener("keydown", function (e) {
                        var key = window.event ? e.keyCode : e.which;
                        if (key === 13) {
                            if (selectedIndex !== -1) {
                                var list = content.find("a.selected");
                                if (list.length) {
                                    item.value = list.text();
                                    item.blur();
                                }
                            }
                            return false;
                        }
                    });

                    item.addEventListener("keyup", function (e) {
                        var value = this.value;
                        var list = content.find("a");
                        switch (e.keyCode) {
                            case 40:
                                if (!list.length) return;
                                selectedIndex++;
                                if (selectedIndex >= list.length) selectedIndex = 0;
                                break;
                            case 38:
                                if (!list.length) return;
                                selectedIndex--;
                                if (selectedIndex < 0) selectedIndex = list.length - 1;
                                break;
                            default:
                                selectedIndex = -1;
                                break;
                        }
                        if (selectedIndex !== -1) {
                            content.find("a.selected").removeClass("selected");
                            $(list[selectedIndex]).addClass("selected");
                        }
                        if (lastValue !== value) {
                            lastValue = value;
                            if (queryStatus && queryStatus.state() === "pending") queryStatus.abort();
                            queryStatus = admin.req({
                                url: url,
                                data: { Key: value },
                                beforesend: function () {
                                    content.addClass("loading");
                                },
                                complete: function () {
                                    content.removeClass("loading");
                                },
                                success: function (res) {
                                    var html = [];
                                    layui.each(res.info, function (index, username) {
                                        html.push("<a href='javascript:'>" + username + "</a>");
                                    });
                                    content.html(html.join(""));
                                }
                            });
                        }
                    });
                });
            }();

            form.render(null, filter);
        },
        // 上传图片的方法  elem：上传元素 / preview：图片预览  / input：要上传至后端的input
        upload: function (elem, options) {
            if (typeof elem === "string") elem = document.getElementById(elem);
            if (!elem) return;

            if (!options) options = {};

            // 单独调用粘贴上传方法
            if (elem.type === "paste") {
                _pasteUpload(elem, options);
                return;
            }

            switch (elem.getAttribute("data-upload")) {
                case "paste":
                    // 粘貼上傳
                    !function () {

                        var dataURLtoBlob = function (dataurl) {
                            var arr = dataurl.split(','),
                                mime = arr[0].match(/:(.*?);/)[1],
                                bstr = atob(arr[1]),
                                len = bstr.length,
                                u8arr = new Uint8Array(len);
                            while (len--) u8arr[len] = bstr.charCodeAt(len);
                            return new Blob([u8arr], { type: mime });
                        }

                        if (!options.callback) {
                            options.callback = function (res) {
                                if (res.code !== 0) {
                                    layer.msg(result.msg, { icon: 2 });
                                    return;
                                }
                                elem.value = res.data.value;
                                elem.style.backgroundImage = "url(" + res.data.src + ")";
                            };
                        }

                        var upload = function (file) {
                            var formData = new FormData();
                            formData.append('file', file);
                            var xhr = new XMLHttpRequest();
                            xhr.onload = function () {
                                try {
                                    // 取得响应消息
                                    var result = JSON.parse(this.responseText);
                                    options.callback(result);
                                } catch (err) {
                                    layer.msg(err.message, { icon: 2 });
                                    console.error("粘貼上傳失敗", err);
                                }
                            };
                            xhr.open('POST', options.url || form.config.upload.url, true);
                            xhr.send(formData);
                        }

                        elem.addEventListener("paste", function (event) {
                            var items = (event.clipboardData && event.clipboardData.items) || [];
                            var file = null;
                            if (items && items.length) {
                                for (var i = 0; i < items.length; i++) {
                                    if (items[i].type.indexOf('image') !== -1) {
                                        file = items[i].getAsFile();
                                        break;
                                    }
                                }
                            }
                            if (!file) {
                                layer.msg("No Image", { icon: 2, time: 1000 });
                                setTimeout(function () { elem.value = ""; }, 500);
                                return;
                            }
                            // 图片格式转换成为jpeg之后再上传
                            !function (blob) {
                                var fr = new FileReader();
                                fr.onload = function (e) {
                                    var dataURL = e.target.result;
                                    var img = new Image();
                                    img.onload = function () {
                                        var canvas = document.createElement('canvas');
                                        var ctx = canvas.getContext('2d');
                                        canvas.width = img.width;
                                        canvas.height = img.height;
                                        ctx.drawImage(img, 0, 0);
                                        var newDataURL = canvas.toDataURL("image/jpeg", 0.9);
                                        var newBlob = dataURLtoBlob(newDataURL);
                                        var newFile = new window.File([newBlob], "screenshot.jpeg", { type: blob.type });
                                        upload(newFile);
                                        canvas = null;
                                        img = null;
                                    };
                                    img.src = dataURL;
                                };
                                fr.readAsDataURL(blob);
                            }(file);
                        });
                    }();
                    break;
                default:
                    // 控件上傳
                    !function () {
                        elem.setAttribute("onclick", "return false;");
                        if (!options.preview) options.preview = elem;
                        if (!options.input) options.input = elem;
                        // 可接受的文件类型 images（图片）、file（所有文件）、video（视频）、audio（音频）
                        if (!options.accept) options.accept = "images";

                        if (!options.done) {
                            options.done = function (res) {
                                if (options.preview) options.preview.setAttribute("src", res.data.src);
                                if (options.input) options.input.setAttribute("value", res.data[options.input.getAttribute("data-value") || "value"]);
                            };
                        }
                        elem = $(elem);
                        var headers = layui.data(layui.setter.tableName);
                        if (options.headers) {
                            headers = $.extend(headers, options.headers);
                        }
                        var uploadIndex;
                        var uploadInst = upload.render({
                            elem: elem //绑定元素
                            , url: options.url || elem.attr("data-url") || form.config.upload.url
                            , data: options.data
                            , accept: options.accept
                            , headers: headers
                            , exts: "jpg|png|gif|bmp|jpeg|svg"
                            , before: function (obj) {
                                elem.attr("disabled", true);
                                uploadIndex = layer.load(10, { time: 10 * 1000 });
                            }
                            , done: function (res) {
                                elem.attr("disabled", false);
                                layer.close(uploadIndex);
                                if (res.code !== 0) {
                                    layer.msg(res.msg, { icon: 2 });
                                    return;
                                }
                                options.done(res);
                            }
                            , error: function () {
                                layer.close(uploadIndex);
                                layer.msg("上传失败", { icon: 2 });
                            }
                        });
                    }();
                    break;
            }
        },
        // 调用编辑器
        kindeditor: function (elem, options) {
            if (!window["KindEditor"]) {
                layer.msg("未引用 KindEditor", { icon: 2 });
                return fail;
            }
            if (!options) options = {};
            if (!options.height) options.height = 480;
            if (!options.zIndex) options.zIndex = 20180831;
            if (!options.uploadJson) options.uploadJson = form.config.upload.kindeditor;
            var editor = KindEditor.create(elem, options);
            return editor;
        },
        // 大文件上传，带进度条
        uploadInst: function (file, options) {
            options = $.extend({
                // 分片大小
                shardSize: 512 * 1024,
                // 初始化
                init: function () { },
                // 上传进度
                progress: function (index, total) { },
                // 上传成功
                success: function (res) {
                    layer.alert(res.msg, { icon: 1 });
                },
                // 失败
                faildure: function (res) {
                    layer.msg(res.msg, { icon: 2 });
                },
                // 成功时候的附加数据
                extend: {}
            }, options);
            var data = {
                size: file.size,
                filename: file.name,
                token: Utils.NewGuid("N")
            };
            var shardCount = Math.ceil(data.size / options.shardSize);

            var upload = function (uploadIndex) {
                var start = uploadIndex * options.shardSize,
                    end = Math.min(data.size, start + options.shardSize);

                if (uploadIndex === 0) {
                    console.log(options.init);
                    if (options.init) options.init();
                }

                var form = new FormData();
                form.append("data", file.slice(start, end));  //slice方法用于切出文件的一部分
                form.append("filename", data.filename);
                form.append("total", shardCount);   //总片数
                form.append("index", uploadIndex + 1);        //当前是第几片
                form.append("token", data.token);   // 当前上传的文件token
                // 成功时候需要附加的数据
                if (uploadIndex + 1 === shardCount) {
                    layui.each(options.extend, function (key, value) {
                        if (value) form.append(key, value);
                    });
                }

                $.ajax({
                    url: options.url,
                    type: "POST",
                    data: form,
                    async: true,         //异步
                    processData: false,  //很重要，告诉jquery不要对form进行处理
                    contentType: false,  //很重要，指定为false才能形成正确的Content-Type
                    success: function (res) {
                        if (res.success) {
                            if (options.progress) options.progress(uploadIndex, shardCount);
                            if (uploadIndex + 1 === shardCount) {
                                if (options.success) options.success(res);
                            } else {
                                upload(uploadIndex + 1);
                            }
                        } else {
                            if (options.faildure) options.faildure(res);
                        }
                    }
                });
            };

            upload(0);
        }
    };

    // 扩展view方法
    betwin.view = {
        // 刷新
        refresh: function (id) {
            var router = layui.router(),
                fn = function (options) {
                    var tpl = laytpl(options.dataElem.html());
                    options.dataElem.after(tpl.render($.extend({
                        params: router.params
                    }, options.res)));

                    typeof callback === 'function' && callback();

                    try {
                        options.done && new Function('d', options.done)(options.res);
                    } catch (e) {
                        console.error(options.dataElem[0], '\n存在错误回调脚本\n\n', e)
                    }
                };
            (function () {
                var dataElem = $(id)
                    , layDone = dataElem.attr('lay-done') || dataElem.attr('lay-then') //获取回调
                    , url = laytpl(dataElem.attr('lay-url') || '').render(router) //接口 url
                    , data = laytpl(dataElem.attr('lay-data') || '').render(router) //接口参数
                    , headers = laytpl(dataElem.attr('lay-headers') || '').render(router); //接口请求的头信息

                try {
                    data = new Function('return ' + data + ';')();
                } catch (e) {
                    hint.error('lay-data: ' + e.message);
                    data = {};
                };

                try {
                    headers = new Function('return ' + headers + ';')();
                } catch (e) {
                    hint.error('lay-headers: ' + e.message);
                    headers = headers || {}
                };

                if (url) {
                    var urlIndex = layer.load(2, { time: 3000 });
                    view.req({
                        type: dataElem.attr('lay-type') || 'get'
                        , url: url
                        , data: data
                        , dataType: 'json'
                        , headers: headers
                        , beforeSend: function () {
                        }
                        , success: function (res) {
                            layer.close(urlIndex);
                            dataElem.next().remove();
                            fn({
                                dataElem: dataElem
                                , res: res
                                , done: layDone
                            });
                        }
                    });
                } else {
                    fn({
                        dataElem: dataElem
                        , done: layDone
                    });
                }
            }());
        }
    };

    betwin.admin = {
        popup: function (id) {
            var page = $(id);
            page.on("click", "[data-popup]", function (e) {
                var obj = this;
                var url = obj.getAttribute("data-popup");
                var data = obj.getAttribute("data-data") === null ? {} : JSON.parse(decodeURI(obj.getAttribute("data-data")));
                var title = obj.getAttribute("data-title");
                admin.popup({
                    id: "popup-" + new Date().getTime(),
                    title: title === "false" ? false : title || obj.innerText,
                    area: GolbalSetting.area[obj.getAttribute("data-area") || "md"],
                    skin: obj.getAttribute("data-skin"),
                    content: "正在加载...",
                    success: function (obj, index) {
                        data["popupId"] = index;
                        view(this.id).render(url, data);
                    }
                });
            });
            page.on("click", "[data-action]", function (e) {
                var obj = this;
                var url = obj.getAttribute("data-action");
                var data = obj.getAttribute("data-data") === null ? {} : JSON.parse(decodeURI(obj.getAttribute("data-data")));
                var callback = obj.getAttribute("data-callback");
                betwin.admin.req({
                    url: url,
                    data: data,
                    success: function (res) {
                        if (callback) {
                            if (BW.callback[callback]) {
                                BW.callback[callback].apply(obj, [res]);
                            } else {
                                var fun = new Function(callback);
                                try {
                                    fun();
                                } catch (ex) {
                                    console.error(ex, "找不到回调方法" + callback);
                                }
                            }
                        }
                    }
                });
            });
        },
        open: function (obj) {
            var options = {
                "data": {},
                "title": "修改",
                "area": "md"
            };
            if (obj instanceof HTMLElement) {
                options = $.extend(options, {
                    data: obj.getAttribute("data-data") === null ? {} : JSON.parse(obj.getAttribute("data-data")),
                    action: obj.getAttribute("data-action"),
                    title: obj.getAttribute("data-title"),
                    area: obj.getAttribute("data-area")
                });
            } else {
                options = $.extend(options, obj);
            }
            if (!options.action) return;
            admin.popup({
                id: "admin-open-" + new Date().getTime(),
                title: options.title,
                content: "正在加载...",
                area: GolbalSetting.area[options.area],
                success: function () {
                    view(this.id).render(options.action, options.data);
                }
            });
        },
        req: function (options) {
            options = $.extend({
                "loading": true,
                "time": 10000,
                "tip": true,
                "layer": {
                    offset: 'auto'
                }
            }, options);
            var loadIndex = 0;
            if (options["loading"]) {
                if (typeof options["loading"] === "function") {
                    options["loading"]();
                } else {
                    loadIndex = layer.load(2, { time: options.time });
                }
            }
            admin.req({
                url: options.url,
                data: options.data,
                complete: function () {
                    if (loadIndex) { layer.close(loadIndex); }
                    if (options.complete) options.complete();
                },
                success: function (res) {
                    if (options["tip"] && (!res.success || !/^\d+ms$/.test(res.msg))) {
                        layer.msg(res.msg, $.extend(options.layer, { icon: res.success ? 1 : 2 }));
                    }
                    if (res.success && options.success) {
                        options.success(res);
                    } else if (!res.success && options.error) {
                        options.error(res);
                    }
                }
            });
        }
    };

    // 表单提交事件
    betwin.submit = betwin.form.submit;

    //接口输出
    exports('betwin', betwin);
});