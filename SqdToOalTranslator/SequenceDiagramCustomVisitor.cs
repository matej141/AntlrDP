using SqdToOalTranslator.SequenceDiagramElements;

namespace SqdToOalTranslator;

public class SequenceDiagramCustomVisitor : SequenceDiagramBaseVisitor<object>
{
    public readonly SequenceDiagram SequenceDiagram = new();

    public override object VisitObj(SequenceDiagramParser.ObjContext context)
    {
        var pairs = context.pair();
        if (pairs == null)
        {
            return base.VisitObj(context);
        }
        
        var element = CreateElement(pairs);
        if (element == null) return base.VisitObj(context);
        SequenceDiagram.Elements.Add(element);
        AddElementToAppropriateCollection(element);
        
        return base.VisitObj(context);
    }

    private void AddElementToAppropriateCollection(SequenceDiagramElement element)
    {
        switch (element)
        {
            case Lifeline lifeline:
                SequenceDiagram.Lifelines.Add(lifeline);
                break;
            case Message message:
                SequenceDiagram.Messages.Add(message);
                break;
            case OccurrenceSpecification occurenceSpecification:
                SequenceDiagram.OccurenceSpecifications.Add(occurenceSpecification);
                break;
            case CombinedFragment combinedFragment:
                SequenceDiagram.CombinedFragments.Add(combinedFragment);
                break;
            case InteractionOperand interactionOperand:
                SequenceDiagram.InteractionOperands.Add(interactionOperand);
                break;
            case InteractionConstraint interactionConstraint:
                SequenceDiagram.InteractionConstraints.Add(interactionConstraint);
                break;
            case OpaqueExpression opaqueExpression:
                SequenceDiagram.OpaqueExpressions.Add(opaqueExpression);
                break;
        }
    }

    private SequenceDiagramElement? CreateElement(SequenceDiagramParser.PairContext[] pairs)
    {
        if (pairs.Any(val => val.lifeline() != null))
        {
            return CreateLifeline(pairs);
        }
        if (pairs.Any(val => val.message() != null))
        {
            return CreateMessage(pairs);
        }
        if (pairs.Any(val => val.occurenceSpecification() != null))
        {
            return CreateOccurenceSpecification(pairs);
        }
        if (pairs.Any(val => val.combinedFragment() != null))
        {
            return CreateCombinedFragment(pairs);
        }
        if (pairs.Any(val => val.interactionOperand() != null))
        {
            return CreateInteractionOperand(pairs);
        }
        if (pairs.Any(val => val.interactionConstraint() != null))
        {
            return CreateInteractionConstraint(pairs);
        }
        if (pairs.Any(val => val.opaqueExpression() != null))
        {
            return CreateOpaqueExpression(pairs);
        }

        return null;
    }

    private static Lifeline CreateLifeline(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var nameContext = pairContexts.First(val => val.name() != null).name();
        var name = nameContext.value().GetText().Replace("\"", "");
        return new Lifeline { XmiId = id, Name = name };
    }

    private static Message CreateMessage(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var name = pairContexts.First(val => val.name() != null).name().value().GetText().Replace("\"", "");
        var receiveEventId = GetReceiveEventOccurrenceId(pairContexts);
        var sentEventId = GetSentEventOccurrenceId(pairContexts);
        return new Message
            { XmiId = id, Name = name, ReceiverEventOccurenceId = receiveEventId, SenderEventOccurenceId = sentEventId };
    }

    private static string GetReceiveEventOccurrenceId(IEnumerable<SequenceDiagramParser.PairContext> pairContexts)
    {
        var receiveEventIdPairContexts =
            pairContexts.First(val => val.receiveEvent() != null).receiveEvent().value().obj().pair();

        return GetXmiIdRef(receiveEventIdPairContexts);
    }

    private static string GetSentEventOccurrenceId(IEnumerable<SequenceDiagramParser.PairContext> pairContexts)
    {
        var senderEventIdPairContexts =
            pairContexts.First(val => val.sendEvent() != null).sendEvent().value().obj().pair();
        return GetXmiIdRef(senderEventIdPairContexts);
    }

    private static string GetXmiIdRef(SequenceDiagramParser.PairContext[] pairContexts)
    {
        return pairContexts.Any(val => val.xmiIdRef() != null)
            ? pairContexts.First(val => val.xmiIdRef() != null).xmiIdRef().value().GetText().Replace("\"", "")
            : "";
    }

    private static OccurrenceSpecification CreateOccurenceSpecification(
        SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var coveredByContext =
            pairContexts.First(val => val.covered() != null).covered().value().arr().value()[0].obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", "");
        return new OccurrenceSpecification { XmiId = id, ReferenceIdOfCoveredObject = coveredByContext };
    }

    private static CombinedFragment CreateCombinedFragment(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var interactionOperator = pairContexts.First(val => val.interactionOperator() != null).interactionOperator()
            .value().GetText().Replace("\"", "");
        var operandsContext =
            pairContexts.First(val => val.operand() != null).operand().value().arr().value();
        var operands = new List<string>();
        foreach (var operand in operandsContext)
        {
            operands.Add(operand.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }

        return new CombinedFragment
            { XmiId = id, InteractionOperatorId = int.Parse(interactionOperator), OperandIds = operands };
    }

    private InteractionOperand CreateInteractionOperand(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var guardId = GetGuardId(pairContexts);
        var fragmentsContext =
            pairContexts.First(val => val.fragments() != null).fragments().value().arr().value();
        var fragments = new List<string>();
        foreach (var fragment in fragmentsContext)
        {
            fragments.Add(fragment.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }

        var ownedElementsContext =
            pairContexts.First(val => val.ownedElements() != null).ownedElements().value().arr().value();
        var ownedElements = new List<string>();
        foreach (var ownedElement in ownedElementsContext)
        {
            ownedElements.Add(ownedElement.obj().pair()[0]
                .xmiIdRef().value().GetText().Replace("\"", ""));
        }
        SequenceDiagram.IdsInOwnedElements.AddRange(ownedElements);
        return new InteractionOperand
            { XmiId = id, InteractionConstraintId = guardId, Fragments = fragments, OwnedElements = ownedElements };
    }

    private static string GetGuardId(IEnumerable<SequenceDiagramParser.PairContext> pairContexts)
    {
        var guardIdPairContexts =
            pairContexts.First(val => val.guard() != null).guard().value().obj().pair();
        return GetXmiIdRef(guardIdPairContexts);
    }

    private static InteractionConstraint CreateInteractionConstraint(
        SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var specificationId = GetSpecificationId(pairContexts);

        return new InteractionConstraint { XmiId = id, SpecificationId = specificationId };
    }

    private static string GetSpecificationId(IEnumerable<SequenceDiagramParser.PairContext> pairContexts)
    {
        var guardIdPairContexts =
            pairContexts.First(val => val.specification() != null).specification().value().obj().pair();
        return GetXmiIdRef(guardIdPairContexts);
    }

    private static OpaqueExpression CreateOpaqueExpression(SequenceDiagramParser.PairContext[] pairContexts)
    {
        var id = pairContexts.First(val => val.xmiId() != null).xmiId().value().GetText().Replace("\"", "");
        var body = pairContexts.First(val => val.body() != null).body().value().GetText().Replace("\"", "");

        return new OpaqueExpression { XmiId = id, Body = body };
    }
}