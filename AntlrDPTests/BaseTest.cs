using Antlr4.Runtime;
using AntlrDP;

namespace AntlrDPTests;

public abstract class BaseTest
{
    protected static SequenceDiagramCustomVisitor Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        return visitor;
    }

    protected static SequenceDiagramParser SetupParser(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var speakLexer = new SequenceDiagramLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(speakLexer);
        var sequenceDiagramParser = new SequenceDiagramParser(commonTokenStream);

        return sequenceDiagramParser;
    }

    protected static SequenceDiagramCustomVisitor InitVisitor(SequenceDiagramParser sequenceDiagramParser)
    {
        var context = sequenceDiagramParser.json();
        var visitor = new SequenceDiagramCustomVisitor();
        visitor.Visit(context);
        return visitor;
    }
}