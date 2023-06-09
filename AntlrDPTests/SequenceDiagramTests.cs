using Antlr4.Runtime;
using AntlrDP;

namespace AntlrDPTests;

[TestClass]
public class SequenceDiagramTests
{
    private SequenceDiagramRepairedVisitor Setup(string input)
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

        Assert.AreEqual(1, visitor.OalProgram.OalClassMethods.Count);
        Assert.AreEqual("hello", visitor.OalProgram.OalClassMethods[0].Name);
    }

    [TestMethod]
    public void TestOalOccurrenceSpecifications()
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
    public void TestSenderClassObjectInOalMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(visitor.OalProgram.OalClasses[0], visitor.OalProgram.OalClassMethods[0].SenderOalClass);
    }

    [TestMethod]
    public void TestReceiverClassObjectInOalMethod()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(visitor.OalProgram.OalClasses[1], visitor.OalProgram.OalClassMethods[0].ReceiverOalClass);
    }

    [TestMethod]
    public void TestMethodInClasses()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual(visitor.OalProgram.OalClassMethods[0], visitor.OalProgram.OalClasses[0].OalClassMethods[0]);
        Assert.AreEqual(0, visitor.OalProgram.OalClasses[1].OalClassMethods.Count);
    }

    [TestMethod]
    public void TestCodeInOalClass()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.hello();\n"
            , visitor.OalProgram.OalClasses[0].OalClassMethods[0].Code);
    }

    [TestMethod]
    public void TestCodeInProgramWithSimpleHello()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.hello();\n"
            , visitor.OalProgram.Code);
    }

    [TestMethod]
    public void TestCodeInProgramWithImprovedHello()
    {
        var json = File.ReadAllText("files/HelloMessageImproved1.json");
        var visitor = Setup(json);

        Assert.AreEqual("create object instance Class2_inst of Class2;\nClass2_inst.helloClass2();\n" +
                        "create object instance Class3_inst of Class3;\nClass3_inst.helloClass3();\n"
            , visitor.OalProgram.Code);
    }

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

    [TestMethod]
    public void TestAnimArchAnimationObject()
    {
        var json = File.ReadAllText("files/HelloMessageImproved2.json");
        var visitor = Setup(json);

        const string jsonExpected =
            "{\"Code\":\"create object instance Class2_inst of Class2;\\nClass2_inst.helloClass2();\\ncreate object instance Class3_inst of Class3;\\nClass3_inst.helloClass3();\\ncreate object instance Class1_inst of Class1;\\nClass1_inst.helloClass1();\\n\",\"AnimationName\":null,\"MethodsCodes\":[{\"Name\":\"Class1\",\"Methods\":[{\"Name\":\"helloClass2\",\"Code\":\"create object instance Class2_inst of Class2;\\nClass2_inst.helloClass2();\\n\"}]},{\"Name\":\"Class2\",\"Methods\":[{\"Name\":\"helloClass3\",\"Code\":\"create object instance Class3_inst of Class3;\\nClass3_inst.helloClass3();\\n\"}]},{\"Name\":\"Class3\",\"Methods\":[{\"Name\":\"helloClass1\",\"Code\":\"create object instance Class1_inst of Class1;\\nClass1_inst.helloClass1();\\n\"}]}]}";
        var jsonGenerated =
            Newtonsoft.Json.JsonConvert.SerializeObject(visitor.OalProgram.CreateAnimArchAnimationObject());
        Assert.AreEqual(jsonExpected, jsonGenerated);
    }
}