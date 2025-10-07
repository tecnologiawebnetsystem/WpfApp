using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class PedidoVisualizacaoModal : Window
    {
        public PedidoVisualizacaoModal(Pedido pedido, List<Pessoa> pessoas, List<Produto> produtos)
        {
            InitializeComponent();
            CarregarDados(pedido, pessoas, produtos);
        }

        private void CarregarDados(Pedido pedido, List<Pessoa> pessoas, List<Produto> produtos)
        {
            // Informações do Pedido
            txtPedidoId.Text = $"Pedido #{pedido.Id}";
            
            // Informações do Cliente
            var pessoa = pessoas.FirstOrDefault(p => p.Id == pedido.PessoaId);
            if (pessoa != null)
            {
                txtNomeCliente.Text = pessoa.Nome;
                txtCpfCliente.Text = $"CPF: {pessoa.CPF}";
            }
            
            // Produtos do Pedido
            var produtosExibicao = pedido.Itens.Select(item => new
            {
                NomeProduto = item.NomeProduto,
                item.Quantidade,
                item.ValorUnitario,
                ValorTotal = item.Quantidade * item.ValorUnitario
            }).ToList();
            
            listProdutos.ItemsSource = produtosExibicao;
            
            // Informações do Pedido
            txtDataVenda.Text = pedido.DataVenda.ToString("dd/MM/yyyy HH:mm");
            txtFormaPagamento.Text = pedido.FormaPagamento.HasValue 
                ? pedido.FormaPagamento.Value.ToString() 
                : "Não definido";
            txtStatus.Text = pedido.Status.ToString();
            txtValorTotal.Text = pedido.ValorTotal.ToString("C");
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
