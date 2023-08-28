﻿using Theatrical.Dto.TransactionDtos;

namespace Theatrical.Dto.LoginDtos.ResponseDto;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool? EmailVerified { get; set; }
    public bool _2FA_enabled { get; set; }
    public string Role { get; set; }
    public List<TransactionDtoFetch>? Transactions { get; set; }
}