"use strict";

// Class definition
var quicklyProductionBOMList = function () {
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
                url: "QuicklyProductionBOM/GetObjectsJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        searchText: $('#quicklyProductionSearchInput').val()
                    };
                }
            },
            columns: [
                { data: 'code' },
                { data: 'amount' },
                { data: 'groupCode' },
                { data: 'brandName' },
                { data: 'warehouseName' },
                { data: 'isVariant' },
                { data: 'trackingType' },

            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var defaultImageUrl = "/assets/media/images/notfound.png"; // Correct path to your default image

                        // Check if imageData is empty or null and use the default image if so
                        /*var imageSrc = full.imageData ? `data:image/jpg;base64,${full.imageData}` : defaultImageUrl;*/

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 2%;"> <!-- Adjusted the margin-right -->
                    <img src="${defaultImageUrl}" alt="image" />
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.code}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.name}</span>
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

                        output = `
                        <div>
                            <span class="fw-bold">${full.stockQuantity.toLocaleString('tr-TR')}</span>
                            <span class="text-muted ms-2">(${full.subUnitsetCode})</span>
                        </div>
                            `;

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 2,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.groupCode + `</span>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 3,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.brandName + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.warehouseName + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 5,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<input type="checkbox" ${full.isVariant ? 'checked' : ''} onclick="return false;" tabindex="-1" />`;
                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<input type="checkbox" ${full.locTracking ? 'checked' : ''} onclick="return false;" tabindex="-1" />`;
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
        const filterSearch = document.querySelector('[quickly_production_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.ajax.reload();
        });
    }

    return {
        init: async function () {
            table = document.querySelector('#quickly_production_list_table');

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
    quicklyProductionBOMList.init();
});