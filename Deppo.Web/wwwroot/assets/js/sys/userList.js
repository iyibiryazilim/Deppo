"use strict";

// Class definition
var userTableList = function () {
    // Shared variables
    var table;
    var datatable;
    let arrayData;


    var getData = async function () {

        const getItem = async () => {
            var postUrl =
                "User/GetObjectsJsonResult";

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
                { data: 'image' },
                { data: 'eMail' },
                { data: 'address' },
                { data: 'telephone' },
                { data: 'logoUserName' },

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
                        var imageSrc = full.image ? `data:image/jpg;base64,${full.image.mediaData}` : defaultImageUrl;

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 2%;"> <!-- Adjusted the margin-right -->
                    <img src="${imageSrc}" alt="image" />
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="#" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.fullName} </a>
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
                        if (full.eMail == null)
                            full.eMail = "";
                        var output;
                        output = `<span class="fw-bold">` + full.eMail + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        if (full.address == null)
                            full.address = "";
                        var output;
                        output = `<span class="fw-bold">` + full.address + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        if (full.telephone == null)
                            full.telephone = "";
                        var output;
                        output = `<span class="fw-bold">` + full.telephone + `</span>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        if (full.logoUserName == null)
                            full.logoUserName = "";
                        var output;
                        output = `<span class="fw-bold">` + full.logoUserName + `</span>`

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
        const filterSearch = document.querySelector('[user_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#user_list_table');

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
    userTableList.init();
});