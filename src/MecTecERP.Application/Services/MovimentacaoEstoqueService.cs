using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Services
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
        private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MovimentacaoEstoqueService> _logger;

        public MovimentacaoEstoqueService(
            IMovimentacaoEstoqueRepository movimentacaoRepository,
            IProdutoRepository produtoRepository,
            IInventarioRepository inventarioRepository,
            IMapper mapper,
            ILogger<MovimentacaoEstoqueService> logger)
        {
            _movimentacaoRepository = movimentacaoRepository;
            _produtoRepository = produtoRepository;
            _inventarioRepository = inventarioRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RespostaDto<PaginacaoDto<MovimentacaoEstoqueListDto>>> ObterTodosAsync(MovimentacaoEstoqueFiltroDto filtro)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterComFiltroAsync(
                    filtro.Tipo,
                    filtro.DataMovimentacaoInicio,
                    filtro.DataMovimentacaoFim,
                    filtro.ProdutoId,
                    filtro.Documento,
                    filtro.Pagina,
                    filtro.ItensPorPagina,
                    filtro.OrdenarPor,
                    filtro.OrdemDecrescente);

                var total = await _movimentacaoRepository.ContarComFiltroAsync(
                    filtro.Tipo,
                    filtro.DataMovimentacaoInicio,
                    filtro.DataMovimentacaoFim,
                    filtro.ProdutoId,
                    filtro.Documento);

                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                var paginacao = new PaginacaoDto<MovimentacaoEstoqueListDto>(dtos, total, filtro.Pagina, filtro.ItensPorPagina);

                return new RespostaDto<PaginacaoDto<MovimentacaoEstoqueListDto>>(true, "Movimentações obtidas com sucesso", paginacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações de estoque");
                return new RespostaDto<PaginacaoDto<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> ObterPorIdAsync(int id)
        {
            try
            {
                var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(id);
                if (movimentacao == null)
                {
                    return new RespostaDto<MovimentacaoEstoqueDto>(false, "Movimentação de estoque não encontrada");
                }
                
                var dto = _mapper.Map<MovimentacaoEstoqueDto>(movimentacao);
                return new RespostaDto<MovimentacaoEstoqueDto>(true, "Movimentação obtida com sucesso", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentação de estoque por ID: {Id}", id);
                return new RespostaDto<MovimentacaoEstoqueDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> CriarAsync(MovimentacaoEstoqueCreateDto dto)
        {
            try
            {
                // Validar se o produto existe
                var produto = await _produtoRepository.ObterPorIdAsync(dto.ProdutoId);
                if (produto == null)
                {
                    return new RespostaDto<MovimentacaoEstoqueDto>(false, "Produto não encontrado");
                }

                // Validar estoque suficiente para saídas
                if (dto.Tipo == TipoMovimentacaoEstoque.Saida)
                {
                    var validacao = await ValidarEstoqueSuficienteAsync(dto.ProdutoId, dto.Quantidade);
                    if (!validacao.Dados)
                    {
                        return new RespostaDto<MovimentacaoEstoqueDto>(false, "Estoque insuficiente para a movimentação");
                    }
                }

                var movimentacao = _mapper.Map<MovimentacaoEstoque>(dto);
                movimentacao.DataMovimentacao = DateTime.Now;
                
                var novaMovimentacao = await _movimentacaoRepository.AdicionarAsync(movimentacao);
                
                // Atualizar estoque do produto
                await AtualizarEstoqueProduto(produto, movimentacao.Tipo, movimentacao.Quantidade);
                await _produtoRepository.AtualizarAsync(produto);

                var resultado = _mapper.Map<MovimentacaoEstoqueDto>(novaMovimentacao);
                _logger.LogInformation("Movimentação de estoque criada com sucesso. ID: {Id}", novaMovimentacao.Id);
                
                return new RespostaDto<MovimentacaoEstoqueDto>(true, "Movimentação criada com sucesso", resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar movimentação de estoque");
                return new RespostaDto<MovimentacaoEstoqueDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> AtualizarAsync(int id, MovimentacaoEstoqueUpdateDto dto)
        {
            try
            {
                var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(id);
                if (movimentacao == null)
                {
                    return new RespostaDto<MovimentacaoEstoqueDto>(false, "Movimentação não encontrada");
                }

                // Reverter movimentação anterior
                var produto = await _produtoRepository.ObterPorIdAsync(movimentacao.ProdutoId);
                if (produto != null)
                {
                    await ReverterMovimentacaoEstoque(produto, movimentacao.Tipo, movimentacao.Quantidade);
                }

                // Aplicar nova movimentação
                _mapper.Map(dto, movimentacao);
                
                if (produto != null)
                {
                    await AtualizarEstoqueProduto(produto, movimentacao.Tipo, movimentacao.Quantidade);
                    await _produtoRepository.AtualizarAsync(produto);
                }

                await _movimentacaoRepository.AtualizarAsync(movimentacao);
                
                var resultado = _mapper.Map<MovimentacaoEstoqueDto>(movimentacao);
                return new RespostaDto<MovimentacaoEstoqueDto>(true, "Movimentação atualizada com sucesso", resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar movimentação de estoque: {Id}", id);
                return new RespostaDto<MovimentacaoEstoqueDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto> ExcluirAsync(int id)
        {
            try
            {
                var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(id);
                if (movimentacao == null)
                {
                    return new RespostaDto(false, "Movimentação não encontrada");
                }

                // Reverter movimentação no estoque
                var produto = await _produtoRepository.ObterPorIdAsync(movimentacao.ProdutoId);
                if (produto != null)
                {
                    await ReverterMovimentacaoEstoque(produto, movimentacao.Tipo, movimentacao.Quantidade);
                    await _produtoRepository.AtualizarAsync(produto);
                }

                await _movimentacaoRepository.ExcluirAsync(id);
                return new RespostaDto(true, "Movimentação excluída com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir movimentação de estoque: {Id}", id);
                return new RespostaDto(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorProdutoAsync(int produtoId, int limite = 10)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterPorProdutoAsync(produtoId, limite);
                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(true, "Movimentações obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações por produto: {ProdutoId}", produtoId);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterRecentesAsync(int limite = 10)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterRecentesAsync(limite);
                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(true, "Movimentações recentes obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações recentes");
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorTipoAsync(TipoMovimentacao tipo, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                var tipoEstoque = (TipoMovimentacaoEstoque)tipo;
                var movimentacoes = await _movimentacaoRepository.ObterPorTipoAsync(tipoEstoque, dataInicio, dataFim);
                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(true, "Movimentações obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações por tipo: {Tipo}", tipo);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorInventarioAsync(int inventarioId)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterPorInventarioAsync(inventarioId);
                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(true, "Movimentações do inventário obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações por inventário: {InventarioId}", inventarioId);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorDocumentoAsync(string documento)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterPorDocumentoAsync(documento);
                var dtos = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(true, "Movimentações obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações por documento: {Documento}", documento);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<MovimentacaoEstoqueResumoDto>> ObterResumoAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                var resumo = await _movimentacaoRepository.ObterResumoAsync(dataInicio, dataFim);
                return new RespostaDto<MovimentacaoEstoqueResumoDto>(true, "Resumo obtido com sucesso", resumo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter resumo de movimentações");
                return new RespostaDto<MovimentacaoEstoqueResumoDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<MovimentacaoPorDiaDto>>> ObterMovimentacoesPorDiaAsync(DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterMovimentacoesPorDiaAsync(dataInicio, dataFim);
                return new RespostaDto<List<MovimentacaoPorDiaDto>>(true, "Movimentações por dia obtidas com sucesso", movimentacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações por dia");
                return new RespostaDto<List<MovimentacaoPorDiaDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<decimal>> CalcularSaldoEstoqueAsync(int produtoId, DateTime? dataLimite = null)
        {
            try
            {
                var saldo = await _movimentacaoRepository.CalcularSaldoEstoqueAsync(produtoId, dataLimite);
                return new RespostaDto<decimal>(true, "Saldo calculado com sucesso", saldo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular saldo do estoque para produto: {ProdutoId}", produtoId);
                return new RespostaDto<decimal>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarEntradaAsync(int produtoId, decimal quantidade, decimal valorUnitario, string? documento = null, string? observacoes = null)
        {
            var dto = new MovimentacaoEstoqueCreateDto
            {
                ProdutoId = produtoId,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                Documento = documento,
                Observacoes = observacoes
            };

            return await CriarAsync(dto);
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarSaidaAsync(int produtoId, decimal quantidade, decimal valorUnitario, string? documento = null, string? observacoes = null)
        {
            var dto = new MovimentacaoEstoqueCreateDto
            {
                ProdutoId = produtoId,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                Documento = documento,
                Observacoes = observacoes
            };

            return await CriarAsync(dto);
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarAjusteAsync(int produtoId, decimal quantidade, decimal valorUnitario, string motivo, string? documento = null)
        {
            var dto = new MovimentacaoEstoqueCreateDto
            {
                ProdutoId = produtoId,
                Tipo = TipoMovimentacaoEstoque.Ajuste,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                Documento = documento,
                Observacoes = motivo
            };

            return await CriarAsync(dto);
        }

        public async Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarInventarioAsync(int produtoId, int inventarioId, decimal estoqueContado, decimal estoqueAnterior, decimal valorUnitario)
        {
            var diferenca = estoqueContado - estoqueAnterior;
            var tipo = diferenca >= 0 ? TipoMovimentacaoEstoque.Entrada : TipoMovimentacaoEstoque.Saida;
            
            var dto = new MovimentacaoEstoqueCreateDto
            {
                ProdutoId = produtoId,
                Tipo = tipo,
                Quantidade = Math.Abs(diferenca),
                ValorUnitario = valorUnitario,
                InventarioId = inventarioId,
                Observacoes = $"Ajuste de inventário - Estoque anterior: {estoqueAnterior}, Estoque contado: {estoqueContado}"
            };

            return await CriarAsync(dto);
        }

        public async Task<RespostaDto<bool>> ValidarEstoqueSuficienteAsync(int produtoId, decimal quantidade)
        {
            try
            {
                var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
                if (produto == null)
                {
                    return new RespostaDto<bool>(false, "Produto não encontrado");
                }

                var suficiente = produto.EstoqueAtual >= quantidade;
                return new RespostaDto<bool>(true, suficiente ? "Estoque suficiente" : "Estoque insuficiente", suficiente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar estoque suficiente para produto: {ProdutoId}", produtoId);
                return new RespostaDto<bool>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<ExportacaoDto>> ExportarAsync(MovimentacaoEstoqueFiltroDto filtro, string formato = "excel")
        {
            try
            {
                // Implementação básica - pode ser expandida
                var movimentacoes = await _movimentacaoRepository.ObterComFiltroAsync(
                    filtro.Tipo,
                    filtro.DataMovimentacaoInicio,
                    filtro.DataMovimentacaoFim,
                    filtro.ProdutoId,
                    filtro.Documento,
                    1,
                    int.MaxValue);

                var exportacao = new ExportacaoDto
                {
                    NomeArquivo = $"movimentacoes_estoque_{DateTime.Now:yyyyMMdd_HHmmss}.{formato}",
                    TipoConteudo = formato.ToLower() == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv",
                    Conteudo = new byte[0] // Implementar geração do arquivo
                };

                return new RespostaDto<ExportacaoDto>(true, "Exportação realizada com sucesso", exportacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar movimentações de estoque");
                return new RespostaDto<ExportacaoDto>(false, "Erro interno do servidor");
            }
        }

        private async Task AtualizarEstoqueProduto(Produto produto, TipoMovimentacaoEstoque tipo, decimal quantidade)
        {
            switch (tipo)
            {
                case TipoMovimentacaoEstoque.Entrada:
                    produto.EstoqueAtual += quantidade;
                    break;
                case TipoMovimentacaoEstoque.Saida:
                    produto.EstoqueAtual -= quantidade;
                    break;
                case TipoMovimentacaoEstoque.Ajuste:
                    produto.EstoqueAtual = quantidade;
                    break;
            }

            produto.DataUltimaMovimentacao = DateTime.Now;
        }

        private async Task ReverterMovimentacaoEstoque(Produto produto, TipoMovimentacaoEstoque tipo, decimal quantidade)
        {
            switch (tipo)
            {
                case TipoMovimentacaoEstoque.Entrada:
                    produto.EstoqueAtual -= quantidade;
                    break;
                case TipoMovimentacaoEstoque.Saida:
                    produto.EstoqueAtual += quantidade;
                    break;
                // Ajustes não podem ser revertidos automaticamente
            }
        }
    }
}