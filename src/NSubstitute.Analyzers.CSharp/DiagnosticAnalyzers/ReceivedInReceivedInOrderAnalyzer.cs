using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ReceivedInReceivedInOrderAnalyzer : AbstractReceivedInReceivedInOrderAnalyzer
{
    public ReceivedInReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}