/// <reference path="bootstrap.min.js" />
/// <reference path="highcharts.js" />

//验证
function CheckIsNull() {
    if ($('#dateStart').datebox('getValue') == "" || $('#dateEnd').datebox('getValue') == "") {
        return false;
    }
    else {
        return true;
    }
}
function CheckDate() {
    var d1 = Date.parse($('#dateStart').datebox('getValue'));
    var d2 = Date.parse($('#dateEnd').datebox('getValue'));
    if (d1 >= d2) {
        return false;
    }
    else {
        return true;
    }
}

//饼状图
var male;
var female;
function GetGenderPieChart() {
    if (CheckIsNull()) {
        if (CheckDate()) {
            $.post("/User/GetPie", { "startTime": $('#dateStart').datebox('getValue'), "endTime": $('#dateEnd').datebox('getValue') }, function (data) {
                //data = eval('(' + data + ')');
                male = data[0].GenderCount;
                female = data[1].GenderCount;
                ShowPieChart();
            });
        }
        else {
            $.messager.alert("提示", "开始时间不能大于等于结束时间");
        }
    }
    else {
        $.messager.alert("提示", "开始时间和结束时间不能为空");
    }
}
//展示饼状图
function ShowPieChart() {
    var chart;
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'container',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: '男女比例饼状图'
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b>: ' + this.percentage + ' %';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function () {
                        return '<b>' + this.point.name + '</b>: ' + this.percentage + ' %';
                    }
                }
            }
        },
        series: [{
            type: 'pie',
            name: '男女比例饼状图',
            data: [
                 ['男', male],
                 ['女', female]
            ]
        }]
    });
}

//曲线图
var aTime1;
var aCount1;
function GetGraphChart() {
    if (CheckIsNull()) {
        if (CheckDate()) {
            $.post("/User/GetGraph", { "starttime": $('#dateStart').datebox('getValue'), "endtime": $('#dateEnd').datebox('getValue') }, function (data) {
               //data = eval('(' + data + ')');
               aTime1 = data.a1;
               aCount1 = data.a2;
               ShowGraphChart();
            });
        }
        else {
            $.messager.alert("提示", "开始时间不能大于等于结束时间");
        }
    }
    else {
        $.messager.alert("提示", "开始时间和结束时间不能为空");
    }
}
//展示曲线图
function ShowGraphChart() {
    var chart;
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'container',
            type: 'line',
            marginRight: 130,
            marginBottom: 25
        },
        title: {
            text: '月度注册量',
            x: -20 //center
        },
        subtitle: {
            text: '来源: 个人网上书店',
            x: -20
        },
        xAxis: {
            categories: aTime1
        },
        yAxis: {
            title: {
                text: '注册人数'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.series.name + '</b><br/>' +
                this.x + ': ' + this.y + '人';
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -10,
            y: 100,
            borderWidth: 0
        },
        series: [{
            name: '注册人数',
            data: aCount1
        }]
    });
}

//柱状图
var aTime2;
var aCount2;
function GetColumnChart() {
    if (CheckIsNull()) {
        if (CheckDate()) {
            $.post("/User/GetGraph", { "starttime": $('#dateStart').datebox('getValue'), "endtime": $('#dateEnd').datebox('getValue') }, function (data) {
               // data = eval('(' + data + ')');
                aTime2 = data.a1;
                aCount2 = data.a2;
                ShowColumnChart();
            });
        }
        else {
            $.messager.alert("提示", "开始时间不能大于等于结束时间");
        }
    }
    else {
        $.messager.alert("提示", "开始时间和结束时间不能为空");
    }
}
function ShowColumnChart() {
    var chart;
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'container',
            type: 'column'
        },
        title: {
            text: '月度注册量'
        },
        subtitle: {
            text: '来源: 个人网上书店'
        },
        xAxis: {
            categories: aTime2
        },
        yAxis: {
            min: 0,
            title: {
                text: '注册人数'
            }
        },
        legend: {
            layout: 'vertical',
            backgroundColor: '#FFFFFF',
            align: 'left',
            verticalAlign: 'top',
            x: 100,
            y: 70,
            floating: true,
            shadow: true
        },
        tooltip: {
            formatter: function () {
                return '' +
                    this.x + ': ' + this.y + ' 人';
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [{
            name: '注册人数',
            data: aCount2

        }]
    });
}