# Documento de Correção - Migração para Dapper

## Análise dos Problemas Identificados

Após a análise dos 165 erros de compilação, foram identificados os seguintes problemas principais causados pela migração do Entity Framework Core para Dapper:

### 1. DTOs Ausentes ou Incompletos

#### Problema:
Vários DTOs estão faltando ou não foram criados adequadamente:
- `ClienteDto`, `ClienteCreateDto`, `ClienteUpdateDto`
- `VeiculoDto`, `VeiculoCreateDto`, `VeiculoUpdateDto`
- `OrdemServicoDto`, `OrdemServicoCreateDto`, `OrdemServicoUpdateDto`
- `InventarioDto`, `InventarioCreateDto`, `InventarioUpdateDto`
- `MovimentacaoEstoqueDto`, `MovimentacaoEstoqueCreateDto`, `MovimentacaoEstoqueUpdateDto`
- `CategoriaFiltroDto`
- `FornecedorFiltroDto`
- `ProdutoFiltroDto`
- `OrdemServicoFiltroDto`

#### Solução:
Criar todos os DTOs ausentes seguindo o padrão já estabelecido nos DTOs existentes.

### 2. Interfaces de Serviços Ausentes

#### Problema:
As seguintes interfaces não foram encontradas:
- `IClienteService`
- `IVeiculoService`
- `IOrdemServicoService`

#### Solução:
Criar as interfaces ausentes seguindo o padrão das interfaces existentes.

### 3. Enums Ausentes

#### Problema:
O enum `UnidadeMedida` não foi encontrado, causando erro no `ProdutoDto`.

#### Solução:
Criar o enum `UnidadeMedida` no namespace `MecTecERP.Domain.Enums`.

### 4. Incompatibilidade de Tipos de Retorno

#### Problema:
Os serviços não estão implementando corretamente as interfaces devido a tipos de retorno incompatíveis. Os métodos estão retornando tipos simples em vez de `RespostaDto<T>`.

#### Solução:
Ajustar todos os métodos dos serviços para retornar `RespostaDto<T>` conforme definido nas interfaces.

### 5. Métodos de Interface Não Implementados

#### Problema:
Vários métodos definidos nas interfaces não estão implementados nos serviços:
- Métodos de filtro (`ObterTodosAsync` com filtros)
- Métodos de validação (`ExisteAsync`, `NomeExisteAsync`, etc.)
- Métodos de exportação (`ExportarAsync`)
- Métodos de ativação/desativação (`AtivarDesativarAsync`)

#### Solução:
Implementar todos os métodos ausentes nos serviços.

## Plano de Correção

### Fase 1: Criação de DTOs Ausentes
1. Criar DTOs para Cliente
2. Criar DTOs para Veículo
3. Criar DTOs para Ordem de Serviço
4. Criar DTOs para Inventário
5. Criar DTOs para Movimentação de Estoque
6. Criar DTOs de Filtro ausentes

### Fase 2: Criação de Enums Ausentes
1. Criar enum `UnidadeMedida`

### Fase 3: Criação de Interfaces de Serviços
1. Criar `IClienteService`
2. Criar `IVeiculoService`
3. Criar `IOrdemServicoService`

### Fase 4: Correção dos Serviços
1. Ajustar tipos de retorno para `RespostaDto<T>`
2. Implementar métodos ausentes
3. Corrigir assinaturas de métodos

### Fase 5: Validação e Testes
1. Compilar o projeto
2. Corrigir erros remanescentes
3. Testar funcionalidades básicas

## Observações Importantes

### Padrões a Seguir:
- Todos os DTOs devem seguir o padrão estabelecido nos DTOs existentes
- Usar `RespostaDto<T>` para encapsular respostas
- Implementar validações usando Data Annotations
- Seguir convenções de nomenclatura do projeto

### Dependências:
- Verificar se todas as referências entre projetos estão corretas
- Garantir que os namespaces estão sendo importados corretamente
- Validar se os mapeamentos AutoMapper estão configurados

### Prioridade de Correção:
1. **Alta**: DTOs básicos (Create, Update, Response)
2. **Média**: DTOs de filtro
3. **Baixa**: Métodos de exportação e funcionalidades avançadas

Este documento servirá como guia para a correção sistemática dos problemas identificados na migração para Dapper.