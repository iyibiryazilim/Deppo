"use strict";

// Class definition
var reasonForRjectionTableList = function () {
    // Shared variables
    var table;
    var datatable;
    let arrayData;


    var getData = async function () {

        const getItem = async () => {
            var postUrl =
                "ReasonsForRejectionProcurement/GetObjectsJsonResult";

            const data = await fetch(postUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            arrayData = await data.json();
            initDatatable();
            console.log(arrayData);

        };
        await getItem();
    };
    // Private functions
    var initDatatable = function () {


        datatable = $(table).DataTable({

            responsive: true,
            autoWidth: false,
            searchDelay: 500,
            destroy: true,
            info: false,
            order: [],
            pageLength: 10,
            "bProcessing": true,
            oLanguage: {
                sLoadingRecords: '<img src="assets/media/avatars/Refresh.gif">'
            },
            data: arrayData.data,
            columns: [
                { data: 'code' },
                { data: 'name' },
                { data: 'isActive' },

            ],

            columnDefs: [
             
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start',
                    render: function (data, type, full, meta) {
                        if (full.code == null)
                            full.code = "";
                        var output;
                        output = `<span class="fw-bold ml-5">` + full.code + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 1,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        if (full.name == null)
                            full.name = "";
                        var output;
                        output = `<span class="fw-bold">` + full.name + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<input type="checkbox" ${full.isActive ? 'checked' : ''} onclick="return false;" tabindex="-1" />`;
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
        const filterSearch = document.querySelector('[reasons_for_rejection_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#reasons_for_rejection_list_table');

            if (!table) {
                return;
            }
            await getData();
            initDatatable();
            handleSearchDatatable();

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    reasonForRjectionTableList.init();
});