using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VoltRpc.Proxy.Generator
{
    [Generator]
    public class ProxyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization(i => i.AddSource("GenerateProxyAttribute", ProxyCodeTemplates.GenerateProxyAttributeCode));
            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is ProxySyntaxReceiver proxySyntax)
            {
                INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(ProxyCodeTemplates.GenerateProxyAttributeName);
                SemanticModel model = null;
                
                foreach (InterfaceDeclarationSyntax interfaceDeclaration in proxySyntax.Interfaces)
                {
                    if (model == null || model.SyntaxTree != interfaceDeclaration.SyntaxTree)
                        model = context.Compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);

                    ISymbol type = ModelExtensions.GetDeclaredSymbol(model, interfaceDeclaration);
                    AttributeData attributeData = type.GetAttributes().FirstOrDefault(x => 
                        SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol));
                    
                    if (attributeData != null)
                        GenerateInterfaceProxy(context, model, interfaceDeclaration);
                }
            }
        }

        private void GenerateInterfaceProxy(GeneratorExecutionContext context, SemanticModel model, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            INamedTypeSymbol interfaceSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
            string interfaceName = $"{interfaceDeclaration.Identifier.ValueText}";
            string interfaceProxyName = $"{interfaceName}_GeneratedProxy";
            string interfaceNamespace = interfaceSymbol.ContainingNamespace.ToString();
                
            StringBuilder methods = new StringBuilder();
            foreach (MemberDeclarationSyntax memberDeclarationSyntax in interfaceDeclaration.Members)
            {
                if(memberDeclarationSyntax.Modifiers.All(modifer => modifer.Kind() != SyntaxKind.PublicKeyword))
                    continue;
                
                MethodDeclarationSyntax method = (MethodDeclarationSyntax) memberDeclarationSyntax;
                methods.Append(CreateMethod(model, method, interfaceNamespace, interfaceName));
            }

            context.AddSource(interfaceProxyName, string.Format(ProxyCodeTemplates.ProxyCodeTemplate, interfaceProxyName, $"{interfaceNamespace}.{interfaceName}", methods));
        }

        private string CreateMethod(SemanticModel model, MethodDeclarationSyntax method, string interfaceNamespace, string interfaceName)
        {
            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method);
            
            StringBuilder methodSb = new StringBuilder();
            methodSb.Append($"\n\t\tpublic {method.ReturnType.ToFullString()}{method.Identifier.ValueText}(");

            //Create parms
            int parametersCount = methodSymbol.Parameters.Length;
            for (int i = 0; i < parametersCount; i++)
            {
                IParameterSymbol symbol = methodSymbol.Parameters[i];
                switch (symbol.RefKind)
                {
                    case RefKind.Ref:
                        methodSb.Append("ref ");
                        break;
                    case RefKind.Out:
                        methodSb.Append("out ");
                        break;
                }
                
                methodSb.Append($"{symbol.Type} {symbol.Name}");
                if (i + 1 != parametersCount)
                    methodSb.Append(", ");
            }
            
            methodSb.Append(")\n");
            methodSb.Append("\t\t{\n");
            methodSb.Append("\t\t\t");
            
            if (!methodSymbol.ReturnsVoid)
                methodSb.Append($"return ({methodSymbol.ReturnType}) ");

            methodSb.Append($"client.InvokeMethod(\"{interfaceNamespace}.{interfaceName}.{method.Identifier.ValueText}\"");
            bool anyArrayParms = methodSymbol.Parameters.Any(x => x.Type is IArrayTypeSymbol);
            
            if (parametersCount > 0)
            {
                methodSb.Append(", ");
                
                if(anyArrayParms)
                    methodSb.Append("new object[]{ ");
                
                for (int i = 0; i < parametersCount; i++)
                {
                    IParameterSymbol symbol = methodSymbol.Parameters[i];
                    methodSb.Append($"{symbol.Name}");
                    if (i + 1 != parametersCount)
                        methodSb.Append(", ");
                }

                if(anyArrayParms)
                    methodSb.Append(" }");
            }

            methodSb.Append(");");
            methodSb.AppendLine("\n\t\t}");
            return methodSb.ToString();
        }
    }
}