﻿using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests;

public class TypeVisibilityConventionTests : IClassFixture<TypeVisibilityConventionFixture>
{
    private readonly TypeVisibilityConventionFixture _typeVisibilityConventionFixture;

    public TypeVisibilityConventionTests(TypeVisibilityConventionFixture typeVisibilityConventionFixture)
    {
        _typeVisibilityConventionFixture = typeVisibilityConventionFixture;
    }

    [Fact]
    public void TypeVisibilityConventionsShouldBeSatisfied()
    {
        _typeVisibilityConventionFixture.AssertTypeVisibilityConventionsFromAssembly(
            typeof(NonSubstitutableMemberAnalyzer).Assembly, typeof(AbstractDiagnosticDescriptorsProvider<>).Assembly);
    }
}