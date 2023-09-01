﻿using NoNameLib.Domain.Validation.PersonalIdentification.Brazil;
using System.ComponentModel.DataAnnotations;

namespace NoNameLib.Domain.Tests.PlayTest;

public class TestDomain : IDomain<string>
{
    private readonly Guid _id;

    [Key]
    public string Id
    {
        get { return _id.ToString(); }
        init { _id = Guid.Parse(value); }
    }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string FullName { get; init; }

    [Required]
    [NotDefault]
    public DateTime BirthDate { get; init; }

    [NotNegative]
    public int IntValue { get; init; }

    [PersonalIdentification(typeof(IdentificationCPF))]
    public string CPF { get; init; }

    [Required]
    [NotDefault]
    [Comparison(
        ComparisonType.GreaterThan,
        nameof(ContractDate))]
    public DateTime BeginDate { get; init; }

    [Required]
    [NotDefault]
    public DateTime ContractDate { get; init; }

    public TestDomain(
        string fullName,
        DateTime birthDate)
    {
        _id = Guid.NewGuid();
        FullName = fullName;
        BirthDate = birthDate;
    }

    public TestDomain() { }
}