using AntlrDP.OalCodeElements;
using AntlrDP.OalCodeElements.FragmentTypes;

namespace AntlrDPTests;

[TestClass]
public class OalCodeTests : BaseTest
{
    private new static OalCode Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        var sequenceDiagram = visitor.SequenceDiagram;
        var oalCode = new OalCode(sequenceDiagram);
        return oalCode;
    }

    [TestMethod]
    public void TestOalCodeHelloMessageSqdClasses()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var oalCode = Setup(json);

        var obtainedClasses = oalCode.Classes;

        Assert.AreEqual(2, obtainedClasses.Count);
        Assert.AreEqual("Class1", obtainedClasses[0].Name);
        Assert.AreEqual("Class2", obtainedClasses[1].Name);
    }

    [TestMethod]
    public void TestOalCodeHelloMessageSqdMessages()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var oalCode = Setup(json);

        var obtainedMethod = (MethodCall)oalCode.CodeElements[2];
        var class1 = oalCode.CodeElements[0];
        var class2 = oalCode.CodeElements[1];

        Assert.AreEqual("hello", obtainedMethod.Name);
        Assert.AreEqual(class1, obtainedMethod.SenderClass);
        Assert.AreEqual(class2, obtainedMethod.ReceiverClass);
    }

    [TestMethod]
    public void TestOalCodeOneOptCombinedFragmentBody()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var oalCode = Setup(json);

        var obtainedFragment = (Statement)oalCode.CodeElements[2];
        Assert.AreEqual("condition1", obtainedFragment.Name);
    }

    [TestMethod]
    public void TestOalCodeOneOptAllElements()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var oalCode = Setup(json);

        Assert.AreEqual(3, oalCode.CodeElements.Count);
    }

    [TestMethod]
    public void TestOalCodeOneOptCombinedFragmentType()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var oalCode = Setup(json);

        var obtainedFragment = (Statement)oalCode.CodeElements[2];
        var ifFragmentObj = new IfStatement();
        Assert.IsInstanceOfType(ifFragmentObj, obtainedFragment.StatementType.GetType());
    }

    [TestMethod]
    public void TestOalCodeOneOptStatementOwnedElements()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var oalCode = Setup(json);

        var obtainedFragment = (Statement)oalCode.CodeElements[2];
        var fragmentElement = obtainedFragment.StatementElements[0];
        var methodCallObj = new MethodCall();
        Assert.AreEqual(1, obtainedFragment.StatementElements.Count);
        Assert.IsInstanceOfType(methodCallObj, fragmentElement.GetType());
        Assert.AreEqual("hello1", fragmentElement.Name);
    }

    [TestMethod]
    public void TestOalCodeFowlerSqdStatementOwnedElements()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[4];
        var forStatementObj = new ForStatement();

        Assert.IsInstanceOfType(forStatementObj, obtainedStatement.StatementType.GetType());
        Assert.AreEqual(2, obtainedStatement.StatementElements.Count);
    }

    [TestMethod]
    public void TestOalCodeFowlerSqdIfStatement()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[4];
        var ifStatementElement = (Statement)obtainedStatement.StatementElements[0];

        var methodCallObj = new MethodCall();
        var ifFragmentObj = new IfStatement();

        Assert.IsInstanceOfType(ifFragmentObj, ifStatementElement.StatementType.GetType());
        Assert.AreEqual("value > $10000", ifStatementElement.Name);
        Assert.IsInstanceOfType(methodCallObj, ifStatementElement.StatementElements[0].GetType());
    }

    [TestMethod]
    public void TestOalCodeFowlerSqdElseStatement()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[4];
        var elseStatementElement = (Statement)obtainedStatement.StatementElements[1];

        var methodCallObj = new MethodCall();
        var elseFragmentObj = new ElseStatement();

        Assert.IsInstanceOfType(elseFragmentObj, elseStatementElement.StatementType.GetType());
        Assert.AreEqual("else", elseStatementElement.Name);
        Assert.IsInstanceOfType(methodCallObj, elseStatementElement.StatementElements[0].GetType());
        Assert.AreEqual("dispatch", elseStatementElement.StatementElements[0].Name);
    }

    [TestMethod]
    public void TestOalCodeNestedAltInLoopStatements()
    {
        var json = File.ReadAllText("files/NestedAltInLoop.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[2];
        var whileFragmentObj = new WhileStatement();

        Assert.AreEqual(3, oalCode.CodeElements.Count);
        Assert.IsInstanceOfType(whileFragmentObj, obtainedStatement.StatementType.GetType());
        Assert.AreEqual(1, obtainedStatement.StatementElements.Count);
    }

    [TestMethod]
    public void TestOalCodeNestedLoopAndAltInLoopStatements()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[2];
        var whileFragmentObj = new WhileStatement();

        Assert.IsInstanceOfType(whileFragmentObj, obtainedStatement.StatementType.GetType());
        Assert.AreEqual(1, obtainedStatement.StatementElements.Count);
    }

    [TestMethod]
    public void TestOalCodeNestedLoopAndAltInLoopNestedLoopStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[2];
        var nestedWhileStatement = (Statement)obtainedStatement.StatementElements[0];
        var whileFragmentObj = new WhileStatement();

        var statementObj = new Statement
        {
            StatementType = new IfStatement()
        };

        Assert.AreEqual(2, nestedWhileStatement.StatementElements.Count);
        Assert.IsInstanceOfType(whileFragmentObj, nestedWhileStatement.StatementType.GetType());
        Assert.AreEqual("condition2", nestedWhileStatement.Name);
        Assert.IsInstanceOfType(statementObj, nestedWhileStatement.StatementElements[0].GetType());
    }

    [TestMethod]
    public void TestOalCodeNestedLoopAndAltInLoopNestedIfStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[2];
        var nestedWhileStatement = (Statement)obtainedStatement.StatementElements[0];
        var nestedIfStatement = (Statement)nestedWhileStatement.StatementElements[0];
        var ifFragmentObj = new IfStatement();
        var methodCallObj = new MethodCall();

        Assert.IsInstanceOfType(ifFragmentObj, nestedIfStatement.StatementType.GetType());
        Assert.AreEqual("condition3", nestedIfStatement.Name);
        Assert.AreEqual(1, nestedIfStatement.StatementElements.Count);
        Assert.IsInstanceOfType(methodCallObj, nestedIfStatement.StatementElements[0].GetType());
        Assert.AreEqual("hello1", nestedIfStatement.StatementElements[0].Name);
    }

    [TestMethod]
    public void TestOalCodeNestedLoopAndAltInLoopNestedElifStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var oalCode = Setup(json);

        var obtainedStatement = (Statement)oalCode.CodeElements[2];
        var nestedWhileStatement = (Statement)obtainedStatement.StatementElements[0];
        var nestedElifStatement = (Statement)nestedWhileStatement.StatementElements[1];

        var methodCallObj = new MethodCall();

        var elifFragmentObj = new ElifStatement();

        Assert.IsInstanceOfType(elifFragmentObj, nestedElifStatement.StatementType.GetType());
        Assert.AreEqual("condition4", nestedElifStatement.Name);
        Assert.AreEqual(1, nestedElifStatement.StatementElements.Count);
        Assert.IsInstanceOfType(methodCallObj, nestedElifStatement.StatementElements[0].GetType());
        Assert.AreEqual("hello2", nestedElifStatement.StatementElements[0].Name);
    }


    [TestMethod]
    public void TestOalCodeOneAltMoreMessagesInConditionIfStatements()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var oalCode = Setup(json);

        var obtainedIfStatement = (Statement)oalCode.CodeElements[2];

        var ifStatementObj = new IfStatement();

        Assert.AreEqual(4, oalCode.CodeElements.Count);
        Assert.IsInstanceOfType(ifStatementObj, obtainedIfStatement.StatementType.GetType());
        Assert.AreEqual("condition1", obtainedIfStatement.Name);
        Assert.AreEqual(2, obtainedIfStatement.StatementElements.Count);
    }

    [TestMethod]
    public void TestOalCodeOneAltMoreMessagesInConditionElifStatements()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var oalCode = Setup(json);

        var obtainedElifStatement = (Statement)oalCode.CodeElements[3];
        var elifStatementObj = new ElifStatement();

        Assert.IsInstanceOfType(elifStatementObj, obtainedElifStatement.StatementType.GetType());
        Assert.AreEqual("condition2", obtainedElifStatement.Name);
        Assert.AreEqual(1, obtainedElifStatement.StatementElements.Count);
        Assert.AreEqual(true, obtainedElifStatement.IsLast);
    }
}