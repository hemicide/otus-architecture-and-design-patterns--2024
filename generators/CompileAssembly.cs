using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace generators
{
    public class CompileAssembly
    {

        public static Type CreateProxyType<T>()
        {
            return CreateProxyType(typeof(T));
        }

        public static Type CreateProxyType(Type interfaceType)
        {
            var className = $"{interfaceType.Name}Adapter";
            var classCode = GenerateClassCode(interfaceType, className);
            var assembly = CompileCode(classCode);

            return assembly.GetType(className);
        }


        private static string GenerateClassCode(Type interfaceType, string className)
        {
            string interfaceName = interfaceType.Name;

            // Получаем методы интерфейса
            var methods = interfaceType.GetMethods();

            // Генерация кода класса
            string methodImplementations = string.Join(Environment.NewLine, methods.Select(method =>
            {
                string parametersWithType = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                string parametersWthioutType = string.Join(", ", method.GetParameters().Select(p => $"{p.Name}"));
                string returnType = method.ReturnType == typeof(void) ? "void" : method.ReturnType.Name;

                if (method.ReturnType == typeof(void))
                {
                    return $@"
                public {returnType} {method.Name}({parametersWithType})
                {{
                    IoC.Resolve<ICommand>(""{interfaceName}.{method.Name}"", _obj, {parametersWthioutType}).Execute();
                }}";
                } else {
                    return $@"
                public {returnType} {method.Name}({parametersWithType})
                {{
                    return IoC.Resolve<{returnType}>(""{interfaceName}.{method.Name}"", _obj);
                }}";
                }
            }));

            return $@"
            using System;
            using System.Numerics;
            using factory;
            using commands;
            using SpaceBattle.Interfaces;

            public class {className} : {interfaceName}
            {{
                private IUObject _obj;
                public {className}(IUObject obj)
                {{
                    _obj = obj;
                }}
                {methodImplementations}
            }}";
        }

        private static Assembly CompileCode(string code)
        {
            // Создаем синтаксическое дерево
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Определяем параметры компиляции
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "CompileAssembly",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new System.IO.MemoryStream();
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
                throw new InvalidOperationException(string.Join("\r\n", result.Diagnostics.Select(r => r.GetMessage())));


            ms.Seek(0, System.IO.SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }
    }
}
