"use strict";

// Class definition
var transactionAuditTableList = function () {
    // Shared variables
    var table;
    var datatable;
    let arrayData;


    var getData = async function () {

        const getItem = async () => {
            var postUrl =
                "TransactionAudit/GetObjectsJsonResult";

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
                { data: 'transactionNumber' },
                { data: 'transactionTypeName' },
                { data: 'transactionDate' },
                { data: 'warehouseName' },
                { data: 'currentCode' },
                { data: 'applicationUser.fullName' },

            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold text-muted">` + full.transactionNumber + `</span>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;

                        if (full.iOType == 1 || full.iOType == 2) {
                            output = `<i class="bi bi-arrow-up" style="color:green"></i> <span class="fw-bold">${full.transactionTypeName}</span>`;


                        } else {
                            output = `<i class="bi bi-arrow-down text-danger"></i> <span class="fw-bold">${full.transactionTypeName}</span>`;

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

                        var parsedDate = new Date(full.transactionDate);
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
                        if (full.warehouseName == null)
                            full.warehouseName = "";
                        var output;
                        output = `<span class="fw-bold">` + full.warehouseName + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;

                        if (full.currentReferenceId == 0 || full.currentReferenceId == null) {
                            output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 10px; display: flex; align-items: center; justify-content: center; border-radius: 50%; width: 40px; height: 40px;">
                    <span class="text-dark fw-bold"></span>
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.currentCode}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.currentName}</span>
                </div>
            </div>
        `;

                        } else {
                            var initials = full.currentName ? full.currentName.substring(0, 2).toUpperCase() : '';

                            output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 10px; display: flex; align-items: center; justify-content: center; background-color: #44B0C0; border-radius: 50%; width: 40px; height: 40px;">
                    <span class="text-dark fw-bold">${initials}</span>
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.currentCode}</a>
                    <span class="text-muted fw-semibold text-muted d-block fs-7">${full.currentName}</span>
                </div>
            </div>
        `;
                        }



                       

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        console.log(full.applicationUser.fullName)
                        var output;
                        output = `<span class="fw-bold">` + full.applicationUser.fullName + `</span>`

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
        const filterSearch = document.querySelector('[transaction_audit_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#transaction_audit_list_table');

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
    transactionAuditTableList.init();
});