"use strict";

// Class definition
var ShowApprovedSupplierModalPageInit = function () {

    var table;
    var datatable;
    var id;
    var productCode;
    var productName;
    var productImage;

    var initDatatable = function () {

        var postUrl = '../Product/GetApprovedSuppliers/?productReferenceId=' + id;
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
                { data: 'code' },
                { data: 'email' },
                { data: 'telephone' },
                { data: 'city' },
                { data: 'country' },


            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var initials = full.title ? full.title.substring(0, 2).toUpperCase() : '';

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 10px; display: flex; align-items: center; justify-content: center; background-color: #44B0C0; border-radius: 50%; width: 40px; height: 40px;">
                    <span class="text-dark fw-bold">${initials}</span>
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="../Supplier/Detail/?referenceId=${full.referenceId}" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.code}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.title}</span>
                </div>
            </div>
        `;

                        return output;
                    },
                },



                {
                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.email + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.telephone + `</span>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.city + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        console.log(full)
                        var output;
                        output = `<span class="fw-bold">` + full.country + `</span>`

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
        $('#product_approved_supplier_modal').on('shown.bs.modal', function () {
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#ProductApprovedSupplier', function () {
            id = $(this).data('product-reference-id');
            productCode = $(this).data('product-code');
            productName = $(this).data('product-name');
            productImage = $(this).data('product-image');

            var defaultImageUrl = "/assets/media/images/notfound.png";
            var imageSrc = productImage ? `data:image/jpg;base64,${productImage}` : defaultImageUrl;
            //<div><img src="${imageSrc}" alt="Product Image" style="max-width:50px; max-height:50px;"></div>

            $('#productApprovedSupplierInformation').html(`
            <div><span class="fw-bold"></span> ${productCode}</div>
            <div><span class="fw-bold"></span> ${productName}</div>
        `);
        });
    };


    // Public methods
    return {
        init: function () {
            table = document.querySelector('#product_approved_supplier_list_table');

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
    ShowApprovedSupplierModalPageInit.init();
});