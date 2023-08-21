using AntlrDP.AnimArchAnimationClasses;

namespace AntlrDP;

public class OalProgram
{
    public List<OalClass> OalClasses = new();
    public List<OalClassMethod> OalClassMethods = new();

    public List<OalOccurrenceSpecification> OccurrenceSpecifications = new();
    public List<OalCombinedFragment> CombinedFragments = new();
    public List<OalInteractionOperand> InteractionOperands = new();
    public List<OalInteractionConstraint> InteractionConstraints = new();
    public List<OalOpaqueExpression> OpaqueExpressions = new();

    public string Code = "";

    public void SetOalClassesInMethods()
    {
        foreach (var classMethod in OalClassMethods)
        {
            SetSenderOalClassInMethod(classMethod);
            SetReceiverOalClassInMethod(classMethod);
        }

        SetConstraintsInMethods();
    }

    public void SetConstraintsInMethods()
    {
        foreach (var classMethod in OalClassMethods)
        {
            SetConstraintInMethod(classMethod);
        }
    }

    private void SetConstraintInMethod(OalClassMethod classMethod)
    {
        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == classMethod.SenderOccurrenceId);
        if (senderOccurrenceSpecification == null) return;

        var operand = InteractionOperands.Find(operand =>
            operand.Fragments.Any(fragment => fragment == senderOccurrenceSpecification.Id));
        if (operand == null) return;

        var constraint = InteractionConstraints.Find(constraint => constraint.Id == operand.GuardId);

        if (constraint == null) return;
        var opaque = OpaqueExpressions.Find(opaque => opaque.Id == constraint.SpecificationId);

        if (opaque == null) return;
        var body = opaque.Body;

        var combinedFragment = CombinedFragments.Find(combinedFragment =>
            combinedFragment.Operands.Any(fragmentOperand => fragmentOperand == operand.Id));

        if (combinedFragment == null) return;
        var methodConstraint = "";

        if (int.Parse(combinedFragment.InteractionOperatorId) == 7)
        {
            methodConstraint = "while (" + body + ")";
        }
        else if (int.Parse(combinedFragment.InteractionOperatorId) == 2)
        {
            var index = combinedFragment.Operands.FindIndex(fragmentOperand => fragmentOperand == operand.Id);
            if (index == 0)
            {
                methodConstraint = "if (" + body + ")";
            }
            else
            {
                methodConstraint = "else if (" + body + ")";
            }
        }

        classMethod.Constraints.Add(methodConstraint);
    }

    private void SetSenderOalClassInMethod(OalClassMethod classMethod)
    {
        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == classMethod.SenderOccurrenceId);
        if (senderOccurrenceSpecification == null) return;

        var senderOalClassId = senderOccurrenceSpecification.RefrenceIdOfCoveredObject;
        var senderOalClass = OalClasses.Find(oalClass => oalClass.Id == senderOalClassId);
        if (senderOalClass == null) return;
        classMethod.SenderOalClass = senderOalClass;
        senderOalClass.OalClassMethods.Add(classMethod);
    }

    private void SetReceiverOalClassInMethod(OalClassMethod classMethod)
    {
        var receiverOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == classMethod.ReceiverOccurrenceId);
        if (receiverOccurrenceSpecification == null) return;

        var receiverOalClassId = receiverOccurrenceSpecification.RefrenceIdOfCoveredObject;
        var receiverOalClass = OalClasses.Find(oalClass => oalClass.Id == receiverOalClassId);
        if (receiverOalClass == null) return;
        classMethod.ReceiverOalClass = receiverOalClass;
    }

    public void SetCodeInClasses()
    {
        foreach (var oalClass in OalClasses)
        {
            foreach (var oalClassMethod in oalClass.OalClassMethods)
            {
                var receiverClass = oalClassMethod.ReceiverOalClass;
                var classInstanceName = CreateNameOfClassInstance(receiverClass.Name);
                var classCreationCode = CreateCodeForCreationOfOalClass(receiverClass.Name);
                var oalClassMethodConstraints = oalClassMethod.Constraints;
                var methodCode = CreateMethodCall(classInstanceName, oalClassMethod.Name);

                methodCode = AddConstraintsToMethodCall(oalClassMethodConstraints, methodCode);

                oalClassMethod.Code += classCreationCode + methodCode;
            }
        }
    }

    private static string CreateCodeForCreationOfOalClass(string className)
    {
        var nameOfInstance = CreateNameOfClassInstance(className);
        return "create object instance " + nameOfInstance + " of " + className + ";\n";
    }

    private static string CreateNameOfClassInstance(string className)
    {
        return className + "_inst";
    }

    private static string CreateMethodCall(string nameOfClassInstance, string nameOfMethod)
    {
        return nameOfClassInstance + "." + nameOfMethod + "();\n";
    }

    private static string AddConstraintsToMethodCall(List<String> constraints, String methodCode)
    {
        var methodCodeWithConstraints = "";
        foreach (var constraint in constraints)
        {
            methodCodeWithConstraints += constraint + "\r\n" + methodCode;
            if (constraint.Contains("while"))
            {
                methodCodeWithConstraints += "\r\nend while;\n";
            }
            if (constraint.Contains("if"))
            {
                methodCodeWithConstraints += "\r\nend if;\n";
            }
            
        }
        return methodCodeWithConstraints;
    }

    public void SetProgramCode()
        {
            var code = "";
            foreach (var oalClass in OalClasses)
            {
                foreach (var oalClassOalClassMethod in oalClass.OalClassMethods)
                {
                    code += oalClassOalClassMethod.Code;
                }
            }

            Code = code;
        }

        public AnimArchAnimation CreateAnimArchAnimationObject()
        {
            var methodCodes = CreateAnimMethodsCodes();
            return new AnimArchAnimation()
            {
                Code = Code,
                MethodsCodes = methodCodes
            };
        }

        private List<MethodsCode> CreateAnimMethodsCodes()
        {
            var methodCodes = new List<MethodsCode>();
            foreach (var oalClass in OalClasses)
            {
                var methodCode = new MethodsCode
                {
                    Name = oalClass.Name
                };
                var animMethods = new List<Method>();
                foreach (var oalClassMethod in oalClass.OalClassMethods)
                {
                    var animMethod = new Method()
                    {
                        Name = oalClassMethod.Name,
                        Code = oalClassMethod.Code
                    };
                    animMethods.Add(animMethod);
                }

                methodCode.Methods = animMethods;
                if (animMethods.Count == 0)
                {
                    continue;
                }

                methodCodes.Add(methodCode);
            }

            return methodCodes;
        }
    }