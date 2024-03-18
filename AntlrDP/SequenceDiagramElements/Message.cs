namespace AntlrDP.SequenceDiagramElements;

public class Message : SequenceDiagramElement
{
    public string XmiId { get; set; }
    public string Name { get; set; }
    public string ReceiverEventOccurenceId { get; set; }
    public string SenderEventOccurenceId { get; set; }
}