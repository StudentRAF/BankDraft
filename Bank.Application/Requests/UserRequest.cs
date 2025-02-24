using System;

using Bank.Application.Domain;

namespace Bank.Application.Requests;

public class UserLoginRequest
{
    public required string Email    { set; get; }
    public required string Password { set; get; }
}

public class UserRegisterRequest
{
    public required string   FirstName                  { set; get; }
    public required string   LastName                   { set; get; }
    public required DateOnly DateOfBirth                { set; get; }
    public required Gender   Gender                     { set; get; }
    public required string   UniqueIdentificationNumber { set; get; }
    public required string   Username                   { set; get; }
    public required string   Email                      { set; get; }
    public required string   PhoneNumber                { set; get; }
    public required string   Address                    { set; get; }
    public required Role     Role                       { set; get; }
    public required string   Department                 { set; get; }
}

public class ActivationRequest
{
    public required string Password        { set; get; }
    public required string ConfirmPassword { set; get; }
}

public class UserPasswordResetRequest
{
    public required string Password        { set; get; }
    public required string ConfirmPassword { set; get; }
}
