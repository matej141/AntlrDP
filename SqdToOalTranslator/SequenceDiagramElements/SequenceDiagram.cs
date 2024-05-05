namespace SqdToOalTranslator.SequenceDiagramElements;

public class SequenceDiagram
{
    public readonly List<SequenceDiagramElement> Elements = new();
    public readonly List<Lifeline> Lifelines = new();
    public readonly List<Message> Messages = new();
    public readonly List<OccurrenceSpecification> OccurenceSpecifications = new();
    public readonly List<CombinedFragment> CombinedFragments = new();
    public readonly List<InteractionOperand> InteractionOperands = new();
    public readonly List<InteractionConstraint> InteractionConstraints = new();
    public readonly List<OpaqueExpression> OpaqueExpressions = new();
    public readonly List<string> IdsInOwnedElements = new();
}