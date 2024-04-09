using AntlrDP.OalCodeElements.FragmentTypes;
using AntlrDP.SequenceDiagramElements;

namespace AntlrDP.OalCodeElements;

public class OalCode
{
    public readonly List<OalCodeElement> CodeElements = new();
    private readonly List<string> _idsInStatements = new();
    public readonly List<Class> Classes = new();
    public readonly List<MethodCall> MethodCalls = new();
    private SequenceDiagram SequenceDiagram { get; }

    public OalCode(SequenceDiagram sequenceDiagram)
    {
        SequenceDiagram = sequenceDiagram;
        ProcessSequenceDiagramElements();
    }

    private void ProcessSequenceDiagramElements()
    {
        ProcessLifelines();
        foreach (var element in SequenceDiagram.Elements)
        {
            switch (element)
            {
                case Message message:
                {
                    ProcessMessage(message);
                    continue;
                }
                case CombinedFragment combinedFragment:
                {
                    ProcessCombinedFragment(combinedFragment);
                    continue;
                }
            }
        }
    }

    private void ProcessMessage(Message message)
    {
        if (SequenceDiagram.IdsInOwnedElements.Contains(message.XmiId) ||
            _idsInStatements.Contains(message.XmiId))
        {
            return;
        }

        var methodCall = CreateMethodCall(message);

        CodeElements.Add(methodCall);
        MethodCalls.Add(methodCall);
    }
    
    private void ProcessCombinedFragment(CombinedFragment combinedFragment)
    {
        if (SequenceDiagram.IdsInOwnedElements.Contains(combinedFragment.XmiId) ||
            _idsInStatements.Contains(combinedFragment.XmiId))
        {
            return;
        }

        var fragments = CreateStatement(combinedFragment);
        CodeElements.AddRange(fragments);
    }

    private void ProcessLifelines()
    {
        foreach (var lifeline in SequenceDiagram.Lifelines)
        {
            var oalCodeClass = new Class { Id = lifeline.XmiId, Body = lifeline.Name };
            CodeElements.Add(oalCodeClass);
            Classes.Add(oalCodeClass);
        }
    }

    private IEnumerable<Statement> CreateStatement(CombinedFragment combinedFragment)
    {
        var statementList = new List<Statement>();
        foreach (var operandId in combinedFragment.OperandIds)
        {
            var interactionOperand =
                SequenceDiagram.InteractionOperands.Find(operand => operand.XmiId == operandId);
            var interactionConstraint = SequenceDiagram.InteractionConstraints.Find(constraint =>
                interactionOperand != null && constraint.XmiId == interactionOperand.InteractionConstraintId);
            var opaqueExpr =
                SequenceDiagram.OpaqueExpressions.Find(expr =>
                    interactionConstraint != null && expr.XmiId == interactionConstraint.SpecificationId);
            if (opaqueExpr == null || interactionOperand == null) continue;
            var statementType = GetStatementType(combinedFragment.InteractionOperatorId,
                opaqueExpr.Body,
                combinedFragment.OperandIds.FindIndex(id => id == operandId));

            var statement = new Statement
            {
                Id = interactionOperand.XmiId, Body = opaqueExpr.Body, StatementType = statementType,
                StatementElements = new List<OalCodeElement>(),
                IsFirst = combinedFragment.OperandIds.First( )== operandId,
                IsLast = combinedFragment.OperandIds.Last() == operandId || statementType is WhileStatement
            };

            ProcessOwnedElements(interactionOperand, statement);

            statementList.Add(statement);
        }

        return statementList;
    }

    private void ProcessOwnedElements(InteractionOperand interactionOperand, Statement statement)
    {
        foreach (var ownedElementId in interactionOperand.OwnedElements)
        {
            var ownedElement =
                SequenceDiagram.Elements.Find(sqdElement => sqdElement.XmiId == ownedElementId);
            if (ownedElement is OccurenceSpecification occurenceSpecification)
            {
                var ownedMessage = SequenceDiagram.Messages.Find(sqdMessage =>
                    sqdMessage.SenderEventOccurenceId == occurenceSpecification.XmiId);
                if (ownedMessage != null)
                {
                    var methodCall = CreateMethodCall(ownedMessage);
                    statement.StatementElements.Add(methodCall);
                    _idsInStatements.Add(methodCall.Id);
                    MethodCalls.Add(methodCall);
                }
                else
                {
                    continue;
                }
            }

            if (ownedElement is not CombinedFragment ownedCombinedFragment)
            {
                continue;
            }

            var anotherStatements = CreateStatement(ownedCombinedFragment);
            statement.StatementElements.AddRange(anotherStatements);

            var ids = statement.StatementElements.Select(o => o.Id).ToList();
            _idsInStatements.AddRange(ids);
        }
    }

    private MethodCall CreateMethodCall(Message message)
    {
        var method = new MethodCall { Id = message.XmiId, Body = message.Name };
        var receiverClassId = SequenceDiagram.OccurenceSpecifications.Find(spec =>
            spec.XmiId == message.ReceiverEventOccurenceId)?.ReferenceIdOfCoveredObject;
        var receiverClass = Classes.Find(oalCodeClass => oalCodeClass.Id == receiverClassId);
        if (receiverClass != null)
        {
            method.ReceiverClass = receiverClass;
        }

        var senderClassId = SequenceDiagram.OccurenceSpecifications.Find(spec =>
            spec.XmiId == message.SenderEventOccurenceId)?.ReferenceIdOfCoveredObject;
        var senderClass = Classes.Find(oalCodeClass => oalCodeClass.Id == senderClassId);
        if (senderClass != null)
        {
            method.SenderClass = senderClass;
        }

        return method;
    }

    private static StatementType GetStatementType(int interactionOperator, string body, int index)
    {
        switch (interactionOperator)
        {
            case 7:
                if (body.Contains("for"))
                {
                    return new ForStatement();
                }
                return new WhileStatement();
            case 5:
                return new ParStatement();
            case 2 when index == 0:
            case 3:
                return new IfStatement();
            case 2 when body == "else":
                return new ElseStatement();
            default:
                return new ElifStatement();
        }
    }
}