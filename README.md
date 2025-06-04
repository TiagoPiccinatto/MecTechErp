# MecTecERP - Sistema ERP para Oficina Mecânica

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 Sobre o Projeto

O **MecTecERP** é um sistema ERP completo desenvolvido especificamente para gestão de oficinas mecânicas. Construído com **Blazor Server (.NET 8)**, oferece uma solução integrada e moderna para gerenciar todos os aspectos operacionais de uma oficina automotiva.

### ✨ Características Principais

- 🌐 **Interface Web Responsiva** - Mobile-first design
- 🌙 **Dark Mode** - Tema escuro disponível
- 🔐 **Segurança Robusta** - Autenticação e autorização por níveis
- 📊 **Relatórios Avançados** - Exportação em PDF/Excel
- 🔄 **Módulos Integrados** - Fluxo completo de trabalho
- 📱 **Mobile-First** - Otimizado para dispositivos móveis

## 🚀 Tecnologias Utilizadas

- **Frontend**: Blazor Server Components
- **Backend**: ASP.NET Core 8
- **Banco de Dados**: SQL Server
- **ORM**: Dapper (Micro ORM)
- **Autenticação**: ASP.NET Core Identity
- **UI**: Bootstrap 5 (última versão)
- **Design**: Layout moderno SaaS, responsivo, mobile-first
- **Ícones**: FontAwesome ou Bootstrap Icons
- **Gráficos**: Chart.js integrado
- **Relatórios**: iTextSharp (PDF) + EPPlus (Excel)
- **APIs**: Integração com API dos Correios

## 📦 Módulos do Sistema

### 🏢 Gestão de Clientes
- Cadastro completo (PF/PJ)
- Histórico de atendimentos
- Integração com API dos Correios
- Controle de status

### 🚗 Controle de Veículos
- Vinculação com clientes
- Histórico de manutenções
- Upload de fotos
- Controle de quilometragem

### 🔧 Ordem de Serviço (OS)
- Criação e gestão de OS
- Orçamento automático
- Aprovação via email/WhatsApp
- Controle de status completo
- Laudo técnico com fotos

### 📦 Controle de Estoque
- Gestão de peças e insumos
- Movimentação automática
- Alertas de estoque baixo
- Relatórios de giro

### 💰 Módulo Financeiro
- Contas a pagar/receber
- Fluxo de caixa
- Controle de inadimplência
- Relatórios financeiros

### 📅 Agenda/Agendamento
- Agenda visual
- Notificações automáticas
- Controle por mecânico/box
- Integração Google Calendar

### 👥 Controle de Funcionários
- Gestão de usuários
- Níveis de permissão
- Controle de comissões
- Relatórios de produtividade

### 📊 Dashboard e Relatórios
- Indicadores em tempo real
- Gráficos dinâmicos
- Relatórios customizáveis
- Exportação múltiplos formatos

## 🛠️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-editions-express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🚀 Instalação e Configuração

### 1. Clone o Repositório

```bash
git clone https://github.com/seu-usuario/MecTecERP.git
cd MecTecERP
```

### 2. Configuração do Banco de Dados

1. **Instale o SQL Server** (se não tiver instalado)
2. **Configure a string de conexão** no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MecTecERP;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Configuração do Banco de Dados

```bash
# Restaurar pacotes NuGet
dotnet restore

# Executar scripts SQL para criar estrutura do banco
# Os scripts estão localizados em: /Database/Scripts/
# Execute na seguinte ordem:
# 1. CreateDatabase.sql
# 2. CreateTables.sql
# 3. CreateIndexes.sql
# 4. InsertInitialData.sql
```

**Nota**: Como utilizamos Dapper, não há migrations automáticas. Os scripts SQL devem ser executados manualmente no SQL Server Management Studio ou via sqlcmd.

### 4. Executar a Aplicação

```bash
# Executar em modo desenvolvimento
dotnet run

# Ou usando o Visual Studio
# Pressione F5 ou Ctrl+F5
```

### 5. Acesso Inicial

- **URL**: `https://localhost:5001` ou `http://localhost:5000`
- **Usuário Padrão**: `admin@mectecerp.com`
- **Senha Padrão**: `Admin@123`

> ⚠️ **Importante**: Altere a senha padrão no primeiro acesso!

## 🔧 Configuração Avançada

### Configuração de Email

No arquivo `appsettings.json`, configure o SMTP:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "seu-email@gmail.com",
    "SmtpPassword": "sua-senha-app",
    "FromEmail": "seu-email@gmail.com",
    "FromName": "MecTecERP"
  }
}
```

### Configuração da API dos Correios

```json
{
  "ExternalApis": {
    "CorreiosApi": {
      "BaseUrl": "https://viacep.com.br/ws/",
      "Timeout": 30
    }
  }
}
```

### Configuração de Upload de Arquivos

```json
{
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".pdf"],
    "UploadPath": "wwwroot/uploads"
  }
}
```

## 🏗️ Estrutura do Projeto

```
MecTecERP/
├── 📁 src/
│   ├── 📁 MecTecERP.Web/              # Aplicação Blazor Server
│   │   ├── 📁 Components/             # Componentes Blazor
│   │   ├── 📁 Pages/                  # Páginas da aplicação
│   │   ├── 📁 Services/               # Serviços da aplicação
│   │   └── 📁 wwwroot/                # Arquivos estáticos
│   ├── 📁 MecTecERP.Domain/           # Entidades e regras de negócio
│   ├── 📁 MecTecERP.Infrastructure/   # Acesso a dados e serviços
│   └── 📁 MecTecERP.Shared/           # DTOs e utilitários
├── 📁 tests/                          # Testes automatizados
├── 📁 docs/                           # Documentação
├── 📄 README.md
└── 📄 DOCUMENTACAO.md
```

## 🧪 Executando Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "Category=Unit"
```

