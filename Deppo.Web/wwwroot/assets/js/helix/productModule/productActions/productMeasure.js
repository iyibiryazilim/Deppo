"use strict";

// Class definition
var ShowProductMeasurePageInit = function () {

    var id;
    var productCode;
    var productName;
    var productImage;

    // Private functions

    var loadModalPage = function () {
        $('#product_mesaure_modal').on('shown.bs.modal', function () {
            $('#ProductMesaureInformation').html(`
                <div><span class="fw-bold"></span> ${productCode}</div>
                <div><span class="fw-bold"></span> ${productName}</div>
            `);
            loadProductMeasure(id);

        });

    };

    var loadProductMeasure = function (productReferenceId) {
        $.ajax({
            url: `/Product/GetProductMeasure`, // Doğru API URL'i
            type: 'POST',
            data: { productReferenceId: productReferenceId },
            success: function (response) {
                if (response.success !== false && response.data && response.data.length > 0) {
                    var item = response.data[0];  // Tek elemanlı dizi
                    console.log(item)

                    $('#productWidth').text(item.width);
                    $('#productLength').text(item.length);
                    $('#productHeight').text(item.height);
                    $('#productWeight').text(item.weight);
                    $('#productVolume').text(item.volume);
                  
                    $('#product_mesaure_modal').modal('show');
                } else {
                    alert(response.message || "Ürün ölçüleri yüklenemedi.");
                }
            },
            error: function (xhr) {
                alert("Bir hata oluştu: " + xhr.responseText);
            }
        });
    };


    var bindEventHandlers = function () {
        $(document).on('click', 'a#ProductMesaure', function () {
            id = $(this).data('product-reference-id');
            productCode = $(this).data('product-code');
            productName = $(this).data('product-name');
            productImage = $(this).data('product-image');

        });
    };

    // Public methods
    return {
        init: function () {
            bindEventHandlers();
            loadModalPage();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    ShowProductMeasurePageInit.init();
});
