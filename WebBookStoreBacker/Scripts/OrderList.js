/// <reference path="jquery.jqprint-0.3.js" />
/// <reference path="jquery-1.10.2.min.js" />

$(function () {
    GetOrderList();
    $("#win").hide();//默认情况下，隐藏窗体
});

//获取订单信息列表
function GetOrderList() {
    $("#dg").datagrid({
        url: "/Order/GetOrderList",//后台页面地址,请求远程地址---在MVC里实际是控制器里的一个方法
        queryParams: { selKeyWords: $.trim($("#selKeyWords").val()) },//传到后台的参数
        pagination: true,//是否允许分页
        rownumbers: true,//是否显示行号
        singleSelect: false,//是否只选择一行
        pageSize: 25,//每一页默认显示多少条数据
        checkOnSelect: false,//选中某一行的时候复选框是否可以勾上
        selectOnCheck: true,//单击复选框选择行
        pageList: [5, 10, 15, 20, 25, 30],
        columns: [[
            {
                field: "BookOrderId",
                checkbox: true//显示复选框
            },
            {
                field: "BookName",
                title: "图书名",
                align: "center",
                width: 200
            },
            {
                field: "UserName",
                title: "用户名",
                align: "center",
                width: 100
            },
            {
                field: "BookAmount",
                title: "购买数目",
                align: "center",
                width: 100
            },
            {
                field: "BookPrice",
                title: "单价",
                align: "center",
                width: 50
            },
            {
                field: "BookTotalPrice",
                title: "图书总价",
                align: "center",
                width: 100
            },
            {
                field: "OrderNumber",
                title: "订单号",
                align: "center",
                width: 180
            }
        ]]
    });
}

//查询
function Sel() {
    GetOrderList();
}

//显示添加/更新窗体
var editbookorderid = 0;
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
            $("#txtBookName").val(n.BookName);
            $("#txtUserName").val(n.UserName);
            $("#txtBookAmount").val(n.BookAmount);
            $("#txtBookPrice").val(n.BookPrice);
            $("#txtBookTotalPrice").val(n.BookTotalPrice);
            $("#txtOrderNumber").val(n.OrderNumber);
            editbookorderid = n.BookOrderId;
        });
        $('#win').dialog({
            title: '更新图书',
            width: 300,
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

//更新订单信息
function EditOrder() {
    var sBookName = $.trim($("#txtBookName").val());
    if (sBookName == "") {
        $.messager.alert('警告', '图书名不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/Order/UpdateOrder/" + editbookorderid,
            success: function (data) {
                var data = eval('(' + data + ')');//把json格式转成js数组
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

//删除用户（支持批量删除）
function DelOrder() {
    var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
    if (!rows || rows.length == 0) {
        $.messager.alert("提示", "请选择要删除的数据");
        return;
    }
    var delbookorderids = "";
    //i是行数，n是这一行的数据对象
    $.each(rows, function (i, n) {
        //若删除多行则用_连接
        if (i == 0) {
            delbookorderids = n.BookOrderId;
            return;
        }
        else {
            delbookorderids += "_" + n.BookOrderId;
            return;
        }
    });
    $.messager.confirm("提示", "你确定删除吗？", function (r) {
        if (r) {
            $.post("/Order/DelOrder", { "delbookorderids": delbookorderids }, function (data) {
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
    $.post("/Order/GetExcel", null, function (data) {
        $("#aexcel").attr("href", data);//data为文件路径，设置给它的超链接，点击直接下载
        $.messager.alert("提示", "生成成功");
    });
}

//打印
function Print() {
    $(".datagrid").jqprint();
}