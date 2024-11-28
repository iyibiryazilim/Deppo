"use strict";

// Class definition
var ShowWaitingsalesOrderModalPageInit = function () {

    var table;
    var datatable;
    var id;
    var productCode;
    var productName;
    var productImage;

    var initDatatable = function () {

        var postUrl = '../Product/GetWaitingSalesOrders/?productReferenceId=' + id;
        console.log(postUrl);

        datatable = $(table).DataTable({
            responsive: true,
            autoWidth: false,
            searchDelay: 500,
            destroy: true,
            info: false,
            order: [],
            pageLength: 10,
            ajax: {
                url: postUrl,
                type: 'POST'
            },
            columns: [
                { data: 'orderNumber' },
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
                    targets: 2,
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
                    targets: 3,
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
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-primary">` + full.quantity + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-success">` + full.shippedQuantity + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="badge badge-light-secondary">` + full.waitingQuantity + `</span>`

                        return output;
                    },
                },


            ]
        });

        // Re-init functions on datatable re-draws
        datatable.on('draw', function () {
            KTMenu.createInstances();
        });
    }
    // Private functions

    var loadModalPage = function () {
        $('#product_waiting_sales_order_modal').on('shown.bs.modal', function () {
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#ProductWaitingSalesOrder', function () {
            id = $(this).data('product-reference-id');
            productCode = $(this).data('product-code');
            productName = $(this).data('product-name');
            productImage = $(this).data('product-image');

            var defaultImageUrl = "/assets/media/images/notfound.png";
            var imageSrc = productImage ? `data:image/jpg;base64,${productImage}` : defaultImageUrl;
            //<div><img src="${imageSrc}" alt="Product Image" style="max-width:50px; max-height:50px;"></div>

            $('#productSalesInformation').html(`
            <div><span class="fw-bold"></span> ${productCode}</div>
            <div><span class="fw-bold"></span> ${productName}</div>
        `);
        });
    };


    // Public methods
    return {
        init: function () {
            table = document.querySelector('#product_waiting_sales_order_list_table');

            if (!table) {
                return;
            }
            bindEventHandlers();
            loadModalPage();


        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    ShowWaitingsalesOrderModalPageInit.init();
});