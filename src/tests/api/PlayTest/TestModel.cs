﻿using NoNameLib.Domain.Interfaces;

namespace NoNameLib.Api.Tests.PlayTest;

public class TestModel : IDomain<string>
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string CPF { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime ContractDate { get; set; }
}