## 📦 Build e Deploy

### Build para Produção

```bash
# Build da aplicação
dotnet build --configuration Release

# Publicar aplicação
dotnet publish --configuration Release --output ./publish
```

### Deploy no IIS

1. **Instale o ASP.NET Core Hosting Bundle** no servidor
2. **Configure o IIS** com o site apontando para a pasta de publicação
3. **Configure a string de conexão** no servidor
4. **Configure permissões** para a pasta de uploads

### Deploy no Azure

1. **Crie um App Service** no Azure Portal
2. **Configure a string de conexão** nas configurações do App Service
3. **Publique via Visual Studio** ou Azure DevOps

## 🔐 Segurança

### Níveis de Acesso

- **👑 Administrador**: Acesso total ao sistema
- **👨‍💼 Gerente**: Relatórios e configurações (exceto usuários)
- **🔧 Mecânico**: OS, estoque e agenda
- **📞 Atendente**: Clientes, veículos e agendamento

### Boas Práticas Implementadas

- ✅ Autenticação robusta com ASP.NET Identity
- ✅ Autorização baseada em roles
- ✅ Validação de entrada em todas as operações
- ✅ Proteção contra SQL Injection
- ✅ Proteção contra XSS
- ✅ HTTPS obrigatório
- ✅ Auditoria completa de ações

## 📊 Relatórios Disponíveis

### Operacionais
- 📋 OS por período e status
- 🔧 Serviços mais realizados
- ⏱️ Tempo médio de execução
- 👨‍🔧 Produtividade por mecânico

### Financeiros
- 💰 Faturamento por período
- 👥 Faturamento por cliente
- 🔧 Faturamento por serviço
- 📈 Análise de lucratividade

### Estoque
- 🔩 Peças mais utilizadas
- 📦 Movimentação de estoque
- 🔄 Giro de estoque
- ⚠️ Itens em falta

## 🤝 Contribuindo

1. **Fork** o projeto
2. **Crie** uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. **Abra** um Pull Request

## 📝 Convenções de Código

- Use **PascalCase** para classes, métodos e propriedades
- Use **camelCase** para variáveis locais e parâmetros
- Use **kebab-case** para arquivos CSS
- Mantenha métodos com no máximo 20 linhas
- Sempre adicione comentários XML para métodos públicos
- Siga os princípios SOLID

## 🐛 Reportando Bugs

Para reportar bugs, por favor:

1. **Verifique** se o bug já foi reportado
2. **Crie** uma issue detalhada
3. **Inclua** passos para reproduzir
4. **Adicione** screenshots se necessário
5. **Especifique** o ambiente (OS, browser, versão)

## 📚 Documentação Adicional

- 📖 [Documentação Completa](./DOCUMENTACAO.md)
- 🏗️ [Guia de Arquitetura](./docs/ARQUITETURA.md)
- 🔧 [Manual de Instalação](./docs/INSTALACAO.md)
- 👥 [Manual do Usuário](./docs/MANUAL_USUARIO.md)
- 🔌 [Guia de APIs](./docs/API.md)

## 🆘 Suporte

- 📧 **Email**: suporte@mectecerp.com
- 💬 **Discord**: [Servidor da Comunidade](https://discord.gg/mectecerp)
- 📱 **WhatsApp**: +55 (11) 99999-9999
- 🐛 **Issues**: [GitHub Issues](https://github.com/seu-usuario/MecTecERP/issues)

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- Equipe do ASP.NET Core
- Comunidade Blazor
- Contribuidores do projeto
- Beta testers

## 🗺️ Roadmap

### 🎯 Versão 1.0 (MVP) - Q1 2024
- [x] Módulos básicos funcionais
- [x] Interface responsiva
- [x] Relatórios essenciais
- [x] Segurança básica

### 🚀 Versão 1.1 - Q2 2024
- [ ] Melhorias de UX/UI
- [ ] Relatórios avançados
- [ ] Integrações básicas
- [ ] App mobile (PWA)

### 🌟 Versão 2.0 - Q3 2024
- [ ] App mobile nativo (MAUI)
- [ ] Integrações avançadas
- [ ] BI e analytics
- [ ] API pública
- [ ] Multi-tenancy

---

<div align="center">

**Desenvolvido com ❤️ para oficinas mecânicas**

[⭐ Star no GitHub](https://github.com/seu-usuario/MecTecERP) • [🐛 Reportar Bug](https://github.com/seu-usuario/MecTecERP/issues) • [💡 Solicitar Feature](https://github.com/seu-usuario/MecTecERP/issues)

</div>