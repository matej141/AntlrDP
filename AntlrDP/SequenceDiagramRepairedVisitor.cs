namespace AntlrDP;

public class SequenceDiagramRepairedVisitor : SequenceDiagramBaseVisitor<object>
{
    public SequenceJsonAttempt Attempt = new();
    public string Message = "";
    public readonly List<OalClass> OalClasses = new();
    public readonly OalProgram OalProgram = new();

    public override object VisitPair(SequenceDiagramParser.PairContext context)
    {
        var val = context.value();
        var name = context.STRING();
        if (name == null)
        {
            return base.VisitPair(context);
        }

        Attempt = new SequenceJsonAttempt()
            { Name = name.GetText().Replace("\"", ""), Value = val.GetText().Replace("\"", "") };
        return base.VisitPair(context);
    }

    public override object VisitName(SequenceDiagramParser.NameContext context)
    {
        Message = "Hello " + context.value().GetText().Replace("\"", "");
        return base.VisitName(context);
    }

    public override object VisitObj(SequenceDiagramParser.ObjContext context)
    {
        var pairs = context.pair();
        if (pairs == null)
        {
            return base.VisitObj(context);
        }

        if (pairs.Any(val => val.lifeline() != null))
        {
            OalClasses.Add(CreateOalClass(pairs));
            var oalClass = CreateOalClass(pairs);
            OalProgram.OalClasses.Add(oalClass);
        }

        if (pairs.Any(val => val.message() != null))
        {
            OalProgram.OalMethodCalls.Add(CreateOalMethodCall(pairs));
        }

        if (pairs.Any(val => val.occurenceSpecification() != null))
        {
            OalProgram.OccurrenceSpecifications.Add(CreateOccurenceSpecification(pairs));
        }

        if (pairs.Any(val => val.combinedFragment() != null))
        {
            OalProgram.CombinedFragments.Add(CreateCombinedFragment(pairs));
        }

        if (pairs.Any(val => val.interactionOperand() != null))
        {
            OalProgram.InteractionOperands.Add(CreateInteractionOperand(pairs));
        }

        if (pairs.Any(val => val.interactionConstraint() != null))
        {
            OalProgram.InteractionConstraints.Add(CreateInteractionConstraint(pairs));
        }

        if (pairs.Any(val => val.opaqueExpression() != null))
        {
            OalProgram.OpaqueExpressions.Add(CreateOpaqueExpression(pairs));
        }

        return base.VisitObj(context);
    }

    private static OalClass CreateOalClass(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var nameContext = pairContexts.First(val => val.name() != null).name();
        var name = nameContext.value().GetText().Replace("\"", "");
        var oalClass = new OalClass() { Id = id, Name = name };
        return oalClass;
    }

    private static OalMethodCall CreateOalMethodCall(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var name = pairContexts.First(val => val.name() != null).name().value().GetText().Replace("\"", "");
        var receiveEventId = GetReceiverOccurrenceId(pairContexts);
        var sendEventId = GetSenderOccurrenceId(pairContexts);
        return new OalMethodCall
            { Name = name, ReceiverOccurrenceId = receiveEventId, SenderOccurrenceId = sendEventId };
    }

    private static string GetReceiverOccurrenceId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var receiveEventIdPairContexts =
            pairContexts.First(val => val.receiveEvent() != null).receiveEvent().value().obj().pair();

        return GetXmiIdRef(receiveEventIdPairContexts);
    }

    private static string GetSenderOccurrenceId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var senderEventIdPairContexts =
            pairContexts.First(val => val.sendEvent() != null).sendEvent().value().obj().pair();
        return GetXmiIdRef(senderEventIdPairContexts);
    }

    private static string GetGuardId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var guardIdPairContexts =
            pairContexts.First(val => val.guard() != null).guard().value().obj().pair();
        return GetXmiIdRef(guardIdPairContexts);
    }

    private static string GetSpecificationId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var guardIdPairContexts =
            pairContexts.First(val => val.specification() != null).specification().value().obj().pair();
        return GetXmiIdRef(guardIdPairContexts);
    }


    private static string GetXmiIdRef(SequenceDiagramParser.PairContext[] pairContexts)
    {
        if (pairContexts.Any(val => val.xmiIdRef() != null))
        {
            return pairContexts.First(val => val.xmiIdRef() != null).xmiIdRef().value().GetText().Replace("\"", "");
        }

        return "";
    }

    private static OalOccurrenceSpecification CreateOccurenceSpecification(
        SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var coveredByContext =
            pairContexts.First(val => val.covered() != null).covered().value().arr().value()[0].obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", "");
        return new OalOccurrenceSpecification { Id = id, RefrenceIdOfCoveredObject = coveredByContext };
    }

    private static OalCombinedFragment CreateCombinedFragment(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var interactionOperator = pairContexts.First(val => val.interactionOperator() != null).interactionOperator()
            .value().GetText().Replace("\"", "");
        var operandsContext =
            pairContexts.First(val => val.operand() != null).operand().value().arr().value();
        var operands = new List<String>();
        foreach (var operand in operandsContext)
        {
            operands.Add(operand.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }

        return new OalCombinedFragment { Id = id, InteractionOperatorId = interactionOperator, Operands = operands };
    }

    private static OalInteractionOperand CreateInteractionOperand(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var guardId = GetGuardId(pairContexts);
        var fragmentsContext =
            pairContexts.First(val => val.fragments() != null).fragments().value().arr().value();
        var fragments = new List<String>();
        foreach (var fragment in fragmentsContext)
        {
            fragments.Add(fragment.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }

        var ownedElementsContext =
            pairContexts.First(val => val.ownedElements() != null).ownedElements().value().arr().value();
        var ownedElements = new List<String>();
        foreach (var ownedElement in ownedElementsContext)
        {
            ownedElements.Add(ownedElement.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }

        return new OalInteractionOperand
            { Id = id, GuardId = guardId, Fragments = fragments, OwnedElements = ownedElements };
    }

    private static OalInteractionConstraint CreateInteractionConstraint(
        SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var specificationId = GetSpecificationId(pairContexts);

        return new OalInteractionConstraint { Id = id, SpecificationId = specificationId };
    }

    private static OalOpaqueExpression CreateOpaqueExpression(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var body = pairContexts.First(val => val.body() != null).body().value().GetText().Replace("\"", "");

        return new OalOpaqueExpression { Id = id, Body = body };
    }

    public override object VisitJson(SequenceDiagramParser.JsonContext context)
    {
        VisitChildren(context);
        OalProgram.SetOalClassesInMethodCalls();
        // OalProgram.SetCodeInClasses();
        OalProgram.SetProgramCode();

        return null;
    }
}