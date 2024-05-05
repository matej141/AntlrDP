using System.Text;
using System.Text.RegularExpressions;
using SqdToOalTranslator.AnimArchAnimationClasses;
using SqdToOalTranslator.PreOalCodeElements;
using SqdToOalTranslator.PreOalCodeElements.FragmentTypes;

namespace SqdToOalTranslator.Translation;

public sealed class Translator
{
    private PreOalCode PreOalCode { get; }
    public List<TranslationClass> Classes = new();
    public const string FirstMethodName = "StartMethod";
    private bool _firstMethodCallUsed; // false by default
    private TranslationContext? CurrentContext { get; set; }
    public bool IsDiagramValid = true;
    private bool _firstStatementNeedsToBeHandled = true;
    private readonly bool _areSelfMessagesBlankMethods;
    public const string InvalidDiagramTxt = "Sequence Diagram is Invalid";
    private string _nameOfLastMethodBeforeStatement = "";
    private CodeFilesGenerator CodeFilesGenerator { get; }

    public Translator(PreOalCode preOalCode, bool areSelfMessagesBlankMethods = true)
    {
        PreOalCode = preOalCode;
        _areSelfMessagesBlankMethods = areSelfMessagesBlankMethods;
        Translate();
        CodeFilesGenerator = new CodeFilesGenerator(this);
    }

    private void Translate()
    {
        // nasjskôr skontrolujeme validitu sqd
        var isValid = IsSequenceDiagramValid();
        if (!isValid)
        {
            FinishDiagramAsInvalid();
        }

        // triedy z PreOalCodeElements konverutjeme do TranslationClasses
        TranslatePreOalClassesToTranslationClasses();

        // prejdeme všetky predpripravené PreOAL elementy a preložíme ich postupne do OAL kódu
        foreach (var codeElement in PreOalCode.CodeElements)
        {
            ProcessCodeElement(codeElement);
            // ak je diagram nevalídny, ukončime konverziu do OAL kódu
            if (!IsDiagramValid)
            {
                Classes.Clear();
                return;
            }
        }

        // na základe posledného elementu PreOAL elementov uzatvoríme konverziu do OAL kódu
        UpdateMethodsForLastCodeElement(PreOalCode.CodeElements[^1]);
        OrderMethodsInClasses();
    }

    private bool IsSequenceDiagramValid()
    {
        // využitie pomocnej triedy
        var validator =
            new SequenceDiagramValidator(PreOalCode.Classes, PreOalCode.MethodCalls);
        var isValid = validator.IsDiagramValid();
        return isValid;
    }

