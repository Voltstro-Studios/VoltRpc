using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        //Interfaces can only be internal or public
        Accessibility interfaceAccessibility = interfaceSymbol.DeclaredAccessibility;
        if (interfaceAccessibility is Accessibility.Protected or Accessibility.Private or Accessibility.ProtectedAndInternal)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.InterfaceInvalidAccessibility, 
                Location.Create(interfaceDeclaration.SyntaxTree, interfaceDeclaration.Span)));
            
            return;
        }
        
        string interfaceName = $"{interfaceDeclaration.Identifier.ValueText}";
        string interfaceNamespace = interfaceSymbol.ContainingNamespace.ToString();
        string interfaceProxyName = $"{interfaceName}_GeneratedProxy";

        //Check if their is specified name
        TypedConstant overrideName = generateProxyData.NamedArguments
            .SingleOrDefault(x => x.Key == ProxyCodeTemplates.GenerateProxyAttributeOverrideName).Value;
        if (!overrideName.IsNull)
        {
            interfaceProxyName = overrideName.Value?.ToString();
            
            //Make sure is not blank or whitespace
            if (string.IsNullOrWhiteSpace(interfaceProxyName))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ProxyNameBlank, 
                    Location.Create(interfaceDeclaration.SyntaxTree, interfaceDeclaration.Span)));
                return;
            }
        }

        //Custom proxy namespace
        string proxyNamespace = ProxyCodeTemplates.GenerateProxyGeneratedDefaultNameSpace;
        TypedConstant overrideNamespace = generateProxyData.NamedArguments
            .SingleOrDefault(x => x.Key == ProxyCodeTemplates.GenerateProxyAttributeOverrideNamespace).Value;
        if (!overrideNamespace.IsNull)
        {
            proxyNamespace = overrideNamespace.Value?.ToString();

            //Make sure namespace is not blank or whitespace
            if (string.IsNullOrWhiteSpace(proxyNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ProxyNamespaceBlank, 
                    Location.Create(interfaceDeclaration.SyntaxTree, interfaceDeclaration.Span)));
                return;
            }
        }

        //Forces the generated proxy to be public
        TypedConstant forcePublicInterface = generateProxyData.NamedArguments
            .SingleOrDefault(x => x.Key == ProxyCodeTemplates.GenerateProxyAttributeOverrideForcePublic).Value;
        if (!forcePublicInterface.IsNull)
        {
            bool value = forcePublicInterface.Value != null && (bool)forcePublicInterface.Value;
            if (value)
                interfaceAccessibility = Accessibility.Public;
        }
        
        //Create all of the methods
        List<Method> generatedMethods = new();
        foreach (MemberDeclarationSyntax memberDeclarationSyntax in interfaceDeclaration.Members)
        {
            if (memberDeclarationSyntax is MethodDeclarationSyntax methodDeclarationSyntax)
            {
                Method? method = CreateMethod(context, model, methodDeclarationSyntax, interfaceNamespace, interfaceName);
                
                //Something went wrong if it is null
                if (method == null)
                    return;

                generatedMethods.Add(method.Value);
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.NotAMethodError, 
                    Location.Create(memberDeclarationSyntax.SyntaxTree, memberDeclarationSyntax.Span)));
                return;
            }
        }

        //Do some cleanup with the methods
        string methods = string.Join("\n\n", generatedMethods);
        string[] splitMethods = methods.Split(new []{"\n"}, StringSplitOptions.None);
        for (int i = 0; i < splitMethods.Length; i++)
        {
            splitMethods[i] = "\t\t" + splitMethods[i];
        }

        methods = string.Join("\n", splitMethods);
        string code = string.Format(ProxyCodeTemplates.ProxyCodeTemplate,
            proxyNamespace,
            interfaceProxyName,
            $"{interfaceNamespace}.{interfaceName}",
            methods,
            interfaceAccessibility.AsString());

        //Add the source
        context.AddSource(interfaceProxyName, code);
    }

    /// <summary>
    ///     Creates source for a <see cref="MethodDeclarationSyntax" />
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="methodSyntax"></param>
    /// <param name="interfaceNamespace"></param>
    /// <param name="interfaceName"></param>
    /// <returns></returns>
    private Method? CreateMethod(GeneratorExecutionContext context, SemanticModel model, MethodDeclarationSyntax methodSyntax, string interfaceNamespace,
        string interfaceName)
    {
        //Get info about the method first
        IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodSyntax);
        if (methodSymbol == null)
            return null;

        Accessibility methodAccessibility = methodSymbol.DeclaredAccessibility;
        
        //We only support methods that are public or internal
        if (methodAccessibility is Accessibility.Protected or Accessibility.ProtectedAndInternal)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.InvalidMethodAccessibility, 
                Location.Create(methodSyntax.SyntaxTree, methodSyntax.Span)));
            
            return null;
        }

        List<Argument> arguments = new();

        int parametersCount = methodSymbol.Parameters.Length;
        for (int i = 0; i < parametersCount; i++)
        {
            IParameterSymbol symbol = methodSymbol.Parameters[i];

            bool isRef = false;
            bool isOut = false;
            switch (symbol.RefKind)
            {
                case RefKind.Ref:
                    isRef = true;
                    break;
                case RefKind.Out:
                    isOut = true;
                    break;
            }

            Argument argument = new(symbol.Name, symbol.Type.ToString(), symbol.Type is IArrayTypeSymbol, isRef, isOut);
            arguments.Add(argument);
        }

        Method method = new($"{interfaceNamespace}.{interfaceName}", methodSyntax.Identifier.ValueText, methodAccessibility, methodSymbol.ReturnsVoid ? null : methodSymbol.ReturnType.ToString(), arguments);

        return method;
    }
}