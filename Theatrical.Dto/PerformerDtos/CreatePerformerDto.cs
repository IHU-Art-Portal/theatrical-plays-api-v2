﻿namespace Theatrical.Dto.PerformerDtos;

public class CreatePerformerDto
{
    public string Fullname { get; set; }
    public List<string>? Images { get; set; }
}