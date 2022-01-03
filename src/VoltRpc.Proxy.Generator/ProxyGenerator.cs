using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using VoltRpc.Proxy.Generator.Entities;

namespace VoltRpc.Proxy.Generator;

//So I have no clue what I am doing with this whole Roslyn .NET Source Generators.
//If you see something that is shit, or should be done some other way, please open
//a PR explaining why something is shit.

/// <summary>
///     The VoltRpc proxy generator
/// </summary>
[Generator]
public class ProxyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //Register the attribute source
        context.RegisterForPostInitialization(i => i.AddSource("GenerateProxyAttribute",
            ProxyCodeTemplates.GenerateProxyAttributeCode));

        //Register Syntax receiver
        context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is ProxySyntaxReceiver proxySyntax)
        {
            //Get the generate proxy attribute symbol
            INamedTypeSymbol attributeSymbol =
                context.Compilation.GetTypeByMetadataName(ProxyCodeTemplates.GenerateProxyAttributeName);
            SemanticModel model = null;

            //Go through all of our interfaces
            foreach (InterfaceDeclarationSyntax interfaceDeclaration in proxySyntax.Interfaces)
            {
                if (model == null || model.SyntaxTree != interfaceDeclaration.SyntaxTree)
                    model = context.Compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);

                //Get the interface symbol
                ISymbol interfaceSymbol = ModelExtensions.GetDeclaredSymbol(model, interfaceDeclaration);

                //Get the generate proxy attribute, if it has it
                AttributeData attributeData = interfaceSymbol?.GetAttributes().FirstOrDefault(x =>
                    SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol));

                if (attributeData != null)
                    GenerateInterfaceProxy(context, model, interfaceDeclaration, attributeData);
            }
        }
    }

    /// <summary>
    ///     Generates a source file for a <see cref="InterfaceDeclarationSyntax" />
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="interfaceDeclaration"></param>
    /// <param name="generateProxyData"></param>
    private void GenerateInterfaceProxy(GeneratorExecutionContext context, SemanticModel model,
        InterfaceDeclarationSyntax interfaceDeclaration, AttributeData generateProxyData)
    {
        //Get info about the interface first
        INamedTypeSymbol interfaceSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
        if (interfaceSymbol == null)
            return;

        string interfaceName = $"{interfaceDeclaration.Identifier.ValueText}";
        string interfaceProxyName = $"{interfaceName}_GeneratedProxy";
        string interfaceNamespace = interfaceSymbol.ContainingNamespace.ToString();

        TypedConstant overrideName = generateProxyData.NamedArguments
            .SingleOrDefault(x => x.Key == ProxyCodeTemplates.GenerateProxyAttributeOverrideName).Value;
        if (!overrideName.IsNull)
            interfaceProxyName = overrideName.Value?.ToString();

        //Create all of the methods
        List<Method> generatedMethods = new();
        foreach (MemberDeclarationSyntax memberDeclarationSyntax in interfaceDeclaration.Members)
        {
            //Member needs to be public
            if (memberDeclarationSyntax.Modifiers.All(modifer => modifer.Kind() != SyntaxKind.PublicKeyword))
                continue;

            if (memberDeclarationSyntax is MethodDeclarationSyntax methodDeclarationSyntax)
                generatedMethods.Add(CreateMethod(model, methodDeclarationSyntax, interfaceNamespace, interfaceName));
        }

        //Generate the code it self
        string code = Template.Parse(ProxyCodeTemplates.ProxyCodeTemplate).Render(new
        {
            classname = interfaceProxyName,
            inheritedtnterface = $"{interfaceNamespace}.{interfaceName}",
            methods = generatedMethods
        });

        //Add the source
        context.AddSource(interfaceProxyName, code);
    }

    /// <summary>
    ///     Creates source for a <see cref="MethodDeclarationSyntax" />
    /// </summary>
    /// <param name="model"></param>
    /// <param name="methodSyntax"></param>
    /// <param name="interfaceNamespace"></param>
    /// <param name="interfaceName"></param>
    /// <returns></returns>
    private Method CreateMethod(SemanticModel model, MethodDeclarationSyntax methodSyntax, string interfaceNamespace,
        string interfaceName)
    {
        //Get info about the method first
        IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodSyntax);
        if (methodSymbol == null)
            return new Method();

        List<Argument> arguments = new();

        int parametersCount = methodSymbol.Parameters.Length;
        for (int i = 0; i < parametersCount; i++)
        {
            IParameterSymbol symbol = methodSymbol.Parameters[i];
            Argument argument = new()
            {
                name = symbol.Name,
                type = symbol.Type.ToString()
            };

            switch (symbol.RefKind)
            {
                case RefKind.Ref:
                    argument.isref = true;
                    break;
                case RefKind.Out:
                    argument.isout = true;
                    break;
            }

            if (i + 1 != parametersCount)
                argument.trailing = ", ";

            arguments.Add(argument);
        }

        Method method = new()
        {
            name = methodSyntax.Identifier.ValueText,
            returntype = methodSymbol.ReturnType.ToString(),
            arguments = arguments,
            returnsvoid = methodSymbol.ReturnsVoid,
            anyarrays = methodSymbol.Parameters.Any(x => x.Type is IArrayTypeSymbol),
            interfacename = interfaceName,
            interfacenamespace = interfaceNamespace
        };

        return method;
    }
}