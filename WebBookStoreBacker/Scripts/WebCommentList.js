/// <reference path="jquery.jqprint-0.3.js" />
/// <reference path="jquery-1.10.2.min.js" />

window.onload = function () {
    GetWebCommentList();
    $("#win").hide();//默认情况下，隐藏窗体
}
//$(function () {
//    GetWebCommentList();
//    $("#win").hide();//默认情况下，隐藏窗体
//});

function ToDateTime(val) {
    var str1 = val.indexOf("(");
    var str2 = val.lastIndexOf(")");
    var dateValue = val.substring(str1 + 1, str2);
    var date = new Date(parseInt(dateValue));
    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
}

//获取订单信息列表
function GetWebCommentList() {
    $("#dg").datagrid({
        url: "/WebComment/GetWebCommentList",//后台页面地址,请求远程地址---在MVC里实际是控制器里的一个方法
        queryParams: { iUserId: $.trim($("#selUserId").val()) },//传到后台的参数
        pagination: true,//是否允许分页
        rownumbers: true,//是否显示行号
        singleSelect: false,//是否只选择一行
        pageSize: 25,//每一页默认显示多少条数据
        checkOnSelect: false,//选中某一行的时候复选框是否可以勾上
        selectOnCheck: true,//单击复选框选择行
        pageList: [5, 10, 15, 20, 25, 30],
        columns: [[
            {
                field: "WebCommentId",
                checkbox: true//显示复选框
            },
            {
                field: "UserId",
                title: "用户Id",
                align: "center",
                width: 50
            },
            {
                field: "CommentTitle",
                title: "评论标题",
                align: "center",
                width: 100
            },
            {
                field: "CommentText",
                title: "评论内容",
                align: "center",
                width: 500,
                formatter: function (val) {
                    var sval = "<xmp>" + val + "</xmp>";
                    return sval;
                }
            },
            {
                field: "CreatedTime",
                title: "创建时间",
                align: "center",
                width: 100,
                formatter: function (val, row) {
                    var str1 = val.indexOf("(")
                    var str2 = val.lastIndexOf(")");
                    var dateValue = val.substring(str1 + 1, str2);
                    var date = new Date(parseInt(dateValue));
                    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + " " + date.getHours() + ":" + date.getMinutes();
                }
            },
            {
                field: "ClientIP",
                title: "IP地址",
                align: "center",
                width: 200
            }
        ]]
    });
}

//查询
function Sel() {
    GetWebCommentList();
}

//显示添加/更新窗体
var editwebcommentid = 0;
function ShowAddEditBox(i) {
    $("#win").show();
    //更新
    if (i == 2) {
        $("#btnAdd").hide();
        $("#btnEdit").show();
        var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
        if (!rows || rows.length == 0) {
            $.messager.alert("提示", "请选择要修改的数据");
            return;
        }
        if (rows.length > 1) {
            $.messager.alert("提示", "只能够选择一行数据进行编辑");
            return;
        }
        $.each(rows, function (i, n) {
            //n就表示选中行的数据对象
            $("#txtUserId").val(n.UserId);
            $("#txtCommentTitle").val(n.CommentTitle);
            $("#txtCommentText").val(n.CommentText);
            $("#txtCreatedTime").val(ToDateTime(n.CreatedTime));
            $("#txtClientIP").val(n.ClientIP);
            editwebcommentid = n.WebCommentId;
        });
        $('#win').dialog({
            title: '更新网站留言',
            width: 400,
            height: 250,
            closed: false,
            cache: false,
            modal: true
        });
    }
}

//重置表单
function ClearForm() {
    $("#fm").form("reset");
}
//关闭窗体
function CloseForm() {
    $("#win").dialog("close");
}

//更新用户
function EditWebComment() {
    var UserId = $.trim($("#txtUserId").val());
    if (UserId == "") {
        $.messager.alert('警告', '用户ID不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/WebComment/UpdateWebComment/" + editwebcommentid,
            success: function (data) {
                var data = eval('(' + data + ')');//把json格式转成js对象
                if (data.success) {
                    window.location.reload();
                }
                else {
                    $.messager.alert("提示", data.infor);
                }
            }
        }
        )
    };
}

//删除留言
function DelWebComment() {
    var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
    if (!rows || rows.length == 0) {
        $.messager.alert("提示", "请选择要修改的数据");
        return;
    }
    var delwebcommentids = "";
    //i是行数，n是这一行的数据对象
    $.each(rows, function (i, n) {
        //若删除多行则用_连接
        if (i == 0) {
            delwebcommentids = n.WebCommentId;
            return;
        }
        else {
            delwebcommentids += "_" + n.WebCommentId;
            return;
        }
    });
    $.messager.confirm("提示", "你确定删除吗？", function (r) {
        if (r) {
            $.post("/WebComment/DelWebComment", { "delwebcommentids": delwebcommentids }, function (data) {
                if (data.success) {
                    window.location.reload();
                }
                else {
                    $.messager.alert("提示", data.infor);
                }
            });
        }
    });
}

//数据导出Excel
function ExportData() {
    $.post("/WebComment/GetExcel", null, function (data) {
        $("#aexcel").attr("href", data);//data为文件路径，设置给它的超链接，点击直接下载
        $.messager.alert("提示", "生成成功");
    });
}

//打印
function Print() {
    $(".datagrid").jqprint();
}