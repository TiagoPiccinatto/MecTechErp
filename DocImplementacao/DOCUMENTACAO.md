# MecTecERP - Sistema ERP para Oficina Mecânica

## Visão Geral

O **MecTecERP** é um sistema ERP completo desenvolvido especificamente para gestão de oficinas mecânicas, utilizando tecnologia **Blazor Server (.NET 8)** com arquitetura monolítica. O sistema oferece uma solução integrada para gerenciar todos os aspectos operacionais de uma oficina, desde o atendimento ao cliente até o controle financeiro.

### Características Principais

- **Tecnologia**: Blazor Server (.NET 8)
- **Arquitetura**: Monolítica
- **Interface**: Web responsiva, mobile-first
- **Design**: Dark mode disponível
- **Segurança**: Autenticação robusta com níveis de permissão
- **Relatórios**: Exportação em PDF/Excel
- **Integração**: Módulos completamente integrados

## Arquitetura do Sistema

### Stack Tecnológico

- **Frontend**: Blazor Server Components
- **Backend**: ASP.NET Core 8
- **Banco de Dados**: SQL Server
- **ORM**: Dapper (Micro ORM)
- **Autenticação**: ASP.NET Core Identity
- **UI Framework**: Bootstrap 5 (última versão)
- **Design**: Layout moderno SaaS, responsivo, mobile-first
- **Ícones**: FontAwesome ou Bootstrap Icons
- **Gráficos**: Chart.js integrado
- **Relatórios**: iTextSharp (PDF) + EPPlus (Excel)
- **APIs Externas**: API dos Correios para CEP

### Design e Layout Moderno

#### Características Visuais
- **Estilo**: Layout limpo com cara de sistema SaaS moderno
- **Responsividade**: Mobile-first design
- **Tema**: Dark mode opcional com troca instantânea
- **Visual**: Design "flat" sem excesso de sombras
- **Cards**: Bordas arredondadas (Bootstrap 5)
- **Cores**: Bootstrap 5 Dark Mode para alternância de tema

#### Estrutura de Layout

