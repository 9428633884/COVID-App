<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="COVIDApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/DataTables-1.10.4/css/jquery.dataTables.css" rel="stylesheet" />
    <script src="Scripts/DataTables-1.10.4/jquery.dataTables.js"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            $.ajaxSetup({
                cache: false
            });

            function showDetails() {
                    alert("Coming Soon...");
            }

            var table = $('#tblData').DataTable({
                "filter": false,
                "paging": false,
                "orderClasses": false,
                "order": [[1, "desc"]],
                "ordering" : false,
                "info": false,
                "scrollY": "450px",
                "scrollCollapse": true,
                "bProcessing": true,
                "bServerSide": true,
                "sAjaxSource": "WebService.asmx/GetTableData",
                "fnServerData": function (sSource, aoData, fnCallback) {
                    aoData.push({ "name": "roleId", "value": "admin" });
                    $.ajax({
                        "dataType": 'json',
                        "contentType": "application/json; charset=utf-8",
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success": function (msg) {
                            var json = jQuery.parseJSON(msg.d);
                            fnCallback(json);
                            $("#tblData").show();
                        },
                        error: function (xhr, textStatus, error) {
                            if (typeof console == "object") {
                                console.log(xhr.status + "," + xhr.responseText + "," + textStatus + "," + error);
                            }
                        }
                    });
                },
                fnDrawCallback: function () {
                    $('.image-details').bind("click", showDetails);
                }
            });
        })
    </script>

    <div style="font-size:35px; color:#3a3434; margin-top:10px; text-align:center; height: 63px;">COVID-19 Coronavirus Pandemic</div>

    <div class="row">
        <table id="tblData" class="hover">  
           <thead>
              <tr class="gridStyle">
                 <th>Country</th>
                 <th>Total Cases</th>
                 <th>New Cases</th>
                 <th>Total Deaths</th>
                 <th>New Deaths</th>
                 <th>Total Recovered</th>
                 <th>Active Cases</th>
                 <th>View Details</th>
              </tr>
           </thead>
           <tbody></tbody>
        </table>  
    </div>

    <div style="font-size:15px; color:#3a3434; margin-top:10px; text-align:left; height: 33px;">Developed by Viral Shah. Source of data<span style="color: rgba(0, 0, 0, 0.87); font-family: Helvetica, Arial, sans-serif; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none">&nbsp;</span><a href="https://github.com/CSSEGISandData/COVID-19" style="-webkit-tap-highlight-color: rgba(255, 255, 255, 0); color: rgb(83, 109, 254); font-weight: 500; font-family: Helvetica, Arial, sans-serif; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px;" target="_BLANK">CSSEGISandData/COVID-19</a>.</div>

</asp:Content>
