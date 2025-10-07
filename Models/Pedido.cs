using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public DateTime DataVenda { get; set; }
        
        public DateTime DataPedido
        {
            get => DataVenda;
            set => DataVenda = value;
        }

        public decimal ValorTotal { get; set; }
        public FormaPagamento? FormaPagamento { get; set; }
        public StatusPedido Status { get; set; }
        public List<ItemPedido> Itens { get; set; }

        public Pedido()
        {
            DataVenda = DateTime.Now;
            Status = StatusPedido.Pendente;
            Itens = new List<ItemPedido>();
        }

        public void CalcularValorTotal()
        {
            ValorTotal = Itens.Sum(i => i.Quantidade * i.ValorUnitario);
        }
    }

    public class ItemPedido
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal => Quantidade * ValorUnitario;
    }

    public enum FormaPagamento
    {
        Dinheiro,
        Cartao,
        Boleto
    }

    public enum StatusPedido
    {
        Pendente,
        Pago,
        Enviado,
        Recebido
    }
}
