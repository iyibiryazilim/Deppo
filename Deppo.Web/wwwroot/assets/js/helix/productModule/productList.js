"use strict";

// Class definition
var productList = function () {
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
                url: "Product/GetObjectsJsonResult",
                type: "POST",
                data: function (d) {
                    return {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        searchText: $('#productSearchInput').val()
                    };
                }
            },
            columns: [
                { data: 'code' },
                { data: 'stockQuantity' },
                { data: 'groupCode' },
                { data: 'brandName' },
                { data: 'isVariant' },
                { data: 'trackingType' },
                { data: 'referenceId' }


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
                    <a href="../Product/Detail/?referenceId=${full.referenceId}" class="text-dark fw-bold text-hover-primary mb-1 fs-6">${full.code}</a>
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
                            <span class="fw-bold badge badge-primary text-dark">${full.stockQuantity.toLocaleString('tr-TR')} ${full.subUnitsetCode}</span>
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
                        output = `<input type="checkbox" ${full.isVariant ? 'checked' : ''} onclick="return false;" tabindex="-1" />`;
                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 5,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<input type="checkbox" ${full.locTracking ? 'checked' : ''} onclick="return false;" tabindex="-1" />`;
                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        console.log(full.referenceId)
                        var output;
                        output = `<a href="#" class="btn btn-sm btn-light btn-flex btn-center btn-active-light-primary" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">Detaylar 
															<i class="ki-outline ki-down fs-5 ms-1"></i></a>
															<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-7 w-250px py-4" data-kt-menu="true">
																<div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductWarehouseTotal" data-bs-target="#product_warehouse_total_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3 warehouse-total-link">Ambar Toplamları</a>
																</div>
																<div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductLocation" data-bs-target="#product_location_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3">Stok Yeri Dağılımı</a>
																</div>
                                                                <div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductWaitingSalesOrder" data-bs-target="#product_waiting_sales_order_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3">Bekleyen Satış Siparişleri</a>
																</div>
                                                                <div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductWaitingPurchaseOrder" data-bs-target="#product_waiting_purchase_order_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3">Bekleyen Satınalma Siparişleri</a>
																</div>
                                                                <div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductApprovedSupplier" data-bs-target="#product_approved_supplier_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3">Onaylı Tedarikçiler</a>
																</div>
                                                                <div class="menu-item px-3">
																	<a data-bs-toggle="modal" id="ProductMesaure" data-bs-target="#product_mesaure_modal" data-product-code="${full.code}" data-product-name="${full.name}" data-product-image="${full.imageData}" data-product-reference-id="${full.referenceId}" class="menu-link px-3">Malzeme Ölçüleri</a>
																</div>
															</div>`;
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
        const filterSearch = document.querySelector('[product_list_table="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.ajax.reload();
        });
    }

    //var handleModalClose = function () {
    //    const closeButton = document.querySelector('[product-warehouse-total-modal-action="close"]');
    //    closeButton.addEventListener('click', function () {
    //        $('#product_warehouse_total_modal').modal('hide');
    //    });
    //}


    return {
        init: async function () {
            table = document.querySelector('#product_list_table');

            if (!table) {
                return;
            }
            initDatatable();
            handleSearchDatatable();
            //handleModalClose();

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    productList.init();
});