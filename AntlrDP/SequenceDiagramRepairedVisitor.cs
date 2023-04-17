using Antlr4.Runtime.Tree;

namespace AntlrDP;

public class SequenceDiagramRepairedVisitor : SequenceDiagramBaseVisitor<object>
{
    public readonly List<SequenceJsonAttempt> Attempts = new List<SequenceJsonAttempt>();
    public SequenceJsonAttempt Attempt = new();
    public string Message = "";
    public OalClass OalClass = new();
    public List<OalClass> OalClasses = new();
    public OalProgram OalProgram = new();

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
        Attempts.Add(Attempt);
        return base.VisitPair(context);
    }

    public override object VisitName(SequenceDiagramParser.NameContext context)
    {
        Message = "Hello " + context.value().GetText().Replace("\"", "");
        return base.VisitName(context);
    }

    public override object VisitObj(SequenceDiagramParser.ObjContext context)
    {
        var pair = context.pair();
        if (pair == null)
        {
            return base.VisitObj(context);
        }

        if (pair.Any(val => val.lifeline() != null))
        {
            OalClass = CreateOalClass(pair);
            OalClasses.Add(CreateOalClass(pair));
            var oalClass = CreateOalClass(pair);
            OalProgram.Classes.Add(oalClass);
        }

        if (pair.Any(val => val.message() != null))
        {
            OalProgram.ClassMethods.Add(CreateOalClassMethod(pair));
        }

        if (pair.Any(val => val.occurenceSpecification() != null))
        {
        }

        return base.VisitObj(context);
    }

    private static OalClass CreateOalClass(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var nameContext = pairContexts.First(val => val.name() != null).name();
        var name = nameContext.value().GetText().Replace("\"", "");
        var oalClass = new OalClass() { Id = id, Name = name, Code = CreateCodeForCreationOfOalClass(name) };
        return oalClass;
    }

    private OalClassMethod CreateOalClassMethod(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var name = pairContexts.First(val => val.name() != null).name().value().GetText().Replace("\"", "");
        var receiveEventId = GetReceiverOccurrenceId(pairContexts);
        var sendEventId = GetSenderOccurrenceId(pairContexts);
        return new OalClassMethod()
            { Name = name, ReceiverOccurrenceId = receiveEventId, SenderOccurrenceId = sendEventId };
    }

    private string GetReceiverOccurrenceId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var receiveEventIdPairContexts =
            pairContexts.First(val => val.receiveEvent() != null).receiveEvent().value().obj().pair();

        return GetXmiIdRef(receiveEventIdPairContexts);
    }

    private string GetSenderOccurrenceId(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var senderEventIdPairContexts =
            pairContexts.First(val => val.sendEvent() != null).sendEvent().value().obj().pair();
        return GetXmiIdRef(senderEventIdPairContexts);
    }

    private string GetXmiIdRef(SequenceDiagramParser.PairContext[] pairContexts)
    {
        if (pairContexts.Any(val => val.xmiIdRef() != null))
        {
            return pairContexts.First(val => val.xmiIdRef() != null).xmiIdRef().value().GetText().Replace("\"", "");
        }

        return "";
    }

    private static string CreateCodeForCreationOfOalClass(String className)
    {
        var nameOfInstance = className + "_inst";
        return "create object instance " + nameOfInstance + " of " + className + ";\n";
    }
}