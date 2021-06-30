

$(document).ready(function () {
    initResultsTable();

    $("#btnSearch").click(function () {
        search();
    });

    
});

function search() {
    let model = {
        Id: $("#Id").val(),
        IdStatusPedido: $("#IdStatusPedido").val(),
        IdMesa: $("#IdMesa").val(),
        IdUsuario: $("#IdUsuario").val(),
        DataInicio: $("#DataInicio").val(),
        DataFim: $("#DataFim").val()
    }
    $.ajax(
        {
            type: 'POST',
            url: hostSite() + "Sales/GetPedidos",
            data: {
                'model':model
            },
            dataType: 'html',

            success: function (data) {
                
                setTimeout(function () {
                    $('#divPedidos').html(data);
                    initResultsTable();
                }, 1000);

            }
        });
}

function getPedidoDetailsById(idpedido) {
    let pedido = [];
    $.each(LISTAPEDIDOS, function (idx, ped) {
        if (ped.Id== idpedido) {
            pedido = ped;
            return;
        }
    });

    return pedido;
}

function showDetails(idpedido) {
    let model = getPedidoDetailsById(idpedido);
    $("#tableDetails tbody").remove();
    var tbody = $('#tableDetails').children('tbody');
    var table = tbody.length ? tbody : $('#tableDetails');

    $.each(model.Itens, function (idx, item) {
        table.append('<tr>' +
            '<td>' + item.DataAtualizacaoFormatada + '</td> ' +
            '<td>' + item.Menu.Nome + '</td> ' +
            '<td>' + item.Status + '</td> ' +
            '<td style="text-align: center;">' + item.ValorFormatado + '</td> ' +
            '<td>' + (isNullOrEmpty(item.PontoCarne) ? "" : item.PontoCarne)+ '</td> ' +
            '</tr>');
    });

    jQuery('#modalDetalhes').modal('show');
   
}




function initResultsTable() {
    $('#example1').DataTable({
        language: {
            decimal: ",",
            thousands: ".",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            lengthMenu: "Mostrar _MENU_",
            zeroRecords: "Nenhum registro encontrado",
            emptyTable: "Nenhum registro encontrado",
            info: "Registros _START_ a _END_ de _TOTAL_",
            infoEmpty: "",//"Nada para mostrar",
            infoFiltered: "(filtrados de _MAX_)",
            infoPostFix: "",
            url: "",
            paginate: {
                First: "Primeiro",
                Previous: "Anterior",
                Next: "Seguinte",
                Last: "Último"
            },
            buttons: {
                pageLength: {
                    _: '<i class="fal fa-align-justify" data-toggle="tooltip" data-placement="top" title="%d Linhas"></i>',
                    '-1': "Mostrar Tudo"
                }
            },
            search: "",
            searchPlaceholder: "Pesquisar...",
            select: true,
            deferRender: true,
            fixedHeader: {
                header: true,
                footer: false
            }
            , bAutoWidth: true

        },
        drawCallback: function (settings) {
            var pagination = $(this).closest('.dataTables_wrapper').find('.dataTables_paginate');
            pagination.toggle(this.api().page.info().pages > 1);
            var info = $(this).closest('.dataTables_wrapper').find('.dataTables_info');
            info.toggle(this.api().page.info().pages > 1);
            $(".dataTables_paginate a").addClass("paginate_button");
        }
    });
}

