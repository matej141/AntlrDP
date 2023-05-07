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
            OalProgram.OalClassMethods.Add(CreateOalClassMethod(pairs));
        }

        if (pairs.Any(val => val.occurenceSpecification() != null))
        {
            OalProgram.OccurrenceSpecifications.Add(CreateOccurenceSpecification(pairs));
        }

        return base.VisitObj(context);
    }

    private static OalClass CreateOalClass(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var nameContext = pairContexts.First(val => val.name() != null).name();
        var name = nameContext.value().GetText().Replace("\"", "");
        var oalClass = new OalClass() { Id = id, Name = name};
        return oalClass;
    }

    private static OalClassMethod CreateOalClassMethod(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var name = pairContexts.First(val => val.name() != null).name().value().GetText().Replace("\"", "");
        var receiveEventId = GetReceiverOccurrenceId(pairContexts);
        var sendEventId = GetSenderOccurrenceId(pairContexts);
        return new OalClassMethod()
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

    private static string GetXmiIdRef(SequenceDiagramParser.PairContext[] pairContexts)
    {
        if (pairContexts.Any(val => val.xmiIdRef() != null))
        {
            return pairContexts.First(val => val.xmiIdRef() != null).xmiIdRef().value().GetText().Replace("\"", "");
        }

        return "";
    }

    private static OalOccurrenceSpecification CreateOccurenceSpecification(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var coveredByContext =
            pairContexts.First(val => val.covered() != null).covered().value().arr().value()[0].obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", "");
        
        return new OalOccurrenceSpecification { Id = id, RefrenceIdOfCoveredObject = coveredByContext };
    }

    public override object VisitJson(SequenceDiagramParser.JsonContext context)
    {
        VisitChildren(context);
        OalProgram.SetOalClassesInMethods();
        OalProgram.SetCodeInClasses();
        OalProgram.SetProgramCode();

        return null;
    }
}