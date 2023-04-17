using Antlr4.Runtime;
using AntlrDP;

namespace AntlrDPTests;

[TestClass]
public class SequenceAttemptTest
{

    private SequenceDiagramRepairedVisitor Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        return visitor;
    }
    private SequenceDiagramParser SetupParser(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var speakLexer = new SequenceDiagramLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(speakLexer);
        var sequenceDiagramParser = new SequenceDiagramParser(commonTokenStream);

        return sequenceDiagramParser;
    }

    private static SequenceDiagramRepairedVisitor InitVisitor(SequenceDiagramParser sequenceDiagramParser)
    {
        var context = sequenceDiagramParser.json();
        var visitor = new SequenceDiagramRepairedVisitor();
        visitor.Visit(context);
        return visitor;
    }

    [TestMethod]
    public void TestJsonParsingAttempt()
    {
        string json = File.ReadAllText("files/VerySimpleExample.json");

        var visitor = Setup(json);

        Assert.AreEqual("hello", visitor.Attempt.Name);
        Assert.AreEqual("Hello", visitor.Attempt.Value);
    }

    [TestMethod]
    public void TestName()
    {
        var json = File.ReadAllText("files/LessSimpleExample1.json");
        var visitor = Setup(json);

        Assert.AreEqual("Hello Obi-Wan", visitor.Message);
    }

    [TestMethod]
    public void TestSequenceSingleClass()
    {
        string json = File.ReadAllText("files/SequenceSingleClass.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance MojaTrieda_inst of MojaTrieda;\n", visitor.OalClass.Code);
    }
    
    [TestMethod]
    public void TestSequenceHelloMessage()
    {
        string json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(2, visitor.OalClasses.Count);
        Assert.AreEqual("Class1", visitor.OalClasses[0].Name);
        Assert.AreEqual("Class2", visitor.OalClasses[1].Name);
    }
    
    [TestMethod]
    public void TestOalProgramMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(1, visitor.OalProgram.ClassMethods.Count);
        Assert.AreEqual("hello", visitor.OalProgram.ClassMethods[0].Name);
    }
}