"use strict";

// Class definition
var warehouseList = function () {
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
                url: "Warehouse/GetObjectsJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        searchText: $('#warehouseSearchInput').val()
                    };
                }
            },
            columns: [
                { data: 'number' },
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
                        console.log(full)
                        var numberElement = `
            <div class="symbol symbol-40px mb-1 d-flex align-items-center justify-content-center rounded-circle text-dark" style="background-color: #44B0C0; margin-right: 2%; width: 40px; height: 40px;">
                ${full.number}
            </div>
        `;

                        output = `
            <div class="d-flex align-items-center">
                ${numberElement}
                <div class="d-flex justify-content-start flex-column">
                    <span class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.name}</span>
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
                        output = `<span class="fw-bold">` + full.city + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        if (full.country == null)
                            full.country = "";
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

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[warehouse_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.ajax.reload();
        });
    }


    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#warehouse_list_table');

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
    warehouseList.init();
});