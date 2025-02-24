using System;

namespace Bank.Application.Responses;

public class AccountResponse
{
    public required Guid               Id            { set; get; }
    public required string             AccountNumber { set; get; }
    public required UserSimpleResponse User          { set; get; }
}

public class AccountSimpleResponse
{
    public required Guid   Id            { set; get; }
    public required string AccountNumber { set; get; }
}
