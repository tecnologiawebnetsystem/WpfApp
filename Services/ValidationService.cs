using System.Linq;
using System.Text.RegularExpressions;

namespace WpfApp.Services
{
    public static class ValidationService
    {
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        public static bool ValidarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            telefone = telefone.Replace("(", "").Replace(")", "")
                              .Replace("-", "").Replace(" ", "").Trim();

            if (telefone.Length != 10 && telefone.Length != 11)
                return false;

            if (!telefone.All(char.IsDigit))
                return false;

            if (telefone.Length == 11 && telefone[2] != '9')
                return false;

            return true;
        }

        public static bool ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            if (nome.Trim().Length < 3)
                return false;

            if (nome.Any(char.IsDigit))
                return false;

            return true;
        }

        public static bool ValidarFormatoCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            cep = cep.Replace("-", "").Replace(".", "").Trim();

            if (cep.Length != 8)
                return false;

            return cep.All(char.IsDigit);
        }

        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
