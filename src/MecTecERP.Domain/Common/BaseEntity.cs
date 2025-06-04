using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataAtualizacao { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public string? UsuarioCriacao { get; set; }
    
    public string? UsuarioAtualizacao { get; set; }
    
    public void MarcarComoAtualizado(string? usuario = null)
    {
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }
    
    public void Inativar(string? usuario = null)
    {
        Ativo = false;
        MarcarComoAtualizado(usuario);
    }
    
    public void Ativar(string? usuario = null)
    {
        Ativo = true;
        MarcarComoAtualizado(usuario);
    }
}