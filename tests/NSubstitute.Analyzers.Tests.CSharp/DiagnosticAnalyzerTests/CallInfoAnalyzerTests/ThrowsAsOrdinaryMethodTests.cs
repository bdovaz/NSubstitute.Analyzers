using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    [CombinatoryData("ExceptionExtensions.Throws", "ExceptionExtensions.ThrowsForAnyArgs")]
    public class ThrowsAsOrdinaryMethodTests : CallInfoDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var returnedValue = {call};
            {method}(returnedValue, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentOutOfRange;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage("There is no argument at position 1"));
        }

        public override async Task
            ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method,
                string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int Barr {{ get; }}

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int Barr {{ get; }}

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, Bar y);

        int this[int x, Bar y] {{ get; }}
    }}

    public class BarBase
    {{
    }}

    public class Bar : BarBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[int x, FooBar bar] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooBar : Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoCouldNotConvertParameterAtPosition;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage("Couldn't convert parameter at position 1 to type MyNamespace.Bar."));
        }

        public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, Bar y);

        int this[int x, Bar y] {{ get; }}
    }}

    public class BarBase
    {{
    }}

    public class Bar : BarBase
    {{
    }}


    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call,
            string argAccess, string message)
        {
            var source = $@"using System;
using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[int x, FooBar bar] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooBar : Bar
    {{
    }}
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoCouldNotConvertParameterAtPosition;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage(message));
        }

        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call,
            string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(
            string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
     public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method,
            string call, string argAccess, string message)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoCouldNotFindArgumentToThisCall;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage(message));
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method,
            string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);

        int Bar(Foo x);

        int this[int x] {{ get; }}

        int this[Foo x] {{ get; }}
    }}

    public class FooBase
    {{
    }}

    public class Foo : FooBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method,
            string call, string argAccess, string message)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoMoreThanOneArgumentOfType;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage(message));
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(
            string method, string call)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                callInfo.Arg<int>();
                return new Exception();
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method,
            string call)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                [|callInfo[1]|] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentIsNotOutOrRef;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage("Could not set argument 1 (double) as it is not an out or ref argument."));
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(ref int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(ref value), callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(out value), callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(out value), callInfo =>
            {{
                [|callInfo[1]|] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentOutOfRange;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage("There is no argument at position 1"));
        }

        public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method,
            string left, string right, string expectedMessage)
        {
            var source = $@"using System;
using NSubstitute;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(out value), callInfo =>
            {{
                [|callInfo[0]|] = {right};
                return new Exception();
            }});
        }}
    }}
}}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;

            await VerifyDiagnostic(source, expectedDiagnostic.OverrideMessage(expectedMessage));
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method,
            string left, string right)
        {
            var source = $@"using System;
using NSubstitute;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(out value), callInfo =>
            {{
                callInfo[0] = {right};
                return new Exception();
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }
    }
}