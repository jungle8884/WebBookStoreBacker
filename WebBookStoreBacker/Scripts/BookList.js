/// <reference path="jquery.jqprint-0.3.js" />
/// <reference path="jquery-1.10.2.min.js" />
$(function () {
    GetBookList();
    $("#win").hide();//默认情况下，隐藏窗体
});
function ToDateTime(val) {
    var str1 = val.indexOf("(");
    var str2 = val.lastIndexOf(")");
    var dateValue = val.substring(str1 + 1, str2);
    var date = new Date(parseInt(dateValue));
    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
}
//获取用户信息列表
function GetBookList() {
    $("#dg").datagrid({
        url: "/Book/GetBookList",//后台页面地址
        queryParams: { sBookName: $.trim($("#selBookName").val()) },//传到后台的参数
        pagination: true,//是否允许分页
        rownumbers: true,//是否显示行号
        singleSelect: false,//是否只选择一行
        pageSize: 25,//每一页默认显示多少条数据
        checkOnSelect: false,//选中某一行的是否复选框是否可以勾上
        pageList: [5, 10, 15, 20, 25, 30],
        columns: [[
            {
                field: "BookId",
                checkbox: true
            },
            {
                field: "BookName",
                title: "图书名",
                align: "center",
                width: 100
            },
            {
                field: "BookPublication",
                title: "出版社",
                align: "center",
                width: 100
            },
            {
                field: "BookPublicTime",
                title: "出版时间",
                align: "center",
                width: 100,
                formatter: function (val, row) {
                    var str1 = val.indexOf("(")
                    var str2 = val.lastIndexOf(")");
                    var dateValue = val.substring(str1 + 1, str2);
                    var date = new Date(parseInt(dateValue));
                    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
                }
            },
            {
                field: "BookPrice",
                title: "价格",
                align: "center",
                width: 50
            },
            {
                field: "BookISBN",
                title: "ISBN",
                align: "center",
                width: 180
            },
            {
                field: "BookImage",
                title: "图片",
                align: "center",
                width: 200
            },
            {
                field: "BookAuthor",
                title: "作者",
                align: "center",
                width: 150,
            },
            {
                field: "BookTranslator",
                title: "译者",
                align: "center",
                width: 50,
            },
            {
                field: "BookIntroduce",
                title: "介绍",
                align: "center",
                width: 250,
            },
            {
                field: "BookClassfication",
                title: "分类",
                align: "center",
                width: 80,
            },
        ]]
    });
}

//查询
function Sel() {
    GetBookList();
}

//显示添加/更新窗体
var editbookid = 0;
function ShowAddEditBox(i) {
    $("#win").show();
    //添加
    if (i == 1) {
        ClearForm();
        $("#btnAdd").show();
        $("#btnEdit").hide();
        $('#win').dialog({
            title: '添加图书',
            width: 300,
            closed: false,
            cache: false,
            modal: true
        });
    }
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
            $("#txtBookPublication").val(n.BookPublication);
            $("#txtBookPublicTime").val(ToDateTime(n.BookPublicTime));
            $("#txtBookPrice").val(n.BookPrice);
            $("#txtBookISBN").val(n.BookISBN);
            $("#txtBookImage").val(n.BookImage);
            $("#txtBookAuthor").val(n.BookAuthor);
            $("#txtBookTranslator").val(n.BookTranslator);
            $("#txtBookIntroduce").val(n.BookIntroduce);
            $("#txtBookClassfication").val(n.BookClassfication);
            editbookid = n.BookId;
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
    $(".preview_box").empty();
    $("#fm").form("reset");
}
//关闭窗体
function CloseForm() {
    $(".preview_box").empty();
    $("#win").dialog("close");
}

//添加图书
function AddBook() {
    $(".preview_box").empty();
    var sBookName = $.trim($("#txtBookName").val());
    if (sBookName == "") {
        $.messager.alert('警告', '图书名不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/Book/AddBook",
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

//更新图书
function EditBook() {
    $(".preview_box").empty();
    var sBookName = $.trim($("#txtBookName").val());
    if (sBookName == "") {
        $.messager.alert('警告', '图书名不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/Book/UpdateBook/" + editbookid,
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
function DelBook() {
    var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
    if (!rows || rows.length == 0) {
        $.messager.alert("提示", "请选择要修改的数据");
        return;
    }
    var delbookids = "";
    //i是行数，n是这一行的数据对象
    $.each(rows, function (i, n) {
        //若删除多行则用_连接
        if (i == 0) {
            delbookids = n.BookId;
            return;
        }
        else {
            delbookids += "_" + n.BookId;
            return;
        }
    });
    $.messager.confirm("提示", "你确定删除吗？", function (r) {
        if (r) {
            $.post("/Book/DelBook", { "delbookids": delbookids }, function (data) {
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
    $.post("/Book/GetExcel", null, function (data) {
        $("#aexcel").attr("href", data);//data为文件路径，设置给它的超链接，点击直接下载
        $.messager.alert("提示", "生成成功");
    });
}

//打印
function Print() {
    $(".datagrid").jqprint();
}