using System.Text;
using AntlrDP.AnimArchAnimationClasses;
using AntlrDP.OalCodeElements;
using AntlrDP.OalCodeElements.FragmentTypes;
using AntlrDP.OalCodeElements;

namespace AntlrDP.Translation;

public class Translator
{
    private OalCode OalCode { get; set; }
    public List<TranslationClass> Classes { get; set; }
    public const string FirstMethodName = "FirstMethod";
    private bool FirstMethodCallUsed = false;
    private TranslationContext? CurrentContext { get; set; }
    public bool IsDiagramValid = true;

    public Translator(OalCode oalCode)
    {
        OalCode = oalCode;
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
    }

    private void ProcessCodeElement(OalCodeElement codeElement)
    {
        switch (codeElement)
        {
            case MethodCall methodCall:
                ProcessMethodCallInGeneral(methodCall);
                break;
            case Statement statement:
                ProcessStatement(statement);
                break;
        }
    }

    private void ProcessStatement(Statement statement)
    {
        if (CurrentContext == null)
        {
            TranslateStatementInGeneral(statement);
            CurrentContext?.Instances.Clear();
        }
        else
        {
            var newContext = TranslateStatement(statement, CurrentContext);
            CurrentContext.CurrentCode += newContext.CurrentCode;
            CurrentContext.LastMethodsCalled = new List<string>(newContext.LastMethodsCalled);
        }
    }

    private void UpdateMethodsForLastCodeElement(OalCodeElement codeElement)
    {
        if (OalCode.CodeElements[^1] != codeElement) return;
        foreach (var method in CurrentContext.MethodNames)
        {
            var senderMethod = CurrentContext.Sender.Methods.Find(translationMethod => translationMethod.Name == method);
            if (senderMethod != null)
            {
                CurrentContext.Sender.Methods.Remove(senderMethod);
            }
            senderMethod = new TranslationMethod { Name = method, Code = CurrentContext.CurrentCode };
            CurrentContext.Sender.Methods.Add(senderMethod);
        }
    }

    private void ProcessMethodCallInGeneral(MethodCall methodCall)
    {
        var senderClass = GetSenderClass(methodCall);
        var receiverClass = GetReceiverClass(methodCall);

        if (senderClass == null || receiverClass == null)
        {
            return;
        }

        var instanceName = CreateNameOfClassInstance(receiverClass);
        if (senderClass == receiverClass)
        {
            instanceName = "self";
        }

        if (!FirstMethodCallUsed)
        {
            InitializeFirstMethodCall(senderClass, receiverClass, instanceName, methodCall.Name);
        }
        else if (senderClass != CurrentContext?.Sender)
        {
            UpdateSenderMethods();
            InitializeNewContext(senderClass, receiverClass, instanceName, methodCall.Name);
        }
        else
        {
            UpdateContext(instanceName, methodCall.Name, receiverClass);
        }
    }

    private void InitializeFirstMethodCall(TranslationClass senderClass, TranslationClass receiverClass,
        string instanceName, string methodName)
    {
        CurrentContext = new TranslationContext
        {
            MethodNames = new List<string> { FirstMethodName },
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = ""
        };

        if (senderClass != receiverClass)
        {
            CurrentContext.CurrentCode += CreateInstance(receiverClass, instanceName);
        }

        CurrentContext.CurrentCode += CreateCall(instanceName, methodName);

        var receiverMethod = new TranslationMethod { Name = methodName, Code = "" };
        receiverClass.Methods.Add(receiverMethod);
        CurrentContext.Instances.Add(receiverClass);
        CurrentContext.LastMethodsCalled.Add(methodName);

        FirstMethodCallUsed = true;
    }

    private void UpdateSenderMethods()
    {
        foreach (var method in CurrentContext.MethodNames)
        {
            var senderMethod =
                CurrentContext.Sender.Methods.Find(translationMethod => translationMethod.Name == method);
            if (senderMethod != null)
            {
                CurrentContext.Sender.Methods.Remove(senderMethod);
            }

            senderMethod = new TranslationMethod { Name = method, Code = CurrentContext.CurrentCode };
            CurrentContext.Sender.Methods.Add(senderMethod);
        }
    }

    private void InitializeNewContext(TranslationClass senderClass, TranslationClass receiverClass, string instanceName,
        string methodName)
    {
        UpdateSenderMethods();
        var newMethodNames = new List<string>(CurrentContext.LastMethodsCalled);
        CurrentContext = new TranslationContext
        {
            MethodNames = newMethodNames,
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = ""
        };
        if (senderClass != receiverClass)
        {
            CurrentContext.CurrentCode += CreateInstance(receiverClass, instanceName);
        }

        CurrentContext.CurrentCode += CreateCall(instanceName, methodName);

        var receiverMethod = new TranslationMethod { Name = methodName, Code = "" };
        receiverClass.Methods.Add(receiverMethod);
        CurrentContext.Instances.Add(receiverClass);
        CurrentContext.LastMethodsCalled.Add(methodName);
    }

    private void UpdateContext(string instanceName, string methodName, TranslationClass receiverClass)
    {
        if (!CurrentContext.Instances.Contains(receiverClass))
        {
            if (CurrentContext.Sender != receiverClass)
            {
                CurrentContext.CurrentCode += CreateInstance(receiverClass, instanceName);
            }

            CurrentContext.Instances.Add(receiverClass);
        }

        CurrentContext.CurrentCode += CreateCall(instanceName, methodName);

        var receiverMethod = new TranslationMethod { Name = methodName, Code = "" };
        receiverClass.Methods.Add(receiverMethod);
    }


    private TranslationContext ProcessMethodCallInStatement(MethodCall methodCall, TranslationContext context)
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

