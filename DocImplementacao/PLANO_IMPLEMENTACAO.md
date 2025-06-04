# Plano de Implementação - MecTecERP

## Visão Geral da Implementação

Este documento detalha o plano de implementação do sistema MecTecERP, dividido em fases e módulos específicos. A implementação seguirá uma abordagem incremental, priorizando funcionalidades essenciais e construindo uma base sólida para expansões futuras.

## Estratégia de Desenvolvimento

### Metodologia
- **Abordagem**: Desenvolvimento incremental por módulos
- **Arquitetura**: Monolítica com separação clara de responsabilidades
- **Padrões**: Clean Architecture, SOLID, Repository Pattern
- **Testes**: TDD (Test-Driven Development) para regras críticas
- **Versionamento**: Git Flow com branches por feature

### Tecnologias e Ferramentas
- **IDE**: Visual Studio 2022 ou VS Code
- **Framework**: .NET 8 com Blazor Server
- **ORM**: Dapper para performance otimizada
- **Banco**: SQL Server 2019+
- **UI**: Bootstrap 5 + FontAwesome
- **Gráficos**: Chart.js
- **Testes**: xUnit + Moq
- **CI/CD**: Azure DevOps ou GitHub Actions

## Fases de Implementação

### 🏗️ Fase 1: Infraestrutura e Base (Semanas 1-2)

**Objetivos:**
- Configurar estrutura do projeto
- Implementar autenticação e autorização
- Criar layout base responsivo
- Configurar banco de dados

**Entregáveis:**
- Estrutura de projeto configurada
- Sistema de login funcional
- Layout responsivo com sidebar e topbar
- Scripts de banco de dados
- Configuração de CI/CD básica

### 🎯 Fase 2: Módulos Essenciais (Semanas 3-6)

**Objetivos:**
- Implementar cadastros básicos
- Criar funcionalidades core do negócio
- Estabelecer fluxo principal de trabalho

**Módulos:**
1. **Cadastro de Clientes** (Semana 3)
2. **Cadastro de Veículos** (Semana 4)
3. **Ordem de Serviço - Básico** (Semanas 5-6)

### 📊 Fase 3: Gestão e Controle (Semanas 7-10)

**Objetivos:**
- Implementar controles operacionais
- Adicionar funcionalidades de gestão
- Criar relatórios básicos

**Módulos:**
1. **Controle de Estoque** (Semanas 7-8)
2. **Módulo Financeiro** (Semanas 9-10)
3. **Dashboard Básico** (Semana 10)

### 🚀 Fase 4: Funcionalidades Avançadas (Semanas 11-14)

**Objetivos:**
- Implementar funcionalidades avançadas
- Adicionar integrações
- Otimizar performance

**Módulos:**
1. **Agenda/Agendamento** (Semana 11)
2. **Relatórios Avançados** (Semana 12)
3. **Controle de Funcionários** (Semana 13)
4. **Configurações e Auditoria** (Semana 14)

### 🔧 Fase 5: Refinamento e Deploy (Semanas 15-16)

**Objetivos:**
- Testes finais e correções
- Otimizações de performance
- Preparação para produção
- Documentação final

## Detalhamento por Módulo

### 1. Infraestrutura e Base

#### 1.1 Estrutura do Projeto
```
MecTecERP/
├── src/
│   ├── MecTecERP.Web/              # Aplicação Blazor Server
│   │   ├── Components/             # Componentes reutilizáveis
│   │   │   ├── Layout/             # Layout components
│   │   │   ├── Forms/              # Form components
│   │   │   ├── Tables/             # Table components
│   │   │   └── Charts/             # Chart components
│   │   ├── Pages/                  # Páginas Blazor
│   │   │   ├── Auth/               # Páginas de autenticação
│   │   │   ├── Dashboard/          # Dashboard
│   │   │   ├── Clientes/           # Páginas de clientes
│   │   │   ├── Veiculos/           # Páginas de veículos
│   │   │   ├── OrdemServico/       # Páginas de OS
│   │   │   ├── Estoque/            # Páginas de estoque
│   │   │   ├── Financeiro/         # Páginas financeiras
│   │   │   ├── Agenda/             # Páginas de agenda
│   │   │   ├── Funcionarios/       # Páginas de funcionários
│   │   │   ├── Relatorios/         # Páginas de relatórios
│   │   │   └── Configuracoes/      # Páginas de configurações
│   │   ├── Services/               # Serviços da aplicação
│   │   ├── Models/                 # ViewModels e DTOs
│   │   └── wwwroot/                # Arquivos estáticos
│   ├── MecTecERP.Domain/           # Entidades e regras de negócio
│   │   ├── Entities/               # Entidades do domínio
│   │   ├── Enums/                  # Enumerações
│   │   ├── Interfaces/             # Interfaces do domínio
│   │   └── Services/               # Serviços de domínio
│   ├── MecTecERP.Infrastructure/   # Acesso a dados e serviços
│   │   ├── Data/                   # Contexto e configurações
│   │   ├── Repositories/           # Repositórios (Dapper)
│   │   ├── Services/               # Serviços de infraestrutura
│   │   └── External/               # APIs externas
│   └── MecTecERP.Shared/           # DTOs e utilitários
│       ├── DTOs/                   # Data Transfer Objects
│       ├── Extensions/             # Extension methods
│       ├── Helpers/                # Classes auxiliares
│       └── Constants/              # Constantes
├── tests/                          # Testes automatizados
│   ├── MecTecERP.UnitTests/        # Testes unitários
│   ├── MecTecERP.IntegrationTests/ # Testes de integração
│   └── MecTecERP.E2ETests/         # Testes end-to-end
├── database/                       # Scripts de banco
│   ├── Scripts/                    # Scripts SQL
│   └── Migrations/                 # Versionamento manual
└── docs/                           # Documentação
```

