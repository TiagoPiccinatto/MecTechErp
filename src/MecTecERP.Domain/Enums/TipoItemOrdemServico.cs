namespace MecTecERP.Domain.Enums
{
    public enum TipoItemOrdemServico // Alinhando com o plano (TipoItem: Servico ou Peca)
    {
        Servico = 1,
        Peca = 2  // No plano é "Peca", o existente usa "Produto". "MaoDeObra" foi removido para simplificar.
    }
}