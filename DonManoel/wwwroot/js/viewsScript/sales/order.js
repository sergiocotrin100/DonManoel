


var PEDIDO = null;
var CATEGORIAS = [];

/*
$(document).ready(function () {

});*/

window.onload = getParams;


function getParams() {
    if (!isNullOrEmpty(objPedido)) {
        PEDIDO = objPedido;
        CATEGORIAS = lstCategorias;
    }
}

function showMenuDetails() {
    debugger;
    $("#modalDetalhesMenu").modal({ backdrop: "static" });
    //$(".modal-right").modal({ backdrop: "static" });
    //modal-right
}