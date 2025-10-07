using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WpfApp.Services
{
    public class CepService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ViaCepUrl = "https://viacep.com.br/ws/{0}/json/";

        public async Task<EnderecoViaCep> BuscarEnderecoPorCep(string cep)
        {
            try
            {
                cep = cep?.Replace("-", "").Replace(".", "").Trim();

                if (string.IsNullOrEmpty(cep) || cep.Length != 8)
                {
                    return null;
                }

                var url = string.Format(ViaCepUrl, cep);
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                
                var endereco = JsonConvert.DeserializeObject<EnderecoViaCep>(json);

                if (endereco?.Erro == true)
                {
                    return null;
                }

                return endereco;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class EnderecoViaCep
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string Uf { get; set; }
        public bool Erro { get; set; }

        public string FormatarEnderecoSemNumero()
        {
            var partes = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(Logradouro))
                partes.Add(Logradouro);

            if (!string.IsNullOrEmpty(Bairro))
                partes.Add(Bairro);

            if (!string.IsNullOrEmpty(Localidade))
                partes.Add(Localidade);

            if (!string.IsNullOrEmpty(Uf))
                partes.Add(Uf);

            return string.Join(", ", partes);
        }
    }
}
