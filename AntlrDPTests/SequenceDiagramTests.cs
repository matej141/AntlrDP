namespace AntlrDPTests;

[TestClass]
public class SequenceDiagramTests : BaseTest
{
    [TestMethod]
    public void TestSqdHelloMessageLifelines()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);
        var obtainedLifelines = visitor.SequenceDiagram.Lifelines;

        Assert.AreEqual(2, obtainedLifelines.Count);
        Assert.AreEqual("Class1", obtainedLifelines[0].Name);
        Assert.AreEqual("Class2", obtainedLifelines[1].Name);
    }

    [TestMethod]
    public void TestSqdHelloMessageMessagesCount()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);
        var obtainedMessages = visitor.SequenceDiagram.Messages;

        Assert.AreEqual(1, obtainedMessages.Count);
    }

    [TestMethod]
    public void TestSqdHelloMessageOccurrenceSpecifications()
    {
        var json = File.ReadAllText("files/HelloMessage.json");
        var visitor = Setup(json);
        var obtainedOccurenceSpecs = visitor.SequenceDiagram.OccurenceSpecifications;
        Assert.AreEqual(2, obtainedOccurenceSpecs.Count);
        Assert.AreEqual("LTEyNjk3NDUyODB1bWw6T2NjdXJyZW5jZVNwZWNpZmljYXRpb24wMy8yMi8yMDIzIDEwOjE0OjA3LjUxMQ==",
            obtainedOccurenceSpecs[0].XmiId);
        Assert.AreEqual("MzUyMDMxNzQ0dW1sOkxpZmVsaW5lMDMvMjIvMjAyMyAxMDoxNDowNy41MTE=",
            obtainedOccurenceSpecs[0].ReferenceIdOfCoveredObject);
    }

    [TestMethod]
    public void TestSqdOneOptCombinedFragment()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);
        var obtainedCombinedFragment = visitor.SequenceDiagram.CombinedFragments[0];
        Assert.AreEqual(3, obtainedCombinedFragment.InteractionOperatorId);
        CollectionAssert.AreEqual(
            new List<string> { "LTMyNzIzOTE2OHVtbDpJbnRlcmFjdGlvbk9wZXJhbmQwMi8yMS8yMDI0IDE3OjQxOjU3LjEwMA==" },
            obtainedCombinedFragment.OperandIds);
    }

    [TestMethod]
    public void TestSqdOneOptInteractionOperand()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);
        var obtainedInteractionOperand = visitor.SequenceDiagram.InteractionOperands[0];
        Assert.AreEqual("LTEwOTUwMDI4OHVtbDpJbnRlcmFjdGlvbkNvbnN0cmFpbnQwMi8yMS8yMDI0IDE3OjQxOjU3LjEwMA==",
            obtainedInteractionOperand.InteractionConstraintId);
        CollectionAssert.AreEqual(
            new List<string>
            {
                "LTIyMDUyMTA4OHVtbDpPY2N1cnJlbmNlU3BlY2lmaWNhdGlvbjAyLzIxLzIwMjQgMTc6NDE6NTcuMTAw",
                "LTExMzgwMzAwOHVtbDpPY2N1cnJlbmNlU3BlY2lmaWNhdGlvbjAyLzIxLzIwMjQgMTc6NDE6NTcuMTAw"
            },
            obtainedInteractionOperand.OwnedElements);
    }

    [TestMethod]
    public void TestSqdOneOptInteractionConstraint()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);
        var obtainedInteractionConstraint = visitor.SequenceDiagram.InteractionConstraints[0];
        Assert.AreEqual("OTI3OTM5ODQwdW1sOk9wYXF1ZUV4cHJlc3Npb24wMi8yMS8yMDI0IDE3OjQxOjU3LjEwMA==",
            obtainedInteractionConstraint.SpecificationId);
    }

    [TestMethod]
    public void TestSqdOneOptOpaqueExpression()
    {
        var json = File.ReadAllText("files/OneOpt.json");
        var visitor = Setup(json);
        var sequenceDiagramOpaqueExpression = visitor.SequenceDiagram.OpaqueExpressions[0];
        Assert.AreEqual("condition1",
            sequenceDiagramOpaqueExpression.Body);
    }


    // do buducna
    // [TestMethod]
    // public void TestSequenceHelloMessageMethodCount()
    // {
    //     var json = File.ReadAllText("files/HelloMessage.json");
    //     var visitor = Setup(json);
    //
    //     Assert.AreEqual(2, visitor.OalProgram.OalClassMethods.Count);
    // }
}