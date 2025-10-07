using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class PedidosWindow : Window
    {
        private readonly DataService _dataService;
        private List<Pedido> _todosPedidos;
        private List<Pessoa> _todasPessoas;
        private List<Produto> _todosProdutos;

        public PedidosWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            CarregarDados();
            AtualizarGrid();
            CmbPesquisaPedido.ItemsSource = _todasPessoas;
        }

        private void CarregarDados()
        {
            _todosPedidos = _dataService.CarregarPedidos();
            _todasPessoas = _dataService.CarregarPessoas();
            _todosProdutos = _dataService.CarregarProdutos();
        }

        private void AtualizarGrid()
        {
            var pedidosFiltrados = FiltrarPedidos();
            
            var pedidosExibicao = pedidosFiltrados.Select(p => new
            {
                p.Id,
                NomePessoa = ObterNomePessoa(p.PessoaId),
                CpfPessoa = ObterCpfPessoa(p.PessoaId),
                p.DataVenda,
                p.ValorTotal,
                FormaPagamentoTexto = ObterFormaPagamentoTexto(p.FormaPagamento),
                StatusTexto = ObterStatusTexto(p.Status),
                StatusCor = ObterStatusCor(p.Status)
            }).ToList();

            listPedidos.ItemsSource = pedidosExibicao;
            txtContador.Text = $"Total de pedidos: {pedidosExibicao.Count}";
            
            borderSemPedidos.Visibility = pedidosExibicao.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private List<Pedido> FiltrarPedidos()
        {
            var pedidos = _todosPedidos.ToList();
            
            // Filtro por texto (CPF ou Nome)
            var textoPesquisa = CmbPesquisaPedido.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(textoPesquisa))
            {
                pedidos = pedidos.Where(p =>
                {
                    var pessoa = _todasPessoas.FirstOrDefault(ps => ps.Id == p.PessoaId);
                    if (pessoa == null) return false;
                    
                    return pessoa.Nome.ToLower().Contains(textoPesquisa) ||
                           pessoa.CPF.Replace(".", "").Replace("-", "").Contains(textoPesquisa.Replace(".", "").Replace("-", ""));
                }).ToList();
            }
            
            // Filtro por Status
            var statusSelecionado = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (statusSelecionado != "Todos os Status")
            {
                StatusPedido? status = null;
                if (statusSelecionado == "Pendente")
                    status = StatusPedido.Pendente;
                else if (statusSelecionado == "Pago")
                    status = StatusPedido.Pago;
                else if (statusSelecionado == "Enviado")
                    status = StatusPedido.Enviado;
                else if (statusSelecionado == "Recebido")
                    status = StatusPedido.Recebido;
                
                if (status.HasValue)
                {
                    pedidos = pedidos.Where(p => p.Status == status.Value).ToList();
                }
            }
            
            return pedidos.OrderByDescending(p => p.DataVenda).ToList();
        }

        private string ObterNomePessoa(int pessoaId)
        {
            var pessoa = _todasPessoas.FirstOrDefault(p => p.Id == pessoaId);
            return pessoa?.Nome ?? "Pessoa não encontrada";
        }

        private string ObterCpfPessoa(int pessoaId)
        {
            var pessoa = _todasPessoas.FirstOrDefault(p => p.Id == pessoaId);
            return pessoa?.CPF ?? "";
        }

        private string ObterFormaPagamentoTexto(FormaPagamento? formaPagamento)
        {
            if (!formaPagamento.HasValue) return "Não definido";
            
            switch (formaPagamento.Value)
            {
                case FormaPagamento.Dinheiro:
                    return "Dinheiro";
                case FormaPagamento.Cartao:
                    return "Cartão";
                case FormaPagamento.Boleto:
                    return "Boleto";
                default:
                    return "Não definido";
            }
        }

        private string ObterStatusTexto(StatusPedido status)
        {
            switch (status)
            {
                case StatusPedido.Pendente:
                    return "Pendente";
                case StatusPedido.Pago:
                    return "Pago";
                case StatusPedido.Enviado:
                    return "Enviado";
                case StatusPedido.Recebido:
                    return "Recebido";
                default:
                    return "Desconhecido";
            }
        }

        private string ObterStatusCor(StatusPedido status)
        {
            switch (status)
            {
                case StatusPedido.Pendente:
                    return "#F39C12";
                case StatusPedido.Pago:
                    return "#27AE60";
                case StatusPedido.Enviado:
                    return "#3498DB";
                case StatusPedido.Recebido:
                    return "#1ABC9C";
                default:
                    return "#95A5A6";
            }
        }

        private void TxtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarGrid();
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPedidos != null)
            {
                AtualizarGrid();
            }
        }

        private void BtnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            AtualizarGrid();
        }

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pedidoId = Convert.ToInt32(button.Tag);
            var pedido = _todosPedidos.FirstOrDefault(p => p.Id == pedidoId);
            
            if (pedido != null)
            {
                var modal = new PedidoVisualizacaoModal(pedido, _todasPessoas, _todosProdutos);
                modal.ShowDialog();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pedidoId = Convert.ToInt32(button.Tag);
            var pedido = _todosPedidos.FirstOrDefault(p => p.Id == pedidoId);
            
            if (pedido != null)
            {
                if (pedido.Status == StatusPedido.Recebido)
                {
                    MessageBox.Show(
                        "Não é possível editar pedidos com status 'Recebido'.\n\nPedidos recebidos são considerados finalizados e não podem ser modificados.",
                        "Edição Bloqueada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                
                var modal = new PedidoEdicaoModal(pedido, _todasPessoas, _todosProdutos);
                if (modal.ShowDialog() == true)
                {
                    _dataService.SalvarPedidos(_todosPedidos);
                    AtualizarGrid();
                }
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pedidoId = Convert.ToInt32(button.Tag);
            
            var resultado = CustomMessageBox.Show(
                "Tem certeza que deseja excluir este pedido?",
                "Confirmar Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            
            if (resultado == MessageBoxResult.Yes)
            {
                _todosPedidos.RemoveAll(p => p.Id == pedidoId);
                _dataService.SalvarPedidos(_todosPedidos);
                AtualizarGrid();
                
                CustomMessageBox.Show(
                    "Pedido excluído com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void CmbPesquisaPedido_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CmbPesquisaPedido == null || _todasPessoas == null)
                return;

            var texto = CmbPesquisaPedido.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(texto))
            {
                CmbPesquisaPedido.ItemsSource = _todasPessoas;
                AtualizarGrid();
                return;
            }

            // Filtrar pessoas por nome ou CPF
            var sugestoes = _todasPessoas
                .Where(p => p.Nome.ToLower().Contains(texto) || 
                           p.CPF.Replace(".", "").Replace("-", "").Contains(texto.Replace(".", "").Replace("-", "")))
                .ToList();

            CmbPesquisaPedido.ItemsSource = sugestoes;
            CmbPesquisaPedido.IsDropDownOpen = sugestoes.Any();
            
            // Atualizar grid automaticamente enquanto digita
            AtualizarGrid();
        }
    }
}
