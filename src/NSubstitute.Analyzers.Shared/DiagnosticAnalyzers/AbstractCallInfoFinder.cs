using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractCallInfoFinder : ICallInfoFinder
{
    public CallInfoContext GetCallInfoContext(SemanticModel semanticModel, IArgumentOperation argumentOperation)
        {
            var callContext = CallInfoContext.Empty;
            foreach (var syntaxNode in argumentOperation.GetSyntaxes())
            {
                var callInfoParameterSymbol = GetCallInfoParameterSymbol(semanticModel, syntaxNode);
                var currentContext = GetCallInfoContextInternal(semanticModel, syntaxNode);
                callContext =
                    callContext.Merge(CreateFilteredCallInfoContext(semanticModel, currentContext, callInfoParameterSymbol));
            }

            return callContext;
    }

    protected abstract CallInfoContext GetCallInfoContextInternal(SemanticModel semanticModel, SyntaxNode syntaxNode);

    private static CallInfoContext CreateFilteredCallInfoContext(
        SemanticModel semanticModel,
        CallInfoContext callContext,
        IParameterSymbol callInfoParameterSymbol)
    {
        return new CallInfoContext(
            argAtInvocations: GetMatchingNodes(semanticModel, callContext.ArgAtInvocations, callInfoParameterSymbol),
            argInvocations: GetMatchingNodes(semanticModel, callContext.ArgInvocations, callInfoParameterSymbol),
            indexerAccesses: GetMatchingNodes(semanticModel, callContext.IndexerAccesses, callInfoParameterSymbol));
    }

    private static IReadOnlyList<T> GetMatchingNodes<T>(
        SemanticModel semanticModel,
        IReadOnlyList<T> nodes,
        IParameterSymbol parameterSymbol) where T : SyntaxNode
    {
        return nodes.Where(node => HasMatchingParameterReference(semanticModel, node, parameterSymbol)).ToList();
    }

    private static bool HasMatchingParameterReference(
        SemanticModel semanticModel,
        SyntaxNode syntaxNode,
        IParameterSymbol callInfoParameterSymbol)
    {
        var parameterReferenceOperation = FindMatchingParameterReference(semanticModel, syntaxNode);

        return parameterReferenceOperation != null &&
               parameterReferenceOperation.Parameter.Equals(callInfoParameterSymbol);
    }

    private static IParameterReferenceOperation FindMatchingParameterReference(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        var operation = semanticModel.GetOperation(syntaxNode);
        return FindMatchingParameterReference(operation);
    }

    private static IParameterReferenceOperation FindMatchingParameterReference(IOperation operation)
    {
        IParameterReferenceOperation parameterReferenceOperation = null;
        switch (operation)
        {
            case IInvocationOperation invocationOperation:
                parameterReferenceOperation = invocationOperation.Instance as IParameterReferenceOperation;
                break;
            case IPropertyReferenceOperation propertyReferenceOperation:
                parameterReferenceOperation = propertyReferenceOperation.Instance as IParameterReferenceOperation;
                break;
        }

        if (parameterReferenceOperation != null)
        {
            return parameterReferenceOperation;
        }

        foreach (var innerOperation in operation?.Children ?? Enumerable.Empty<IOperation>())
        {
            parameterReferenceOperation = FindMatchingParameterReference(innerOperation);
            if (parameterReferenceOperation != null)
            {
                return parameterReferenceOperation;
            }
        }

        return null;
    }

    private static IParameterSymbol GetCallInfoParameterSymbol(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (semanticModel.GetSymbolInfo(syntaxNode).Symbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind != MethodKind.Constructor)
        {
            return methodSymbol.Parameters.FirstOrDefault();
        }

        return null;
    }
}