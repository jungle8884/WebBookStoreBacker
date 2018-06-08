/// <reference path="jquery-1.10.2.min.js" />

$(function () {
    $(".easyui-tree").tree({
        //当去点击easyui-tree里面的节点的时候
        onClick: function (node) {
            //参数为标签的title和id所保存的信息
            AddTab(node.text, node.id);
        }
    });
});
//添加选项卡
function AddTab(title, url) {
    var content = createFrame(url);
    if ($('#tabs').tabs('exists', title)) {
        $('#tabs').tabs('select', title);
    }
    else {
        $("#tabs").tabs("add", {
            title: title,
            content: content,
            closable: true
        });
    }
}
//创建iframe
function createFrame(url) {
    var tabHeight = $("#tabs").height() - 35;
    var s = '<iframe scrolling="auto" frameborder="0" src="' + url + '" style="width:100%;height:' + tabHeight + 'px;"></iframe>';
    return s;
}

//退出
function Exit() {
    $.cookie('username', null);
    location.href = "/User/Index";
}