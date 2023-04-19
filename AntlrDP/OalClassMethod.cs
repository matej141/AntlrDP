namespace AntlrDP;

public class OalClassMethod
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string SenderOccurrenceId { get; set; }
    public string ReceiverOccurrenceId { get; set; }
    public OalClass SenderOalClass { get; set; }
    public OalClass ReceiverOalClass { get; set; }
}