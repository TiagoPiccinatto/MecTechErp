using AutoMapper;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos.Count));
        
        CreateMap<CategoriaCreateDto, Categoria>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
        
        CreateMap<CategoriaUpdateDto, Categoria>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
        
        CreateMap<Categoria, CategoriaListDto>()
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos.Count));

        // Fornecedor Mappings
        CreateMap<Fornecedor, FornecedorDto>()
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos.Count));
        
        CreateMap<FornecedorCreateDto, Fornecedor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
        
        CreateMap<FornecedorUpdateDto, Fornecedor>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
        
        CreateMap<Fornecedor, FornecedorListDto>()
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos.Count));

        // Produto Mappings
        CreateMap<Produto, ProdutoDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria.Nome))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome))
            .ForMember(dest => dest.ValorTotalEstoque, opt => opt.MapFrom(src => src.EstoqueAtual * src.PrecoVenda))
            .ForMember(dest => dest.StatusEstoque, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? StatusEstoque.Zerado :
                src.EstoqueAtual <= src.EstoqueMinimo ? StatusEstoque.Baixo :
                src.EstoqueAtual >= src.EstoqueMaximo ? StatusEstoque.Excesso :
                StatusEstoque.Normal))
            .ForMember(dest => dest.StatusEstoqueTexto, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? "Zerado" :
                src.EstoqueAtual <= src.EstoqueMinimo ? "Baixo" :
                src.EstoqueAtual >= src.EstoqueMaximo ? "Excesso" :
                "Normal"))
            .ForMember(dest => dest.StatusEstoqueClass, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? "danger" :
                src.EstoqueAtual <= src.EstoqueMinimo ? "warning" :
                src.EstoqueAtual >= src.EstoqueMaximo ? "info" :
                "success"));
        
        CreateMap<ProdutoCreateDto, Produto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.MovimentacoesEstoque, opt => opt.Ignore())
            .ForMember(dest => dest.ItensInventario, opt => opt.Ignore());
        
        CreateMap<ProdutoUpdateDto, Produto>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.MovimentacoesEstoque, opt => opt.Ignore())
            .ForMember(dest => dest.ItensInventario, opt => opt.Ignore());
        
        CreateMap<Produto, ProdutoListDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria.Nome))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome))
            .ForMember(dest => dest.ValorTotalEstoque, opt => opt.MapFrom(src => src.EstoqueAtual * src.PrecoVenda))
            .ForMember(dest => dest.StatusEstoque, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? StatusEstoque.Zerado :
                src.EstoqueAtual <= src.EstoqueMinimo ? StatusEstoque.Baixo :
                src.EstoqueAtual >= src.EstoqueMaximo ? StatusEstoque.Excesso :
                StatusEstoque.Normal))
            .ForMember(dest => dest.StatusEstoqueTexto, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? "Zerado" :
                src.EstoqueAtual <= src.EstoqueMinimo ? "Baixo" :
                src.EstoqueAtual >= src.EstoqueMaximo ? "Excesso" :
                "Normal"))
            .ForMember(dest => dest.StatusEstoqueClass, opt => opt.MapFrom(src => 
                src.EstoqueAtual <= 0 ? "danger" :
                src.EstoqueAtual <= src.EstoqueMinimo ? "warning" :
                src.EstoqueAtual >= src.EstoqueMaximo ? "info" :
                "success"));
        
        CreateMap<Produto, ProdutoSelectDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria.Nome))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome));

        // MovimentacaoEstoque Mappings
        CreateMap<MovimentacaoEstoque, MovimentacaoEstoqueDto>()
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto.Codigo))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome))
            .ForMember(dest => dest.TipoTexto, opt => opt.MapFrom(src =>
                src.Tipo == TipoMovimentacaoEstoque.Entrada ? "Entrada" :
                src.Tipo == TipoMovimentacaoEstoque.Saida ? "Saída" :
                src.Tipo == TipoMovimentacaoEstoque.Ajuste ? "Ajuste" :
                src.Tipo == TipoMovimentacaoEstoque.Transferencia ? "Transferência" :
                "Inventário"))
            .ForMember(dest => dest.TipoClass, opt => opt.MapFrom(src =>
                src.Tipo == TipoMovimentacaoEstoque.Entrada ? "success" :
                src.Tipo == TipoMovimentacaoEstoque.Saida ? "danger" :
                src.Tipo == TipoMovimentacaoEstoque.Ajuste ? "warning" :
                src.Tipo == TipoMovimentacaoEstoque.Transferencia ? "info" :
                "secondary"))
            .ForMember(dest => dest.InventarioDescricao, opt => opt.MapFrom(src => src.Inventario != null ? src.Inventario.Descricao : null));
        
        CreateMap<MovimentacaoEstoqueCreateDto, MovimentacaoEstoque>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produto, opt => opt.Ignore())
            .ForMember(dest => dest.Inventario, opt => opt.Ignore());
        
        CreateMap<MovimentacaoEstoqueUpdateDto, MovimentacaoEstoque>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Produto, opt => opt.Ignore())
            .ForMember(dest => dest.Inventario, opt => opt.Ignore());
        
        CreateMap<MovimentacaoEstoque, MovimentacaoEstoqueListDto>()
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto.Codigo))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome))
            .ForMember(dest => dest.TipoTexto, opt => opt.MapFrom(src =>
                src.Tipo == TipoMovimentacaoEstoque.Entrada ? "Entrada" :
                src.Tipo == TipoMovimentacaoEstoque.Saida ? "Saída" :
                src.Tipo == TipoMovimentacaoEstoque.Ajuste ? "Ajuste" :
                src.Tipo == TipoMovimentacaoEstoque.Transferencia ? "Transferência" :
                "Inventário"))
            .ForMember(dest => dest.TipoClass, opt => opt.MapFrom(src =>
                src.Tipo == TipoMovimentacaoEstoque.Entrada ? "success" :
                src.Tipo == TipoMovimentacaoEstoque.Saida ? "danger" :
                src.Tipo == TipoMovimentacaoEstoque.Ajuste ? "warning" :
                src.Tipo == TipoMovimentacaoEstoque.Transferencia ? "info" :
                "secondary"));

        // Inventario Mappings
        CreateMap<Inventario, InventarioDto>()
            .ForMember(dest => dest.StatusTexto, opt => opt.MapFrom(src => 
                src.Status == StatusInventario.Planejado ? "Planejado" :
                src.Status == StatusInventario.EmAndamento ? "Em Andamento" :
                src.Status == StatusInventario.Finalizado ? "Finalizado" :
                "Cancelado"))
            .ForMember(dest => dest.TotalItens, opt => opt.MapFrom(src => src.Itens.Count))
            .ForMember(dest => dest.ItensContados, opt => opt.MapFrom(src => src.Itens.Count(i => i.FoiContado())))
            .ForMember(dest => dest.ItensPendentes, opt => opt.MapFrom(src => src.Itens.Count(i => !i.FoiContado())))
            .ForMember(dest => dest.ItensComDivergencia, opt => opt.MapFrom(src => src.Itens.Count(i => i.TemDiferenca())))
            .ForMember(dest => dest.PercentualConcluido, opt => opt.MapFrom(src =>
                src.Itens.Count > 0 ? (decimal)src.Itens.Count(i => i.FoiContado()) / src.Itens.Count * 100 : 0));
        
        CreateMap<InventarioCreateDto, Inventario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusInventario.Planejado))
            .ForMember(dest => dest.Itens, opt => opt.Ignore());
        
        CreateMap<InventarioUpdateDto, Inventario>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Itens, opt => opt.Ignore());
        
        CreateMap<Inventario, InventarioListDto>()
            .ForMember(dest => dest.StatusTexto, opt => opt.MapFrom(src => 
                src.Status == StatusInventario.Planejado ? "Planejado" :
                src.Status == StatusInventario.EmAndamento ? "Em Andamento" :
                src.Status == StatusInventario.Finalizado ? "Finalizado" :
                "Cancelado"))
            .ForMember(dest => dest.TotalItens, opt => opt.MapFrom(src => src.Itens.Count))
            .ForMember(dest => dest.ItensContados, opt => opt.MapFrom(src => src.Itens.Count(i => i.FoiContado())))
            .ForMember(dest => dest.ItensComDivergencia, opt => opt.MapFrom(src => src.Itens.Count(i => i.TemDiferenca())))
            .ForMember(dest => dest.PercentualConcluido, opt => opt.MapFrom(src =>
                src.Itens.Count > 0 ? (decimal)src.Itens.Count(i => i.FoiContado()) / src.Itens.Count * 100 : 0));

        // InventarioItem Mappings
        CreateMap<InventarioItem, InventarioItemDto>()
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto.Codigo))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome))
            .ForMember(dest => dest.Diferenca, opt => opt.MapFrom(src => src.Diferenca))
            .ForMember(dest => dest.TemDivergencia, opt => opt.MapFrom(src => src.TemDiferenca()))
            .ForMember(dest => dest.Contado, opt => opt.MapFrom(src => src.FoiContado()));
        
        CreateMap<InventarioItemUpdateDto, InventarioItem>()
            .ForMember(dest => dest.InventarioId, opt => opt.Ignore())
            .ForMember(dest => dest.ProdutoId, opt => opt.Ignore())
            .ForMember(dest => dest.EstoqueSistema, opt => opt.Ignore())
            .ForMember(dest => dest.DataContagem, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.UsuarioContagem, opt => opt.Ignore())
            .ForMember(dest => dest.Inventario, opt => opt.Ignore())
            .ForMember(dest => dest.Produto, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Ativo, opt => opt.Ignore());
    }
}