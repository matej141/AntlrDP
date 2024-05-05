namespace SqdToOalTranslator.Translation;

public class TranslationContext
{
    public string MethodName { get; set; }
    public TranslationClass Sender { get; set; }
    public TranslationClass Receiver { get; set; }
    public string CurrentCode { get; set; }
    public List<TranslationClass> Instances = new();
    public string LastMethodCalled { get; set; }
    public TranslationClass LastReceiver = new();

    public TranslationContext()
    {
    }

    public TranslationContext(TranslationContext original)
    {
        MethodName = original.MethodName;
        Sender = original.Sender;
        Receiver = original.Receiver;
        CurrentCode = original.CurrentCode;
        Instances = new List<TranslationClass>(original.Instances);
        LastMethodCalled = original.LastMethodCalled;
    }
}