namespace AntlrDP.SequenceDiagramElements;

public class InteractionOperand : SequenceDiagramElement
{
    public string InteractionConstraintId { get; set; }
    public List<string> Fragments { get; set; }
    public List<string> OwnedElements { get; set; }
}