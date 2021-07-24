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
        private const string AttributeName = "VoltRpc.Proxy.GenerateProxyAttribute";

        private const string AttributeText = @"using System;

namespace VoltRpc.Proxy
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class GenerateProxyAttribute : Attribute
    {
        public GenerateProxyAttribute()
        {
        }
    }
}";

        private const string BaseText = @"using VoltRpc.Communication;

namespace VoltRpc.Proxy.Generated
{{
    public class {0} : {1}
    {{
        private readonly Client client;

        public {0}(Client client)
        {{
            this.client = client;
        }}

        {2}
    }}
}}";
        
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization(i => i.AddSource("GenerateProxyAttribute", AttributeText));
            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is ProxySyntaxReceiver proxySyntax)
            {
                INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeName);
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
                
            StringBuilder stringBuilder = new StringBuilder();
            foreach (MemberDeclarationSyntax memberDeclarationSyntax in interfaceDeclaration.Members)
            {
                if(memberDeclarationSyntax.Modifiers.All(modifer => modifer.Kind() != SyntaxKind.PublicKeyword))
                    continue;
                
                MethodDeclarationSyntax method = (MethodDeclarationSyntax) memberDeclarationSyntax;
                stringBuilder.Append(CreateMethod(model, method));
            }
            
            context.AddSource(interfaceProxyName, string.Format(BaseText, interfaceProxyName, $"{interfaceNamespace}.{interfaceName}", stringBuilder));
        }

        private string CreateMethod(SemanticModel model, MethodDeclarationSyntax method)
        {
            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method);
            
            StringBuilder methodSb = new StringBuilder();
            methodSb.Append($"\npublic {method.ReturnType.ToFullString()}{method.Identifier.ValueText}(");

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

            methodSb.Append(")\n{\n");

            //TODO: Implement 
            if (!methodSymbol.ReturnsVoid)
            {
                methodSb.Append("return null;\n");
            }

            methodSb.Append("}\n");
            
            return methodSb.ToString();
        }
    }
}