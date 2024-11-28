"use strict";

// Class definition
var waitingSalesOrderList = function () {
    // Shared variables
    var table;
    var datatable;
    let arrayData;

    var initDatatable = function () {


        datatable = $(table).DataTable({

            responsive: true,
            autoWidth: false,
            searchDelay: 500,
            destroy: true,
            info: false,
            order: [],
            pageLength: 10,
            serverSide: true,
            ajax: {
                url: "WaitingSalesOrder/GetObjectsJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        searchText: $('#waitingSalesOrderSearchInput').val()
                    };
                }
            },
            columns: [
                { data: 'orderNumber' },
                { data: 'productCode' },
                { data: 'orderDate' },
                { data: 'customerCode' },
                { data: 'dueDate' },
                { data: 'quantity' },
                { data: 'shippedQuantity' },
                { data: 'waitingQuantity' },
            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.orderNumber + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var defaultImageUrl = "/assets/media/images/notfound.png"; // Correct path to your default image

                        // Check if imageData is empty or null and use the default image if so
                       // var imageSrc = full.imageData ? `data:image/jpg;base64,${full.imageData}` : defaultImageUrl;

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 2%;"> <!-- Adjusted the margin-right -->
                    <img src="${defaultImageUrl}" alt="image" />
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.productCode}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.productName}</span>
                </div>
            </div>
        `;

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;

                        var parsedDate = new Date(full.orderDate);
                        var newDate = new Date(parsedDate.getFullYear(), parsedDate.getMonth(), parsedDate.getDate());
                        if (type == 'display') {

                            let formattedDate = newDate.toLocaleString('tr-TR', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            });

                            output = ` <span class="fw-bold">` + formattedDate + `</span>`
                        } else {
                            var orderableDate = parsedDate.toISOString();
                            return orderableDate;
                        }


                        return output;

                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var initials = full.customerName ? full.customerName.substring(0, 2).toUpperCase() : '';

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 10px; display: flex; align-items: center; justify-content: center; background-color: #44B0C0; border-radius: 50%; width: 40px; height: 40px;">
                    <span class="text-dark fw-bold">${initials}</span>
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.customerCode}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.customerName}</span>
                </div>
            </div>
        `;

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;

                        var parsedDate = new Date(full.dueDate);
                        var newDate = new Date(parsedDate.getFullYear(), parsedDate.getMonth(), parsedDate.getDate());
                        if (type == 'display') {

                            let formattedDate = newDate.toLocaleString('tr-TR', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            });

                            output = ` <span class="fw-bold">` + formattedDate + `</span>`
                        } else {
                            var orderableDate = parsedDate.toISOString();
                            return orderableDate;
                        }


                        return output;

                    },
                },
                {
                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-primary">` + full.quantity + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-success">` + full.shippedQuantity + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 7,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-secondary">` + full.waitingQuantity + `</span>`

                        return output;
                    },
                },
              

            ]
        });

        datatable.on('draw', function () {
            KTMenu.createInstances();
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[waiting_sales_order_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.ajax.reload();
        });
    }

    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#waiting_sales_order_list_table');

            if (!table) {
                return;
            }
            initDatatable();
            handleSearchDatatable();

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    waitingSalesOrderList.init();
});