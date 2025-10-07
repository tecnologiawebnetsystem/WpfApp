# Sistema de Gestão Profissional

Fala pessoal! Esse aqui é um sistema completo de gerenciamento feito em WPF.

## Tecnologias que utilizei.

- **WPF** - Pra fazer a interface gráfica (aquelas janelas bonitas)
- **C#** - A linguagem de programação
- **.NET Framework 4.6**
- **XML** - Pra salvar os dados
- **ViaCEP API** - Pra buscar endereço

### Abrir no Visual Studio

1. Abre o Visual Studio
2. Clica em "Abrir um projeto ou solução"
3. Navega até a pasta do projeto e abre o arquivo `WpfApp.sln`

### Restaurar os pacotes NuGet

Quando você abrir o projeto, o Visual Studio vai automaticamente baixar os pacotes necessários. 

1. Clica com o botão direito no projeto (no Solution Explorer)
2. Clica em "Restaurar Pacotes NuGet"
3. Daí você espera terminar

**Pacotes que o projeto está usando:**
- `Newtonsoft.Json` (versão 13.0.3) - Pra trabalhar com JSON da API de CEP
- `System.Numerics.Vectors` (versão 4.5.0)
- `System.Runtime.CompilerServices.Unsafe` (versão 6.0.0)
- `System.Text.Encodings.Web` (versão 8.0.0)
- `System.Threading.Tasks.Extensions` (versão 4.5.4)
- `System.ValueTuple` (versão 4.5.0)

Não precisa instalar nada manualmente, o Visual Studio vai fazer tudo sozinho!

### Compilar e rodar

1. Antes de Rodar o sistema, compila o projeto pra garantir que tá tudo certo:
   - Menu "Build" → "Compilar ou Recompilar"
   - Espera terminar (não pod ter erros nenhum)
2. É só apertar **F5** ou só clicar no "Iniciar"
3. Pronto! O sistema vai abrir em perfeito estado.

Se der algum erro de compilação, tenta:
- Limpar a solução: Menu "Build" → "Limpar Solução"
- Recompilar: Menu "Build" → "Recompilar Solução"

## Onde vai estar os arquivos salvos?

Tudo é salvo em arquivos XML na pasta bin\debug\Data:

Esses arquivos já estão com alguns dados de exemplo pra você testar:
- `pessoas.xml` - Todas as pessoas cadastradas
- `produtos.xml` - Todos os produtos cadastrados
- `pedidos.xml` - Todos os pedidos realizados

**Importante:** Esses arquivos são criados automaticamente quando você compila o projeto. E se você apagar, o sistema cria de novo com dados de exemplo.

## Problemas comuns e soluções

### "Não consigo compilar o projeto"
- Tenta limpar e recompilar: Menu "Build" → "Limpar Solução" → "Recompilar Solução"
- Verifica se os pacotes NuGet foram restaurados

### "Dá erro ao abrir uma tela"
- Verifica se os arquivos XML existem na pasta `Data/`
- Tenta recompilar o projeto

### "A busca de CEP não funciona"
- Verifica se você está conectado na internet
- A API do ViaCEP pode estar fora do ar;

