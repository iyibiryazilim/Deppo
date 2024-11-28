"use strict";

// Class definition
var ShowLocationModalPageInit = function () {

    var table;
    var datatable;
    var id;
    var productCode;
    var productName;
    var productImage;

    var initDatatable = function () {

        var postUrl = '../Product/GetLocationTransactions/?productReferenceId=' + id;
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
                { data: 'warehouseNumber' },
                { data: 'remainingQuantity' },
            ],
            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        console.log(full)
                        var numberElement = `
            <div class="symbol symbol-40px mb-1 d-flex align-items-center justify-content-center rounded-circle text-dark" style="background-color: #44B0C0; margin-right: 2%; width: 40px; height: 40px;">
                ${full.warehouseNumber}
            </div>
        `;

                        output = `
            <div class="d-flex align-items-center">
                ${numberElement}
                <div class="d-flex justify-content-start flex-column">
                    <span class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.locationCode}</span>
                </div>
            </div>
        `;

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 1,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.remainingQuantity + `</span>`

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
        $('#product_location_modal').on('shown.bs.modal', function () {
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#ProductLocation', function () {
            id = $(this).data('product-reference-id');
            productCode = $(this).data('product-code');
            productName = $(this).data('product-name');
            productImage = $(this).data('product-image');

            var defaultImageUrl = "/assets/media/images/notfound.png";
            var imageSrc = productImage ? `data:image/jpg;base64,${productImage}` : defaultImageUrl;
            //<div><img src="${imageSrc}" alt="Product Image" style="max-width:50px; max-height:50px;"></div>

            $('#productLocationInformation').html(`
            <div><span class="fw-bold"></span> ${productCode}</div>
            <div><span class="fw-bold"></span> ${productName}</div>
        `);
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#product_location_list_table');

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
    ShowLocationModalPageInit.init();
});