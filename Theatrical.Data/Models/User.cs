using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public bool? Enabled { get; set; }
    public string? VerificationCode { get; set; }
    public virtual List<UserAuthority> UserAuthorities { get; set; }
    public virtual List<Transaction> UserTransactions { get; set; }
    
    public bool _2FA_enabled { get; set; }
    public string? _2FA_code { get; set; }
}