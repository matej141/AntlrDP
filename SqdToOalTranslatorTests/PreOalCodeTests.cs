using SqdToOalTranslator.PreOalCodeElements;
using SqdToOalTranslator.PreOalCodeElements.FragmentTypes;

namespace SqdToOalTranslatorTests;

[TestClass]
public class PreOalCodeTests : BaseTest
{
    private new static PreOalCode Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        var sequenceDiagram = visitor.SequenceDiagram;
        var preOalCodeElements = new PreOalCode(sequenceDiagram);
        return preOalCodeElements;
    }

    [TestMethod]
    public void Test_PreOalElements_HelloMessage_Classes()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var preOalCodeElements = Setup(json);

        var obtainedClasses = preOalCodeElements.Classes;

        Assert.AreEqual(2, obtainedClasses.Count);
        Assert.AreEqual("Class1", obtainedClasses[0].Name);
        Assert.AreEqual("Class2", obtainedClasses[1].Name);
    }

    [TestMethod]
    public void Test_PreOalElements_HelloMessage_MethodCalls()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var preOalCodeElements = Setup(json);

        var obtainedMethodCall = (MethodCall)preOalCodeElements.CodeElements[2];
        var class1 = preOalCodeElements.CodeElements[0];
        var class2 = preOalCodeElements.CodeElements[1];

        Assert.AreEqual("hello", obtainedMethodCall.Name);
        Assert.AreEqual(class1, obtainedMethodCall.SenderClass);
        Assert.AreEqual(class2, obtainedMethodCall.ReceiverClass);
    }

    [TestMethod]
    public void Test_PreOalElements_OneOpt_CombinedFragmentBodyAsStatementName()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var preOalCodeElements = Setup(json);

        var obtainedFragment = (Statement)preOalCodeElements.CodeElements[2];
        Assert.AreEqual("condition1", obtainedFragment.Name);
    }

    [TestMethod]
    public void Test_PreOalElements_OneOpt_AllElements()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var preOalCodeElements = Setup(json);

        Assert.AreEqual(3, preOalCodeElements.CodeElements.Count);
    }

    [TestMethod]
    public void Test_PreOalElements_OneOpt_StatementType()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var preOalCodeElements = Setup(json);

        var obtainedFragment = (Statement)preOalCodeElements.CodeElements[2];
        var ifFragmentObj = new IfStatement();
        Assert.IsInstanceOfType(ifFragmentObj, obtainedFragment.StatementType.GetType());
    }

    [TestMethod]
    public void Test_PreOalElements_OneOpt_StatementOwnedElements()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var preOalCodeElements = Setup(json);

        var obtainedFragment = (Statement)preOalCodeElements.CodeElements[2];
        var fragmentElement = obtainedFragment.StatementElements[0];
        var methodCallObj = new MethodCall();
        Assert.AreEqual(1, obtainedFragment.StatementElements.Count);
        Assert.IsInstanceOfType(methodCallObj, fragmentElement.GetType());
        Assert.AreEqual("hello1", fragmentElement.Name);
    }

    [TestMethod]
    public void Test_PreOalElements_FowlerUMLDistilled_StatementOwnedElements()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[4];
        var forStatementObj = new ForEachStatement();

        Assert.IsInstanceOfType(forStatementObj, obtainedStatement.StatementType.GetType());
        Assert.AreEqual(2, obtainedStatement.StatementElements.Count);
    }

    [TestMethod]
    public void Test_PreOalElements_FowlerUMLDistilled_IfStatement()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[4];
        var ifStatementElement = (Statement)obtainedStatement.StatementElements[0];

        var methodCallObj = new MethodCall();
        var ifFragmentObj = new IfStatement();

        Assert.IsInstanceOfType(ifFragmentObj, ifStatementElement.StatementType.GetType());
        Assert.AreEqual("value > $10000", ifStatementElement.Name);
        Assert.IsInstanceOfType(methodCallObj, ifStatementElement.StatementElements[0].GetType());
    }

    [TestMethod]
    public void Test_PreOalElements_FowlerUMLDistilled_ElseStatement()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[4];
        var elseStatementElement = (Statement)obtainedStatement.StatementElements[1];

        var methodCallObj = new MethodCall();
        var elseFragmentObj = new ElseStatement();

        Assert.IsInstanceOfType(elseFragmentObj, elseStatementElement.StatementType.GetType());
        Assert.AreEqual("else", elseStatementElement.Name);
        Assert.IsInstanceOfType(methodCallObj, elseStatementElement.StatementElements[0].GetType());
        Assert.AreEqual("dispatch", elseStatementElement.StatementElements[0].Name);
    }

    [TestMethod]
    public void Test_PreOalElements_NestedAltInLoop_Statements()
    {
        var json = File.ReadAllText("files/NestedAltInLoop.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[2];
        var whileFragmentObj = new WhileStatement();

        Assert.AreEqual(3, preOalCodeElements.CodeElements.Count);
        Assert.IsInstanceOfType(whileFragmentObj, obtainedStatement.StatementType.GetType());
        Assert.AreEqual(1, obtainedStatement.StatementElements.Count);
    }

    [TestMethod]
    public void Test_PreOalElements_NestedLoopAndAltInLoopNested_WhileStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[2];
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
    public void Test_PreOalElements_NestedLoopAndAltInLoopNested_IfStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[2];
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
    public void Test_PreOalElements_NestedLoopAndAltInLoopNested_ElifStatement()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var preOalCodeElements = Setup(json);

        var obtainedStatement = (Statement)preOalCodeElements.CodeElements[2];
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
    public void Test_PreOalElements_OneAltMoreMessagesInCondition_IfStatements()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var preOalCodeElements = Setup(json);

        var obtainedIfStatement = (Statement)preOalCodeElements.CodeElements[2];

        var ifStatementObj = new IfStatement();

        Assert.AreEqual(4, preOalCodeElements.CodeElements.Count);
        Assert.IsInstanceOfType(ifStatementObj, obtainedIfStatement.StatementType.GetType());
        Assert.AreEqual("condition1", obtainedIfStatement.Name);
        Assert.AreEqual(2, obtainedIfStatement.StatementElements.Count);
    }

    [TestMethod]
    public void Test_PreOalElements_OneAltMoreMessagesInCondition_ElifStatements()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var preOalCodeElements = Setup(json);

        var obtainedElifStatement = (Statement)preOalCodeElements.CodeElements[3];
        var elifStatementObj = new ElifStatement();

        Assert.IsInstanceOfType(elifStatementObj, obtainedElifStatement.StatementType.GetType());
        Assert.AreEqual("condition2", obtainedElifStatement.Name);
        Assert.AreEqual(1, obtainedElifStatement.StatementElements.Count);
        Assert.AreEqual(true, obtainedElifStatement.IsLast);
    }
    
    [TestMethod]
    public void Test_PreOalElements_OnePar_FirstAndLastStatement()
    {
        var json = File.ReadAllText("files/OnePar.json");
        var preOalCodeElements = Setup(json);

        var obtainedFirstParStatement = (Statement)preOalCodeElements.CodeElements[2];
        var obtainedLastParStatement = (Statement)preOalCodeElements.CodeElements[4];
        var parStatementObj = new ParStatement();

        Assert.IsInstanceOfType(parStatementObj, obtainedFirstParStatement.StatementType.GetType());
        Assert.IsInstanceOfType(parStatementObj, obtainedLastParStatement.StatementType.GetType());
        Assert.AreEqual("", obtainedFirstParStatement.Name);
        Assert.AreEqual("", obtainedLastParStatement.Name);
        
        Assert.AreEqual(1, obtainedFirstParStatement.StatementElements.Count);
        Assert.AreEqual(1, obtainedLastParStatement.StatementElements.Count);
        
        Assert.AreEqual(true, obtainedFirstParStatement.IsFirst);
        Assert.AreEqual(false, obtainedFirstParStatement.IsLast);
        Assert.AreEqual(false, obtainedLastParStatement.IsFirst);
        Assert.AreEqual(true, obtainedLastParStatement.IsLast);
    }
}