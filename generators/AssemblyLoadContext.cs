using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace generators
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base(isCollectible: true) { }

        public Assembly CompileAndLoad(string code)
        {
            // Создаем синтаксическое дерево
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Определяем параметры компиляции
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
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

            ms.Seek(0, SeekOrigin.Begin);
            return LoadFromStream(ms);
        }
    }
}
