using Antlr4.Runtime;
using AntlrDP;

namespace AntlrDPTests;

[TestClass]
public class SequenceDiagramOldTests
{
    private string firstMethodName = "FirstMethod";

    private SequenceDiagramRepairedArchivedVisitor Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        return visitor;
    }

    private static SequenceDiagramParser SetupParser(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var speakLexer = new SequenceDiagramLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(speakLexer);
        var sequenceDiagramParser = new SequenceDiagramParser(commonTokenStream);

        return sequenceDiagramParser;
    }

    private static SequenceDiagramRepairedArchivedVisitor InitVisitor(SequenceDiagramParser sequenceDiagramParser)
    {
        var context = sequenceDiagramParser.json();
        var visitor = new SequenceDiagramRepairedArchivedVisitor();
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
    public void TestSequenceHelloMessageClasses()
    {
        string json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(2, visitor.OalClasses.Count);
        Assert.AreEqual("Class1", visitor.OalClasses[0].Name);
        Assert.AreEqual("Class2", visitor.OalClasses[1].Name);
    }

    [TestMethod]
    public void TestSequenceHelloMessageMethodCount()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(2, visitor.OalProgram.OalClassMethods.Count);
    }

    [TestMethod]
    public void TestSequenceHelloMessageFirstMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("FirstMethod", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello();\n", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHelloMessageSecondMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHelloMessageOccurrenceSpecifications()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(2, visitor.OalProgram.OccurrenceSpecifications.Count);
        Assert.AreEqual("LTEyNjk3NDUyODB1bWw6T2NjdXJyZW5jZVNwZWNpZmljYXRpb24wMy8yMi8yMDIzIDEwOjE0OjA3LjUxMQ==",
            visitor.OalProgram.OccurrenceSpecifications[0].Id);
        Assert.AreEqual("MzUyMDMxNzQ0dW1sOkxpZmVsaW5lMDMvMjIvMjAyMyAxMDoxNDowNy41MTE=",
            visitor.OalProgram.OccurrenceSpecifications[0].RefrenceIdOfCoveredObject);
    }

    [TestMethod]
    public void TestSequenceHelloMessageWholeCode()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.hello();\n"
            , visitor.OalProgram.Code);
    }

    [TestMethod]
    public void TestSequenceHelloTwoMessagesMethodCount()
    {
        var json = File.ReadAllText("files/HelloTwoMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(3, visitor.OalProgram.OalClassMethods.Count);
    }

    [TestMethod]
    public void TestSequenceHelloTwoMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/HelloTwoMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n" +
                        "Class2_inst.hello2();\n", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHelloTwoMessagesSecondClassMethods()
    {
        var json = File.ReadAllText("files/HelloTwoMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello1", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual("hello2", visitor.OalProgram.OalClasses[1].OalClassMethods[1].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[1].OalClassMethods[1].Code);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturn()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var visitor = Setup(json);

        Assert.AreEqual(3, visitor.OalProgram.OalClassMethods.Count);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturnFirstMethod()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturnSecondClassFirstMethod()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello1", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual("create object instance Class1_inst of Class1;\n" +
                        "Class1_inst.hello2();\n", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturnFirstClassSecondMethod()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello2", visitor.OalProgram.OalClasses[0].OalClassMethods[1].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[0].OalClassMethods[1].Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesMethodCount()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var visitor = Setup(json);

        Assert.AreEqual(4, visitor.OalProgram.OalClassMethods.Count);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesFirstMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.helloClass2();\n" +
                        "create object instance Class3_inst of Class3;\n" +
                        "Class3_inst.helloClass3();\n" +
                        "Class2_inst.helloClass2Again();\n", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesSecondClassMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var visitor = Setup(json);

        Assert.AreEqual("helloClass2", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesThirdClassMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var visitor = Setup(json);

        Assert.AreEqual("helloClass3", visitor.OalProgram.OalClasses[2].OalClassMethods[0].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[2].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneOptFirstMethod()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual("if (condition1)\n" +
                        "create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n" +
                        "end if;\n", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneOptSecondMethod()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello1", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual("", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoConditionsFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoConditions.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltBeforeAfterMessagesMethodCount()
    {
        var json = File.ReadAllText("files/OneAltBeforeAfterMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(5, visitor.OalProgram.OalClassMethods.Count);
    }

    [TestMethod]
    public void TestSequenceOneAltWithElseFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltWithElse.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "else\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltBeforeAfterMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltBeforeAfterMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "if (condition1)\n" +
            "Class2_inst.hello2();\n" +
            "elif (condition2)\n" +
            "Class2_inst.hello3();\n" +
            "end if;\n" +
            "Class2_inst.hello4();\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }


    [TestMethod]
    public void TestSequenceOneLoopBeforeAfterMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/OneLoopBeforeAfterMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "while (condition1)\n" +
            "Class2_inst.hello2();\n" +
            "end while;\n" +
            "Class2_inst.hello3();\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }


    [TestMethod]
    public void TestSequenceOneAltMoreMessagesInConditionFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "Class2_inst.hello2();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltSecondClassFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello1", visitor.OalProgram.OalClasses[1].OalClassMethods[0].Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\n" +
            "Class1_inst.hello3();\n" +
            "Class1_inst.hello4();\n"
            ,
            visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltSecondClassThirdMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello2", visitor.OalProgram.OalClasses[1].OalClassMethods[1].Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\n" +
            "Class1_inst.hello3();\n" +
            "Class1_inst.hello4();\n",
            visitor.OalProgram.OalClasses[1].OalClassMethods[1].Code);
    }

    [TestMethod]
    public void TestSequenceNestedAltInLoopMethod()
    {
        var json = File.ReadAllText("files/NestedAltInLoop.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "while (condition1)\n" +
            "if (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "end if;\n" +
            "end while;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceNestedAltInLoopWithMessagesMethod()
    {
        var json = File.ReadAllText("files/NestedAltInLoopWithMessages.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "while (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "if (condition2)\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n" +
            "Class2_inst.hello3();\n" +
            "end while;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceFowlerUmlDistilledMethod()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "while (for each line item)\n" +
            "if (value > $10000)\n" +
            "create object instance Careful_inst of Careful;\n" +
            "Careful_inst.dispatch();\n" +
            "else\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.dispatch();\n" +
            "end if;\n" +
            "end while;\n" +
            "if (needs confirmation)\n" +
            "create object instance Messenger_inst of Messenger;\n" +
            "Messenger_inst.confirm();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMessageStartMethod()
    {
        var json = File.ReadAllText("files/HelloSelfMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[1].Name);
        Assert.AreEqual(
            "self.hello1();\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[1].Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMessageSelfMethod()
    {
        var json = File.ReadAllText("files/HelloSelfMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello1", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageStartMethod()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[1].Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "self.hello2();\n" +
            "Class2_inst.hello3();\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[1].Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageSelfMethod()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("hello2", visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceOneAltOppositeDirectionMessageFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltOppositeDirectionMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(false, visitor.OalProgram.IsDiagramValid);
    }

    [TestMethod]
    public void TestSequenceNestedLoopAndAltInLoopFirstMethod()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var visitor = Setup(json);

        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "while (condition1)\n" +
            "while (condition2)\n" +
            "if (condition3)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition4)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n" +
            "end while;\n" +
            "end while;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    // advanced
    [TestMethod]
    public void TestSequenceFowlerUmlDistilledImprovedFirstMethod()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilledImproved.json");
        var visitor = Setup(json);

        Assert.AreEqual(true, visitor.OalProgram.IsDiagramValid);
        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "create object instance Careful_inst of Careful;\n" +
            "Careful_inst.hello1();\n" +
            "while (for each line item)\n" +
            "Careful_inst.hello2();\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.hello3();\n" +
            "if (value > $10000)\n" +
            "Careful_inst.dispatch();\n" +
            "else\n" +
            "Regular_inst.dispatch();\n" +
            "end if;\n" +
            "Careful_inst.hello5();\n" +
            "end while;\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.hello6();\n" +
            "if (needs confirmation)\n" +
            "create object instance Messenger_inst of Messenger;\n" +
            "Messenger_inst.confirm();\n" +
            "Regular_inst.hello7();\n" +
            "Careful_inst.hello8();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestSequenceCrazyFirstMethod()
    {
        var json = File.ReadAllText("files/Crazy.json");
        var visitor = Setup(json);

        Assert.AreEqual(true, visitor.OalProgram.IsDiagramValid);
        Assert.AreEqual(firstMethodName, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Name);
        Assert.AreEqual(
            "create object instance Careful_inst of Careful;\n" +
            "Careful_inst.hello1();\n" +
            "while (for each line item)\n" +
            "Careful_inst.hello2();\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.hello3();\n" +
            "if (value > $10000)\n" +
            "Careful_inst.dispatch();\n" +
            "else\n" +
            "Regular_inst.dispatch();\n" +
            "end if;\n" +
            "Careful_inst.hello5();\n" +
            "end while;\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.hello6();\n" +
            "if (needs confirmation)\n" +
            "create object instance Messenger_inst of Messenger;\n" +
            "Messenger_inst.confirm();\n" +
            "Regular_inst.hello7();\n" +
            "Careful_inst.hello8();\n" +
            "end if;\n",
            visitor.OalProgram.OalClasses[0].OalClassMethods[1].Code);
    }

    // // bad test, update it or remove it
    // [TestMethod]
    // public void TestSenderClassObjectInOalMethod()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual(visitor.OalProgram.OalClasses[0], visitor.OalProgram.OalClassMethods[0].SenderOalClass);
    // }

    // // bad test, update it or remove it
    // [TestMethod]
    // public void TestReceiverClassObjectInOalMethod()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual(visitor.OalProgram.OalClasses[1], visitor.OalProgram.OalClassMethods[0].ReceiverOalClass);
    // }

    // [TestMethod]
    // public void TestMethodsInClasses()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual(visitor.OalProgram.OalClassMethods[0].Code, visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    //     Assert.AreEqual(1, visitor.OalProgram.OalClasses[1].OalClassMethods.Count);
    // }

    // [TestMethod]
    // public void TestCodeInOalClass()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.hello();\n"
    //         , visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    // }

    // stary test
    [TestMethod]
    public void TestCodeInProgramWithImprovedHello()
    {
        var json = File.ReadAllText("files/HelloMessageImproved1.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.helloClass2();\n" +
                        "create object instance Class3_inst of Class3;\nClass3_inst.helloClass3();\n"
            , visitor.OalProgram.Code);
    }

    // stary test
    [TestMethod]
    public void TestCodeInProgramWithMoreMethods()
    {
        var json = File.ReadAllText("files/HelloMessageImproved2.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.helloClass2();\n" +
                        "create object instance Class3_inst of Class3;\nClass3_inst.helloClass3();\n" +
                        "create object instance Class1_inst of Class1;\nClass1_inst.helloClass1();\n"
            , visitor.OalProgram.Code);
    }

    // stary test
    [TestMethod]
    public void TestCodesInMultipleMethods()
    {
        var json = File.ReadAllText("files/HelloMessageImproved2.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.helloClass2();\n"
            , visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
        Assert.AreEqual("create object instance Class3_inst of Class3;\nClass3_inst.helloClass3();\n"
            , visitor.OalProgram.OalClasses[1].OalClassMethods[0].Code);
        Assert.AreEqual("create object instance Class1_inst of Class1;\nClass1_inst.helloClass1();\n"
            , visitor.OalProgram.OalClasses[2].OalClassMethods[0].Code);
    }

    // [TestMethod]
    // public void TestAnimArchAnimationObject()
    // {
    //     var json = File.ReadAllText("files/HelloMessageImproved2.json");
    //     var visitor = Setup(json);
    //
    //     const string jsonExpected =
    //         "{\"Code\":\"create object instance Class2_inst of Class2;\\nClass2_inst.helloClass2();\\ncreate object instance Class3_inst of Class3;\\nClass3_inst.helloClass3();\\ncreate object instance Class1_inst of Class1;\\nClass1_inst.helloClass1();\\n\",\"AnimationName\":null,\"MethodsCodes\":[{\"Name\":\"Class1\",\"Methods\":[{\"Name\":\"helloClass2\",\"Code\":\"create object instance Class2_inst of Class2;\\nClass2_inst.helloClass2();\\n\"}]},{\"Name\":\"Class2\",\"Methods\":[{\"Name\":\"helloClass3\",\"Code\":\"create object instance Class3_inst of Class3;\\nClass3_inst.helloClass3();\\n\"}]},{\"Name\":\"Class3\",\"Methods\":[{\"Name\":\"helloClass1\",\"Code\":\"create object instance Class1_inst of Class1;\\nClass1_inst.helloClass1();\\n\"}]}]}";
    //     var jsonGenerated =
    //         Newtonsoft.Json.JsonConvert.SerializeObject(visitor.OalProgram.CreateAnimArchAnimationObject());
    //     Assert.AreEqual(jsonExpected, jsonGenerated);
    // }

    // [TestMethod]
    // public void TestDiagramWithLoopFragment()
    // {
    //     var json = File.ReadAllText("files/SimpleALT.json");
    //     var visitor = Setup(json);
    //
    //     const string jsonExpected =
    //         "{\"Code\":\"create object instance Class2_inst of Class2;\\nwhile (a>5)\\r\\nClass2_inst.hello();\\n\",\"AnimationName\":null,\"MethodsCodes\":[{\"Name\":\"Class1\",\"Methods\":[{\"Name\":\"hello\",\"Code\":\"create object instance Class2_inst of Class2;\\nwhile (a>5)\\r\\nClass2_inst.hello();\\n\"}]}]}";
    //     var jsonGenerated =
    //         Newtonsoft.Json.JsonConvert.SerializeObject(visitor.OalProgram.CreateAnimArchAnimationObject());
    //     Assert.AreEqual(jsonExpected, jsonGenerated);
    // }
}