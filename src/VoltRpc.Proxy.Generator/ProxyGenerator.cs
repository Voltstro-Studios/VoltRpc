using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VoltRpc.Proxy.Generator
{
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
                INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(ProxyCodeTemplates.GenerateProxyAttributeName);
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
                        GenerateInterfaceProxy(context, model, interfaceDeclaration);
                }
            }
        }

        /// <summary>
        ///     Generates a source file for a <see cref="InterfaceDeclarationSyntax"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="interfaceDeclaration"></param>
        private void GenerateInterfaceProxy(GeneratorExecutionContext context, SemanticModel model, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            //Get info about the interface first
            INamedTypeSymbol interfaceSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
            if(interfaceSymbol == null)
                return;
            
            string interfaceName = $"{interfaceDeclaration.Identifier.ValueText}";
            string interfaceProxyName = $"{interfaceName}_GeneratedProxy";
            string interfaceNamespace = interfaceSymbol.ContainingNamespace.ToString();
                
            //Create all of the methods
            StringBuilder methods = new StringBuilder();
            foreach (MemberDeclarationSyntax memberDeclarationSyntax in interfaceDeclaration.Members)
            {
                //Member needs to be public
                if(memberDeclarationSyntax.Modifiers.All(modifer => modifer.Kind() != SyntaxKind.PublicKeyword))
                    continue;
                
                MethodDeclarationSyntax method = (MethodDeclarationSyntax) memberDeclarationSyntax;
                methods.Append(CreateMethod(model, method, interfaceNamespace, interfaceName));
            }

            //Add the source
            context.AddSource(interfaceProxyName, string.Format(ProxyCodeTemplates.ProxyCodeTemplate, interfaceProxyName, $"{interfaceNamespace}.{interfaceName}", methods));
        }

        /// <summary>
        ///     Creates source for a <see cref="MethodDeclarationSyntax"/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="method"></param>
        /// <param name="interfaceNamespace"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        private string CreateMethod(SemanticModel model, MethodDeclarationSyntax method, string interfaceNamespace, string interfaceName)
        {
            //Get info about the method first
            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method);
            if (methodSymbol == null)
                return string.Empty;
            
            StringBuilder methodSb = new StringBuilder();
            
            //Declaration of the method
            methodSb.Append($"\n\t\tpublic {method.ReturnType.ToFullString()}{method.Identifier.ValueText}(");

            //Create parms
            int parametersCount = methodSymbol.Parameters.Length;
            for (int i = 0; i < parametersCount; i++)
            {
                IParameterSymbol symbol = methodSymbol.Parameters[i];
                
                //If it is a ref or out we need to declare that too
                switch (symbol.RefKind)
                {
                    case RefKind.Ref:
                        methodSb.Append("ref ");
                        break;
                    case RefKind.Out:
                        methodSb.Append("out ");
                        break;
                }
                
                //Write the parm type and name
                methodSb.Append($"{symbol.Type} {symbol.Name}");
                
                //Add a ',' for the next parm
                if (i + 1 != parametersCount)
                    methodSb.Append(", ");
            }
            
            methodSb.Append(")\n");
            methodSb.Append("\t\t{\n");
            methodSb.Append("\t\t\t");
            
            bool anyOutOrRefParms =
                methodSymbol.Parameters.Any(x => x.RefKind == RefKind.Ref || x.RefKind == RefKind.Out);
            
            //If the method returns and object, we need to cast the result from client.InvokeMethod to the return type
            if (!methodSymbol.ReturnsVoid && !anyOutOrRefParms)
                methodSb.Append($"return ({methodSymbol.ReturnType}) ");

            if (anyOutOrRefParms)
                methodSb.Append("object[] returnObjects = ");

            methodSb.Append($"client.InvokeMethod(\"{interfaceNamespace}.{interfaceName}.{method.Identifier.ValueText}\"");
            
            //Write parameters for InvokeMethod
            methodSb.Append(WriteInvokeMethodParameters(methodSymbol.Parameters));

            methodSb.Append(")");
            if (!anyOutOrRefParms && !methodSymbol.ReturnsVoid)
                methodSb.Append("[0]");
            methodSb.Append(";");

            int parameterIndex = 0;
            if (!methodSymbol.ReturnsVoid)
                parameterIndex++;
            
            for (int i = 0; i < parametersCount; i++)
            {
                IParameterSymbol symbol = methodSymbol.Parameters[i];
                if (symbol.RefKind == RefKind.Ref || symbol.RefKind == RefKind.Out)
                {
                    methodSb.Append($"\n\t\t\t{symbol.Name} = ({symbol.Type}) returnObjects[{parameterIndex}];");
                    parameterIndex++; 
                }
            }

            if (anyOutOrRefParms && !methodSymbol.ReturnsVoid)
                methodSb.Append($"\n\t\t\treturn ({methodSymbol.ReturnType}) returnObjects[0];");
            
            methodSb.AppendLine("\n\t\t}");
            return methodSb.ToString();
        }

        private string WriteInvokeMethodParameters(ImmutableArray<IParameterSymbol> parameterSymbols)
        {
            StringBuilder parameterString = new StringBuilder();
            List<IParameterSymbol> parameters = parameterSymbols.Where(parameterSymbol => parameterSymbol.RefKind != RefKind.Out).ToList();

            if (parameters.Count > 0)
            {
                parameterString.Append(", ");
                bool anyArrays = parameters.Any(x => x.Type is IArrayTypeSymbol);
                
                //If any parms is an array we need to do some extra stuff, as client.InvokeMethod arguments is an params object[]
                if(anyArrays)
                    parameterString.Append("new object[]{ ");

                int parametersCount = parameters.Count;
                for (int i = 0; i < parametersCount; i++)
                {
                    IParameterSymbol symbol = parameters[i];

                    parameterString.Append($"{symbol.Name}");
                    if (i + 1 != parametersCount)
                        parameterString.Append(", ");
                }

                if (anyArrays)
                    parameterString.Append(" }");
            }

            return parameterString.ToString();
        }
    }
}