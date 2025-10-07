using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class PessoasWindow : Window
    {
        private readonly DataService _dataService;
        private List<Pessoa> _todasPessoas;
        private List<Pessoa> _pessoasFiltradas;

        public PessoasWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            CarregarPessoas();
        }

        private void CarregarPessoas()
        {
            _todasPessoas = _dataService.CarregarPessoas();
            _pessoasFiltradas = new List<Pessoa>(_todasPessoas);
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            DgPessoas.ItemsSource = null;
            DgPessoas.ItemsSource = _pessoasFiltradas;
            TxtTotal.Text = _pessoasFiltradas.Count.ToString();
        }

        private void BtnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            var termo = TxtPesquisa.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(termo))
            {
                _pessoasFiltradas = new List<Pessoa>(_todasPessoas);
            }
            else
            {
                if (CmbTipoPesquisa.SelectedIndex == 0) // Nome
                {
                    _pessoasFiltradas = _todasPessoas
                        .Where(p => p.Nome.ToLower().Contains(termo))
                        .ToList();
                }
                else // CPF
                {
                    _pessoasFiltradas = _todasPessoas
                        .Where(p => p.CPF.Replace(".", "").Replace("-", "").Contains(termo.Replace(".", "").Replace("-", "")))
                        .ToList();
                }
            }

            AtualizarGrid();
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarPessoas();
            TxtPesquisa.Text = string.Empty;
            CmbTipoPesquisa.SelectedIndex = 0;
        }

        private void BtnNovaPessoa_Click(object sender, RoutedEventArgs e)
        {
            var modal = new PessoaCadastroModal(null, _dataService);
            if (modal.ShowDialog() == true)
            {
                CarregarPessoas();
            }
        }

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pessoa = button?.Tag as Pessoa;
            
            if (pessoa != null)
            {
                var modal = new PessoaVisualizacaoModal(pessoa);
                modal.ShowDialog();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pessoa = button?.Tag as Pessoa;
            
            if (pessoa != null)
            {
                var modal = new PessoaCadastroModal(pessoa, _dataService);
                if (modal.ShowDialog() == true)
                {
                    CarregarPessoas();
                }
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pessoa = button?.Tag as Pessoa;
            
            if (pessoa != null)
            {
                var resultado = MessageBox.Show(
                    $"Deseja realmente excluir '{pessoa.Nome}'?",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    _dataService.ExcluirPessoa(pessoa.Id);
                    CarregarPessoas();
                }
            }
        }

        private void BtnIncluirPedido_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pessoa = button?.Tag as Pessoa;
            
            if (pessoa != null)
            {
                var modal = new IncluirPedidoModal(pessoa, _dataService);
                if (modal.ShowDialog() == true)
                {
                    MessageBox.Show(
                        $"Pedido #{modal.PedidoCriado.Id} criado com sucesso!",
                        "Sucesso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }
    }
}
