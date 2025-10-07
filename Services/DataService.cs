using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class DataService
    {
        private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        
        private readonly string _pessoasFile;
        private readonly string _produtosFile;
        private readonly string _pedidosFile;

        private List<Pessoa> _pessoas;
        private List<Produto> _produtos;
        private List<Pedido> _pedidos;

        public DataService()
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);

            _pessoasFile = Path.Combine(_dataPath, "pessoas.xml");
            _produtosFile = Path.Combine(_dataPath, "produtos.xml");
            _pedidosFile = Path.Combine(_dataPath, "pedidos.xml");

            CarregarDados();
        }

        private void CarregarDados()
        {
            _pessoas = CarregarArquivo<List<Pessoa>>(_pessoasFile) ?? new List<Pessoa>();
            _produtos = CarregarArquivo<List<Produto>>(_produtosFile) ?? new List<Produto>();
            _pedidos = CarregarArquivo<List<Pedido>>(_pedidosFile) ?? new List<Pedido>();
        }

        private T CarregarArquivo<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return serializer.Deserialize(stream) as T;
                }
            }
            catch
            {
                return null;
            }
        }

        private void SalvarArquivo<T>(string filePath, T data)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }

        public List<Pessoa> ObterPessoas() => _pessoas;

        public Pessoa ObterPessoaPorId(int id) => _pessoas.FirstOrDefault(p => p.Id == id);

        public void SalvarPessoa(Pessoa pessoa)
        {
            if (pessoa.Id == 0)
            {
                pessoa.Id = _pessoas.Any() ? _pessoas.Max(p => p.Id) + 1 : 1;
                _pessoas.Add(pessoa);
            }
            else
            {
                var index = _pessoas.FindIndex(p => p.Id == pessoa.Id);
                if (index >= 0)
                    _pessoas[index] = pessoa;
            }
            
            SalvarArquivo(_pessoasFile, _pessoas);
        }

        public void ExcluirPessoa(int id)
        {
            _pessoas.RemoveAll(p => p.Id == id);
            SalvarArquivo(_pessoasFile, _pessoas);
        }

        public List<Pessoa> PesquisarPessoas(string nome, string cpf)
        {
            return _pessoas.Where(p =>
                (string.IsNullOrEmpty(nome) || p.Nome.Contains(nome)) &&
                (string.IsNullOrEmpty(cpf) || p.CPF.Contains(cpf))
            ).ToList();
        }

        public List<Produto> ObterProdutos() => _produtos;

        public Produto ObterProdutoPorId(int id) => _produtos.FirstOrDefault(p => p.Id == id);

        public void SalvarProduto(Produto produto)
        {
            if (produto.Id == 0)
            {
                produto.Id = _produtos.Any() ? _produtos.Max(p => p.Id) + 1 : 1;
                _produtos.Add(produto);
            }
            else
            {
                var index = _produtos.FindIndex(p => p.Id == produto.Id);
                if (index >= 0)
                    _produtos[index] = produto;
            }
            
            SalvarArquivo(_produtosFile, _produtos);
        }

        public void ExcluirProduto(int id)
        {
            _produtos.RemoveAll(p => p.Id == id);
            SalvarArquivo(_produtosFile, _produtos);
        }

        public List<Produto> PesquisarProdutos(string nome, string codigo, decimal? valorMin, decimal? valorMax)
        {
            return _produtos.Where(p =>
                (string.IsNullOrEmpty(nome) || p.Nome.Contains(nome)) &&
                (string.IsNullOrEmpty(codigo) || p.Codigo.Contains(codigo)) &&
                (!valorMin.HasValue || p.Valor >= valorMin.Value) &&
                (!valorMax.HasValue || p.Valor <= valorMax.Value)
            ).ToList();
        }

        public List<Pedido> ObterPedidos() => _pedidos;

        public List<Pedido> CarregarPedidos() => ObterPedidos();

        public List<Pessoa> CarregarPessoas() => ObterPessoas();

        public List<Produto> CarregarProdutos() => ObterProdutos();

        public void SalvarPedidos(List<Pedido> pedidos)
        {
            _pedidos = pedidos;
            SalvarArquivo(_pedidosFile, _pedidos);
        }

        public void SalvarProdutos(List<Produto> produtos)
        {
            _produtos = produtos;
            SalvarArquivo(_produtosFile, _produtos);
        }

        public List<Pedido> ObterPedidosPorPessoa(int pessoaId)
        {
            return _pedidos.Where(p => p.PessoaId == pessoaId).ToList();
        }

        public void SalvarPedido(Pedido pedido)
        {
            if (pedido.Id == 0)
            {
                pedido.Id = _pedidos.Any() ? _pedidos.Max(p => p.Id) + 1 : 1;
                _pedidos.Add(pedido);
            }
            else
            {
                var index = _pedidos.FindIndex(p => p.Id == pedido.Id);
                if (index >= 0)
                    _pedidos[index] = pedido;
            }
            
            SalvarArquivo(_pedidosFile, _pedidos);
        }

        public void ExcluirPedido(int id)
        {
            _pedidos.RemoveAll(p => p.Id == id);
            SalvarArquivo(_pedidosFile, _pedidos);
        }

        public void AdicionarPedido(Pedido pedido)
        {
            SalvarPedido(pedido);
        }

        public void AtualizarStatusPedido(int pedidoId, StatusPedido novoStatus)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == pedidoId);
            if (pedido != null)
            {
                pedido.Status = novoStatus;
                SalvarArquivo(_pedidosFile, _pedidos);
            }
        }
    }
}