#### 1.2 Configuração do Banco de Dados

**Scripts SQL necessários:**
1. `CreateDatabase.sql` - Criação do banco
2. `CreateTables.sql` - Criação das tabelas
3. `CreateIndexes.sql` - Índices para performance
4. `CreateViews.sql` - Views para relatórios
5. `CreateProcedures.sql` - Stored procedures
6. `InsertInitialData.sql` - Dados iniciais

#### 1.3 Autenticação e Autorização

**Implementação:**
- ASP.NET Core Identity para autenticação
- Roles customizadas para autorização
- Middleware para controle de sessão
- Páginas de login/logout responsivas

**Roles do Sistema:**
- `Administrador` - Acesso total
- `Gerente` - Relatórios e configurações
- `Mecanico` - OS, estoque e agenda
- `Atendente` - Clientes, veículos e agendamento

#### 1.4 Layout Base

**Componentes principais:**
- `MainLayout.razor` - Layout principal
- `Sidebar.razor` - Barra lateral navegação
- `TopBar.razor` - Barra superior
- `ThemeToggle.razor` - Alternador de tema
- `UserMenu.razor` - Menu do usuário

**Características:**
- Responsivo (Bootstrap 5)
- Dark mode com localStorage
- Sidebar colapsável
- Breadcrumb navigation
- Toast notifications

### 2. Módulo: Cadastro de Clientes

#### 2.1 Entidades
```csharp
public class Cliente
{
    public int Id { get; set; }
    public string TipoCliente { get; set; } // PF ou PJ
    public string NomeRazaoSocial { get; set; }
    public string CpfCnpj { get; set; }
    public string Telefone1 { get; set; }
    public string Telefone2 { get; set; }
    public string Email { get; set; }
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Uf { get; set; }
    public string Observacoes { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public int UsuarioCadastro { get; set; }
    public int? UsuarioUltimaAtualizacao { get; set; }
}
```

#### 2.2 Funcionalidades
- **CRUD completo** de clientes
- **Validação** de CPF/CNPJ
- **Integração** com API dos Correios
- **Busca** por nome, CPF/CNPJ, telefone
- **Filtros** por status, tipo, cidade
- **Histórico** de atendimentos
- **Listagem** de veículos vinculados

#### 2.3 Páginas
- `Index.razor` - Listagem de clientes
- `Create.razor` - Cadastro de cliente
- `Edit.razor` - Edição de cliente
- `Details.razor` - Detalhes do cliente
- `Delete.razor` - Confirmação de exclusão

#### 2.4 Componentes
- `ClienteForm.razor` - Formulário de cliente
- `ClienteTable.razor` - Tabela de clientes
- `ClienteCard.razor` - Card de cliente
- `CepLookup.razor` - Busca de CEP
- `CpfCnpjValidator.razor` - Validador de CPF/CNPJ

### 3. Módulo: Cadastro de Veículos

#### 3.1 Entidades
```csharp
public class Veiculo
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Placa { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int AnoFabricacao { get; set; }
    public int AnoModelo { get; set; }
    public string Cor { get; set; }
    public string Chassi { get; set; }
    public int KmAtual { get; set; }
    public string TipoCombustivel { get; set; }
    public string Foto { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public int UsuarioCadastro { get; set; }
    public int? UsuarioUltimaAtualizacao { get; set; }
    
    // Navigation Properties
    public Cliente Cliente { get; set; }
}
```

#### 3.2 Funcionalidades
- **CRUD completo** de veículos
- **Vinculação obrigatória** com cliente
- **Upload de fotos** do veículo
- **Validação** de placa (formato brasileiro)
- **Histórico** de manutenções
- **Controle** de quilometragem

### 4. Módulo: Ordem de Serviço

