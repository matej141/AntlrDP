using System.Text;
using System.Text.RegularExpressions;
using AntlrDP.AnimArchAnimationClasses;
using AntlrDP.OalCodeElements;
using AntlrDP.OalCodeElements.FragmentTypes;

namespace AntlrDP.Translation;

public sealed class Translator
{
    private OalCode OalCode { get; }
    public List<TranslationClass> Classes = new();
    public const string FirstMethodName = "FirstMethod";
    private bool _firstMethodCallUsed; // false by default
    private TranslationContext? CurrentContext { get; set; }
    public bool IsDiagramValid = true;
    private bool _firstStatementNeedsToBeHandled = true;
    private readonly bool _areSelfMessagesBlankMethods;
    private const string InvalidDiagramTxt = "Sequence Diagram is Invalid";

    public Translator(OalCode oalCode, bool areSelfMessagesBlankMethods = true)
    {
        OalCode = oalCode;
        _areSelfMessagesBlankMethods = areSelfMessagesBlankMethods;
        Translate();
    }

    private void Translate()
    {
        TranslateOalClassesToTranslatorClasses();
        foreach (var codeElement in OalCode.CodeElements)
        {
            ProcessCodeElement(codeElement);
            if (!IsDiagramValid)
            {
                Classes.Clear();
                return;
            }
        }

        UpdateMethodsForLastCodeElement(OalCode.CodeElements[^1]);
        OrderMethodsInClasses();
    }

    private void ProcessCodeElement(OalCodeElement codeElement)
    {
        switch (codeElement)
        {
            case MethodCall methodCall:
                if (!CheckMagic(OalCode.CodeElements, methodCall))
                {
                    FinishDiagramAsInvalid();
                    return;
                }

                CurrentContext = TranslateMethodCall(methodCall, CurrentContext);
                break;
            case Statement statement:
                ProcessStatement(statement);
                _firstStatementNeedsToBeHandled = false;
                break;
        }
    }

    private void ProcessStatement(Statement statement)
    {
        if (statement.StatementType is WhileStatement or ForStatement && !statement.IsFirst)
        {
            // Todo: pridat do DP ako dalsi invalid... Ked je fragment LOOP a ma viacero statements...
            FinishDiagramAsInvalid();
            return;
        }

        if (statement.StatementElements.Count == 0)
        {
            if (CurrentContext == null)
            {
                // Todo: Pridat do DP mozno aj tento pripad... Ked prvy element je prazdny fragment...
                FinishDiagramAsInvalid();
                return;
            }

            HandleEmptyStatement(statement, CurrentContext);
            return;
        }

        if (CurrentContext == null)
        {
            TranslateStatementWhenContextIsNull(statement);
            CurrentContext?.Instances.Clear();
        }
        else
        {
            var newContext = TranslateStatement(statement, CurrentContext, _firstStatementNeedsToBeHandled);
            UpdateCurrentContext(newContext);
        }
    }

    private void UpdateMethodsForLastCodeElement(OalCodeElement codeElement)
    {
        if (OalCode.CodeElements[^1] != codeElement) return;
        UpdateSenderMethods(CurrentContext);
    }

    private void OrderMethodsInClasses()
    {
        var methodCallNames = OalCode.MethodCalls.Select(obj => obj.Body).ToList();
        methodCallNames.Insert(0, FirstMethodName);

        foreach (var translationClass in Classes)
        {
            var methodNames = translationClass.Methods.Select(obj => obj.Name).ToList();
            var reorderedListOfTranslationMethods = new List<TranslationMethod>();
            var indexMapOfMethodCallNames = new Dictionary<string, int>();
            for (var i = 0; i < methodCallNames.Count; i++)
            {
                if (methodNames.Contains(methodCallNames[i]) &&
                    !indexMapOfMethodCallNames.ContainsKey(methodCallNames[i]))
                {
                    indexMapOfMethodCallNames.Add(methodCallNames[i], i);
                }
            }

            var sortedMapOfMethodCallNames =
                indexMapOfMethodCallNames.OrderBy(x => x.Value);

            foreach (var kvp in sortedMapOfMethodCallNames)
            {
                reorderedListOfTranslationMethods.Add(
                    translationClass.Methods.FirstOrDefault(x => GetName(x) == kvp.Key));
            }

            translationClass.Methods = reorderedListOfTranslationMethods;
        }
    }