**1. Barra Lateral (Sidebar)**
- Fixa à esquerda, recolhível para ícones no mobile/tablet
- Cor de fundo: cinza escuro (#212529) no dark mode, branco/cinza claro no light
- Ícones grandes e identificáveis (FontAwesome ou Bootstrap Icons)
- Menus principais:
  - Dashboard
  - Clientes
  - Veículos
  - Ordens de Serviço
  - Estoque
  - Financeiro
  - Agenda
  - Funcionários
  - Relatórios
  - Configurações

**2. Topbar (Header)**
- Ocupa topo da tela, sticky
- Nome/logotipo da oficina à esquerda
- À direita: avatar do usuário (menu de perfil/sair), botão de notificação, troca de tema
- Barra de busca global opcional

**3. Conteúdo Principal**
- Espaço branco, visual "flat"
- Cards para separar blocos de informação
- Botões de ação claros ("+ Novo", "Editar", "Excluir", "Detalhes")
- Tabelas responsivas com filtros acima das listas
- Paginação no rodapé das tabelas
- Badges para status (OS em andamento, aguardando peça, finalizada)
- Gráficos no dashboard (Chart.js integrado)

**4. Mobile**
- Sidebar vira menu hamburguer
- Cards empilham verticalmente
- Ações principais em destaque (botão flutuante no canto inferior direito)

**5. Dark Mode**
- Switch no topo para alternar
- Cores Bootstrap: bg-dark, text-light, e variantes
- Evitar fundos 100% pretos para não cansar a vista

#### Organização de Tela (Dashboard)
```
+----------------------+----------------------------------+
|  Sidebar            |    Topbar                         |
|  (col 2, fixo)      +-----------------------------------+
|                     |   Cards com indicadores:          |
|  Ícones dos         |   [OS abertas] [Faturamento mês]  |
|  módulos            |   [Peças em falta] [Agendamentos] |
+---------------------+-----------------------------------+
|   Menu colapsa no mobile, conteúdo empilha verticalmente |
+---------------------------------------------------------+
```

### Boas Práticas UI/UX

#### Princípios de Usabilidade
- **Evitar poluição visual**: Muitos dados, mas agrupados por cards
- **Botões principais**: Sempre visíveis no topo ou como FAB no mobile
- **Ícones**: Usar sempre que possível para facilitar identificação rápida
- **Tabelas responsivas**: Com ações rápidas em dropdown
- **Inputs grandes**: Fáceis de clicar no mobile
- **Toast**: Para feedbacks rápidos (sucesso, erro, alerta)
- **Modais**: Para confirmações e cadastros rápidos

#### Diretrizes Específicas para ERP Oficina

**1. Acesso Rápido**
- Menus principais sempre visíveis no sidebar, sem submenus ocultos demais
- Botão "+ Novo" destacado em cada tela de cadastro (cliente, veículo, OS, etc.)
- Busca global na topbar para acessar clientes, veículos ou OS em poucos cliques

**2. Fluxo de Trabalho Simples**
- Ao salvar um cliente/veículo, exibir opção para criar OS imediatamente
- OS: após lançamento do orçamento, botão para enviar por WhatsApp ou e-mail
- Permitir duplicar OS para serviços recorrentes

**3. Feedback Imediato**
- Toasts para avisos rápidos de sucesso, erro ou alerta
- Loading spinner sempre que houver ação que demore

**4. Tabelas e Listas**
- Filtros rápidos no topo (ex: filtrar OS por status, data, cliente)
- Colunas reordenáveis e ocultáveis conforme preferência do usuário
- Botões de ação (ver, editar, excluir) agrupados em dropdown no final de cada linha
- Seleção múltipla de registros para ações em lote

**5. Responsividade Real**
- Sidebar colapsável; no mobile, vira menu hamburguer acessível com o polegar
- Cards empilhados no mobile, botões grandes e fáceis de clicar
- Inputs e áreas clicáveis com espaçamento adequado para toque

**6. Dark Mode**
- Troca de tema instantânea (sem reload)
- Preferência salva em localStorage/cookie
- Feedback visual claro para manter contraste e legibilidade

**7. Acessibilidade**
- Navegação por teclado (tab, enter, setas) em todos os formulários e tabelas
- Labels e placeholders claros
- Alt texts em ícones e imagens

**8. Confirmação de Ações Críticas**
- Modais de confirmação antes de excluir ou cancelar OS, clientes ou veículos
- Destaque em vermelho para ações destrutivas

**9. Retorno ao Usuário**
- Após criar/editar registro, redirecionar para tela de detalhes do item
- Opções de próxima ação (ex: imprimir, enviar, criar novo)

**10. Documentação Visual**
- Tooltip em ícones, campos e botões para explicar função
- Ajuda rápida (FAQ ou tutorial) acessível do dashboard

### Estrutura de Camadas

```
MecTecERP/
├── Presentation/           # Blazor Server Pages e Components
├── Application/            # Serviços de Aplicação e DTOs
├── Domain/                 # Entidades e Regras de Negócio
├── Infrastructure/         # Acesso a Dados e Serviços Externos (Dapper)
└── Shared/                 # Utilitários e Constantes
```

## Regras de Negócio

### Regras Fundamentais

#### 1. Cliente Obrigatório
- Não é possível abrir Ordem de Serviço (OS) sem cliente e veículo cadastrados
- Cada veículo deve estar vinculado a um cliente
- Cliente inativo não pode ter novas OS criadas

#### 2. Fluxo da Ordem de Serviço
- **Criação**: OS só pode ser criada nos status "Protocolo" ou "Orçamento"
- **Aprovação**: Só é possível alterar OS para "Execução" após aprovação do orçamento
- **Aprovação do Orçamento**: Pode ser aprovação manual no sistema ou via link enviado ao cliente
- **Finalização**: Status "Finalizado" só disponível após:
  - Todos os itens/serviços marcados como concluídos
  - Recebimento total confirmado
- **Proteção**: Não permite edição da OS após finalizada, apenas visualização ou duplicação
- **Cancelamento**: OS pode ser cancelada em qualquer status, exceto "Finalizado"

#### 3. Controle de Estoque
- **Reserva Automática**: Peças são reservadas automaticamente ao adicionar na OS
- **Baixa Automática**: Estoque é baixado automaticamente ao finalizar a OS
- **Validação**: Não permite usar peças com estoque insuficiente
- **Alertas**: Sistema alerta quando estoque atinge quantidade mínima

#### 4. Controle Financeiro
- **Geração Automática**: Contas a receber são geradas automaticamente ao finalizar OS
- **Vinculação**: Toda movimentação financeira deve estar vinculada a um documento (OS, compra, etc.)
- **Baixa**: Não permite baixa de valores superiores ao saldo em aberto
- **Estorno**: Permite estorno de baixas com justificativa

#### 5. Segurança e Auditoria
- **Log Completo**: Todas as ações são registradas com usuário, data/hora e IP
- **Permissões**: Cada ação é validada conforme nível de permissão do usuário
- **Backup**: Sistema realiza backup automático diário
- **Sessão**: Logout automático após período de inatividade

#### 6. Validações de Dados
- **CPF/CNPJ**: Validação de formato e dígitos verificadores
- **Placa**: Validação de formato brasileiro (antigo e Mercosul)
- **E-mail**: Validação de formato válido
- **Telefone**: Formatação automática conforme padrão brasileiro
- **CEP**: Integração com API dos Correios para validação e autocomplete

#### 7. Integridade Referencial
- **Exclusão Protegida**: Não permite excluir registros com dependências
- **Inativação**: Registros com dependências podem ser inativados
- **Cascata**: Algumas operações são executadas em cascata (ex: inativar cliente inativa seus veículos)

## Módulos do Sistema

### 1. Cadastro de Clientes

**Objetivo**: Gerenciar informações completas dos clientes da oficina.

**Funcionalidades**:
- Cadastro de pessoas físicas (CPF) e jurídicas (CNPJ)
- Validação automática de CPF/CNPJ
- Autocomplete de endereço via API dos Correios
- Histórico completo de atendimentos
- Listagem de veículos vinculados
- Status ativo/inativo
- Campo de observações

**Campos**:
- Nome/Razão Social
- CPF/CNPJ
- Telefone(s)
- E-mail
- Endereço completo (CEP, logradouro, número, complemento, bairro, cidade, UF)
- Observações
- Status (ativo/inativo)
- Data de cadastro
- Última atualização

### 2. Cadastro de Veículos

**Objetivo**: Controlar informações dos veículos atendidos na oficina.

**Funcionalidades**:
- Vinculação obrigatória com cliente
- Upload de fotos do veículo
- Histórico de manutenções
- Controle de quilometragem
- Status ativo/inativo

**Campos**:
- Placa (validação formato brasileiro)
- Marca
- Modelo
- Ano de fabricação
- Ano do modelo
- Cor
- Chassi
- Quilometragem atual
- Tipo de combustível
- Status (ativo/inativo)
- Foto(s)
- Cliente vinculado
- Data de cadastro

### 3. Ordem de Serviço (OS)

**Objetivo**: Gerenciar todo o ciclo de vida dos serviços prestados.

**Funcionalidades**:
- Criação e edição de OS
- Orçamento automático com itens de serviço e peças
- Upload de fotos para laudo técnico
- Aprovação de orçamento via e-mail/WhatsApp
- Controle de status da OS
- Histórico de atualizações (log de ações)
- Impressão e exportação
- Notificações automáticas

**Status da OS**:
- Protocolo
- Orçamento
- Aprovado
- Em execução
- Aguardando peça
- Finalizado
- Cancelado

**Campos**:
- Número da OS (gerado automaticamente)
- Cliente
- Veículo
- Data de entrada
- Problema relatado
- Diagnóstico técnico
- Itens de serviço
- Peças utilizadas
- Valor total
- Previsão de entrega
- Status
- Observações internas
- Observações para o cliente
- Fotos do laudo
- Mecânico responsável

### 4. Controle de Estoque

**Objetivo**: Gerenciar peças e insumos da oficina.

**Funcionalidades**:
- Cadastro de peças e insumos
- Controle de movimentação (entrada/saída)
- Alertas de estoque baixo
- Relatório de giro de peças
- Inventário
- Ajustes de estoque
- Localização física no estoque

**Tipos de Movimentação**:
- Entrada: Compra, Devolução de cliente, Ajuste positivo
- Saída: Uso em OS, Venda avulsa, Perda, Ajuste negativo

**Campos**:
- Código da peça
- Nome/Descrição
- Categoria
- Fornecedor
- Custo unitário
- Preço de venda
- Quantidade em estoque
- Quantidade mínima
- Localização no estoque
- Status (ativo/inativo)
- Unidade de medida

### 5. Cadastro de Fornecedores

**Objetivo**: Gerenciar informações dos fornecedores de peças e serviços.

**Funcionalidades**:
- Cadastro completo de fornecedores
- Histórico de compras
- Avaliação de fornecedores
- Status ativo/inativo

**Campos**:
- Razão Social
- CNPJ
- Contato principal
- Telefone(s)
- E-mail
- Endereço completo
- Observações
- Status (ativo/inativo)
- Data de cadastro

### 6. Financeiro

**Objetivo**: Controlar toda a movimentação financeira da oficina.

**Funcionalidades**:

#### Contas a Pagar:
- Cadastro de fornecedores
- Despesas fixas e variáveis
- Impostos
- Controle de vencimentos
- Recorrência automática
- Baixa manual e automática

#### Contas a Receber:
- Faturamento de OS
- Vendas avulsas
- Controle de recebíveis
- Baixa manual e automática
- Controle de inadimplência

#### Fluxo de Caixa:
- Visualização diária, semanal e mensal
- Saldo projetado
- Categorização de receitas e despesas

**Relatórios Financeiros**:
- Extrato detalhado
- Relatório de inadimplência
- Movimentações por período
- Demonstrativo de resultados
- Exportação PDF/Excel

### 7. Agenda / Agendamento

**Objetivo**: Organizar e controlar os agendamentos de serviços.

**Funcionalidades**:
- Agenda visual (dia/semana/mês)
- Visualização por mecânico e box
- Agendamento de serviços
- Controle de prioridade
- Notificações automáticas
- Integração opcional com Google Agenda

**Tipos de Notificação**:
- Confirmação de agendamento
- Lembrete (24h antes)
- Conclusão do serviço
- Canais: E-mail, SMS, WhatsApp

**Campos do Agendamento**:
- Data e hora
- Cliente
- Veículo
- Serviço solicitado
- Mecânico responsável
- Box/Área
- Prioridade (baixa/normal/alta/urgente)
- Observações
- Status (agendado/confirmado/em andamento/concluído/cancelado)

### 8. Relatórios

**Objetivo**: Fornecer informações gerenciais através de relatórios customizáveis.

**Tipos de Relatórios**:

#### Operacionais:
- OS por período e status
- Serviços mais realizados
- Tempo médio de execução
- Produtividade por mecânico

#### Financeiros:
- Faturamento por período
- Faturamento por cliente
- Faturamento por tipo de serviço
- Análise de lucratividade

#### Estoque:
- Peças mais utilizadas
- Movimentação de estoque
- Giro de estoque
- Itens em falta

#### Clientes:
- Ranking de clientes
- Frequência de atendimento
- Análise de inadimplência

**Formatos de Exportação**:
- PDF (relatórios formatados)
- Excel (dados para análise)
- CSV (integração com outros sistemas)

### 9. Controle de Funcionários

**Objetivo**: Gerenciar funcionários e controlar acesso ao sistema.

**Funcionalidades**:
- Cadastro completo de funcionários
- Controle de acesso por níveis
- Comissão por OS
- Relatório de produtividade
- Histórico de ações no sistema

**Níveis de Permissão**:
- **Administrador**: Acesso total ao sistema
- **Gerente**: Acesso a relatórios e configurações (exceto usuários)
- **Mecânico**: Acesso a OS, estoque e agenda
- **Atendente**: Acesso a clientes, veículos e agendamento

**Campos**:
- Nome completo
- CPF
- Função/Cargo
- E-mail
- Telefone
- Nível de acesso
- Status (ativo/inativo)
- Data de admissão
- Comissão (%)

### 10. Login/Segurança

**Objetivo**: Garantir acesso seguro e controlado ao sistema.

**Funcionalidades de Segurança**:
- Autenticação por usuário e senha
- Recuperação de senha por e-mail
- Autenticação em 2 fatores (opcional)
- Controle de sessão
- Histórico de acessos
- Bloqueio após tentativas falhas
- Política de senhas
- Logout automático por inatividade

**Auditoria de Acesso**:
- Log de login/logout
- IP de acesso
- Tentativas de acesso negadas
- Ações realizadas por usuário

### 11. Dashboard

**Objetivo**: Fornecer visão geral e indicadores principais do negócio.

**Cards Informativos**:
- OS em aberto
- OS concluídas (período)
- Faturamento atual
- Agendamentos do dia
- Peças em falta
- Avisos importantes
- Contas a vencer

**Gráficos Dinâmicos**:
- Faturamento mensal
- Serviços mais realizados
- Movimentação de estoque
- Produtividade por mecânico
- Status das OS

**Filtros**:
- Período personalizado
- Por mecânico
- Por tipo de serviço
- Por cliente

### 12. Histórico/Auditoria

**Objetivo**: Manter registro completo de todas as ações realizadas no sistema.

**Ações Auditadas**:
- Cadastro/edição/exclusão de registros
- Alterações em OS
- Movimentações de estoque
- Operações financeiras
- Alterações de configuração
- Acessos ao sistema

**Informações do Log**:
- Data e hora da ação
- Usuário responsável
- Tipo de ação
- Registro afetado
- Valores anteriores e novos
- IP de origem

**Consultas**:
- Por período
- Por usuário
- Por tipo de ação
- Por módulo
- Exportação de logs

### 13. Configurações Gerais

**Objetivo**: Personalizar o sistema conforme necessidades da oficina.

**Configurações da Empresa**:
- Razão social
- CNPJ
- Endereço completo
- Telefones
- E-mail
- Logo da empresa
- Dados fiscais

**Parâmetros do Sistema**:
- Formatos de documentos
- Modelos de impressão (OS, recibos)
- Alertas automáticos
- Configurações de e-mail
- Integração WhatsApp
- Backup automático
- Tema padrão (claro/escuro)

**Configurações de Notificação**:
- Templates de e-mail
- Horários de envio
- Tipos de alerta
- Canais de comunicação

## Requisitos Técnicos

### Requisitos Funcionais

1. **Responsividade**: Interface adaptável para desktop, tablet e mobile
2. **Performance**: Tempo de resposta inferior a 2 segundos
3. **Usabilidade**: Interface intuitiva e fácil navegação
4. **Integração**: Módulos completamente integrados
5. **Relatórios**: Exportação em múltiplos formatos
6. **Segurança**: Controle de acesso robusto
7. **Auditoria**: Rastreabilidade completa de ações

### Requisitos Não Funcionais

1. **Disponibilidade**: 99.5% de uptime
2. **Escalabilidade**: Suporte a múltiplos usuários simultâneos
3. **Backup**: Backup automático diário
4. **Segurança**: Criptografia de dados sensíveis
5. **Manutenibilidade**: Código limpo e documentado
6. **Compatibilidade**: Suporte aos principais navegadores

## Fluxos Principais

### Fluxo de Atendimento

1. **Recepção do Cliente**
   - Cadastro/atualização de dados do cliente
   - Cadastro/atualização do veículo
   - Agendamento (se necessário)

2. **Criação da OS**
   - Registro do problema relatado
   - Diagnóstico inicial
   - Criação da OS

3. **Orçamento**
   - Diagnóstico detalhado
   - Seleção de serviços e peças
   - Cálculo automático do orçamento
   - Envio para aprovação do cliente

4. **Execução**
   - Aprovação do orçamento
   - Execução dos serviços
   - Atualização do status da OS
   - Baixa de peças do estoque

5. **Finalização**
   - Conclusão dos serviços
   - Geração da fatura
   - Entrega do veículo
   - Fechamento da OS

### Fluxo Financeiro

1. **Contas a Receber**
   - Geração automática via OS
   - Controle de vencimentos
   - Baixa de recebimentos
   - Controle de inadimplência

2. **Contas a Pagar**
   - Cadastro de despesas
   - Controle de vencimentos
   - Programação de pagamentos
   - Baixa de pagamentos

3. **Fluxo de Caixa**
   - Consolidação de recebimentos e pagamentos
   - Projeção de saldos
   - Relatórios gerenciais

## Integrações

### APIs Externas

1. **API dos Correios**
   - Consulta de CEP
   - Autocomplete de endereços

2. **Gateway de Pagamento** (futuro)
   - Processamento de cartões
   - PIX
   - Boletos

3. **WhatsApp Business API** (futuro)
   - Envio de notificações
   - Aprovação de orçamentos

4. **Google Calendar API** (opcional)
   - Sincronização de agendamentos

### Exportações

1. **PDF**
   - OS formatadas
   - Relatórios
   - Recibos

2. **Excel**
   - Dados para análise
   - Relatórios customizáveis

3. **CSV**
   - Integração com outros sistemas
   - Backup de dados

## Segurança

### Autenticação
- ASP.NET Core Identity
- Hash de senhas (bcrypt)
- Política de senhas robusta
- Recuperação segura de senha
- 2FA opcional

### Autorização
- Controle baseado em roles
- Permissões granulares
- Validação em todas as operações

### Proteção de Dados
- Criptografia de dados sensíveis
- Validação de entrada
- Proteção contra SQL Injection
- Proteção contra XSS
- HTTPS obrigatório

### Auditoria
- Log de todas as ações
- Rastreabilidade completa
- Retenção de logs configurável

## Performance

### Otimizações
- Lazy loading de dados
- Paginação em listagens
- Cache de consultas frequentes
- Compressão de resposta
- Minificação de assets

### Monitoramento
- Logs de performance
- Métricas de uso
- Alertas de degradação

## Deployment

### Ambientes
1. **Desenvolvimento**: Ambiente local
2. **Homologação**: Testes e validação
3. **Produção**: Ambiente final

### Infraestrutura
- **Servidor Web**: IIS ou Kestrel
- **Banco de Dados**: SQL Server
- **Backup**: Automático e incremental
- **SSL**: Certificado válido

## Roadmap

### Versão 1.0 (MVP)
- Módulos básicos funcionais
- Interface responsiva
- Relatórios essenciais
- Segurança básica

### Versão 1.1
- Melhorias de UX
- Relatórios avançados
- Integrações básicas

### Versão 2.0
- App mobile nativo
- Integrações avançadas
- BI e analytics
- API pública

## Conclusão

O **MecTecERP** representa uma solução completa e moderna para gestão de oficinas mecânicas, combinando tecnologia de ponta com funcionalidades específicas do setor. O sistema foi projetado para crescer junto com o negócio, oferecendo escalabilidade e flexibilidade para atender diferentes portes de oficina.

A arquitetura monolítica escolhida garante simplicidade de deployment e manutenção, enquanto o uso do Blazor Server proporciona uma experiência rica e responsiva para os usuários.

Com foco na usabilidade e integração completa entre módulos, o sistema promete aumentar a eficiência operacional e fornecer insights valiosos para tomada de decisões estratégicas.