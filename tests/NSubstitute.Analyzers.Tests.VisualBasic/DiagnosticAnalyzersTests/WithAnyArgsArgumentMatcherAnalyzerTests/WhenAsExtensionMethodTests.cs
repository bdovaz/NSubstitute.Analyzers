using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class WhenAsExtensionMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                          x.Foo = {arg}
                                      End Function)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                          x.Foo = {arg}
                                      End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                          x.Foo = {arg}
                                      End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Action(Of Integer)
        Default Property Item(ByVal x As Integer?) As Action(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                  x.Foo = {arg}
                              End Function)
            substitute.{method}(Function(x)
                                  x(1) = {arg}
                              End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                    x({arg}) = {arg}
                        End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                    x({arg}) = {arg}
                        End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                    x({arg}) = {arg}
                        End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Action(Of Integer)
        Default Property Item(ByVal x As Integer?) As Action(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(Function(x)
                                  x.Foo = {arg}
                              End Function)
            substitute.{method}(Function(x)
                                  x(1) = {arg}
                              End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Sub Bar(ByVal x As Integer?)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(x)
                                          x.Bar({arg})
                                      End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Sub Bar(ByVal x As Integer?)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(x)
                                          x.Bar({arg})
                                      End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Sub Bar(ByVal x As Integer?)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(x)
                                          x.Bar({arg})
                                      End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}