#### 4.1 Entidades Principais
```csharp
public class OrdemServico
{
    public int Id { get; set; }
    public string Numero { get; set; }
    public int ClienteId { get; set; }
    public int VeiculoId { get; set; }
    public DateTime DataEntrada { get; set; }
    public string ProblemaRelatado { get; set; }
    public string DiagnosticoTecnico { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime? PrevisaoEntrega { get; set; }
    public StatusOS Status { get; set; }
    public string ObservacoesInternas { get; set; }
    public string ObservacoesCliente { get; set; }
    public int MecanicoResponsavel { get; set; }
    public bool OrcamentoAprovado { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public int UsuarioCadastro { get; set; }
    
    // Navigation Properties
    public Cliente Cliente { get; set; }
    public Veiculo Veiculo { get; set; }
    public List<OSItem> Itens { get; set; }
    public List<OSFoto> Fotos { get; set; }
}

public class OSItem
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public TipoItem Tipo { get; set; } // Servico ou Peca
    public int? PecaId { get; set; }
    public int? ServicoId { get; set; }
    public string Descricao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}

public enum StatusOS
{
    Protocolo = 1,
    Orcamento = 2,
    Aprovado = 3,
    Execucao = 4,
    AguardandoPeca = 5,
    Finalizado = 6,
    Cancelado = 7
}
```

#### 4.2 Funcionalidades Principais
- **Criação** de OS com cliente e veículo
- **Orçamento automático** com itens
- **Controle de status** rigoroso
- **Upload de fotos** para laudo
- **Aprovação** via email/WhatsApp
- **Impressão** de OS
- **Histórico** de alterações

## Cronograma Detalhado

### Semana 1: Configuração Inicial
- **Dia 1-2**: Configuração do projeto e estrutura
- **Dia 3-4**: Scripts de banco de dados
- **Dia 5**: Configuração de autenticação

### Semana 2: Layout e Base
- **Dia 1-2**: Layout responsivo base
- **Dia 3-4**: Componentes comuns
- **Dia 5**: Dark mode e temas

### Semana 3: Cadastro de Clientes
- **Dia 1-2**: Entidades e repositórios
- **Dia 3-4**: Páginas e componentes
- **Dia 5**: Integração API Correios

### Semana 4: Cadastro de Veículos
- **Dia 1-2**: Entidades e repositórios
- **Dia 3-4**: Páginas e componentes
- **Dia 5**: Upload de fotos

### Semana 5-6: Ordem de Serviço
- **Semana 5**: Estrutura básica e CRUD
- **Semana 6**: Orçamento e aprovação

### Semana 7-8: Controle de Estoque
- **Semana 7**: Cadastros e movimentação
- **Semana 8**: Alertas e relatórios

### Semana 9-10: Módulo Financeiro
- **Semana 9**: Contas a pagar/receber
- **Semana 10**: Fluxo de caixa

### Semana 11: Agenda
- Calendário visual e agendamentos

### Semana 12: Relatórios
- Relatórios operacionais e financeiros

### Semana 13: Funcionários
- Cadastro e controle de acesso

### Semana 14: Configurações
- Parâmetros e auditoria

### Semana 15-16: Finalização
- Testes, otimizações e deploy

## Critérios de Qualidade

### Performance
- Tempo de resposta < 2 segundos
- Paginação em listagens > 100 registros
- Lazy loading de dados relacionados
- Cache de consultas frequentes

### Segurança
- Validação de entrada em todos os formulários
- Autorização em todas as ações
- Log de auditoria completo
- Proteção contra ataques comuns

### Usabilidade
- Interface responsiva em todos os dispositivos
- Feedback visual para todas as ações
- Navegação intuitiva
- Acessibilidade básica

### Manutenibilidade
- Código limpo e documentado
- Testes unitários para regras críticas
- Separação clara de responsabilidades
- Padrões de código consistentes

## Riscos e Mitigações

### Riscos Técnicos
- **Performance com Dapper**: Mitigação através de queries otimizadas
- **Complexidade do layout**: Uso de componentes reutilizáveis
- **Integração APIs**: Implementação de fallbacks

### Riscos de Prazo
- **Escopo creep**: Definição clara de MVP
- **Complexidade subestimada**: Buffer de 20% no cronograma
- **Dependências externas**: Identificação antecipada

### Riscos de Qualidade
- **Bugs em produção**: Testes automatizados
- **Performance degradada**: Monitoramento contínuo
- **Usabilidade ruim**: Testes com usuários

## Próximos Passos

1. **Aprovação do plano** pela equipe
2. **Configuração do ambiente** de desenvolvimento
3. **Criação do repositório** Git
4. **Início da implementação** da Fase 1
5. **Configuração de CI/CD** básico

Este plano será atualizado conforme o progresso da implementação e feedback recebido durante o desenvolvimento.