    private void ProcessCodeElement(PreOalCodeElement codeElement)
    {
        switch (codeElement)
        {
            case MethodCall methodCall:
                // kontrola pravidla 6
                if (AnyOfTheLastMessagesInPreviousStatementHasSameReceiverClass(PreOalCode.CodeElements, methodCall))
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
        if (statement.StatementType is WhileStatement or ForEachStatement && !statement.IsFirst)
        {
            FinishDiagramAsInvalid();
            return;
        }

        if (statement.StatementElements.Count == 0)
        {
            if (CurrentContext == null)
            {
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

    private void UpdateMethodsForLastCodeElement(PreOalCodeElement codeElement)
    {
        if (PreOalCode.CodeElements[^1] != codeElement) return;
        UpdateSenderMethods(CurrentContext);
    }

    // Ak je poradie správ v triedach náhodou iné ako poradie správ v SQD, tak prehodiť...
    private void OrderMethodsInClasses()
    {
        var methodCallNames = PreOalCode.MethodCalls.Select(obj => obj.Name).ToList();
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
            MethodCall methodCall => methodCall.Name,
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
        _firstMethodCallUsed = true;
        return FinishContext(methodCall, context);
    }

    private TranslationContext FinishContext(MethodCall methodCall, TranslationContext context)
    {
        var (senderClass, receiverClass, instanceName, methodName) = GetBasicElementsOfMethodCall(methodCall);
        if (senderClass == null || receiverClass == null)
        {
            return FinishDiagramAsInvalid();
        }

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
        if (!receiverClass.Methods.Contains(receiverMethod))
        {
            receiverClass.Methods.Add(receiverMethod);
        }

        context.Instances.Add(receiverClass);
        context.LastMethodCalled = methodName;
        return context;
    }

    private (TranslationClass? senderClass, TranslationClass? receiverClass, string instanceName, string methodName)
        GetBasicElementsOfMethodCall(MethodCall methodCall)
    {
        var senderClass = GetSenderClass(methodCall);
        var receiverClass = GetReceiverClass(methodCall);
        var instanceName = GetInstanceName(methodCall);
        var methodName = methodCall.Name;
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

        return FinishContext(methodCall, context);
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

    private void TranslatePreOalClassesToTranslationClasses()
    {
        Classes = new List<TranslationClass>();
        foreach (var oalClass in PreOalCode.Classes)
        {
            var translationClass = new TranslationClass
                { Id = oalClass.Id, Name = oalClass.Name, Methods = new List<TranslationMethod>() };
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

        if (statement.IsFirst)
        {
            _nameOfLastMethodBeforeStatement = GetNameOfLastMethodBeforeStatement(statement);
        }

        var firstMethodInStatement =
            (MethodCall?)statement.StatementElements.FirstOrDefault(element => element is MethodCall);
        if (firstMethodInStatement != null)
            return UpdateContextByFirstMethodBeforeStatement(statement, properContext, firstMethodInStatement);
        properContext.CurrentCode = CreateTxtOfStatement(statement) + properContext.CurrentCode +
                                    GetEndOfStatement(statement);
        return properContext;
    }

    private string GetNameOfLastMethodBeforeStatement(Statement statement)
    {
        var firstMethodInStatements =
            (MethodCall?)statement.StatementElements.FirstOrDefault(element => element is MethodCall);
        var methodCalls =
            statement.StatementElements.Where(statementElement => statementElement is MethodCall).ToList();
        var methodCallNames = methodCalls.Select(methodCall => methodCall.Name).ToList();
        if (firstMethodInStatements == null) return "";
        var methods = Classes.Find(@class => firstMethodInStatements.SenderClass.Id == @class.Id)?.Methods;
        for (var i = methods.Count - 1; i >= 0; i--)
        {
            var method = methods[i];

            if (method.IsSelfMethod && _areSelfMessagesBlankMethods)
            {
                continue;
            }

            if (methodCallNames.Contains(method.Name)) continue;

            return method.Name;
        }

        return "";
    }

    private TranslationContext UpdateContextByFirstMethodBeforeStatement(Statement statement,
        TranslationContext properContext,
        MethodCall firstMethodInStatement)
    {
        var methods = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id)?.Methods;
        if (methods == null)
        {
            return FinishDiagramAsInvalid();
        }

        var method = methods.Find(method => method.Name == _nameOfLastMethodBeforeStatement);
        if (method == null)
        {
            return properContext;
        }

        properContext.CurrentCode = CreateTxtOfStatement(statement) + method.Code +
                                    GetEndOfStatement(statement);
        properContext.MethodName = method.Name;
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

    private bool AnyOfTheLastMessagesInPreviousStatementHasSameReceiverClass(List<PreOalCodeElement> elements,
        MethodCall methodCall)
    {
        var indexOfMethodCallInElements = elements.FindIndex(element => element == methodCall);
        if (indexOfMethodCallInElements == 0)
        {
            return false;
        }

        var previousElement = elements[indexOfMethodCallInElements - 1];
        if (previousElement is not Statement statement)
        {
            return false;
        }

        if (statement.StatementElements.Count == 0)
        {
            return false;
        }

        var lastMethods = GetLastMethodsOfFragment(statement, elements);
        return lastMethods.Any(methodCallInLast => methodCallInLast.ReceiverClass == methodCall.SenderClass ||
                                                   (methodCallInLast.SenderClass == methodCallInLast.ReceiverClass &&
                                                    _areSelfMessagesBlankMethods));
    }

    private IEnumerable<MethodCall> GetLastMethodsOfFragment(Statement statement, List<PreOalCodeElement> elements)
    {
        var lastMethodInStatement = FindLastMethodCallInStatement(statement);
        var lastMethods = new List<MethodCall>();
        if (lastMethodInStatement != null)
        {
            lastMethods.Add(lastMethodInStatement);
        }

        var lastStatement = statement;
        var indexOfLastStatementInElements = elements.FindIndex(element => element == lastStatement);
        while (indexOfLastStatementInElements > 0)
        {
            var previousElement = elements[indexOfLastStatementInElements - 1];

            if (SomeOfTheAttributesOfStatementIsNotFulfilled(statement, previousElement)) break;
            var previousStatement = (Statement)previousElement;
            lastMethodInStatement = FindLastMethodCallInStatement(previousStatement);
            if (lastMethodInStatement != null)
            {
                lastMethods.Add(lastMethodInStatement);
            }

            statement = previousStatement;
            indexOfLastStatementInElements -= 1;
        }

        return lastMethods;
    }

    private static bool SomeOfTheAttributesOfStatementIsNotFulfilled(Statement statement,
        PreOalCodeElement previousElement)
    {
        if (previousElement is not Statement previousStatement)
        {
            return true;
        }

        switch (statement.StatementType)
        {
            case ParStatement when previousStatement.StatementType is not ParStatement:
            case IfStatement when previousStatement.StatementType is IfStatement:
                return true;
        }

        return previousStatement.StatementType is WhileStatement or ForEachStatement;
    }

    private static MethodCall? FindLastMethodCallInStatement(Statement statement)
    {
        if (statement.StatementElements.Count == 0)
        {
            return null;
        }

        var lastElement = statement.StatementElements.Last();
        if (lastElement is not MethodCall methodCall)
        {
            return FindLastMethodCallInStatement((Statement)lastElement);
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
        if (statementEl.StatementElements.Count == 0)
        {
            return properContext;
        }

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
        var statementBody = ExtractContentWithinBrackets(statement.Name);
        return statement.StatementType switch
        {
            WhileStatement =>
                "while (" + statementBody + ")\n",
            ForEachStatement => statementBody + "\n",
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
            ForEachStatement => "end for;\n",
            WhileStatement => "end while;\n",
            ParStatement when !statement.IsLast => "end thread;\n",
            ParStatement when statement.IsLast => "end thread;\nend par;\n",
            _ => ""
        };
    }

    // vytvorenie animačného súboru
    public AnimArchAnimation CreateAnimArchAnimationObject()
    {
        return CodeFilesGenerator.CreateAnimArchAnimationObject();
    }

    // získanie OAL kódu na vytvorenie txt súboru s OAL kódom (nie animačný súbor)
    public string GetCompleteOalCode()
    {
        return CodeFilesGenerator.GetCompleteOalCode();
    }
}