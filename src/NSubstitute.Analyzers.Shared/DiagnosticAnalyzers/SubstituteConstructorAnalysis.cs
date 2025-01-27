﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal sealed class SubstituteConstructorAnalysis : ISubstituteConstructorAnalysis
{
    public static SubstituteConstructorAnalysis Instance { get; } = new();

    public ConstructorContext CollectConstructorContext(SubstituteContext substituteContext, ITypeSymbol proxyTypeSymbol)
    {
        if (proxyTypeSymbol.Kind == SymbolKind.TypeParameter)
        {
            return new ConstructorContext(proxyTypeSymbol, null, null, null);
        }

        var invocationParameterTypes = GetInvocationInfo(substituteContext);

        bool IsPossibleConstructor(IMethodSymbol methodSymbol)
        {
            var nonParamsParametersCount = methodSymbol.Parameters.Count(parameter => !parameter.IsParams);

            if (nonParamsParametersCount == methodSymbol.Parameters.Length)
            {
                return methodSymbol.Parameters.Length == invocationParameterTypes.Length;
            }

            return invocationParameterTypes.Length >= nonParamsParametersCount;
        }

        var accessibleConstructors = GetAccessibleConstructors(proxyTypeSymbol);
        var possibleConstructors = invocationParameterTypes != null && accessibleConstructors != null
            ? accessibleConstructors.Where(IsPossibleConstructor)
                .ToArray()
            : null;

        return new ConstructorContext(
            proxyTypeSymbol,
            accessibleConstructors,
            possibleConstructors,
            invocationParameterTypes);
    }

    private ITypeSymbol[] GetInvocationInfo(SubstituteContext substituteContext)
    {
        var infos = substituteContext.InvocationOperation.TargetMethod.IsGenericMethod
            ? GetGenericInvocationArgumentTypes(substituteContext)
            : GetNonGenericInvocationArgumentTypes(substituteContext);

        return infos;
    }

    private ITypeSymbol[] GetGenericInvocationArgumentTypes(SubstituteContext substituteContext)
    {
        var arguments = substituteContext.InvocationOperation.Arguments;

        if (arguments.Length == 0)
        {
            return Array.Empty<ITypeSymbol>();
        }

        // if passing array of objects as a sole element
        if (arguments.Length == 1 && arguments.Single().Value is IArrayCreationOperation arrayTypeSymbol)
        {
            return TypeSymbols(arrayTypeSymbol);
        }

        return arguments.SelectMany(GetArgumentType).ToArray();
    }

    private ITypeSymbol[] GetNonGenericInvocationArgumentTypes(SubstituteContext substituteContext)
    {
        // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
        var arrayArgument = substituteContext.InvocationOperation.GetOrderedArgumentOperations().Skip(1)
            .FirstOrDefault();

        if (arrayArgument == null)
        {
            return null;
        }

        // if passing array of objects as a sole element
        if (arrayArgument.Value is IArrayCreationOperation arrayTypeSymbol)
        {
            return TypeSymbols(arrayTypeSymbol);
        }

        return GetArgumentType(arrayArgument);
    }

    private IMethodSymbol[] GetAccessibleConstructors(ITypeSymbol genericArgument)
    {
        var internalsVisibleToProxy = genericArgument.InternalsVisibleToProxyGenerator();

        bool IsAccessible(IMethodSymbol symbol)
        {
            return symbol.DeclaredAccessibility == Accessibility.Protected ||
                   symbol.DeclaredAccessibility == Accessibility.Public;
        }

        bool IsVisibleToProxy(IMethodSymbol symbol)
        {
            if (internalsVisibleToProxy == false)
            {
                return false;
            }

            return symbol.DeclaredAccessibility is Accessibility.Internal or Accessibility.ProtectedOrInternal;
        }

        return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
            symbol.MethodKind == MethodKind.Constructor &&
            symbol.IsStatic == false &&
            (IsAccessible(symbol) || IsVisibleToProxy(symbol))).ToArray();
    }

    private ITypeSymbol[] TypeSymbols(IArrayCreationOperation arrayInitializerOperation)
    {
        return arrayInitializerOperation.Initializer.ElementValues.Select(item =>
                item is IConversionOperation conversionOperation
                    ? conversionOperation.Operand.Type
                    : item.Type)
            .ToArray();
    }

    private ITypeSymbol[] GetArgumentType(IArgumentOperation argumentOperation)
    {
        if (argumentOperation.ArgumentKind != ArgumentKind.ParamArray)
        {
            return Array.Empty<ITypeSymbol>();
        }

        var arrayInitializerOperation = argumentOperation.Value as IArrayCreationOperation;

        return TypeSymbols(arrayInitializerOperation);
    }
}