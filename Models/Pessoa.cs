using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WpfApp.Models
{
    public class Pessoa : INotifyPropertyChanged
    {
        private string _nome;
        private string _cpf;
        private string _email;
        private string _telefone;
        private string _cep;
        private string _logradouro;
        private string _numero;
        private string _complemento;
        private string _bairro;
        private string _cidade;
        private string _estado;
        private string _endereco;
        private DateTime? _dataNascimento;

        public int Id { get; set; }

        public string Nome
        {
            get => _nome;
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    OnPropertyChanged(nameof(Nome));
                }
            }
        }

        public string CPF
        {
            get => _cpf;
            set
            {
                if (_cpf != value)
                {
                    _cpf = value;
                    OnPropertyChanged(nameof(CPF));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Telefone
        {
            get => _telefone;
            set
            {
                if (_telefone != value)
                {
                    _telefone = value;
                    OnPropertyChanged(nameof(Telefone));
                }
            }
        }

        public string CEP
        {
            get => _cep;
            set
            {
                if (_cep != value)
                {
                    _cep = value;
                    OnPropertyChanged(nameof(CEP));
                }
            }
        }

        public string Logradouro
        {
            get => _logradouro;
            set
            {
                if (_logradouro != value)
                {
                    _logradouro = value;
                    OnPropertyChanged(nameof(Logradouro));
                }
            }
        }

        public string Numero
        {
            get => _numero;
            set
            {
                if (_numero != value)
                {
                    _numero = value;
                    OnPropertyChanged(nameof(Numero));
                }
            }
        }

        public string Complemento
        {
            get => _complemento;
            set
            {
                if (_complemento != value)
                {
                    _complemento = value;
                    OnPropertyChanged(nameof(Complemento));
                }
            }
        }

        public string Bairro
        {
            get => _bairro;
            set
            {
                if (_bairro != value)
                {
                    _bairro = value;
                    OnPropertyChanged(nameof(Bairro));
                }
            }
        }

        public string Cidade
        {
            get => _cidade;
            set
            {
                if (_cidade != value)
                {
                    _cidade = value;
                    OnPropertyChanged(nameof(Cidade));
                }
            }
        }

        public string Estado
        {
            get => _estado;
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }

        public string Endereco
        {
            get => _endereco;
            set
            {
                if (_endereco != value)
                {
                    _endereco = value;
                    OnPropertyChanged(nameof(Endereco));
                }
            }
        }

        public DateTime? DataNascimento
        {
            get => _dataNascimento;
            set
            {
                if (_dataNascimento != value)
                {
                    _dataNascimento = value;
                    OnPropertyChanged(nameof(DataNascimento));
                }
            }
        }

        public List<Pedido> Pedidos { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Pessoa()
        {
            Pedidos = new List<Pedido>();
        }
    }
}
