using AntlrDP.OalCodeElements;
using AntlrDP.Translation;

namespace AntlrDPTests;

[TestClass]
public class TranslatorTests : BaseTest
{
    private new static Translator Setup(string input)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        var sequenceDiagram = visitor.SequenceDiagram;
        var oalCode = new OalCode(sequenceDiagram);
        var translator = new Translator(oalCode);
        return translator;
    }

    [TestMethod]
    public void TestTranslatorHelloMessageTranslationClasses()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var translator = Setup(json);

        var obtainedClasses = translator.Classes;

        Assert.AreEqual(2, obtainedClasses.Count);
        Assert.AreEqual("Class1", obtainedClasses[0].Name);
        Assert.AreEqual("Class2", obtainedClasses[1].Name);
    }

    [TestMethod]
    public void TestSequenceHelloMessageFirstMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var translator = Setup(json);
        var sentMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual("FirstMethod", sentMethod.Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello();\n", sentMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHelloMessageSecondMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var translator = Setup(json);

        var receivedMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual("hello", receivedMethod.Name);
        Assert.AreEqual("", receivedMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHelloTwoMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/HelloTwoMessages.json");
        var translator = Setup(json);

        var sentMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, sentMethod.Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n" +
                        "Class2_inst.hello2();\n", sentMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHelloTwoMessagesSecondClassMethods()
    {
        var json = File.ReadAllText("files/HelloTwoMessages.json");
        var translator = Setup(json);

        var secondClassFirstMethod = translator.Classes[1].Methods[0];
        var secondClassSecondMethod = translator.Classes[1].Methods[1];

        Assert.AreEqual("hello1", secondClassFirstMethod.Name);
        Assert.AreEqual("hello2", secondClassSecondMethod.Name);
        Assert.AreEqual("", secondClassFirstMethod.Code);
        Assert.AreEqual("", secondClassSecondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturnFirstMethod()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];
        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n", method.Code);
    }

    [TestMethod]
    public void TestSequenceOneHelloOneReturnFirstClassSecondMethod()
    {
        var json = File.ReadAllText("files/OneHelloOneReturn.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[1];

        Assert.AreEqual("hello2", method.Name);
        Assert.AreEqual("", method.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesFirstMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.helloClass2();\n" +
                        "create object instance Class3_inst of Class3;\n" +
                        "Class3_inst.helloClass3();\n" +
                        "Class2_inst.helloClass2Again();\n", method.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesSecondClassMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var translator = Setup(json);

        var secondClassMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual("helloClass2", secondClassMethod.Name);
        Assert.AreEqual("", secondClassMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesThirdClassMethod()
    {
        var json = File.ReadAllText("files/Hello3Classes.json");
        var translator = Setup(json);

        var thirdClassMethod = translator.Classes[2].Methods[0];

        Assert.AreEqual("helloClass3", thirdClassMethod.Name);
        Assert.AreEqual("", thirdClassMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesWaterfallFirstMethod()
    {
        var json = File.ReadAllText("files/Hello3ClassesWaterfall.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.helloClass2();\n", firstMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesWaterfallSecondMethod()
    {
        var json = File.ReadAllText("files/Hello3ClassesWaterfall.json");
        var translator = Setup(json);

        var secondMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual("helloClass2", secondMethod.Name);
        Assert.AreEqual("create object instance Class3_inst of Class3;\n" +
                        "Class3_inst.helloClass3();\n", secondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHello3ClassesWaterfallThirdMethod()
    {
        var json = File.ReadAllText("files/Hello3ClassesWaterfall.json");
        var translator = Setup(json);

        var thirdMethod = translator.Classes[2].Methods[0];

        Assert.AreEqual("helloClass3", thirdMethod.Name);
        Assert.AreEqual("create object instance Class4_inst of Class4;\n" +
                        "Class4_inst.helloClass4();\n", thirdMethod.Code);
    }


    [TestMethod]
    public void TestSequenceOneOptFirstMethod()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual("if (condition1)\n" +
                        "create object instance Class2_inst of Class2;\n" +
                        "Class2_inst.hello1();\n" +
                        "end if;\n", method.Code);
    }

    [TestMethod]
    public void TestSequenceOneOptSecondMethod()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var translator = Setup(json);
        var secondMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual("hello1", secondMethod.Name);
        Assert.AreEqual("", secondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoConditionsFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoConditions.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltWithElseFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltWithElse.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
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
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltBeforeAfterMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltBeforeAfterMessages.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "if (condition1)\n" +
            "Class2_inst.hello2();\n" +
            "elif (condition2)\n" +
            "Class2_inst.hello3();\n" +
            "end if;\n" +
            "Class2_inst.hello4();\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneLoopBeforeAfterMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/OneLoopBeforeAfterMessages.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "while (condition1)\n" +
            "Class2_inst.hello2();\n" +
            "end while;\n" +
            "Class2_inst.hello3();\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltMoreMessagesInConditionFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltMoreMessagesInCondition.json");
        var translator = Setup(json);
        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "Class2_inst.hello2();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n" +
            "end if;\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var translator = Setup(json);
        var firstMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "if (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n",
            firstMethod.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltSecondClassFirstMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var translator = Setup(json);
        var secondClassFirstMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual("hello1", secondClassFirstMethod.Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\n" +
            "Class1_inst.hello3();\n" +
            "Class1_inst.hello4();\n",
            secondClassFirstMethod.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltTwoRespondMessagesAfterAltSecondClassThirdMethod()
    {
        var json = File.ReadAllText("files/OneAltTwoRespondMessagesAfterAlt.json");
        var translator = Setup(json);

        var secondClassSecondMethod = translator.Classes[1].Methods[1];

        Assert.AreEqual("hello2", secondClassSecondMethod.Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\n" +
            "Class1_inst.hello3();\n" +
            "Class1_inst.hello4();\n",
            secondClassSecondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceNestedAltInLoopMethod()
    {
        var json = File.ReadAllText("files/NestedAltInLoop.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "while (condition1)\n" +
            "if (condition2)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "end if;\n" +
            "end while;\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceNestedAltInLoopWithMessagesMethod()
    {
        var json = File.ReadAllText("files/NestedAltInLoopWithMessages.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "while (condition1)\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "if (condition2)\n" +
            "Class2_inst.hello2();\n" +
            "end if;\n" +
            "Class2_inst.hello3();\n" +
            "end while;\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceFowlerUmlDistilledMethod()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilled.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "for each line item\n" +
            "if (value > $10000)\n" +
            "create object instance Careful_inst of Careful;\n" +
            "Careful_inst.dispatch();\n" +
            "else\n" +
            "create object instance Regular_inst of Regular;\n" +
            "Regular_inst.dispatch();\n" +
            "end if;\n" +
            "end for;\n" +
            "if (needs confirmation)\n" +
            "create object instance Messenger_inst of Messenger;\n" +
            "Messenger_inst.confirm();\n" +
            "end if;\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMessageStartMethod()
    {
        var json = File.ReadAllText("files/HelloSelfMessage.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[1];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "self.hello1();\n" +
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello2();\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMessageSelfMethod()
    {
        var json = File.ReadAllText("files/HelloSelfMessage.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual("hello1", method.Name);
        Assert.AreEqual(
            "",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageStartMethod()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[1];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "self.hello2();\n" +
            "Class2_inst.hello3();\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageSelfMethod()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var translator = Setup(json);

        var selfMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual("hello2", selfMethod.Name);
        Assert.AreEqual(
            "",
            selfMethod.Code);
    }

    [TestMethod]
    public void TestSequenceNestedLoopAndAltInLoopFirstMethod()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoop.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
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
            method.Code);
    }

    [TestMethod]
    public void TestSequenceOneAltOppositeDirectionValidity()
    {
        var json = File.ReadAllText("files/OneAltOppositeDirectionMessage.json");
        var translator = Setup(json);


        Assert.AreEqual(false, translator.IsDiagramValid);
    }

    [TestMethod]
    public void TestSequenceChainOfResponsibilityFirstMethod()
    {
        var json = File.ReadAllText("files/ChainOfResponsibility.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        Assert.AreEqual(true, translator.IsDiagramValid);
        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance ClassB_inst of ClassB;\n" +
                        "ClassB_inst.methodB();\n", firstMethod.Code);
    }

    [TestMethod]
    public void TestSequenceChainOfResponsibilitySecondMethod()
    {
        var json = File.ReadAllText("files/ChainOfResponsibility.json");
        var translator = Setup(json);

        var secondMethod = translator.Classes[1].Methods[0];
        Assert.AreEqual("methodB", secondMethod.Name);
        Assert.AreEqual("create object instance ClassC_inst of ClassC;\n" +
                        "ClassC_inst.methodC();\n", secondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceChainOfResponsibilityThirdMethod()
    {
        var json = File.ReadAllText("files/ChainOfResponsibility.json");
        var translator = Setup(json);

        var thirdMethod = translator.Classes[2].Methods[0];
        Assert.AreEqual("methodC", thirdMethod.Name);
        Assert.AreEqual("create object instance ClassA_inst of ClassA;\n" +
                        "ClassA_inst.methodA();\n", thirdMethod.Code);
    }

    [TestMethod]
    public void TestSequenceMediatorMethodCount()
    {
        var json = File.ReadAllText("files/Mediator.json");
        var translator = Setup(json);

        var methodsA = translator.Classes[0].Methods;
        var methodsB = translator.Classes[1].Methods;
        var methodsC = translator.Classes[2].Methods;
        Assert.AreEqual(1, methodsA.Count);
        Assert.AreEqual(2, methodsB.Count);
        Assert.AreEqual(1, methodsC.Count);
    }

    [TestMethod]
    public void TestSequenceMediatorClassAMethod()
    {
        var json = File.ReadAllText("files/Mediator.json");
        var translator = Setup(json);

        var classAMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, classAMethod.Name);
        Assert.AreEqual("create object instance ClassB_inst of ClassB;\n" +
                        "ClassB_inst.methodB1();\n" +
                        "create object instance ClassC_inst of ClassC;\n" +
                        "ClassC_inst.methodC();\n" +
                        "ClassB_inst.methodB2();\n", classAMethod.Code);
    }

    [TestMethod]
    public void TestSequenceMediatorClassBMethods()
    {
        var json = File.ReadAllText("files/Mediator.json");
        var translator = Setup(json);

        var classBFirstMethod = translator.Classes[1].Methods[0];
        var classBSecondMethod = translator.Classes[1].Methods[1];

        Assert.AreEqual(2, translator.Classes[1].Methods.Count);
        Assert.AreEqual("methodB1", classBFirstMethod.Name);
        Assert.AreEqual("methodB2", classBSecondMethod.Name);
        Assert.AreEqual("", classBFirstMethod.Code);
        Assert.AreEqual("", classBSecondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceMediatorClassCMethod()
    {
        var json = File.ReadAllText("files/Mediator.json");
        var translator = Setup(json);

        var classCMethod = translator.Classes[2].Methods[0];

        Assert.AreEqual("methodC", classCMethod.Name);
        Assert.AreEqual("", classCMethod.Code);
    }

    // advanced
    // [TestMethod]
    public void TestSequenceFowlerUmlDistilledImprovedFirstMethod()
    {
        var json = File.ReadAllText("files/FowlerUMLDistilledImproved.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        // Assert.AreEqual(true, visitor.OalProgram.IsDiagramValid);
        Assert.AreEqual(Translator.FirstMethodName, method.Name);
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
            method.Code);
    }

    // future:
    // [TestMethod]
    // public void TestSequenceHelloMessageWholeCode()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.hello();\n"
    //         , visitor.OalProgram.Code);
    // }
    //
}