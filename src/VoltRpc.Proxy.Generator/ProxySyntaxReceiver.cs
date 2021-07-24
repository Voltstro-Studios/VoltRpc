using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace VoltRpc.Proxy.Generator
{
    public class ProxySyntaxReceiver : ISyntaxContextReceiver
    {
        public List<InterfaceDeclarationSyntax> Interfaces { get; } = new List<InterfaceDeclarationSyntax>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is InterfaceDeclarationSyntax interfaceDeclarationSyntax &&
                interfaceDeclarationSyntax.AttributeLists.Count > 0)
            {
                Interfaces.Add(interfaceDeclarationSyntax);
            }
        }
    }
}