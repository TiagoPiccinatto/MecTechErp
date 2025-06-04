namespace MecTecERP.Application.DTOs;

public class PaginacaoDto<T>
{
    public List<T> Itens { get; set; } = new();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior { get; set; }
    public bool TemProximaPagina { get; set; }
    public int PrimeiraPagina { get; set; } = 1;
    public int UltimaPagina { get; set; }
    public int? PaginaAnterior { get; set; }
    public int? ProximaPagina { get; set; }

    public PaginacaoDto()
    {
    }

    public PaginacaoDto(List<T> itens, int totalItens, int paginaAtual, int itensPorPagina)
    {
        Itens = itens;
        TotalItens = totalItens;
        PaginaAtual = paginaAtual;
        ItensPorPagina = itensPorPagina;
        TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina);
        UltimaPagina = TotalPaginas;
        TemPaginaAnterior = paginaAtual > 1;
        TemProximaPagina = paginaAtual < TotalPaginas;
        PaginaAnterior = TemPaginaAnterior ? paginaAtual - 1 : null;
        ProximaPagina = TemProximaPagina ? paginaAtual + 1 : null;
    }
}

public class RespostaDto<T>
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public T? Dados { get; set; }
    public List<string> Erros { get; set; } = new();
    public DateTime DataResposta { get; set; } = DateTime.Now;

    public RespostaDto()
    {
    }

    public RespostaDto(bool sucesso, string mensagem, T? dados = default)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        Dados = dados;
    }

    public static RespostaDto<T> ComSucesso(T dados, string mensagem = "Operação realizada com sucesso")
    {
        return new RespostaDto<T>(true, mensagem, dados);
    }

    public static RespostaDto<T> ComErro(string mensagem, List<string>? erros = null)
    {
        var resposta = new RespostaDto<T>(false, mensagem);
        if (erros != null)
            resposta.Erros = erros;
        return resposta;
    }

    public static RespostaDto<T> ComErro(string mensagem, string erro)
    {
        return new RespostaDto<T>(false, mensagem) { Erros = new List<string> { erro } };
    }
}

public class RespostaDto : RespostaDto<object>
{
    public RespostaDto() : base()
    {
    }

    public RespostaDto(bool sucesso, string mensagem) : base(sucesso, mensagem)
    {
    }

    public static RespostaDto ComSucesso(string mensagem = "Operação realizada com sucesso")
    {
        return new RespostaDto(true, mensagem);
    }

    public new static RespostaDto ComErro(string mensagem, List<string>? erros = null)
    {
        var resposta = new RespostaDto(false, mensagem);
        if (erros != null)
            resposta.Erros = erros;
        return resposta;
    }

    public new static RespostaDto ComErro(string mensagem, string erro)
    {
        return new RespostaDto(false, mensagem) { Erros = new List<string> { erro } };
    }
}

public class FiltroBaseDto
{
    public int Pagina { get; set; } = 1;
    public int ItensPorPagina { get; set; } = 20;
    public string? OrdenarPor { get; set; }
    public bool OrdemDecrescente { get; set; } = false;
    public string? Busca { get; set; }
    public bool? Ativo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}

public class SelectItemDto
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
    public bool Disabled { get; set; }
    public string? Group { get; set; }
    public object? Data { get; set; }

    public SelectItemDto()
    {
    }

    public SelectItemDto(int value, string text, bool selected = false)
    {
        Value = value;
        Text = text;
        Selected = selected;
    }
}

public class ExportacaoDto
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoConteudo { get; set; } = string.Empty;
    public byte[] Conteudo { get; set; } = Array.Empty<byte>();
    public DateTime DataGeracao { get; set; } = DateTime.Now;
}

public class ValidacaoDto
{
    public bool Valido { get; set; }
    public List<string> Erros { get; set; } = new();
    public Dictionary<string, List<string>> ErrosPorCampo { get; set; } = new();

    public void AdicionarErro(string erro)
    {
        Erros.Add(erro);
        Valido = false;
    }

    public void AdicionarErro(string campo, string erro)
    {
        if (!ErrosPorCampo.ContainsKey(campo))
            ErrosPorCampo[campo] = new List<string>();
        
        ErrosPorCampo[campo].Add(erro);
        Valido = false;
    }
}