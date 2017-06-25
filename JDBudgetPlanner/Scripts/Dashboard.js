(function () {

    var Dashboard = function () { }

    Dashboard.prototype = {

            LoadTransactionsPieChart: function (response) {
                var chart = new CanvasJS.Chart("pieChart", {
                    theme: "theme3",
                    backgroundColor: null,
                    data: [
                    {
                        type: "pie",
                        indexLabelFontFamily: "Roboto",
                        indexLabelFontSize: 16,
                        dataPoints: $.parseJSON(response)
                    }
                    ]
                });
                chart.render();
            },

            LoadBudgetVsTransactionsChart: function (response) {
                var object = $.parseJSON(response);
                var chart = new CanvasJS.Chart("comparisonChart",
                    {
                        theme: "theme3",
                        backgroundColor: null,
                        animationEnabled: true,
                        toolTip: {
                            shared: true
                        },
                        axisY: {
                            title: "Dollars ($)"
                        },
                        axisX: {
                            labelFontFamily: "Roboto",
                            labelFontSize: 16
                        },
                        data: [
                        {
                            type: "column",
                            name: "My Budget",
                            legendText: "My Budget",
                            showInLegend: true,
                            dataPoints:
                            object.budgetData
                        },
                        {
                            type: "column",
                            name: "Transactions",
                            legendText: "Transactions",
                            showInLegend: true,
                            dataPoints:
                            object.transactionData
                        }

                        ],
                        legend: {
                            fontFamily: "Roboto",
                            fontSize: 16,
                            cursor: "pointer",
                            itemclick: function (e) {
                                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                                    e.dataSeries.visible = false;
                                }
                                else {
                                    e.dataSeries.visible = true;
                                }
                                chart.render();
                            }
                        }
                    });
                chart.render();
            },

            SetDropDownSelected: function (chartElement, month, year) {
                var monthDropdown = $(chartElement + " .monthDropdown");
                var yearDropdown = $(chartElement + " .yearDropdown");
                monthDropdown.val(month).attr("selected");
                yearDropdown.val(year).attr("selected");
            },

            EnableCharts:

            function () {
                var that = this;
                $("#displayTransactions").on("click", function () {
                    var month = $("#transactionsByMonth .monthDropdown").val();
                    var year = $("#transactionsByMonth .yearDropdown").val();
                    $('#pieChart').load('/Home/_TransactionsByMonth/' + month + '/' + year, function (response) {
                        that.LoadTransactionsPieChart(response);
                    });
                });

                $("#displayBudgetVsTransactions").on("click", function () {
                    var month = $("#budgetVsTransactions .monthDropdown").val();
                    var year = $("#budgetVsTransactions .yearDropdown").val();
                    $('#comparisonChart').load('/Home/_BudgetVsTransactions/' + month + '/' + year, function (response) {
                        that.LoadBudgetVsTransactionsChart(response);
                    });
                });
            }
        }

    window.Dashboard = Dashboard;
})();