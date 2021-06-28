using CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class Pedido
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdMesa { get; set; }
        public long IdStatusPedido { get; set; }
        public string Cliente { get; set; }
        public int TaxaServico { get; set; }
        public decimal ValorItens { get; set; }
        public decimal ValorTaxaServico { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public string Status { get; set; }
        public DateTime Data { get; set; }
        public string Atendente { get; set; }
        public List<PedidoItem> Itens { get; set; }
        public List<LogPedidoStatus> LogStatus { get; set; }
        public bool CanCancel
        {
            get
            {
                if (this.Id > 0)
                {
                    if(this.IdStatusPedido == (int)Settings.Status.Pedido.Pendente 
                        || this.IdStatusPedido == (int)Settings.Status.Pedido.AguardandoPreparacao
                        || this.IdStatusPedido == (int)Settings.Status.Pedido.EmPreparacao
                        )
                    {
                        if (this.Itens == null || this.Itens.Count == 0) return true;

                        bool existeItemEntregue = this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Pronto);
                        return !existeItemEntregue;
                    }
                }
                return false;
            }
        }
        public bool CanSend
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.IdStatusPedido != (int)Settings.Status.Pedido.Cancelado && this.IdStatusPedido != (int)Settings.Status.Pedido.Pago)
                    {
                        if (this.Itens == null || this.Itens.Count == 0) return true;
                        return this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Solicitado);
                    }
                }
                return false;
            }
        }
        public bool CanFecharPedido
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.IdStatusPedido == (int)Settings.Status.Pedido.Pronto)
                    {
                        if (this.Itens == null || this.Itens.Count == 0) return false;
                        if (this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Solicitado)) return false;
                        if (this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Enviado)) return false;
                        return this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Pronto);
                    }
                }
                return false;
            }
        }
        public bool CanPrint
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.IdStatusPedido == (int)Settings.Status.Pedido.ContaFechada)
                    {
                        if (this.Itens == null || this.Itens.Count == 0) return false;
                        if (this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Solicitado)) return false;
                        if (this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Enviado)) return false;
                        return this.Itens.Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Pronto);
                    }
                }
                return false;
            }
        }
        public bool IsAtrasado
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.TempoEspera.HasValue)
                    {
                        if(this.TempoPreparoAdultoCozinha>0)
                        return this.TempoEspera.Value.TotalMinutes>this.TempoPreparoAdultoCozinha;
                    }
                }
                return false;
            }
        }
        public DateTime? DataEnvioCozinha
        {
            get
            {
                if(this.Id>0)
                {
                    var status = this.LogStatus.Where(ped => ped.IdStatusPedido == (int)Settings.Status.Pedido.EmPreparacao).ToList();
                    if (status.Count > 0)
                    {
                        var enviado = status.OrderByDescending(x => x.Data).First();
                        return enviado.Data;
                     }
                }
                return null;
            }
        }

        private TimeSpan? TempoEspera
        {
            get
            {
                if(this.Id>0)
                {
                    if (this.TempoPreparoAdultoCozinha > 0)
                    {
                        if (this.DataEnvioCozinha.HasValue)
                        {
                            TimeSpan time = DateTime.Now - this.DataEnvioCozinha.Value;
                            return time;
                        }
                    }

                }
                return null;
            }
        }

        public string Tempo
        {
            get
            {
                if (this.Id > 0)
                {
                    if(this.TempoEspera.HasValue)
                    {
                        return $"{this.TempoEspera.Value.Hours.ToString("00")}:{this.TempoEspera.Value.Minutes.ToString("00")}";
                    }
                }
                return string.Empty;
            }
        }
        private int TempoPreparoAdultoCozinha
        {
            get
            {
                if (this.Id > 0)
                {
                    if(this.Itens != null && this.Itens.Count>0)
                    {
                        int _tempoPreparo = this.Itens.Where(item => item.Menu.TipoCategoria==Settings.TipoCategoria.Cozinha && item.Menu.IsPratoKids==false).Sum(item => item.TempoPreparo);
                        int _quantidade = this.Itens.Where(item => item.Menu.TipoCategoria == Settings.TipoCategoria.Cozinha && item.Menu.IsPratoKids == false).ToList().Count();
                        if (_quantidade > 0)
                            return _tempoPreparo / _quantidade;
                        else
                            return 0;
                    }
                }
                return 0;
            }
        }
        public string TempoPreparoAdultoCozinhaFormatado
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.TempoPreparoAdultoCozinha > 0)
                        return $"{this.TempoPreparoAdultoCozinha.ToString()} Minutos";
                }
                return "";
            }
        }
        public int TempoPreparBar
        {
            get
            {
                if (this.Id > 0)
                {
                    if (this.Itens != null && this.Itens.Count > 0)
                    {
                        int _tempoPreparo = this.Itens.Where(item => item.Menu.TipoCategoria == Settings.TipoCategoria.Bar).Sum(item => item.TempoPreparo);
                        int _quantidade = this.Itens.Where(item => item.Menu.TipoCategoria == Settings.TipoCategoria.Bar).ToList().Count();
                        if (_quantidade > 0)
                            return _tempoPreparo / _quantidade;
                        else
                            return 0;
                    }
                }
                return 0;
            }
        }
        public string DataPedidoImpressao
        {
            get
            {
                if(this.Id >0)
                {
                    return this.Data.FormatDate();
                }
                return string.Empty;    
            }
        }
        public string HoraPedidoImpressao
        {
            get
            {
                if (this.Id > 0)
                {
                    return this.Data.ToString("hh:mm:ss");
                }
                return string.Empty;
            }
        }
        public dynamic itensImpressao
        {
            get
            {
                if(this.Id>0 && this.Itens != null && this.Itens.Count>0)
                {
                    var Query = from p in this.Itens.GroupBy(p => p.IdMenu)
                                select new
                                {
                                    Quantidade = p.Count(),
                                    p.First().IdMenu,
                                    p.First().Valor,
                                    p.First().Menu.Descricao,
                                    p.First().ValorFormatado,
                                };
                    return Query;
                }

                return new List<PedidoItem>();
            }
        }
        public decimal ValorTotalPedido
        {
            get
            {
                if(this.Id > 0)
                {
                    if(this.TaxaServico>0)
                    {
                        var valortaxa = (this.TaxaServico * this.ValorItens) / 100;
                        return this.ValorItens + valortaxa;

                    } 
                    else
                    {
                        return this.ValorItens;
                    }                   
                }
                return 0;
            }
        }
    }
}
