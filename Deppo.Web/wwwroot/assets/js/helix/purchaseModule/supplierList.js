"use strict";

// Class definition
var supplierList = function () {
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
                url: "Supplier/GetObjectsJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        searchText: $('#supplierSearchInput').val()
                    };
                }
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

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[supplier_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.ajax.reload();
        });
    }


    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#supplier_list_table');

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
    supplierList.init();
});