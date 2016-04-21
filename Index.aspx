<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebApplication2.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Historical Hazard Data Repository</title>

    <link rel="stylesheet" href="jquery-ui.css">
    <link rel="stylesheet" type="text/css" media="screen" href="ui.jqgrid.css" />
    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <script src="jquery.jqGrid.min.js"></script>
    <script type="text/ecmascript" src="grid.locale-tw.js"></script>

        <style type="text/css">
        #prodId {
            width: 133px;
        }
        #hazardType {
            width: 140px;
        }
        #errorMsg_div {
            visibility: hidden;
        }
        #datalist_tb {
            border-collapse: collapse;
            margin: 0 auto;
            visibility: hidden;            
        }
        table, .datalisttr, th, td {
            border: 1px solid black;
            padding-left: 15px;
            padding-right: 15px;
            padding-top: 10px;
            padding-bottom: 10px;
            text-align: center;
        }
        .dataSourceLogo {
            border: 1px solid black;
        }
        .ui-datepicker { 
            z-index: 9999 !important; 
        }
        #jqGridDiv {
            margin:0px auto;
            text-align:center;
            width: 1000px;
            height: 1200px;
        }
        #jqGrid {
            visibility: hidden;
        }
        #BackDiv {
            float:left;
            width:50%;
        }
        #uploadBack {
            float:left;
            width:40%;
        }
        #searchBack {
            float: right;
            width:60%;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
    <div id ="BackDiv">
        <div id="uploadBack">
            <h2>Upload to repository</h2>
            <div id="uploadDiv">
                <asp:FileUpload ID="FileUp" runat="server" />
                <br />
                <asp:TextBox ID="uploadName" runat="server"></asp:TextBox>
                <br />
                <asp:DropDownList ID="DropDownList1" runat="server">
                    <asp:ListItem>Earthquake</asp:ListItem>
                    <asp:ListItem>Typhoon</asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />

            <asp:Button ID="upload_Btn" runat="server" OnClick="Button1_Click1" Text="Upload" />
            </div>
        </div>
        <div id="searchBack">
            <h2>Search</h2>
            <div id="datepicker_div">
                <input id="startdatepicker" type="text" />
                <input id="enddatepicker" type="text" />
            </div>
            <br/>
            <select id="hazardType" name="D1">
                <option>請選擇</option>
                <option>Earthquake</option>
                <option>Typhoon</option>
            </select><input type="button" value="Search" onclick="find();" /><br />
            <div id="jqGridDiv">
                <div>
                    <table id="jqGrid"></table>
                </div>
            </div>
            <div id="jqGridPager">
            </div>
            <div id ="datalist_div">
                <div id="errorMsg_div">ERROR</div>
                    <table id="datalist_tb">
                    <tr><td>時間</td><td>經度</td><td>緯度</td><td>深度</td><td>規模</td><td>資料連結</td></tr>
                        </table>
            </div>
        </div>
    </div>
    <script>
      
    var uri = 'api';

