namespace AntlrDP.SequenceDiagramElements;

public class CombinedFragment : SequenceDiagramElement
{
    public int InteractionOperatorId { get; set; }
    public List<string> OperandIds { get; set; }
}