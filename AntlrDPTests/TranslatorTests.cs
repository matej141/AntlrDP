using AntlrDP.OalCodeElements;
using AntlrDP.Translation;

namespace AntlrDPTests;

[TestClass]
public class TranslatorTests : BaseTest
{
    private new static Translator Setup(string input, bool areSelfMessagesBlankMethods = true)
    {
        var sequenceDiagramParser = SetupParser(input);
        var visitor = InitVisitor(sequenceDiagramParser);
        var sequenceDiagram = visitor.SequenceDiagram;
        var oalCode = new OalCode(sequenceDiagram);
        var translator = new Translator(oalCode, areSelfMessagesBlankMethods);
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

        var method = translator.Classes[0].Methods[0];

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

        var method = translator.Classes[0].Methods[1];

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

        var method = translator.Classes[0].Methods[0];

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

        var selfMethod = translator.Classes[0].Methods[1];

        Assert.AreEqual("hello2", selfMethod.Name);
        Assert.AreEqual(
            "",
            selfMethod.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageStartMethod_OtherModeOfSelfMessages()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var translator = Setup(json, false);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "self.hello2();\n",
            method.Code);
    }

    [TestMethod]
    public void TestSequenceHelloSelfMiddleMessageSelfMethod_OtherModeOfSelfMessages()
    {
        var json = File.ReadAllText("files/HelloMiddleSelfMessage.json");
        var translator = Setup(json, false);

        var method = translator.Classes[0].Methods[1];

        Assert.AreEqual("hello2", method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n",
            method.Code);
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
    public void TestSequenceNestedLoopAndAltInLoopWithMessagesFirstMethod()
    {
        var json = File.ReadAllText("files/NestedLoopAndAltInLoopWithMessages.json");
        var translator = Setup(json);

        var method = translator.Classes[0].Methods[0];

        Assert.AreEqual(1, translator.Classes[0].Methods.Count);
        Assert.AreEqual(3, translator.Classes[1].Methods.Count);
        Assert.AreEqual(Translator.FirstMethodName, method.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.helloClass2();\nwhile (condition1)\nwhile (condition2)\n" +
            "if (condition3)\nClass2_inst.hello1();\nelif (condition4)\nClass2_inst.hello2();\n" +
            "end if;\nend while;\nend while;\n",
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
        var json = File.ReadAllText("files/chainOfResp.json");
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
        var json = File.ReadAllText("files/chainOfResp.json");
        var translator = Setup(json);

        var secondMethod = translator.Classes[1].Methods[0];
        Assert.AreEqual("methodB", secondMethod.Name);
        Assert.AreEqual("create object instance ClassC_inst of ClassC;\n" +
                        "ClassC_inst.methodC();\n", secondMethod.Code);
    }

    [TestMethod]
    public void TestSequenceChainOfResponsibilityThirdMethod()
    {
        var json = File.ReadAllText("files/chainOfResp.json");
        var translator = Setup(json);

        var thirdMethod = translator.Classes[2].Methods[0];
        Assert.AreEqual("methodC", thirdMethod.Name);
        Assert.AreEqual("create object instance ClassA_inst of ClassA;\n" +
                        "ClassA_inst.methodA();\n", thirdMethod.Code);
    }

    [TestMethod]
    public void TestSequenceMediatorMethodCount()
    {
        var json = File.ReadAllText("files/Mediator_Z.json");
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
        var json = File.ReadAllText("files/Mediator_Z.json");
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
        var json = File.ReadAllText("files/Mediator_Z.json");
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
        var json = File.ReadAllText("files/Mediator_Z.json");
        var translator = Setup(json);

        var classCMethod = translator.Classes[2].Methods[0];

        Assert.AreEqual("methodC", classCMethod.Name);
        Assert.AreEqual("", classCMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGameFirstMethod()
    {
        var json = File.ReadAllText("files/AbstractFactoryGame.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance Game_inst of Game;\nGame_inst.CreateLevel();\n", firstMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGame_SecondAndThirdMethod_SelfMessagesAreBlankMethods()
    {
        var json = File.ReadAllText("files/AbstractFactoryGame.json");
        var translator = Setup(json);

        var secondMethod = translator.Classes[1].Methods[0];
        var thirdMethod = translator.Classes[1].Methods[1];

        Assert.AreEqual("CreateLevel", secondMethod.Name);
        Assert.AreEqual(
            "self.CreateHumanArmy();\ncreate object instance HumanFactory_inst of HumanFactory;\n" +
            "HumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateRanger();\n" +
            "HumanFactory_inst.CreateMage();\n",
            secondMethod.Code);
        Assert.AreEqual("CreateHumanArmy", thirdMethod.Name);
        Assert.AreEqual(
            "",
            thirdMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGame_SecondAndThirdMethod_SelfMessagesAreNOTBlankMethods()
    {
        var json = File.ReadAllText("files/AbstractFactoryGame.json");
        var translator = Setup(json, false);

        var secondMethod = translator.Classes[1].Methods[0];
        var thirdMethod = translator.Classes[1].Methods[1];

        Assert.AreEqual("CreateLevel", secondMethod.Name);
        Assert.AreEqual(
            "self.CreateHumanArmy();\n",
            secondMethod.Code);
        Assert.AreEqual("CreateHumanArmy", thirdMethod.Name);
        Assert.AreEqual(
            "create object instance HumanFactory_inst of HumanFactory;\n" +
            "HumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateRanger();\n" +
            "HumanFactory_inst.CreateMage();\n",
            thirdMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGameWithManyMethods_ManyMethods_SelfMessagesAreNOTBlankMethods()
    {
        var json = File.ReadAllText("files/AbstractFactoryGameManyMessages.json");
        var translator = Setup(json, false);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[1].Methods[0];
        var thirdMethod = translator.Classes[1].Methods[1];
        var forthMethod = translator.Classes[1].Methods[2];

        var fifthMethod = translator.Classes[2].Methods[0];
        var sixthMethod = translator.Classes[2].Methods[1];
        var seventhMethod = translator.Classes[2].Methods[2];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance Game_inst of Game;\n" +
                        "Game_inst.CreateLevel();\n", firstMethod.Code);

        Assert.AreEqual("CreateLevel", secondMethod.Name);
        Assert.AreEqual(
            "self.CreateHumanArmy();\n",
            secondMethod.Code);
        Assert.AreEqual("CreateHumanArmy", thirdMethod.Name);
        Assert.AreEqual(
            "create object instance HumanFactory_inst of HumanFactory;\nHumanFactory_inst.CreateWarrior();\n" +
            "HumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateWarrior();\n" +
            "HumanFactory_inst.CreateRanger();\nHumanFactory_inst.CreateRanger();\n" +
            "HumanFactory_inst.CreateMage();\nself.CreateElvenArmy();\n",
            thirdMethod.Code);

        Assert.AreEqual("CreateElvenArmy", forthMethod.Name);
        Assert.AreEqual(
            "create object instance ElvenFactory_inst of ElvenFactory;\nElvenFactory_inst.CreateWarrior();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nself.CreateTrollArmy();\n",
            forthMethod.Code);

        Assert.AreEqual("CreateWarrior", fifthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanWarrior_inst of HumanWarrior;\n" +
            "HumanWarrior_inst.HumanWarrior();\n",
            fifthMethod.Code);

        Assert.AreEqual("CreateRanger", sixthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanRanger_inst of HumanRanger;\n" +
            "HumanRanger_inst.HumanRanger();\n",
            sixthMethod.Code);

        Assert.AreEqual("CreateMage", seventhMethod.Name);
        Assert.AreEqual(
            "create object instance HumanMage_inst of HumanMage;\n" +
            "HumanMage_inst.HumanMage();\n",
            seventhMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGameParallel_ManyMethods_SelfMessagesAreBlankMethods()
    {
        var json = File.ReadAllText("files/AbstractFactoryGameParallel.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[1].Methods[0];
        var thirdMethod = translator.Classes[1].Methods[1];
        var forthMethod = translator.Classes[1].Methods[2];
        var fifthMethod = translator.Classes[1].Methods[3];

        var sixthMethod = translator.Classes[2].Methods[0];
        var seventhMethod = translator.Classes[2].Methods[1];
        var eighthMethod = translator.Classes[2].Methods[2];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance Game_inst of Game;\n" +
                        "Game_inst.CreateLevel();\n", firstMethod.Code);

        Assert.AreEqual("CreateLevel", secondMethod.Name);
        Assert.AreEqual(
            "par\nthread\nself.CreateHumanArmy();\ncreate object instance HumanFactory_inst of HumanFactory;\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateRanger();\nHumanFactory_inst.CreateRanger();\nHumanFactory_inst.CreateMage();\nend thread;\nthread\nself.CreateElvenArmy();\ncreate object instance ElvenFactory_inst of ElvenFactory;\nElvenFactory_inst.CreateWarrior();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateMage();\nend thread;\nthread\nself.CreateTrollArmy();\ncreate object instance TrollFactory_inst of TrollFactory;\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateMage();\nend thread;\nend par;\n",
            secondMethod.Code);
        Assert.AreEqual("CreateHumanArmy", thirdMethod.Name);
        Assert.AreEqual(
            "",
            thirdMethod.Code);

        Assert.AreEqual("CreateElvenArmy", forthMethod.Name);
        Assert.AreEqual(
            "",
            forthMethod.Code);

        Assert.AreEqual("CreateTrollArmy", fifthMethod.Name);
        Assert.AreEqual(
            "",
            fifthMethod.Code);

        Assert.AreEqual("CreateWarrior", sixthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanWarrior_inst of HumanWarrior;\n" +
            "HumanWarrior_inst.HumanWarrior();\n",
            sixthMethod.Code);

        Assert.AreEqual("CreateRanger", seventhMethod.Name);
        Assert.AreEqual(
            "create object instance HumanRanger_inst of HumanRanger;\n" +
            "HumanRanger_inst.HumanRanger();\n",
            seventhMethod.Code);

        Assert.AreEqual("CreateMage", eighthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanMage_inst of HumanMage;\n" +
            "HumanMage_inst.HumanMage();\n",
            eighthMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAbstractFactoryGameParallel_Methods_SelfMessagesAreNotBlankMethods()
    {
        var json = File.ReadAllText("files/AbstractFactoryGameParallel.json");
        var translator = Setup(json, false);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[1].Methods[0];
        var thirdMethod = translator.Classes[1].Methods[1];
        var forthMethod = translator.Classes[1].Methods[2];
        var fifthMethod = translator.Classes[1].Methods[3];

        var sixthMethod = translator.Classes[2].Methods[0];
        var seventhMethod = translator.Classes[2].Methods[1];
        var eighthMethod = translator.Classes[2].Methods[2];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual("create object instance Game_inst of Game;\n" +
                        "Game_inst.CreateLevel();\n", firstMethod.Code);

        Assert.AreEqual("CreateLevel", secondMethod.Name);
        Assert.AreEqual(
            "par\nthread\nself.CreateHumanArmy();\nend thread;\nthread\nself.CreateElvenArmy();\nend thread;\nthread\nself.CreateTrollArmy();\nend thread;\nend par;\n",
            secondMethod.Code);
        Assert.AreEqual("CreateHumanArmy", thirdMethod.Name);
        Assert.AreEqual(
            "create object instance HumanFactory_inst of HumanFactory;\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateWarrior();\nHumanFactory_inst.CreateRanger();\nHumanFactory_inst.CreateRanger();\nHumanFactory_inst.CreateMage();\n",
            thirdMethod.Code);

        Assert.AreEqual("CreateElvenArmy", forthMethod.Name);
        Assert.AreEqual(
            "create object instance ElvenFactory_inst of ElvenFactory;\nElvenFactory_inst.CreateWarrior();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateRanger();\nElvenFactory_inst.CreateMage();\n",
            forthMethod.Code);

        Assert.AreEqual("CreateTrollArmy", fifthMethod.Name);
        Assert.AreEqual(
            "create object instance TrollFactory_inst of TrollFactory;\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateWarrior();\nTrollFactory_inst.CreateMage();\n",
            fifthMethod.Code);

        Assert.AreEqual("CreateWarrior", sixthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanWarrior_inst of HumanWarrior;\n" +
            "HumanWarrior_inst.HumanWarrior();\n",
            sixthMethod.Code);

        Assert.AreEqual("CreateRanger", seventhMethod.Name);
        Assert.AreEqual(
            "create object instance HumanRanger_inst of HumanRanger;\n" +
            "HumanRanger_inst.HumanRanger();\n",
            seventhMethod.Code);

        Assert.AreEqual("CreateMage", eighthMethod.Name);
        Assert.AreEqual(
            "create object instance HumanMage_inst of HumanMage;\n" +
            "HumanMage_inst.HumanMage();\n",
            eighthMethod.Code);
    }

    [TestMethod]
    public void TestSequenceDifferentDirectionsWithoutAlt_AllMethods()
    {
        var json = File.ReadAllText("files/DifferentDirectionsWithoutAlt.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethodOfFirstClass = translator.Classes[0].Methods[1];

        var thirdMethod = translator.Classes[1].Methods[0];
        var forthMethod = translator.Classes[1].Methods[1];
        var fifthMethod = translator.Classes[1].Methods[2];

        Assert.AreEqual(2, translator.Classes[0].Methods.Count);
        Assert.AreEqual(3, translator.Classes[1].Methods.Count);
        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\nClass2_inst.hello1();\n",
            firstMethod.Code);

        Assert.AreEqual("hello2", secondMethodOfFirstClass.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n" +
            "Class2_inst.hello4();\n",
            secondMethodOfFirstClass.Code);

        Assert.AreEqual("hello1", thirdMethod.Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\nClass1_inst.hello2();\n",
            thirdMethod.Code);

        Assert.AreEqual("hello3", forthMethod.Name);
        Assert.AreEqual(
            "",
            forthMethod.Code);

        Assert.AreEqual("hello4", fifthMethod.Name);
        Assert.AreEqual(
            "",
            fifthMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAltWithDifferentDirections_AllMethods()
    {
        var json = File.ReadAllText("files/AltWithDifferentDirections.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[0].Methods[1];

        var thirdMethod = translator.Classes[1].Methods[0];
        var forthMethod = translator.Classes[1].Methods[1];
        var fifthMethod = translator.Classes[1].Methods[2];

        Assert.AreEqual(2, translator.Classes[0].Methods.Count);
        Assert.AreEqual(3, translator.Classes[1].Methods.Count);
        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "if (condition1)\ncreate object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello1();\n" +
            "elif (condition2)\ncreate object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello4();\nend if;\n",
            firstMethod.Code);

        Assert.AreEqual("hello2", secondMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n",
            secondMethod.Code);

        Assert.AreEqual("hello1", thirdMethod.Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\nClass1_inst.hello2();\n",
            thirdMethod.Code);

        Assert.AreEqual("hello3", forthMethod.Name);
        Assert.AreEqual(
            "",
            forthMethod.Code);

        Assert.AreEqual("hello4", fifthMethod.Name);
        Assert.AreEqual(
            "",
            fifthMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAltWithDifferentDirectionsWithMessagesBeforeAfterAlt_AllMethods()
    {
        var json = File.ReadAllText("files/AltWithDifferentDirectionsWithMessagesBeforeAfterAlt.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[0].Methods[1];

        var thirdMethod = translator.Classes[1].Methods[0];
        var forthMethod = translator.Classes[1].Methods[1];
        var fifthMethod = translator.Classes[1].Methods[2];
        var sixthMethod = translator.Classes[1].Methods[3];
        var seventhMethod = translator.Classes[1].Methods[4];

        Assert.AreEqual(2, translator.Classes[0].Methods.Count);
        Assert.AreEqual(5, translator.Classes[1].Methods.Count);
        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\nClass2_inst.helloBeforeAlt();\n" +
            "if (condition1)\nClass2_inst.hello1();\nelif (condition2)\nClass2_inst.hello4();\nend if;\n" +
            "Class2_inst.helloAfterAlt();\n",
            firstMethod.Code);

        Assert.AreEqual("hello2", secondMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\n" +
            "Class2_inst.hello3();\n",
            secondMethod.Code);

        Assert.AreEqual("helloBeforeAlt", thirdMethod.Name);
        Assert.AreEqual(
            "",
            thirdMethod.Code);

        Assert.AreEqual("hello1", forthMethod.Name);
        Assert.AreEqual(
            "create object instance Class1_inst of Class1;\nClass1_inst.hello2();\n",
            forthMethod.Code);

        Assert.AreEqual("hello3", fifthMethod.Name);
        Assert.AreEqual(
            "",
            fifthMethod.Code);

        Assert.AreEqual("hello4", sixthMethod.Name);
        Assert.AreEqual(
            "",
            sixthMethod.Code);

        Assert.AreEqual("helloAfterAlt", seventhMethod.Name);
        Assert.AreEqual(
            "",
            seventhMethod.Code);
    }

    [TestMethod]
    public void TestSequenceAltFirstMethodFromDifferentClass_AllMethods()
    {
        var json = File.ReadAllText("files/AltFirstMethodFromDifferentClass.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[1].Methods[0];

        var thirdMethod = translator.Classes[2].Methods[0];
        var forthMethod = translator.Classes[2].Methods[1];

        Assert.AreEqual(1, translator.Classes[0].Methods.Count);
        Assert.AreEqual(1, translator.Classes[1].Methods.Count);
        Assert.AreEqual(2, translator.Classes[2].Methods.Count);
        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\nClass2_inst.hello1();\n",
            firstMethod.Code);

        Assert.AreEqual("hello1", secondMethod.Name);
        Assert.AreEqual(
            "if (condition1)\ncreate object instance Class3_inst of Class3;\n" +
            "Class3_inst.hello2();\nelif (condition2)\n" +
            "Class3_inst.hello3();\nend if;\n",
            secondMethod.Code);

        Assert.AreEqual("hello2", thirdMethod.Name);
        Assert.AreEqual(
            "",
            thirdMethod.Code);

        Assert.AreEqual("hello3", forthMethod.Name);
        Assert.AreEqual(
            "",
            forthMethod.Code);
    }


    [TestMethod]
    public void TestSequenceEmptyOpt_AllMethods()
    {
        var json = File.ReadAllText("files/EmptyOpt.json");
        var translator = Setup(json);

        var firstMethod = translator.Classes[0].Methods[0];
        var secondMethod = translator.Classes[1].Methods[0];

        Assert.AreEqual(Translator.FirstMethodName, firstMethod.Name);
        Assert.AreEqual(
            "create object instance Class2_inst of Class2;\nClass2_inst.hello1();\n",
            firstMethod.Code);

        Assert.AreEqual("hello1", secondMethod.Name);
        Assert.AreEqual(
            "if (condition1)\nend if;\n",
            secondMethod.Code);
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