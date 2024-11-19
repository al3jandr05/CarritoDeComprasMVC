$(document).ready(function () {
    // Función para manejar el botón "Añadir al Carrito"
    $('.add-to-cart-button').click(function () {
        var button = $(this);
        var form = button.closest('.add-to-cart-form');
        var productId = form.data('product-id');

        $.ajax({
            url: '/Producto/AddToCart',
            method: 'POST',
            data: {
                productId: productId,
                quantity: 1
            },
            success: function (response) {
                // Actualizar el contador del carrito en tiempo real
                $('#cart-count').text(response.cartItemCount);
            },
            error: function () {
                alert("Ocurrió un error al añadir el producto al carrito.");
            }
        });
    });

    // Llama al servidor para obtener el conteo actual del carrito al cargar la página
    $.ajax({
        url: '/Carrito/GetCartItemCount',
        method: 'GET',
        success: function (response) {
            $('#cart-count').text(response.cartItemCount);
        }
    });
});
