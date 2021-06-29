(function ($, undefined) {

    
    $.fn.extend({
       
        FWDataTableDefault: function (obj) {
            //tratar o objeto que veio da tela
            var objetoRecebido = this;

            var config = {
                bFilter: true,
                bJQueryUI: true,
                bRetrieve: true,
                dom: "<'row'<'col-lg-6 col-md-8 col-sm-12'B><'col-lg-6 col-md-4 col-sm-12'f>><'row'<'col-12't>><'row'<'col-6'i><'col-6'p>>",
                searching: true,
                paging: true,
                ordering: true,
                info: true,
                pagingType: "numbers",
                responsive: true,
                scrollX: true,
                lengthMenu: [[10, 25, 50, 100, 200, 500, 1000, 2000], ["10 Linhas", "25 Linhas", "50 Linhas", "100 Linhas", "200 Linhas", "500 Linhas", "1000 Linhas", "2000 Linhas"]],
                buttons: [
                    {
                        extend: 'pageLength',
                        className: 'btn-top-grid cinza',
                        text: '<i class="fal fa-align-justify" data-toggle="tooltip" data-placement="top" title="Núm. Linhas"></i>'
                    },
                    {
                        extend: 'colvis',
                        className: 'btn-top-grid cinza',
                        columns: ':not(.noVis)',

                        text: '<i class="fal fa-chart-bar" data-toggle="tooltip" data-placement="top" title="Colunas"></i>'
                    },
                    {
                        extend: 'excelHtml5',
                        className: 'btn-top-grid verde',
                        columns: ':not(.noVis)',
                        text: '<i class="fal fa-file-excel" data-toggle="tooltip" data-placement="top" title="Exportar para Excel"></i>',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        className: 'btn-top-grid vermelho',
                        columns: ':not(.noVis)',
                        text: '<i class="fal fa-file-pdf" data-toggle="tooltip" data-placement="top" title="Gerar PDF"></i>',
                        customize: function (doc) {
                            doc.styles.tableHeader.fontSize = 8;
                            doc.defaultStyle.fontSize = 6; //<-- set fontsize to 16 instead of 10 
                        } ,
                        exportOptions: {
                            //columns: []
                            stripNewlines: false,
                            columns: ':visible'
                        },
                        //teste exportação

                        orientation: 'landscape',
                        pageSize: 'A4',
                        pageMargins: [0, 0, 0, 0], // tentantiva 1 tento configurar margin, pode usar de 1 em 1
                        margin: [0, 0, 0, 0], //tentantiva 2 tentativa de regular a margin
                        //text: '<u>ETeste</u>texto (PDF)',
                         // configura uma tecla para exportar no caso aqui indica a tecla e
                        key: {
                            key: 'e',
                            altKey: false
                        }
                        , content: [{ style: 'fullWidth' }]
                        , styles: { // style do corpo do PDF
                            fullWidth: { fontSize: 8, bold: true, alignment: 'right', margin: [0, 0, 0, 0] }
                        }                        
                        //teste exportação
                    }
                ],
                oClasses: {
                    sTable: "compact stripe order-column dataTable class100",
                    sNoFooter: "",
                    sFilterInput: "form-control",
                    sLengthSelect: "form-control",
                    sProcessing: "dataTables_processing panel panel-default",
                    sPageButton: ""
                },
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
                    searchPlaceholder: "Filtro do GRID...",
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
            };

            var doc = {
                pageSize: config.pageSize,
                pageOrientation: config.orientation,
                styles: {
                    tableHeader: {
                        bold: true,
                        fontSize: 8,
                        color: 'white',
                        fillColor: '#2d4154',
                        alignment: 'center'
                    },
                    tableBodyEven: {
                        fontSize: 8,
                    },
                    tableBodyOdd: {
                        fillColor: '#f3f3f3',
                        fontSize: 8,
                    },
                    tableFooter: {
                        bold: true,
                        fontSize: 10,
                        color: 'white',
                        fillColor: '#2d4154'
                    },
                    title: {
                        alignment: 'center',
                        fontSize: 12
                    },
                    message: {}
                },
                defaultStyle: {
                    fontSize: 9
                }
            };


            if (objetoRecebido.length > 0 && $(this).dataTable.isDataTable("#" + this[0].id)) {
                objetoRecebido.DataTable().destroy();
            }


            if (obj) {
                for (var item in obj) {
                    if (item === "oClasses") {
                        for (var c in obj.oClasses) {
                            if (config.oClasses[c])
                                config.oClasses[c] += " " + obj.oClasses[c];
                            else
                                config.oClasses[c] = obj.oClasses[c];
                        }
                    }
                    if (item === "NoButtons") {
                        //utlizar esta regra para tirar de um em um botao 
                        if (obj.NoButtons) {
                            config.buttons = [];
                        }
                    }

                    // tratar paginacao 
                    // 1 se for menor  que  10  - sem paginação
                    if (obj.data != null && obj.data.length < 10) {
                        config.paging = false;
                        config.info = false;
                    }

                    // 2 - paramentro paginacao
                    if (item === "NoPaging") {
                        //utlizar esta regra para tirar a paginação
                        config.paging = false;
                        config.info = false;
                    }


                    if (item === "RemoveSeacrhInput") {
                        //escolhi retirar no dom porque desativando o searching, mais eventlistners eram desativados
                        if (obj.NoButtons) {
                            config.dom = "<'row'<'col-md-8'B><'col-md-4'>><'row'<'col-md-12't>><'row'<'col-md-4'i><'col-md-8'p>>";
                        }
                    }
                    else if (item !== "bJQueryUI" && item !== "bRetrieve" && item !== "oLanguage") {
                        config[item] = obj[item];
                    }

                }
            }

            try {
                var table = $(this).dataTable(config).DataTable();
            } catch (ex) {
                alert("Ocorreu um erro ao criar a tabela: " + (ex.message || ex.name || ex) + " (Não é certeza, mas esse tipo de erro costuma ocorrer quando a quantidade de colunas do corpo difere entre linhas, ou é diferente da quantidade de colunas do cabeçalho)");
            }

            

            var table2 = $(this).DataTable();
            return table2;
        },
    });
   
})(jQuery);

