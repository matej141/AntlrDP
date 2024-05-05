using Antlr4.Runtime;
using SqdToOalTranslator.AnimArchAnimationClasses;
using SqdToOalTranslator.PreOalCodeElements;
using SqdToOalTranslator.SequenceDiagramElements;
using SqdToOalTranslator.Translation;

namespace SqdToOalTranslator.Builder;

public class SqdToOalTranslatorBuilder : ISqdToOalTranslatorBuilder
{
    private string _json;
    private SequenceDiagram _sequenceDiagram;
    private SequenceDiagramParser.JsonContext _context;
    private PreOalCode _preOalCode;
    private AnimArchAnimation _animArchAnimationObject;
    private bool _selfMessagesAreEmpty;

    public SqdToOalTranslatorBuilder(bool selfMessagesAreEmpty = true)
    {
        _selfMessagesAreEmpty = selfMessagesAreEmpty;
    }

    public void ReadJson(string path)
    {
        _json = File.ReadAllText(path);
    }

    public void ParseSequenceDiagram()
    {
        var inputStream = new AntlrInputStream(_json);
        var lexer = new SequenceDiagramLexer(inputStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new SequenceDiagramParser(tokens);
        _context = parser.json();
    }

    public void VisitParsedTreeOfSequenceDiagramElements()
    {
        var visitor = new SequenceDiagramCustomVisitor();
        visitor.Visit(_context);
        _sequenceDiagram = visitor.SequenceDiagram;
    }

    public void TranslateSqdToOalCode()
    {
        _preOalCode = new PreOalCode(_sequenceDiagram);
        var translator = new Translator(_preOalCode, _selfMessagesAreEmpty);
        _animArchAnimationObject = translator.CreateAnimArchAnimationObject();
    }

    public void CreateAnimationFile(string path)
    {
        var jsonGenerated = Newtonsoft.Json.JsonConvert.SerializeObject(_animArchAnimationObject);
        Console.Write(jsonGenerated);
        File.WriteAllText(path, jsonGenerated);
    }

    public void CreateFileWithOalCode(string path)
    {
        var oalCodeTxt = new Translator(_preOalCode, _selfMessagesAreEmpty).GetCompleteOalCode();
        File.WriteAllText(path, oalCodeTxt);
    }
}