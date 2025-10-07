using System;
using System.Linq;
using System.Windows;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class PessoaCadastroModal : Window
    {
        private Pessoa _pessoa;
        private DataService _dataService;
        private CepService _cepService;
        private bool _isNovoRegistro;

        public PessoaCadastroModal(Pessoa pessoa, DataService dataService)
        {
            InitializeComponent();
            
            _dataService = dataService;
            _cepService = new CepService();
            
            // Se pessoa é null, é um novo registro
            _isNovoRegistro = (pessoa == null);
            
            if (_isNovoRegistro)
            {
                _pessoa = new Pessoa
                {
                    Nome = string.Empty,
                    CPF = string.Empty,
                    Email = string.Empty,
                    Telefone = string.Empty,
                    DataNascimento = DateTime.Now,
                    CEP = string.Empty,
                    Logradouro = string.Empty,
                    Numero = string.Empty,
                    Complemento = string.Empty,
                    Bairro = string.Empty,
                    Cidade = string.Empty,
                    Estado = string.Empty
                };
            }
            else
            {
                // Clona a pessoa para edição
                _pessoa = new Pessoa
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    CPF = pessoa.CPF,
                    Email = pessoa.Email,
                    Telefone = pessoa.Telefone,
                    DataNascimento = pessoa.DataNascimento,
                    CEP = pessoa.CEP,
                    Logradouro = pessoa.Logradouro,
                    Numero = pessoa.Numero,
                    Complemento = pessoa.Complemento,
                    Bairro = pessoa.Bairro,
                    Cidade = pessoa.Cidade,
                    Estado = pessoa.Estado
                };
            }
            
            DataContext = _pessoa;
            
            // Foca no campo Nome
            Loaded += (s, e) => txtNome.Focus();
        }

        private async void BtnBuscarCep_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_pessoa.CEP))
            {
                return;
            }

            var cep = _pessoa.CEP.Replace("-", "").Replace(".", "").Trim();

            if (cep.Length != 8)
            {
                CustomMessageBox.Show("CEP deve ter 8 dígitos!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var endereco = await _cepService.BuscarEnderecoPorCep(cep);

                if (endereco != null)
                {
                    _pessoa.Logradouro = endereco.Logradouro;
                    _pessoa.Bairro = endereco.Bairro;
                    _pessoa.Cidade = endereco.Localidade;
                    _pessoa.Estado = endereco.Uf;
                    
                    CustomMessageBox.Show("Endereço encontrado! Preencha apenas o número.", 
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBox.Show("CEP não encontrado. Verifique se digitou corretamente.", 
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Erro ao buscar CEP: {ex.Message}", 
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(_pessoa.Nome))
            {
                CustomMessageBox.Show("Nome é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationService.ValidarNome(_pessoa.Nome))
            {
                CustomMessageBox.Show("Nome inválido! Deve ter no mínimo 3 caracteres e não pode conter números.", 
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_pessoa.CPF))
            {
                CustomMessageBox.Show("CPF é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationService.ValidarCPF(_pessoa.CPF))
            {
                CustomMessageBox.Show("CPF inválido!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica CPF duplicado
            var cpfLimpo = _pessoa.CPF.Replace(".", "").Replace("-", "").Trim();
            var pessoasExistentes = _dataService.ObterPessoas();
            var cpfDuplicado = pessoasExistentes.Any(p => 
                p.Id != _pessoa.Id && 
                p.CPF.Replace(".", "").Replace("-", "").Trim() == cpfLimpo);

            if (cpfDuplicado)
            {
                CustomMessageBox.Show("Este CPF já está cadastrado no sistema!", 
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(_pessoa.Email))
            {
                if (!ValidationService.ValidarEmail(_pessoa.Email))
                {
                    CustomMessageBox.Show("Email inválido! Use o formato: exemplo@dominio.com", 
                        "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(_pessoa.Telefone))
            {
                if (!ValidationService.ValidarTelefone(_pessoa.Telefone))
                {
                    CustomMessageBox.Show("Telefone inválido! Use o formato: (11) 98765-4321", 
                        "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[v0] Salvando pessoa: {_pessoa.Nome}, CPF: {_pessoa.CPF}, ID: {_pessoa.Id}");
            System.Diagnostics.Debug.WriteLine($"[v0] É novo registro: {_isNovoRegistro}");
            
            // Salva no XML
            _dataService.SalvarPessoa(_pessoa);
            
            System.Diagnostics.Debug.WriteLine($"[v0] Pessoa salva com ID: {_pessoa.Id}");
            System.Diagnostics.Debug.WriteLine($"[v0] Total de pessoas após salvar: {_dataService.ObterPessoas().Count}");
            
            CustomMessageBox.Show("Pessoa salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            
            DialogResult = true;
            Close();
        }
    }
}
