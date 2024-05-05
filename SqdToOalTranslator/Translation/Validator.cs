using SqdToOalTranslator.PreOalCodeElements;

namespace SqdToOalTranslator.Translation;

// pomocna trieda na určenie validity sqd
public class SequenceDiagramValidator
{
    private readonly List<Class> _lifelines;
    private readonly List<MethodCall> _methodCalls;

    public SequenceDiagramValidator(List<Class> lifelines, List<MethodCall> methodCalls)
    {
        _lifelines = lifelines;
        _methodCalls = methodCalls;
    }

    public bool IsDiagramValid()
    {
        // Kontrola podmienky 3
        for (var i = 1; i < _methodCalls.Count; i++)
        {
            var currentCall = _methodCalls[i];
            var isFirstSenderMessage = _methodCalls.Take(i).All(mc => mc.SenderClass != currentCall.SenderClass);

            if (!isFirstSenderMessage) continue;

            var messageToCurrentSenderExists =
                _methodCalls.Take(i).Any(mc => mc.ReceiverClass == currentCall.SenderClass);
            if (!messageToCurrentSenderExists)
            {
                return false;
            }
        }

        // Kontrola podmienky 4
        for (var i = 0; i < _methodCalls.Count; i++)
        {
            var currentCall = _methodCalls[i];
            var senderIndex = _lifelines.IndexOf(currentCall.SenderClass);

            // Posledná správa smerujúca do aktuálnej lifeline, ktorá je vyššie v diagrame
            var lastMessageToCurrentSender =
                _methodCalls.Take(i).LastOrDefault(mc => mc.ReceiverClass == currentCall.SenderClass);

            if (lastMessageToCurrentSender == null) continue;
            var indexLastMessageToSender = _methodCalls.IndexOf(lastMessageToCurrentSender);

            // Pre správy medzi poslednou správou smerujúcou do aktuálnej životnej čiary a aktuálnou správou
            for (var j = indexLastMessageToSender + 1; j < i; j++)
            {
                var messageBetween = _methodCalls[j];
                var messageBetweenSenderIndex = _lifelines.IndexOf(messageBetween.SenderClass);

                // Ak je správa medzi odoslaná z životnej čiary, ktorá je naľavo od aktuálnej životnej čiary, tak false
                if (messageBetweenSenderIndex < senderIndex)
                {
                    return false;
                }
            }
        }

        return true;
    }
}