/*    $(document).ready(function () {
      // Send an AJAX request
      $.getJSON(uri)
          .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
              // Add a list item for the product.
              $('<li>', { text: formatItem(item) }).appendTo($('#products'));
            });
          });
    });*/

    $(document).ready(function () {
        $("#startdatepicker").datepicker({ "dateFormat": "yy-mm-dd" });
        $("#enddatepicker").datepicker({ "dateFormat": "yy-mm-dd" });
    });


    function formatItem(item) {

        var re;
        var date;
        var ds;
        var test_data;

        date = item.timeString.substr(0, 4) + "年" + item.timeString.substr(4, 2) + "月" + item.timeString.substr(6, 2) + "日 <br />" +
             item.timeString.substr(8, 2) + "時" + item.timeString.substr(10, 2) + "分";

        switch (item.dataSource) {
            case 0:
                ds = '<a href ="' + item.dataLink + '" target="_blank">' + '<img src="http://i.imgur.com/qMlE9il.png" height="64" width="64"></a>';
                break;
            case 1:
                ds = '<a href ="' + item.dataLink + '" target="_blank">' + '<img src="http://i.imgur.com/ykvvL9w.png" height="64" width="64"></a>';
                break;
            case 2:
                ds = '<a href ="' + item.dataLink + '" target="_blank">' + '<img src="http://i.imgur.com/hkf5d1u.png" height="64" width="64"></a>';
                break;
            case 3:
                ds = '<a href ="' + item.dataLink + '" target="_blank">' + '<img src="http://i.imgur.com/nTyfXKn.jpg" height="64" width="64"></a>';
                break;
            default:
                ds = item.dataSource;
        }
        return '<td>' + date + '</td><td>' + item.lng + '</td><td>' + item.lat + '</td><td>' +
            item.depth + '</td><td>' + item.magnitude + '</td><td class="dataSourceLogo">' + ds + '</td></tr>';
    }

    function timeStringFm(cellvalue, options, rowObject) {
        var date = cellvalue.substr(0, 4) + "年" + cellvalue.substr(4, 2) + "月" + cellvalue.substr(6, 2) + "日 <br />" +
                    cellvalue.substr(8, 2) + "時" + cellvalue.substr(10, 2) + "分";
        return date;
    }

    function dataLinkFm(cellvalue, options, rowObject) {

        return '<a href ="' + cellvalue + '" target="_blank">' + '<img src="http://i.imgur.com/56V6Qxb.png" height="32" width="32"></a>';
    }

    function dataSourceFm(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case 0:
                return '<img src="http://i.imgur.com/qMlE9il.png" height="64" width="64">';
            case 1:
                return '<img src="http://i.imgur.com/ykvvL9w.png" height="64" width="64">';
            case 2:
                return '<img src="http://i.imgur.com/hkf5d1u.png" height="64" width="64">';
            case 3:
                return '<img src="http://i.imgur.com/nTyfXKn.jpg" height="64" width="64">';
            default:
                return null;
        }
    }

    function find() {
      var id = $('#startdatepicker').val() + '$' + $('#enddatepicker').val();
      var type = $('#hazardType').val();
      $('.datalisttr').remove();   
      switch (type) {
          case 'Earthquake':
            $.getJSON(uri + '/' + type + '/' + id)
            .done(function (data) {

                $('#errorMsg_div').css({ "visibility": "hidden" });
                $('#jqGrid').css({ "visibility": "visible" });
                /*$.each(data, function (key, item) {
                    $('#datalist_tb').append('<tr class="datalisttr">' + formatItem(item));
                });
                $('#datalist_tb').css({ "visibility": "visible" });*/
                $('#jqGrid').jqGrid({
                    datastr: data,
                    datatype: "jsonstring",
                    colModel: [
                        { label: 'Time', name: 'timeString', width: 120,  formatter: timeStringFm },
                        { label: 'Lat', name: 'lat', width: 60 },
                        { label: 'Lng', name: 'lng', width: 60 },
                        { label: 'Depth', name: 'depth', width: 60 },
                        { label: 'Magnitude', name: 'magnitude', width: 60 },
                        { label: 'Source', name: 'dataSource', width: 60, formatter: dataSourceFm },
                        { label: 'Link', name: 'dataLink', width: 60, formatter: dataLinkFm }
                    ],
                    viewrecords: true,
                    height: 700,
                    rowNum: 300,
                    autowidth: true,
                    pager: "#jqGridPager"
                });
                jQuery('#jqGrid').jqGrid('clearGridData');
                jQuery('#jqGrid').jqGrid('setGridParam', { data: data });
                jQuery('#jqGrid').trigger('reloadGrid');
            })
            .fail(function (jqXHR, textStatus, err) {
                $('#datalist_tb').css({ "visibility": "hidden" });
                $('#errorMsg_div').css({ "visibility": "visible" }).html("ERROR");
            });
            break;
          case 'Typhoon':
            $('#datalist_tb').css({ "visibility": "hidden" });
            $('#errorMsg_div').css({ "visibility": "visible" }).html("TYPHOON");
            break;
        case 3:
            break;
        default:
            break;
      }
    }
    </script>
    </form>
</body>

</html>