    private static string GetName(object obj)
    {
        return obj switch
        {
            TranslationMethod translationMethod => translationMethod.Name,
            MethodCall methodCall => methodCall.Body,
            _ => throw new ArgumentException("Unsupported type")
        };
    }

    private TranslationContext InitializeFirstMethodCall(MethodCall methodCall)
    {
        var (senderClass, receiverClass, instanceName, methodName) = GetBasicElementsOfMethodCall(methodCall);
        if (senderClass == null || receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        var context = new TranslationContext
        {
            MethodName = FirstMethodName,
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = ""
        };

        if (senderClass != receiverClass)
        {
            context.CurrentCode += CreateInstance(receiverClass, instanceName);
        }

        context.CurrentCode += CreateCall(instanceName, methodName);

        if (context.Sender == receiverClass && !_areSelfMessagesBlankMethods)
        {
            context = InitSelfMethod(methodName, receiverClass, context);
            return context;
        }

        var receiverMethod = new TranslationMethod
            { Name = methodName, Code = "", IsSelfMethod = senderClass == receiverClass };
        receiverClass.Methods.Add(receiverMethod);
        context.Instances.Add(receiverClass);
        context.LastMethodCalled = methodName;

        _firstMethodCallUsed = true;
        return context;
    }

    private (TranslationClass? senderClass, TranslationClass? receiverClass, string instanceName, string methodName)
        GetBasicElementsOfMethodCall(MethodCall methodCall)
    {
        var senderClass = GetSenderClass(methodCall);
        var receiverClass = GetReceiverClass(methodCall);
        var instanceName = GetInstanceName(methodCall);
        var methodName = methodCall.Body;
        return (senderClass, receiverClass, instanceName, methodName);
    }

    private TranslationContext UpdateSenderMethods(TranslationContext? context)
    {
        if (context == null)
        {
            return FinishDiagramAsInvalid();
        }
        
        var senderMethod =
            context.Sender.Methods.Find(translationMethod => translationMethod.Name == context.MethodName);
        if (senderMethod != null)
        {
            var ind = context.Sender.Methods.FindIndex(
                translationMethod => translationMethod.Name == context.MethodName);
            context.Sender.Methods.RemoveAll(translationMethod => translationMethod.Name == context.MethodName);
            senderMethod = new TranslationMethod
                { Name = context.MethodName, Code = context.CurrentCode, Instances = context.Instances };
            context.Sender.Methods.Insert(ind, senderMethod);
        }
        else
        {
            senderMethod = new TranslationMethod
                { Name = context.MethodName, Code = context.CurrentCode, Instances = context.Instances };

            context.Sender.Methods.Add(senderMethod);
        }


        return context;
    }

    private TranslationContext InitializeNewContext(MethodCall methodCall, TranslationContext context)
    {
        var (senderClass, receiverClass, instanceName, methodName) = GetBasicElementsOfMethodCall(methodCall);
        if (senderClass == null || receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        var newMethodNames = context.LastMethodCalled;
        context = new TranslationContext
        {
            MethodName = newMethodNames,
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = ""
        };
        if (senderClass != receiverClass)
        {
            context.CurrentCode += CreateInstance(receiverClass, instanceName);
        }

        context.Instances.Add(receiverClass);

        context.CurrentCode += CreateCall(instanceName, methodName);

        if (context.Sender == receiverClass && !_areSelfMessagesBlankMethods)
        {
            context = InitSelfMethod(methodName, receiverClass, context);
            return context;
        }

        var receiverMethod = new TranslationMethod
            { Name = methodName, Code = "", IsSelfMethod = senderClass == receiverClass };
        if (!receiverClass.Methods.Contains(receiverMethod))
        {
            receiverClass.Methods.Add(receiverMethod);
        }

        context.LastMethodCalled = methodName;
        return context;
    }

    private TranslationMethod? FindLastMethodOfSenderClass(TranslationClass senderClass)
    {
        if (senderClass.Methods.Count == 0)
        {
            return null;
        }

        var lastMethod = senderClass.Methods.Last();
        foreach (var method in senderClass.Methods.AsEnumerable().Reverse())
        {
            if (method.IsSelfMethod && _areSelfMessagesBlankMethods)
            {
                continue;
            }

            lastMethod = new TranslationMethod(method);
            return lastMethod;
        }

        return lastMethod;
    }

    private TranslationContext InitializeContextFromLastMethodOfSenderClass(MethodCall methodCall,
        TranslationContext context)
    {
        var (senderClass, receiverClass, instanceName, methodName) =
            GetBasicElementsOfMethodCall(methodCall);
        if (senderClass == null || receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        var lastMethod = FindLastMethodOfSenderClass(senderClass);
        if (lastMethod == null)
        {
            return context;
        }

        context = new TranslationContext
        {
            MethodName = lastMethod.Name,
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = lastMethod.Code,
            Instances = lastMethod.Instances
        };

        if (senderClass != receiverClass && !lastMethod.Instances.Contains(receiverClass))
        {
            context.CurrentCode += CreateInstance(receiverClass, instanceName);
            context.Instances.Add(receiverClass);
        }

        context.CurrentCode += CreateCall(instanceName, methodName);

        if (context.Sender == receiverClass && !_areSelfMessagesBlankMethods)
        {
            context = InitSelfMethod(methodName, receiverClass, context);
            return context;
        }

        var receiverMethod = new TranslationMethod
            { Name = methodName, Code = "", IsSelfMethod = senderClass == receiverClass };
        if (!receiverClass.Methods.Contains(receiverMethod))
        {
            receiverClass.Methods.Add(receiverMethod);
        }

        context.LastMethodCalled = methodName;
        return context;
    }

    private TranslationContext InitSelfMethod(string methodName, TranslationClass senderAndReceiverClass,
        TranslationContext context)
    {
        UpdateSenderMethods(context);
        context = new TranslationContext
        {
            MethodName = methodName,
            Sender = senderAndReceiverClass,
            Receiver = senderAndReceiverClass,
            CurrentCode = ""
        };
        return context;
    }

    private TranslationContext TranslateMethodCall(MethodCall methodCall, TranslationContext? context)
    {
        var senderClass = GetSenderClass(methodCall);
        var receiverClass = GetReceiverClass(methodCall);

        if (senderClass == null || receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        if (!_firstMethodCallUsed)
        {
            context = InitializeFirstMethodCall(methodCall);
        }
        else if (senderClass != context?.Sender && senderClass == context?.Receiver)
        {
            context = UpdateSenderMethods(context);
            context = InitializeNewContext(methodCall, context);
        }

        else if (senderClass != context?.Sender)
        {
            if (OrderOfSenderClassesIsWrong(context, senderClass)) return FinishDiagramAsInvalid();
            context = UpdateSenderMethods(context);
            context = InitializeContextFromLastMethodOfSenderClass(methodCall, context);
        }
        else
        {
            context = UpdateTranslationContext(methodCall, context);
        }

        context.LastReceiver = receiverClass;
        return context;
    }

    private bool OrderOfSenderClassesIsWrong(TranslationContext? context, TranslationClass senderClass)
    {
        var indexOfNewSenderClass = Classes.FindIndex(@class => @class == senderClass);
        var indexOfPreviousSenderClass = Classes.FindIndex(@class => @class == context?.Sender);
        if (indexOfNewSenderClass > indexOfPreviousSenderClass)
        {
            return true;
        }

        return false;
    }

    private TranslationContext FinishDiagramAsInvalid()
    {
        IsDiagramValid = false;
        return new TranslationContext();
    }

    private string GetInstanceName(MethodCall methodCall)
    {
        if (GetSenderClass(methodCall) == GetReceiverClass(methodCall))
        {
            return "self";
        }

        return CreateNameOfClassInstance(GetReceiverClass(methodCall));
    }

    private TranslationContext UpdateTranslationContext(MethodCall methodCall, TranslationContext context)
    {
        var (_, receiverClass, instanceName, methodName) = GetBasicElementsOfMethodCall(methodCall);
        if (receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        if (!context.Instances.Contains(receiverClass))
        {
            if (context.Sender != receiverClass)
            {
                context.CurrentCode += CreateInstance(receiverClass, instanceName);
                context.Instances.Add(receiverClass);
            }
        }

        context.CurrentCode += CreateCall(instanceName, methodName);
        context.LastMethodCalled = methodName;
        context.Receiver = receiverClass;
        if (context.Sender == receiverClass && !_areSelfMessagesBlankMethods)
        {
            context = InitSelfMethod(methodName, receiverClass, context);
            return context;
        }

        var receiverMethod = new TranslationMethod
            { Name = methodName, Code = "", IsSelfMethod = context.Sender == receiverClass };
        if (!receiverClass.Methods.Contains(receiverMethod))
        {
            receiverClass.Methods.Add(receiverMethod);
        }

        return context;
    }

    private TranslationClass? GetReceiverClass(MethodCall methodCall)
    {
        var receiverClass =
            Classes.Find(translationClass => translationClass.Id == methodCall.ReceiverClass.Id);
        return receiverClass;
    }

    private TranslationClass? GetSenderClass(MethodCall methodCall)
    {
        var senderClass =
            Classes.Find(translationClass => translationClass.Id == methodCall.SenderClass.Id);
        return senderClass;
    }

    private void TranslateOalClassesToTranslatorClasses()
    {
        Classes = new List<TranslationClass>();
        foreach (var oalClass in OalCode.Classes)
        {
            var translationClass = new TranslationClass
                { Id = oalClass.Id, Name = oalClass.Body, Methods = new List<TranslationMethod>() };
            Classes.Add(translationClass);
        }
    }

    private static string CreateNameOfClassInstance(TranslationClass? receiverClass)
    {
        if (receiverClass == null)
        {
            return "";
        }

        return receiverClass.Name + "_inst";
    }

    private static string CreateInstance(TranslationClass receiverClass, string instanceName)
    {
        return "create object instance " + instanceName + " of " + receiverClass.Name + ";\n";
    }

    private static string CreateCall(string instanceName, string methodName)
    {
        return instanceName + "." + methodName + "();\n";
    }

    private void TranslateStatementWhenContextIsNull(Statement statement)
    {
        foreach (var element in statement.StatementElements)
        {
            switch (element)
            {
                case MethodCall methodCall:
                    CurrentContext = TranslateMethodCall(methodCall, CurrentContext);
                    break;
                case Statement statementElement when CurrentContext == null:
                    TranslateStatementWhenContextIsNull(statementElement);
                    CurrentContext?.Instances.Clear();
                    break;
                case Statement statementElement:
                    var newContext = TranslateStatement(statementElement, CurrentContext);
                    UpdateCurrentContext(newContext);
                    break;
            }
        }

        if (CurrentContext == null) return;

        UpdateContextAfterProcessingStatementElements(statement, CurrentContext);
    }

    private void UpdateCurrentContext(TranslationContext newContext)
    {
        if (CurrentContext == null || !IsDiagramValid) return;
        if (CurrentContext.Sender != newContext.Sender)
        {
            UpdateSenderMethods(newContext);
            var originalInstances = CurrentContext.Instances;
            CurrentContext = new TranslationContext(newContext)
            {
                Instances = originalInstances
            };
            return;
        }

        CurrentContext.CurrentCode += newContext.CurrentCode;
        CurrentContext.LastMethodCalled = newContext.LastMethodCalled;
    }

    private TranslationContext TranslateStatement(Statement statement, TranslationContext originalContext,
        bool firstStatementNeedsToBeHandled = false)
    {
        UpdateSenderMethods(originalContext);
        if (statement.StatementElements.Count == 0)
        {
            HandleEmptyStatement(statement, originalContext);
            return originalContext;
        }

        var properContext = new TranslationContext(originalContext)
        {
            CurrentCode = ""
        };

        if (DiagramIsInvalidInStatement(statement, originalContext)) return FinishDiagramAsInvalid();

        properContext =
            ProcessStatementElements(statement, originalContext, firstStatementNeedsToBeHandled, properContext);

        properContext = UpdateContextAfterProcessingStatementElements(statement, properContext);

        return properContext;
    }

    private TranslationContext UpdateContextAfterProcessingStatementElements(Statement statement,
        TranslationContext properContext)
    {
        properContext = UpdateSenderMethods(properContext);
        var firstMethodInStatement =
            (MethodCall?)statement.StatementElements.FirstOrDefault(element => element is MethodCall);
        if (firstMethodInStatement != null)
            return UpdateContextByFirstMethodInStatement(statement, properContext, firstMethodInStatement);
        properContext.CurrentCode = CreateTxtOfStatement(statement) + properContext.CurrentCode +
                                    GetEndOfStatement(statement);
        return properContext;
    }

    private TranslationContext UpdateContextByFirstMethodInStatement(Statement statement,
        TranslationContext properContext,
        MethodCall firstMethodInStatement)
    {
        var methods = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id)?.Methods;
        if (methods == null)
        {
            return FinishDiagramAsInvalid();
        }

        var method = methods.First();

        properContext.CurrentCode = CreateTxtOfStatement(statement) + method.Code +
                                    GetEndOfStatement(statement);
        properContext.MethodName =  method.Name;
        var translationSenderClass = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id);
        var translationReceiverClass = Classes.Find(@class => firstMethodInStatement.ReceiverClass.Id == @class.Id);
        if (translationSenderClass == null || translationReceiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

        properContext.Sender = translationSenderClass;
        properContext.Receiver = translationReceiverClass;

        return properContext;
    }

    private bool CheckMagic(List<OalCodeElement> elements, MethodCall methodCall)
    {
        var indexOfMethodCallInElements = elements.FindIndex(element => element == methodCall);
        if (indexOfMethodCallInElements == 0)
        {
            return true;
        }

        var previousElement = elements[indexOfMethodCallInElements - 1];
        if (previousElement is not Statement statement)
        {
            return true;
        }

        if (statement.StatementElements.Count == 0)
        {
            return true;
        }

        var firstMethodInStatement = FindFirstMethodCallInStatement(statement);
        if (firstMethodInStatement.SenderClass == methodCall.SenderClass)
        {
            return true;
        }

        var lastMethodCallInElements = FindLastMethodCallInElements(elements, indexOfMethodCallInElements);
        if (lastMethodCallInElements == null)
        {
            return false;
        }

        if (lastMethodCallInElements.SenderClass == methodCall.SenderClass)
        {
            return true;
        }

        return false;
    }

    private static MethodCall? FindLastMethodCallInElements(IReadOnlyList<OalCodeElement> elements,
        int indexOfOriginalMethodCallInElements)
    {
        for (var i = indexOfOriginalMethodCallInElements - 1; i >= 0; i--)
        {
            if (elements[i] is MethodCall)
            {
                return (MethodCall)elements[i];
            }
        }

        return null;
    }

    private MethodCall FindFirstMethodCallInStatement(Statement statement)
    {
        var firstElement = statement.StatementElements.First();
        if (firstElement is not MethodCall methodCall)
        {
            return FindFirstMethodCallInStatement((Statement)firstElement);
        }

        return methodCall;
    }

    private TranslationContext ProcessStatementElements(Statement statement, TranslationContext originalContext,
        bool firstStatementNeedsToBeHandled,
        TranslationContext properContext)
    {
        var firstStatementOfTheseElementsNeedsToBeHandled = true;
        foreach (var statementElement in statement.StatementElements)
        {
            switch (statementElement)
            {
                case MethodCall methodCall:
                {
                    if (!CheckMagic(statement.StatementElements, methodCall))
                    {
                        return FinishDiagramAsInvalid();
                    }

                    properContext = ProcessMethodCallInStatementElements(originalContext,
                        firstStatementNeedsToBeHandled, properContext, methodCall);
                    firstStatementNeedsToBeHandled = false;
                    continue;
                }
                case Statement statementEl:
                {
                    properContext = ProcessStatementElementInStatementElement(properContext, statementEl,
                        firstStatementOfTheseElementsNeedsToBeHandled);
                    firstStatementOfTheseElementsNeedsToBeHandled = false;
                    break;
                }
            }
        }

        return properContext;
    }

    private TranslationContext ProcessMethodCallInStatementElements(TranslationContext originalContext,
        bool firstStatementNeedsToBeHandled, TranslationContext properContext, MethodCall methodCall)
    {
        if (GetSenderClass(methodCall) != originalContext.Sender &&
            GetReceiverClass(methodCall) != originalContext.Sender &&
            firstStatementNeedsToBeHandled)
        {
            properContext.CurrentCode = originalContext.CurrentCode;
            firstStatementNeedsToBeHandled = false;
        }

        properContext = TranslateMethodCall(methodCall, properContext);
        if (!firstStatementNeedsToBeHandled) return properContext;
        properContext.LastMethodCalled = properContext.LastMethodCalled;
        return properContext;
    }

    private TranslationContext ProcessStatementElementInStatementElement(TranslationContext properContext,
        Statement statementEl, bool firstStatementOfTheseElementsNeedToBeHandled)
    {
        var code = properContext.CurrentCode;
        properContext = TranslateStatement(statementEl, properContext, firstStatementOfTheseElementsNeedToBeHandled);
        properContext.CurrentCode = code + properContext.CurrentCode;
        return properContext;
    }

    private static void HandleEmptyStatement(Statement statement, TranslationContext context)
    {
        if (context.LastReceiver.Methods.Count == 0)
        {
            return;
        }

        var method = context.LastReceiver.Methods.Last();

        method.Code = CreateTxtOfStatement(statement) + GetEndOfStatement(statement);
    }

    private bool DiagramIsInvalidInStatement(Statement statement, TranslationContext context)
    {
        if (statement.StatementElements.Count == 0)
        {
            return false;
        }

        if (statement.StatementElements.First() is not MethodCall ||
            statement.StatementType is not (ElifStatement or ElseStatement) ||
            GetSenderClass((MethodCall)statement.StatementElements.First()) == context.Sender) return false;
        IsDiagramValid = false;
        return true;
    }

    private static string CreateTxtOfStatement(Statement statement)
    {
        var statementBody = ExtractContentWithinBrackets(statement.Body);
        return statement.StatementType switch
        {
            WhileStatement =>
                "while (" + statementBody + ")\n",
            ForStatement => statementBody + "\n",
            IfStatement => "if (" + statementBody + ")\n",
            ElifStatement => "elif (" + statementBody + ")\n",
            ElseStatement => "else\n",
            ParStatement when statement.IsFirst => "par\nthread\n",
            ParStatement when !statement.IsFirst => "thread\n",
            _ => ""
        };
    }

    private static string ExtractContentWithinBrackets(string input)
    {
        const string pattern = @"\((.*?)\)";

        var match = Regex.Match(input, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return input;
    }

    private static string GetEndOfStatement(Statement statement)
    {
        if (statement is { IsLast: false, StatementType: not ParStatement })
        {
            return "";
        }

        return statement.StatementType switch
        {
            IfStatement or ElifStatement or ElseStatement => "end if;\n",
            ForStatement => "end for;\n",
            WhileStatement => "end while;\n",
            ParStatement when !statement.IsLast => "end thread;\n",
            ParStatement when statement.IsLast => "end thread;\nend par;\n",
            _ => ""
        };
    }

    public AnimArchAnimation CreateAnimArchAnimationObject()
    {
        var methodCodes = CreateAnimMethodsCodes();
        return new AnimArchAnimation
        {
            Code = GetWholeCode(),
            MethodsCodes = methodCodes,
            StartClass = GetFirstClass(),
            StartMethod = GetFirstMethod()
        };
    }

    private string GetFirstClass()
    {
        if (Classes.Count == 0)
        {
            return "";
        }

        return Classes.First().Name;
    }

    private string GetFirstMethod()
    {
        if (Classes.Count == 0)
        {
            return "";
        }

        if (Classes.First().Methods.Count == 0)
        {
            return "";
        }

        return Classes.First().Methods.First().Name;
    }

    private List<AnimationMethodCode> CreateAnimMethodsCodes()
    {
        var methodCodes = new List<AnimationMethodCode>();

        foreach (var translationClass in Classes)
        {
            var animMethods = translationClass.Methods.Select(oalClassMethod => new AnimationMethod
            {
                Name = oalClassMethod.Name,
                Code = oalClassMethod.Code
            }).ToList();

            if (animMethods.Count == 0)
                continue;

            methodCodes.Add(new AnimationMethodCode
            {
                Name = translationClass.Name,
                Methods = animMethods
            });
        }

        return methodCodes;
    }

    private string GetWholeCode()
    {
        var code = "";
        foreach (var oalClass in Classes)
        {
            foreach (var translationMethod in oalClass.Methods)
            {
                code += translationMethod.Code;
            }
        }

        return code;
    }

    public string GetCompleteOalCode()
    {
        if (!IsDiagramValid)
        {
            return InvalidDiagramTxt;
        }

        var code = new StringBuilder();

        foreach (var translationClass in Classes)
        {
            AppendLineWithIndent(code, $"class {translationClass.Name}", 0);

            foreach (var translationMethod in translationClass.Methods)
            {
                AppendLineWithIndent(code, $"method {translationMethod.Name}()", 1);
                IndentAndAppendCodeLines(code, translationMethod.Code);
                AppendLineWithIndent(code, "end method;", 1);
                code.AppendLine();
            }

            AppendLineWithIndent(code, "end class;", 0);
            code.AppendLine();
        }

        return code.ToString();
    }

    private void IndentAndAppendCodeLines(StringBuilder sb, string code)
    {
        var lines = code.Split("\n");
        var indentation = 2;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.Contains("elif") || line.Contains("else"))
            {
                AppendLineWithIndent(sb, line, indentation - 1);
                continue;
            }

            if (line.Contains("end"))
            {
                indentation--;
                AppendLineWithIndent(sb, line, indentation);
                //indentation--;
                continue;
            }

            if (line.Contains("if") || line.Contains("for") || line.Contains("while") || line.Contains("par") ||
                line.Contains(
                    "thread"))
            {
                //indentation++;
                AppendLineWithIndent(sb, line, indentation);
                indentation++;
                continue;
            }

            AppendLineWithIndent(sb, line, indentation);
        }
    }

    private void AppendLineWithIndent(StringBuilder sb, string line, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        sb.AppendLine($"{indent}{line}");
    }
}