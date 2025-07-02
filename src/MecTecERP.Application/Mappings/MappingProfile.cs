using AutoMapper;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Método auxiliar para classes CSS de TipoMovimentacaoEstoque
        string GetMovimentacaoTipoClass(TipoMovimentacaoEstoque tipo)
        {
            return tipo switch
            {
                TipoMovimentacaoEstoque.Entrada => "success",
                TipoMovimentacaoEstoque.Saida => "danger",
                TipoMovimentacaoEstoque.Ajuste => "warning",
                TipoMovimentacaoEstoque.Transferencia => "info",
                TipoMovimentacaoEstoque.Inventario => "secondary",
                _ => "default",
            };
        }

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

        // Cliente Mappings
        CreateMap<ClienteCreateDto, Cliente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Veiculos, opt => opt.Ignore())
            .ForMember(dest => dest.OrdensServico, opt => opt.Ignore())
            // Ativo é definido no DTO ou default na entidade
            ;
        CreateMap<ClienteUpdateDto, Cliente>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore()) // Não alterar na atualização
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Veiculos, opt => opt.Ignore())
            .ForMember(dest => dest.OrdensServico, opt => opt.Ignore());

        CreateMap<Cliente, ClienteDto>()
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao))
            // TotalVeiculos e TotalOrdensServico precisariam ser calculados no serviço ou por projeção.
            // Veiculos e OrdensServico (coleções de DTOs) seriam mapeados se carregados na entidade.
            ;
        CreateMap<Cliente, ClienteListDto>()
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao))
            // TotalVeiculos e TotalOrdensServico precisariam ser calculados.
            ;

        // Fornecedor Mappings
        CreateMap<FornecedorCreateDto, Fornecedor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
            // Assumindo que FornecedorCreateDto tem os campos: RazaoSocial, NomeFantasia, Cnpj, InscricaoEstadual, Telefone1, Telefone2, Email, Cep, Logradouro, Numero, Complemento, Bairro, Cidade, Uf, NomeContato, TelefoneContato, EmailContato, Observacoes, Ativo

        CreateMap<FornecedorUpdateDto, Fornecedor>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
            // Assumindo que FornecedorUpdateDto tem os mesmos campos de Create + Id

        CreateMap<Fornecedor, FornecedorDto>()
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao))
            // Assumindo que FornecedorDto espelha a entidade Fornecedor com os campos ajustados
            // TotalProdutos seria calculado no serviço ou projeção.
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos != null ? src.Produtos.Count : 0));

        CreateMap<Fornecedor, FornecedorListDto>()
             .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao));
            // Assumindo que FornecedorListDto tem campos como Id, RazaoSocial, NomeFantasia, Cnpj, Cidade, Uf, Ativo, DataCadastro
            // TotalProdutos seria calculado no serviço.

        // Veiculo Mappings
        CreateMap<VeiculoCreateDto, Veiculo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.OrdensServico, opt => opt.Ignore());

        CreateMap<VeiculoUpdateDto, Veiculo>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.OrdensServico, opt => opt.Ignore());

        CreateMap<Veiculo, VeiculoDto>()
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao))
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NomeRazaoSocial : null));
            // TotalOrdensServico e OrdensServico (coleção) seriam preenchidos no serviço.

        CreateMap<Veiculo, VeiculoListDto>()
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCriacao))
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NomeRazaoSocial : null));
            // TotalOrdensServico seria preenchido no serviço.

        // OrdemServico Mappings
        CreateMap<OrdemServicoCreateDto, OrdemServico>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Veiculo, opt => opt.Ignore())
            .ForMember(dest => dest.Itens, opt => opt.Ignore()) // Itens são tratados separadamente
            .ForMember(dest => dest.Fotos, opt => opt.Ignore()); // Fotos são tratadas separadamente

        CreateMap<OrdemServicoUpdateDto, OrdemServico>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Veiculo, opt => opt.Ignore())
            .ForMember(dest => dest.Itens, opt => opt.Ignore())
            .ForMember(dest => dest.Fotos, opt => opt.Ignore());

        CreateMap<OrdemServico, OrdemServicoDto>()
            .ForMember(dest => dest.DataCadastroOs, opt => opt.MapFrom(src => src.DataCriacao)) // Assumindo que OrdemServicoDto tem DataCadastroOs
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NomeRazaoSocial : null))
            .ForMember(dest => dest.VeiculoPlaca, opt => opt.MapFrom(src => src.Veiculo != null ? src.Veiculo.Placa : null))
            .ForMember(dest => dest.VeiculoDescricao, opt => opt.MapFrom(src => src.Veiculo != null ? $"{src.Veiculo.Marca} {src.Veiculo.Modelo}" : null))
            .ForMember(dest => dest.MecanicoResponsavelNome, opt => opt.MapFrom(src => src.MecanicoResponsavelId.HasValue ? "Obter Nome Mecanico" : null)) // Necessita buscar nome do mecânico
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens)) // Mapeia coleção de entidade para coleção de DTO
            .ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Fotos));

        CreateMap<OrdemServico, OrdemServicoListDto>()
            .ForMember(dest => dest.DataEntrada, opt => opt.MapFrom(src => src.DataEntrada)) // Assegurar que o DTO tem DataEntrada
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NomeRazaoSocial : null))
            .ForMember(dest => dest.VeiculoPlaca, opt => opt.MapFrom(src => src.Veiculo != null ? src.Veiculo.Placa : null))
            .ForMember(dest => dest.VeiculoDescricao, opt => opt.MapFrom(src => src.Veiculo != null ? $"{src.Veiculo.Marca} {src.Veiculo.Modelo}" : null))
            .ForMember(dest => dest.MecanicoResponsavelNome, opt => opt.MapFrom(src => src.MecanicoResponsavelId.HasValue ? "Obter Nome Mecanico" : null));

        // OrdemServicoItem Mappings
        CreateMap<OrdemServicoItemCreateDto, OrdemServicoItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.OrdemServico, opt => opt.Ignore())
            .ForMember(dest => dest.Produto, opt => opt.Ignore());

        CreateMap<OrdemServicoItemUpdateDto, OrdemServicoItem>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.OrdemServico, opt => opt.Ignore())
            .ForMember(dest => dest.Produto, opt => opt.Ignore());

        CreateMap<OrdemServicoItem, OrdemServicoItemDto>()
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Nome : null));

        // OrdemServicoFoto Mappings
        CreateMap<OrdemServicoFotoCreateDto, OrdemServicoFoto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.OrdemServico, opt => opt.Ignore());

        CreateMap<OrdemServicoFoto, OrdemServicoFotoDto>()
            .ForMember(dest => dest.DataUpload, opt => opt.MapFrom(src => src.DataCriacao));


        // Produto Mappings
        CreateMap<Produto, ProdutoDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : null))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? (src.Fornecedor.NomeFantasia ?? src.Fornecedor.RazaoSocial) : null))
            .ForMember(dest => dest.UnidadeMedidaTexto, opt => opt.MapFrom(src => src.Unidade.ToString())) // Assumindo que ProdutoDto tem UnidadeMedidaTexto
            .ForMember(dest => dest.StatusEstoque, opt => opt.MapFrom(src => src.StatusEstoqueEnum)) // Usando a propriedade da entidade
            .ForMember(dest => dest.StatusEstoqueTexto, opt => opt.MapFrom(src => src.ObterStatusEstoque())) // Usando o método da entidade
             // FotoUrl é mapeado diretamente se os nomes forem iguais
            .ForMember(dest => dest.ValorTotalEstoque, opt => opt.MapFrom(src => src.EstoqueAtual * src.PrecoVenda)); // Mantido, mas talvez PrecoCusto seja mais apropriado para valor de estoque.

        CreateMap<ProdutoCreateDto, Produto>() // Assumindo que ProdutoCreateDto tem UnidadeMedida (enum) e FotoUrl
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.MovimentacoesEstoque, opt => opt.Ignore())
            .ForMember(dest => dest.InventarioItens, opt => opt.Ignore())
            .ForMember(dest => dest.OrdemServicoItens, opt => opt.Ignore());
        
        CreateMap<ProdutoUpdateDto, Produto>() // Assumindo que ProdutoUpdateDto tem UnidadeMedida (enum) e FotoUrl
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.MovimentacoesEstoque, opt => opt.Ignore())
            .ForMember(dest => dest.InventarioItens, opt => opt.Ignore())
            .ForMember(dest => dest.OrdemServicoItens, opt => opt.Ignore());
        
        CreateMap<Produto, ProdutoListDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : null))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? (src.Fornecedor.NomeFantasia ?? src.Fornecedor.RazaoSocial) : null))
            .ForMember(dest => dest.StatusEstoque, opt => opt.MapFrom(src => src.StatusEstoqueEnum))
            .ForMember(dest => dest.StatusEstoqueTexto, opt => opt.MapFrom(src => src.ObterStatusEstoque()))
            .ForMember(dest => dest.UnidadeMedidaTexto, opt => opt.MapFrom(src => src.Unidade.ToString()))
            .ForMember(dest => dest.ValorTotalEstoque, opt => opt.MapFrom(src => src.EstoqueAtual * src.PrecoVenda));

        CreateMap<Produto, ProdutoSelectDto>() // Assumindo que ProdutoSelectDto existe e é simples
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : null))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? (src.Fornecedor.NomeFantasia ?? src.Fornecedor.RazaoSocial) : null))
            .ForMember(dest => dest.UnidadeMedida, opt => opt.MapFrom(src => src.Unidade.ToString()));


        // MovimentacaoEstoque Mappings
        CreateMap<MovimentacaoEstoque, MovimentacaoEstoqueDto>()
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Codigo : null))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Nome : null))
            .ForMember(dest => dest.TipoTexto, opt => opt.MapFrom(src => src.Tipo.ToString())) // Simplesmente usar ToString() do enum
            .ForMember(dest => dest.TipoClass, opt => opt.MapFrom(src => GetMovimentacaoTipoClass(src.Tipo))) // Usar um método auxiliar para a classe CSS
            .ForMember(dest => dest.InventarioDescricao, opt => opt.MapFrom(src => src.Inventario != null ? src.Inventario.Descricao : null))
            .ForMember(dest => dest.OrdemServicoItemId, opt => opt.MapFrom(src => src.OrdemServicoItemId)); // Mapeamento adicionado
            // Assumindo que MovimentacaoEstoqueDto tem OrdemServicoItemId
        
        CreateMap<MovimentacaoEstoqueCreateDto, MovimentacaoEstoque>() // Assumindo que MovimentacaoEstoqueCreateDto tem OrdemServicoItemId
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.UseValue(DateTime.UtcNow))
            .ForMember(dest => dest.Produto, opt => opt.Ignore())
            .ForMember(dest => dest.Inventario, opt => opt.Ignore())
            .ForMember(dest => dest.OrdemServicoItem, opt => opt.Ignore());
        
        CreateMap<MovimentacaoEstoqueUpdateDto, MovimentacaoEstoque>() // Assumindo que MovimentacaoEstoqueUpdateDto tem OrdemServicoItemId
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Produto, opt => opt.Ignore())
            .ForMember(dest => dest.Inventario, opt => opt.Ignore())
            .ForMember(dest => dest.OrdemServicoItem, opt => opt.Ignore());
        
        CreateMap<MovimentacaoEstoque, MovimentacaoEstoqueListDto>()
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Codigo : null))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Nome : null))
            .ForMember(dest => dest.TipoTexto, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.TipoClass, opt => opt.MapFrom(src => GetMovimentacaoTipoClass(src.Tipo)));
            // Assumindo que MovimentacaoEstoqueListDto também pode precisar de OrdemServicoItemId ou info relacionada.

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