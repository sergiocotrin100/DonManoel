


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

function showMenuDetails(idmenu) {
    
    let menu = getMenu(idmenu);
    

    $("#modalDetalhesMenu #MenuDescricao").html(menu.Descricao);
    

    var myModal = new bootstrap.Modal(document.getElementById('modalDetalhesMenu'), {
        keyboard: false,
        backdrop: "static"
    });

    $(myModal).data('bs.modal').options.backdrop = 'static';
    $(myModal).modal({ backdrop: true, keyboard: false, show: true });    

}

function getMenu(idMenu) {
    let menu = [];

    $.each(CATEGORIAS, function (idx, categoria) {
        let item = $.grep(categoria.Menu, function (e) { return e.Id == idMenu; })[0];
        if (!isNullOrEmpty(item) && item.Id > 0) {
            menu = item;
            return menu;
        }
    });

    return menu;
}