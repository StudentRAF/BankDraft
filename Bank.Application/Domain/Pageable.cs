namespace Bank.Application.Domain;

public class Pageable
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
