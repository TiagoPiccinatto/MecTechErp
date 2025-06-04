namespace MecTecERP.Application.DTOs;

public abstract class BaseDto
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
}

public abstract class BaseCreateDto
{
    public bool Ativo { get; set; } = true;
}

public abstract class BaseUpdateDto
{
    public int Id { get; set; }
    public bool Ativo { get; set; } = true;
}