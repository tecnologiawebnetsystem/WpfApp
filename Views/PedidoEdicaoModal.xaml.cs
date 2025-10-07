using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class PedidoEdicaoModal : Window
    {
        private readonly Pedido _pedido;
        private readonly List<Pessoa> _pessoas;
        private readonly List<Produto> _produtos;
        // </CHANGE>

        public PedidoEdicaoModal(Pedido pedido, List<Pessoa> pessoas, List<Produto> produtos)
        {
            InitializeComponent();
            _pedido = pedido;
            _pessoas = pessoas;
            _produtos = produtos;
            CarregarDados();
        }
        // </CHANGE>

        private void CarregarDados()
        {
            txtPedidoId.Text = $"Pedido #{_pedido.Id}";
            
            // Forma de Pagamento
            if (_pedido.FormaPagamento.HasValue)
            {
                var formaPagamento = _pedido.FormaPagamento.Value.ToString();
                var item = cmbFormaPagamento.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Tag?.ToString() == formaPagamento);
                if (item != null)
                {
                    cmbFormaPagamento.SelectedItem = item;
                }
            }
            else
            {
                cmbFormaPagamento.SelectedIndex = 0; // Não definido
            }
            
            // Status
            var status = _pedido.Status.ToString();
            ComboBoxItem statusItem = null;
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Tag?.ToString() == status)
                {
                    statusItem = item;
                    break;
                }
            }
            // </CHANGE>
            if (statusItem != null)
            {
                cmbStatus.SelectedItem = statusItem;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Atualizar Forma de Pagamento
            var formaPagamentoItem = cmbFormaPagamento.SelectedItem as ComboBoxItem;
            var formaPagamentoTag = formaPagamentoItem?.Tag?.ToString();
            
            if (string.IsNullOrEmpty(formaPagamentoTag))
            {
                _pedido.FormaPagamento = null;
            }
            else
            {
                switch (formaPagamentoTag)
                {
                    case "Dinheiro":
                        _pedido.FormaPagamento = Models.FormaPagamento.Dinheiro;
                        break;
                    case "Cartao":
                        _pedido.FormaPagamento = Models.FormaPagamento.Cartao;
                        break;
                    case "Boleto":
                        _pedido.FormaPagamento = Models.FormaPagamento.Boleto;
                        break;
                    default:
                        _pedido.FormaPagamento = null;
                        break;
                }
            }
            // </CHANGE>
            
            // Atualizar Status
            var statusItem = cmbStatus.SelectedItem as ComboBoxItem;
            var statusTag = statusItem?.Tag?.ToString();
            
            switch (statusTag)
            {
                case "Pendente":
                    _pedido.Status = StatusPedido.Pendente;
                    break;
                case "Pago":
                    _pedido.Status = StatusPedido.Pago;
                    break;
                case "Enviado":
                    _pedido.Status = StatusPedido.Enviado;
                    break;
                case "Recebido":
                    _pedido.Status = StatusPedido.Recebido;
                    break;
                default:
                    _pedido.Status = StatusPedido.Pendente;
                    break;
            }
            // </CHANGE>
            
            DialogResult = true;
            Close();
        }
    }
}
