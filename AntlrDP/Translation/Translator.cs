using System.Text;
using System.Text.RegularExpressions;
using AntlrDP.AnimArchAnimationClasses;
using AntlrDP.OalCodeElements;
using AntlrDP.OalCodeElements.FragmentTypes;

namespace AntlrDP.Translation;

public class Translator
{
    private OalCode OalCode { get; }
    public List<TranslationClass> Classes { get; set; }
    public const string FirstMethodName = "FirstMethod";
    private bool _firstMethodCallUsed = false;
    private TranslationContext? CurrentContext { get; set; }
    public bool IsDiagramValid = true;
    private bool _firstStatementNeedToBeHandled = true;
    private readonly bool _areSelfMessagesBlankMethods;

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
                CurrentContext = TranslateMethodCall(methodCall, CurrentContext);
                break;
            case Statement statement:
                ProcessStatement(statement);
                _firstStatementNeedToBeHandled = false;
                break;
        }
    }

    private void ProcessStatement(Statement statement)
    {
        if (statement.StatementElements.Count == 0)
        {
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
            var newContext = TranslateStatement(statement, CurrentContext, _firstStatementNeedToBeHandled);
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
        var methodCallNames = OalCode.MethodCalls.Select(obj => obj.Name).ToList();
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

    private TranslationContext InitializeFirstMethodCall(TranslationClass senderClass,
        TranslationClass receiverClass,
        string instanceName, string methodName)
    {
        var context = new TranslationContext
        {
            MethodNames = new List<string> { FirstMethodName },
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
        context.LastMethodsCalled.Add(methodName);

        _firstMethodCallUsed = true;
        return context;
    }

    protected virtual TranslationContext UpdateSenderMethods(TranslationContext context)
    {
        foreach (var method in context.MethodNames)
        {
            var senderMethod =
                context.Sender.Methods.Find(translationMethod => translationMethod.Name == method);
            if (senderMethod != null)
            {
                var ind = context.Sender.Methods.FindIndex(translationMethod => translationMethod.Name == method);
                context.Sender.Methods.RemoveAll(translationMethod => translationMethod.Name == method);
                senderMethod = new TranslationMethod
                    { Name = method, Code = context.CurrentCode, Instances = context.Instances };
                context.Sender.Methods.Insert(ind, senderMethod);
            }
            else
            {
                senderMethod = new TranslationMethod
                    { Name = method, Code = context.CurrentCode, Instances = context.Instances };

                context.Sender.Methods.Add(senderMethod);
            }
        }

        return context;
    }

    private TranslationContext InitializeNewContext(TranslationClass senderClass, TranslationClass receiverClass,
        string instanceName,
        string methodName, TranslationContext context)
    {
        var newMethodNames = new List<string>(context.LastMethodsCalled);
        context = new TranslationContext
        {
            MethodNames = newMethodNames,
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
        
        context.LastMethodsCalled.Add(methodName);
        return context;
    }

    private TranslationMethod FindLastMethodOfSenderClass(TranslationClass senderClass)
    {
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

    private TranslationContext InitializeContextFromLastMethodOfSenderClass(TranslationClass senderClass,
        TranslationClass receiverClass, string instanceName,
        string methodName, TranslationContext context)
    {
        var lastMethod = FindLastMethodOfSenderClass(senderClass);
        if (lastMethod == null)
        {
            return context;
        }

        context = new TranslationContext
        {
            MethodNames = new List<string> { lastMethod.Name },
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
        
        context.LastMethodsCalled.Add(methodName);
        return context;
    }

    private TranslationContext InitSelfMethod(string methodName, TranslationClass senderAndReceiverClass,
        TranslationContext context)
    {
        UpdateSenderMethods(context);
        context = new TranslationContext
        {
            MethodNames = new List<string> { methodName },
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
            return context;
        }

        var instanceName = CreateNameOfClassInstance(receiverClass);
        if (senderClass == receiverClass)
        {
            instanceName = "self";
        }

        if (!_firstMethodCallUsed)
        {
            context = InitializeFirstMethodCall(senderClass, receiverClass, instanceName, methodCall.Name);
        }
        else if (senderClass != context?.Sender && senderClass == context?.Receiver)
        {
            context = UpdateSenderMethods(context);
            context = InitializeNewContext(senderClass, receiverClass, instanceName, methodCall.Name, context);
        }

        else if (senderClass != context?.Sender)
        {
            context = UpdateSenderMethods(context);
            context = InitializeContextFromLastMethodOfSenderClass(senderClass, receiverClass, instanceName,
                methodCall.Name, context);
        }
        else
        {
            context = UpdateTranslationContext(instanceName, methodCall.Name, receiverClass, context);
        }

        context.LastReceiver = receiverClass;
        return context;
    }

    private TranslationContext UpdateTranslationContext(string instanceName, string methodName,
        TranslationClass receiverClass, TranslationContext context)
    {
        if (!context.Instances.Contains(receiverClass))
        {
            if (context.Sender != receiverClass)
            {
                context.CurrentCode += CreateInstance(receiverClass, instanceName);
                context.Instances.Add(receiverClass);
            }
        }

        context.CurrentCode += CreateCall(instanceName, methodName);
        context.LastMethodsCalled.Add(methodName);
        if (context.Sender == receiverClass && !_areSelfMessagesBlankMethods)
        {
            context = InitSelfMethod(methodName, receiverClass, context);
            return context;
        }

        var receiverMethod = new TranslationMethod
            { Name = methodName, Code = "", IsSelfMethod = context.Sender == receiverClass };
        receiverClass.Methods.Add(receiverMethod);

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
                { Id = oalClass.Id, Name = oalClass.Name, Methods = new List<TranslationMethod>() };
            Classes.Add(translationClass);
        }
    }

    private static string CreateNameOfClassInstance(TranslationClass receiverClass)
    {
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
                {
                    var newContext = TranslateStatement(statementElement, CurrentContext);
                    UpdateCurrentContext(newContext);
                    break;
                }
            }
        }

        UpdateSenderMethods(CurrentContext);
        var firstMethodInStatement =
            (MethodCall?)statement.StatementElements.FirstOrDefault(element => element is MethodCall);
        if (firstMethodInStatement == null)
        {
            CurrentContext.CurrentCode = CreateTxtOfStatement(statement) + CurrentContext?.CurrentCode +
                                         GetEndOfStatement(statement);
            return;
        }

        var method = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id).Methods.First();
        CurrentContext.CurrentCode = CreateTxtOfStatement(statement) + method.Code +
                                     GetEndOfStatement(statement);
        CurrentContext.MethodNames = new List<string> { method.Name };
    }

    private void UpdateCurrentContext(TranslationContext newContext)
    {
        if (CurrentContext == null) return;
        if (CurrentContext.Sender != newContext.Sender)
        {
            UpdateSenderMethods(newContext);
            CurrentContext = new TranslationContext(newContext);
            return;
        }

        CurrentContext.CurrentCode += newContext.CurrentCode;
        CurrentContext.LastMethodsCalled = new List<string>(newContext.LastMethodsCalled);
    }

    private TranslationContext TranslateStatement(Statement statement, TranslationContext context, bool ff = false)
    {
        UpdateSenderMethods(context);
        if (statement.StatementElements.Count == 0)
        {
            HandleEmptyStatement(statement, context);
            return context;
        }

        var firstStatementNeedToBeHandled = true;
        var txtOfStatement = CreateTxtOfStatement(statement);
        var properContext = new TranslationContext(context);
        properContext.CurrentCode = "";


        if (CheckIfDiagramIsValid(statement, context)) return context;

        properContext = ProcessStatementElements(statement, context, ff, properContext, firstStatementNeedToBeHandled);

        properContext = UpdateSenderMethods(properContext);
        var firstMethodInStatement =
            (MethodCall?)statement.StatementElements.FirstOrDefault(element => element is MethodCall);
        if (firstMethodInStatement == null)
        {
            properContext.CurrentCode = txtOfStatement + properContext.CurrentCode + GetEndOfStatement(statement);
            return properContext;
        }

        var method = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id).Methods.First();
        var translationSenderClass = Classes.Find(@class => firstMethodInStatement.SenderClass.Id == @class.Id);
        var translationReceiverClass = Classes.Find(@class => firstMethodInStatement.ReceiverClass.Id == @class.Id);
        properContext.CurrentCode = CreateTxtOfStatement(statement) + method.Code +
                                    GetEndOfStatement(statement);
        properContext.Sender = translationSenderClass;
        properContext.Receiver = translationReceiverClass;
        properContext.MethodNames = new List<string> { method.Name };

        return properContext;
    }

    private TranslationContext ProcessStatementElements(Statement statement, TranslationContext context, bool ff,
        TranslationContext properContext, bool firstStatementNeedToBeHandled)
    {
        foreach (var statementElement in statement.StatementElements)
        {
            if (statementElement is MethodCall methodCall)
            {
                if (GetSenderClass(methodCall) != context.Sender && GetReceiverClass(methodCall) != context.Sender &&
                    ff)
                {
                    properContext.CurrentCode = context.CurrentCode;
                    ff = false;
                }

                properContext = TranslateMethodCall(methodCall, properContext);
                if (ff)
                {
                    properContext.LastMethodsCalled = new List<string> { properContext.LastMethodsCalled.Last() };
                    ff = false;
                }
            }

            else if (statementElement is Statement statementEl)
            {
                var code = properContext.CurrentCode;
                properContext = TranslateStatement(statementEl, properContext, firstStatementNeedToBeHandled);
                firstStatementNeedToBeHandled = false;
                properContext.CurrentCode = code + properContext.CurrentCode;
            }
        }

        return properContext;
    }

    private static void HandleEmptyStatement(Statement statement, TranslationContext context)
    {
        var method = context.LastReceiver.Methods.Last();
        if (method == null)
        {
            return;
        }

        method.Code = CreateTxtOfStatement(statement) + GetEndOfStatement(statement);
    }

    private bool CheckIfDiagramIsValid(Statement statement, TranslationContext context)
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
                indentation--;
                continue;
            }

            if (line.Contains("if") || line.Contains("for") || line.Contains("while") || line.Contains("par") ||
                line.Contains(
                    "thread"))
            {
                indentation++;
                AppendLineWithIndent(sb, line, indentation);
                indentation++;
                continue;
            }

            AppendLineWithIndent(sb, line, indentation);
        }
    }

    protected virtual void AppendLineWithIndent(StringBuilder sb, string line, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        sb.AppendLine($"{indent}{line}");
    }
}