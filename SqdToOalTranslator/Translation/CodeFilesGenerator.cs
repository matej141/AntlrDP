using System.Text;
using SqdToOalTranslator.AnimArchAnimationClasses;

namespace SqdToOalTranslator.Translation;

// pomocná trieda slúžiaca na vytvorenie súborov s OAL kódom
public sealed class CodeFilesGenerator
{
    private readonly Translator _translator;

    public CodeFilesGenerator(Translator translator)
    {
        _translator = translator;
    }
    
    // vytvorenie animačného súboru
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
        if (_translator.Classes.Count == 0)
        {
            return "";
        }

        return _translator.Classes.First().Name;
    }

    private string GetFirstMethod()
    {
        if (_translator.Classes.Count == 0)
        {
            return "";
        }

        if (_translator.Classes.First().Methods.Count == 0)
        {
            return "";
        }

        return _translator.Classes.First().Methods.First().Name;
    }

    private List<AnimationMethodCode> CreateAnimMethodsCodes()
    {
        var methodCodes = new List<AnimationMethodCode>();

        foreach (var translationClass in _translator.Classes)
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
        foreach (var oalClass in _translator.Classes)
        {
            foreach (var translationMethod in oalClass.Methods)
            {
                code += translationMethod.Code;
            }
        }

        return code;
    }
    
    // získanie OAL kódu na vytvorenie txt súboru s OAL kódom (nie animačný súbor)
    public string GetCompleteOalCode()
    {
        if (!_translator.IsDiagramValid)
        {
            return Translator.InvalidDiagramTxt;
        }

        var code = new StringBuilder();

        foreach (var translationClass in _translator.Classes)
        {
            AppendLineWithIndent(code, $"class {translationClass.Name}", 0);
            AppendConstructor(code, translationClass.Name);

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

    private void AppendConstructor(StringBuilder sb, string className)
    {
        var constructorTxt = "constructor " + className + "()";
        AppendLineWithIndent(sb, constructorTxt, 1);
        AppendLineWithIndent(sb, "end constructor;\n", 1);
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
                continue;
            }

            if (line.Contains("if ") || line.Contains("for ") || line.Contains("while ") || line.Contains("par") ||
                line.Contains(
                    "thread"))
            {
                AppendLineWithIndent(sb, line, indentation);
                indentation++;
                continue;
            }

            AppendLineWithIndent(sb, line, indentation);
        }
    }

    private static void AppendLineWithIndent(StringBuilder sb, string line, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        sb.AppendLine($"{indent}{line}");
    }
}