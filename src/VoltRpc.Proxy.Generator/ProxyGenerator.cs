using System.Linq;
using Microsoft.CodeAnalysis;
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
    public class {0}
    {{
        private readonly Client client;

        public {0}(Client client)
        {{
            this.client = client;
        }}
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

                    ISymbol type = model.GetDeclaredSymbol(interfaceDeclaration);
                    AttributeData attributeData = type.GetAttributes().FirstOrDefault(x => 
                        SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol));
                    
                    if (attributeData != null)
                        GenerateInterfaceProxy(context, interfaceDeclaration);
                }
            }
        }

        private void GenerateInterfaceProxy(GeneratorExecutionContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            string interfaceProxyName = $"{interfaceDeclaration.Identifier.ValueText}Proxy";
            context.AddSource(interfaceProxyName, string.Format(BaseText, interfaceProxyName));
        }
    }
}