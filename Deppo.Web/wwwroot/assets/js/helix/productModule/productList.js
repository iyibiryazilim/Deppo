"use strict";

// Class definition
var productList = function () {
    // Shared variables
    var table;
    var datatable;
    let arrayData;


    var getData = async function () {
        document.getElementById("loadingIndicator").style.display = "block";

        const getItem = async () => {
            var postUrl =
                "Product/GetProductJsonResult";

            const data = await fetch(postUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            arrayData = await data.json();
            initDatatable();
            console.log(arrayData);
            document.getElementById("loadingIndicator").style.display = "none";

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
            serverSide: true, 
            ajax: {
                url: "Product/GetProductJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length
                    };
                }
            },
            columns: [
                { data: 'code' },
                { data: 'stockQuantity' },
                { data: 'unitsetCode' },
                { data: 'subUnitsetCode' },
                { data: 'brandName' },
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
                        var imageSrc = full.imageData ? `data:image/jpg;base64,${full.imageData}` : defaultImageUrl;

                        output = `
            <div class="d-flex align-items-center">
                <div class="symbol symbol-40px mb-1" style="margin-right: 2%;"> <!-- Adjusted the margin-right -->
                    <img src="${imageSrc}" alt="image" />
                </div>
                <div class="d-flex justify-content-start flex-column">
                    <a href="../Vehicle/Detail/?VehicleOid=${full.referenceId}" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.code}</a>
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
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.stockQuantity.toLocaleString('tr-TR') + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.unitsetCode + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.subUnitsetCode + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<span class="fw-bold">` + full.brandName + `</span>`

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

        // Re-init functions on datatable re-draws
        datatable.on('draw', function () {
            KTMenu.createInstances();
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[product_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Handle status filter dropdown
    var handleStatusFilter = () => {
        const filterStatus = document.querySelector('[data-kt-ecommerce-product-filter="status"]');
        $(filterStatus).on('change', e => {
            let value = e.target.value;
            if (value === 'all') {
                value = '';
            }
            datatable.column(1).search(value).draw();
        });
    }

    var exportButtons = () => {
        const documentTitle = 'Trafik Cezaları';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Delete cateogry
    var handleDeleteRows = () => {
        // Select all delete buttons
        const deleteButtons = table.querySelectorAll('[data-kt-ecommerce-product-filter="delete_row"]');

        deleteButtons.forEach(d => {
            // Delete button on click
            d.addEventListener('click', function (e) {
                e.preventDefault();

                // Select parent row
                const parent = e.target.closest('tr');

                // Get category name
                const productName = parent.querySelector('[data-kt-ecommerce-product-filter="product_name"]').innerText;

                // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
                Swal.fire({
                    text: "Are you sure you want to delete " + productName + "?",
                    icon: "warning",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: "Yes, delete!",
                    cancelButtonText: "No, cancel",
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    }
                }).then(function (result) {
                    if (result.value) {
                        Swal.fire({
                            text: "You have deleted " + productName + "!.",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        }).then(function () {
                            // Remove current row
                            datatable.row($(parent)).remove().draw();
                        });
                    } else if (result.dismiss === 'cancel') {
                        Swal.fire({
                            text: productName + " was not deleted.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        });
                    }
                });
            })
        });
    }


    // Public methods
    return {
        init: async function () {
            table = document.querySelector('#product_list_table');

            if (!table) {
                return;
            }
            await getData();
            initDatatable();
            exportButtons();
            handleSearchDatatable();
            //handleStatusFilter();
            //handleDeleteRows();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    productList.init();
});