        if (senderClass != context.Sender)
        {
            UpdateSenderMethodNames();
            context = InitializeTranslationContext(senderClass, receiverClass, instanceName, methodCall.Name);
        }
        else
        {
            context = UpdateTranslationContext(instanceName, methodCall.Name, receiverClass, context);
        }

        return context;
    }

    private TranslationContext InitializeTranslationContext(TranslationClass senderClass,
        TranslationClass receiverClass, string instanceName, string methodName)
    {
        CurrentContext = new TranslationContext
        {
            MethodNames = new List<string> { FirstMethodName },
            Sender = senderClass,
            Receiver = receiverClass,
            CurrentCode = ""
        };

        if (senderClass != receiverClass)
        {
            CurrentContext.CurrentCode += CreateInstance(receiverClass, instanceName);
        }

        CurrentContext.CurrentCode += CreateCall(instanceName, methodName);

        var receiverMethod = new TranslationMethod { Name = methodName, Code = "" };
        receiverClass.Methods.Add(receiverMethod);
        CurrentContext.Instances.Add(receiverClass);
        CurrentContext.LastMethodsCalled.Add(methodName);

        return CurrentContext;
    }

    private void UpdateSenderMethodNames()
    {
        foreach (var method in CurrentContext.MethodNames)
        {
            var senderMethod =
                CurrentContext.Sender.Methods.Find(translationMethod => translationMethod.Name == method);
            if (senderMethod != null)
            {
                CurrentContext.Sender.Methods.Remove(senderMethod);
            }

            senderMethod = new TranslationMethod { Name = method, Code = CurrentContext.CurrentCode };
            CurrentContext.Sender.Methods.Add(senderMethod);
        }
    }

    protected virtual TranslationContext UpdateTranslationContext(string instanceName, string methodName,
        TranslationClass receiverClass, TranslationContext context)
    {
        if (!context.Instances.Contains(receiverClass))
        {
            if (context.Sender != receiverClass)
            {
                context.CurrentCode += CreateInstance(receiverClass, instanceName);
            }

            context.Instances.Add(receiverClass);
        }

        context.CurrentCode += CreateCall(instanceName, methodName);

        var receiverMethod = new TranslationMethod { Name = methodName, Code = "" };
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

    private void TranslateStatementInGeneral(Statement statement)
    {

        foreach (var element in statement.StatementElements)
        {
            switch (element)
            {
                case MethodCall methodCall:
                    ProcessMethodCall(methodCall);
                    break;
                case Statement statementElement when CurrentContext == null:
                    TranslateStatementInGeneral(statementElement);
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

        AppendToCurrentCode(statement);
    }

    private void ProcessMethodCall(MethodCall methodCall)
    {
        if (CurrentContext == null)
        {
            ProcessMethodCallInGeneral(methodCall);
        }
        else
        {
            ProcessMethodCallInStatement(methodCall, CurrentContext);
        }
    }

    private void UpdateCurrentContext(TranslationContext newContext)
    {
        if (CurrentContext == null) return;
    
        CurrentContext.CurrentCode += newContext.CurrentCode;
        CurrentContext.LastMethodsCalled = new List<string>(newContext.LastMethodsCalled);
    }

    private void AppendToCurrentCode(Statement statement)
    {
        CurrentContext.CurrentCode = CreateTxtOfStatement(statement) + CurrentContext?.CurrentCode + GetEndOfStatement(statement);
    }

    private TranslationContext TranslateStatement(Statement statement, TranslationContext context)
    {
        var txtOfStatement = CreateTxtOfStatement(statement);
        var properContext = new TranslationContext(context);
        properContext.CurrentCode = "";
        if (CheckIfDiagramIsValid(statement, context)) return context;

        foreach (var statementElement in statement.StatementElements)
        {
            if (statementElement is MethodCall methodCall)
            {
                properContext = ProcessMethodCallInStatement(methodCall, properContext);
            }
        }

        var lastMethodCall = statement.StatementElements.FindLast(element => element is MethodCall);
        if (lastMethodCall != null) properContext.LastMethodsCalled.Add(lastMethodCall.Name);
        properContext.CurrentCode = txtOfStatement + properContext.CurrentCode + GetEndOfStatement(statement);
        return properContext;
    }

    private bool CheckIfDiagramIsValid(Statement statement, TranslationContext context)
    {
        if (statement.StatementElements.First() is not MethodCall ||
            statement.StatementType is not (ElifStatement or ElseStatement) ||
            GetSenderClass((MethodCall)statement.StatementElements.First()) == context.Sender) return false;
        IsDiagramValid = false;
        return true;

    }

    private static string CreateTxtOfStatement(Statement statement)
    {
        return statement.StatementType switch
        {
            WhileStatement => "while (" + statement.Name + ")\n",
            ForStatement => statement.Name + "\n",
            IfStatement => "if (" + statement.Name + ")\n",
            ElifStatement => "elif (" + statement.Name + ")\n",
            ElseStatement => "else\n",
            _ => ""
        };
    }

    private static string GetEndOfStatement(Statement statement)
    {
        if (!statement.IsLast)
        {
            return "";
        }

        return statement.StatementType switch
        {
            IfStatement or ElifStatement or ElseStatement => "end if;\n",
            ForStatement => "end for;\n",
            WhileStatement => "end while;\n",
            _ => ""
        };
    }

    public AnimArchAnimation CreateAnimArchAnimationObject()
    {
        var methodCodes = CreateAnimMethodsCodes();
        return new AnimArchAnimation()
        {
            Code = GetWholeCode(),
            MethodsCodes = methodCodes
        };
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

    public string GetCompleteOALCode()
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

    protected virtual void IndentAndAppendCodeLines(StringBuilder sb, string code)
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

            if (line.Contains("if") || line.Contains("for") || line.Contains("while"))
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