using System.Collections;
using AntlrDP.AnimArchAnimationClasses;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


namespace AntlrDP;

public class OalProgram
{
    public List<OalClass> OalClasses = new();
    public List<OalMethodCall> OalMethodCalls = new();
    public List<OalClassMethod> OalClassMethods = new();

    public List<OalOccurrenceSpecification> OccurrenceSpecifications = new();
    public List<OalCombinedFragment> CombinedFragments = new();
    public List<OalInteractionOperand> InteractionOperands = new();
    public List<OalInteractionConstraint> InteractionConstraints = new();
    public List<OalOpaqueExpression> OpaqueExpressions = new();
    public Boolean IsDiagramValid = true;

    public string Code = "";

    public void SetOalClassesInMethodCalls()
    {
        foreach (var classMethod in OalMethodCalls)
        {
            SetSenderOalClassInMethodCall(classMethod);
            SetReceiverOalClassInMethodCall(classMethod);
        }

        // SetConstraintsInMethods();
        CreateMethods();
    }
    
    // refactor is needed
    public void CreateMethods()
    {
        var name = "FirstMethod";
        var method = new OalClassMethod();
        var creations = new List<string>();
        var creationsInsideConstraint = new List<string>();
        var creationsInsideAlt = new List<string>();
        var workingMethodCode = "";
        var senderClass = new OalClass();
        var constraints = new List<string>();
        var wholeCode = "";
        var methodCallsInAlt = new List<string>();
        foreach (var oalMethodCall in OalMethodCalls)
        {
            var currentIndex = OalMethodCalls.IndexOf(oalMethodCall);
            // prve volanie v poradi
            if (currentIndex == 0)
            {
                senderClass = HandleSenderOalMethod(oalMethodCall, creations, constraints, creationsInsideConstraint,
                    methodCallsInAlt, creationsInsideAlt,
                    ref workingMethodCode);

                HandleReceiverOalMethod(oalMethodCall);
            }
            // ine ako prve volanie v poradi...
            else if (currentIndex > 0 &&
                     OalMethodCalls[currentIndex - 1].SenderOalClass == oalMethodCall.SenderOalClass)
            {
                var combinedFragment = FindCombinedFragmentForSendingElement(oalMethodCall.SenderOccurrenceId);
                var constraint = FindConstraintForSendingElement(oalMethodCall.SenderOccurrenceId);
                // nasla sa podmienka a nejedna sa o vnoreny fragment
                if (constraint != "" && !IsNestedFragment(combinedFragment?.Id))
                {
                    wholeCode += workingMethodCode;

                    // ak doteraz nepouzita podmienka
                    if (!constraints.Contains(constraint))
                    {
                        wholeCode = AddEndStatements(wholeCode, constraint);
                        workingMethodCode = constraint + "\n";
                        creationsInsideConstraint.Clear();
                        creationsInsideAlt.Clear();
                        constraints.Add(constraint);
                        if ((constraints.Last().Contains("if") || constraints.Last().Contains("elif")) &&
                            (!constraint.Contains("elif") || constraint.Contains("while")))
                        {
                            methodCallsInAlt.Clear();
                        }
                    }
                    else if (constraints.Contains(constraint)
                             && constraints.Last() != constraint)
                    {
                        // ak uz bola pouzita podmienka, uzatvorime predoslu podmienku
                        // situacia -> while -> vnoreny alt -> message po vnorenom alte,
                        // avsak musime zamedzit situacii, ked sa jedna o alt s viac spravami
                        if (workingMethodCode.Contains("elif"))
                        {
                            wholeCode += "end if;\n";
                        }
                        else if (workingMethodCode.Contains("if"))
                        {
                            wholeCode += "end if;\n";
                        }
                        else if (workingMethodCode.Contains("while"))
                        {
                            wholeCode += "end while;\n";
                        }
                        else if (workingMethodCode.Contains("else"))
                        {
                            wholeCode += "end if;\n";
                        }

                        workingMethodCode = "";
                    }
                    else
                    {
                        workingMethodCode = "";
                        // ak je if s viacerymi spravami, potrebujeme len posledne volanie
                        if (methodCallsInAlt.Count > 0)
                        {
                            methodCallsInAlt.RemoveAt(methodCallsInAlt.Count - 1);
                        }
                    }
                }
                // ak sa nasla podmienka a jedna sa o vnreny fragment
                else if (constraint != "" && IsNestedFragment(combinedFragment?.Id))
                {
                    // kontrolujeme, ci nahodou predosly constraint nebol if alebo elif...
                    if (constraints.Last().Contains("elif") || constraints.Last().Contains("if"))
                    {
                        // creationsInsideConstraint.Clear();
                        creationsInsideAlt.Clear();
                    }

                    var previousFragment =
                        FindCombinedFragmentForSendingElement(OalMethodCalls[currentIndex - 1].SenderOccurrenceId);
                    bool areEqualUnordered = finalParentOperand(oalMethodCall.SenderOccurrenceId, previousFragment?.Id);

                    if (areEqualUnordered)
                    {
                        if (constraints.Last().Contains("elif"))
                        {
                            workingMethodCode += "end if;\n";
                        }
                        else if (constraints.Last().Contains("if"))
                        {
                            workingMethodCode += "end if;\n";
                            constraints.RemoveAt(constraints.Count - 1);
                        }
                        else if (constraints.Last().Contains("while"))
                        {
                            workingMethodCode += "end while;\n";
                        }
                        else if (constraints.Last().Contains("else"))
                        {
                            workingMethodCode += "end if;\n";
                        }
                    }

                    else
                    {
                        workingMethodCode += constraint + "\n";
                        constraints.Add(constraint);
                    }
                }

                else
                {
                    // inak pridame a uzatvorime pridadne podmienky
                    wholeCode += workingMethodCode;
                    wholeCode = AddEndStatements(wholeCode);
                    workingMethodCode = "";
                }

                var classInstance = "self";
                // nejedna sa o self message
                if (oalMethodCall.ReceiverOalClass != oalMethodCall.SenderOalClass)
                {
                    // ak instancia triedy nebola vytvorena a nebola pouzita podmienka
                    if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint == "")
                    {
                        workingMethodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                        creations.Add(oalMethodCall.ReceiverOalClass.Id);
                    }
                    // ak instancia triedy nebola vytvorena, sme v podmienke a v ramci
                    // podmienky nebolo pouzite toto vytvorenie (ked mame if a elif, tak vytvorene instancia platia
                    // len v ramci danej vetvy podmienky)
                    else if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint != ""
                             && !constraint.Contains("if")
                             && !constraint.Contains("else") &&
                             !creationsInsideConstraint.Contains(oalMethodCall.ReceiverOalClass.Id))
                    {
                        workingMethodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                        creationsInsideConstraint.Add(oalMethodCall.ReceiverOalClass.Id);
                    }

                    else if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint != ""
                             && !creationsInsideConstraint.Contains(oalMethodCall.ReceiverOalClass.Id)
                             &&
                             !creationsInsideAlt.Contains(oalMethodCall.ReceiverOalClass.Id))
                    {
                        workingMethodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                        creationsInsideAlt.Add(oalMethodCall.ReceiverOalClass.Id);
                    }

                    classInstance = CreateNameOfClassInstance(oalMethodCall.ReceiverOalClass.Name);
                }

                var methodCall = CreateMethodCall(classInstance,
                    oalMethodCall.Name);
                // ak sa jedna o alt, tak pri pripadnom zmene toku volani
                // bude potrebne replikovat kod
                if (constraint.Contains("if") || constraint.Contains("elif"))
                {
                    methodCallsInAlt.Add(oalMethodCall.Name);
                }

                workingMethodCode += methodCall;

                HandleReceiverOalMethod(oalMethodCall);
            }
            // ked sender je iny ako predosly, zacneme vytvarat volania z inych tried
            else
            {
                var constraint = FindConstraintForSendingElement(oalMethodCall.SenderOccurrenceId);
                var combinedFragment = FindCombinedFragmentForSendingElement(oalMethodCall.SenderOccurrenceId);
                var usedConstraint = constraints.Contains(constraint);
                var isNested = IsNestedFragment(combinedFragment?.Id);
                if (constraint.Contains("elif") || constraint.Contains("else"))
                {
                    IsDiagramValid = false;
                    return;
                }

                // vymazeme predoslu metodu (budeme ju vytvarat odznovu) - bude lepsie upravovat metody

                wholeCode += workingMethodCode;
                if (!usedConstraint || isNested)
                {
                    wholeCode = AddEndStatements(wholeCode);
                }

                // ulozime
                SaveSenderMethod(method, name, wholeCode, constraints, senderClass);
                var sameReceiverMethod = OalMethodCalls.GetRange(0, currentIndex + 1)
                    .Find(it => it.ReceiverOalClass == oalMethodCall.SenderOalClass);
                if (sameReceiverMethod == null)
                {
                    // kontrolujeme pripad, ked by metoda mala pokracovat v jednej zo svojich predoslych metod
                    var oldMethod = OalClassMethods.FindLast(it => it.SenderOalClass == oalMethodCall.SenderOalClass);
                    if (oldMethod == null)
                    {
                        continue;
                    }

                    var index = OalClassMethods.IndexOf(oldMethod);

                    if (index == -1)
                    {
                        continue;
                    }

                    var senderOalClass = OalClasses.Find(oalClass => oalClass == oalMethodCall.SenderOalClass);
                    if (senderOalClass == null)
                    {
                        continue;
                    }

                    var indexInClass = senderOalClass.OalClassMethods.IndexOf(oldMethod);

                    if (indexInClass == -1)
                    {
                        continue;
                    }

                    ClearMethodCreateData(currentIndex, creations, constraints, out method, out workingMethodCode,
                        out wholeCode, usedConstraint, isNested);
                    name = oldMethod.Name;
                    workingMethodCode = oldMethod.Code;
                    senderClass = HandleSenderOalMethod(oalMethodCall, creations, constraints,
                        creationsInsideConstraint,
                        methodCallsInAlt, creationsInsideAlt,
                        ref workingMethodCode);

                    HandleReceiverOalMethod(oalMethodCall);
                    continue;
                }

                // if (ClearPreviousMethod(currentIndex)) continue;
                // vycistime predosle pouzivane atributy a ziskame meno novej metody
                name = ClearMethodCreateData(currentIndex, creations, constraints, out method, out workingMethodCode,
                    out wholeCode, usedConstraint, isNested);

                senderClass = HandleSenderOalMethod(oalMethodCall, creations, constraints, creationsInsideConstraint,
                    methodCallsInAlt, creationsInsideAlt,
                    ref workingMethodCode);

                HandleReceiverOalMethod(oalMethodCall);
            }
        }

        // uzatvorime vsetko
        wholeCode += workingMethodCode;
        wholeCode = AddEndStatements(wholeCode);
        foreach (var methodName in methodCallsInAlt)
        {
            var newMethod = new OalClassMethod
            {
                Name = methodName,
                Code = wholeCode
            };
            var oldMethod = OalClassMethods.Find(it => it.Name == methodName);
            // nahradit metodu v OalClassMethods a v metodach danej classy
            if (oldMethod == null)
            {
                continue;
            }

            var index = OalClassMethods.IndexOf(oldMethod);

            if (index == -1)
            {
                continue;
            }

            OalClassMethods.RemoveAt(index);
            OalClassMethods.Insert(index, newMethod);
            var senderOalClass = OalClasses.Find(oalClass => oalClass.Id == senderClass.Id);
            var indexClass = (int)senderOalClass?.OalClassMethods.IndexOf(oldMethod)!;
            if (indexClass == -1)
            {
                continue;
            }

            senderOalClass?.OalClassMethods.RemoveAt(indexClass);
            senderOalClass?.OalClassMethods.Insert(indexClass, newMethod);
        }

        // ulozime metodu
        SaveSenderMethod(method, name, wholeCode, constraints, senderClass);
    }

    private string AddEndStatements(string input, string constraint = "")
    {
        List<string> statements = new List<string>();
        Stack<string> controlFlowStack = new Stack<string>();

        string[] lines = input.Split('\n');

        foreach (string line in lines)
        {
            if (line == "")
            {
                continue;
            }

            string keyword = Regex.Match(line, @"^\s*(\w+)\s*").Groups[1].Value;

            statements.Add(line);

            var keywords = new[] { "if", "while", "for" };
            if (constraint.Contains("elif") || constraint.Contains("else"))
            {
                keywords = new[] { "while", "for" };
            }

            if (keywords.Contains(keyword))
            {
                controlFlowStack.Push(keyword);
            }
            else if (keyword == "end")
            {
                controlFlowStack.Pop();
            }
        }

        while (controlFlowStack.Count > 0)
        {
            string lastControlFlow = controlFlowStack.Pop();
            statements.Add($"end {lastControlFlow};");
        }

        statements.Add("");
        return string.Join("\n", statements);
    }

    private OalClass HandleSenderOalMethod(OalMethodCall oalMethodCall, List<string> creations,
        List<string> constraints, List<string> creationsInsideConstraint, List<string> methodCallsInAlt,
        List<string> creationsInsideAlt,
        ref string methodCode)
    {
        var senderClass = oalMethodCall.SenderOalClass;
        var constraint = FindConstraintForSendingElement(oalMethodCall.SenderOccurrenceId);

        if (constraint != "" && !constraints.Contains(constraint))
        {
            methodCode += constraint + "\n";
            constraints.Add(constraint);
            if (constraint.Contains("if") || constraint.Contains("elif"))
            {
                methodCallsInAlt.Add(oalMethodCall.Name);
            }
        }

        var classInstance = "self";
        if (oalMethodCall.ReceiverOalClass != oalMethodCall.SenderOalClass)
        {
            if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint == "")
            {
                methodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                creations.Add(oalMethodCall.ReceiverOalClass.Id);
            }
            // ak instancia triedy nebola vytvorena, sme v podmienke a v ramci
            // podmienky nebolo pouzite toto vytvorenie (ked mame if a elif, tak vytvorene instancia platia
            // len v ramci danej vetvy podmienky)
            else if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint != ""
                                                                            && !constraint.Contains("if")
                                                                            && !constraint.Contains("else") &&
                                                                            !creationsInsideConstraint.Contains(
                                                                                oalMethodCall.ReceiverOalClass.Id))
            {
                methodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                creationsInsideConstraint.Add(oalMethodCall.ReceiverOalClass.Id);
            }

            else if (!creations.Contains(oalMethodCall.ReceiverOalClass.Id) && constraint != ""
                                                                            && !creationsInsideConstraint.Contains(
                                                                                oalMethodCall.ReceiverOalClass.Id)
                                                                            &&
                                                                            !creationsInsideAlt.Contains(oalMethodCall
                                                                                .ReceiverOalClass.Id))
            {
                methodCode += CreateCodeForCreationOfOalClass(oalMethodCall.ReceiverOalClass.Name);
                creationsInsideAlt.Add(oalMethodCall.ReceiverOalClass.Id);
            }

            classInstance = CreateNameOfClassInstance(oalMethodCall.ReceiverOalClass.Name);
        }

        var methodCall = CreateMethodCall(classInstance,
            oalMethodCall.Name);
        methodCode += methodCall;

        return senderClass;
    }

    private OalCombinedFragment? FindCombinedFragmentForSendingElement(string id)
    {
        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == id);
        if (senderOccurrenceSpecification == null) return null;

        var operand = InteractionOperands.Find(operand =>
            operand.Fragments.Any(fragment => fragment == senderOccurrenceSpecification.Id));
        if (operand == null) return null;

        var combinedFragment = CombinedFragments.Find(combinedFragment =>
            combinedFragment.Operands.Any(fragmentOperand => fragmentOperand == operand.Id));

        return combinedFragment ?? null;
    }

    private bool IsNestedFragment(string? id)
    {
        if (id == null)
        {
            return false;
        }

        var operand = InteractionOperands.Find(operand =>
            operand.OwnedElements.Any(fragment => fragment == id));
        return operand != null;
    }

    private Boolean finalParentOperand(string? idParent, string? idChild)
    {
        if (idParent == null || idChild == null)
        {
            return false;
        }

        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == idParent);
        if (senderOccurrenceSpecification == null) return false;

        var operands = InteractionOperands.FindAll(operand =>
            operand.Fragments.Any(fragment => fragment == senderOccurrenceSpecification.Id));
        if (operands == null) return false;

        var operandParents = InteractionOperands.FindAll(operand =>
            operand.OwnedElements.Any(fragment => fragment == idParent));
        return operands.Any(operand => operand.OwnedElements.Contains(idChild));
    }

    private int FragmentOperandIndex(OalCombinedFragment combinedFragment, string operandId)
    {
        return combinedFragment.Operands.FindIndex(fragmentOperand => fragmentOperand == operandId);
    }

    // pokus najst parent of nested fagment
    private string FindConstraintForAnotherConstraint(string id)
    {
        var methodConstraint = "";

        var operand = InteractionOperands.Find(operand =>
            operand.OwnedElements.Any(fragment => fragment == id));
        if (operand == null) return methodConstraint;
        if (operand.OwnedElements[0] != id) return methodConstraint;

        var constraint = InteractionConstraints.Find(constraint => constraint.Id == operand.GuardId);

        if (constraint == null) return methodConstraint;
        var opaque = OpaqueExpressions.Find(opaque => opaque.Id == constraint.SpecificationId);

        if (opaque == null) return methodConstraint;
        var body = opaque.Body;

        var combinedFragment = CombinedFragments.Find(combinedFragment =>
            combinedFragment.Operands.Any(fragmentOperand => fragmentOperand == operand.Id));

        if (combinedFragment == null) return methodConstraint;
        if (IsNestedFragment(combinedFragment.Id) && FragmentOperandIndex(combinedFragment, operand.Id) == 0)
        {
            var anotherConstraint = FindConstraintForAnotherConstraint(combinedFragment.Id);
            if (anotherConstraint != "")
            {
                methodConstraint += anotherConstraint + "\n";
            }
        }

        if (int.Parse(combinedFragment.InteractionOperatorId) == 7)
        {
            methodConstraint += "while (" + body + ")";
        }
        else if (int.Parse(combinedFragment.InteractionOperatorId) == 2)
        {
            var index = FragmentOperandIndex(combinedFragment, operand.Id);
            if (index == 0)
            {
                methodConstraint += "if (" + body + ")";
            }
            else if (body == "else")
            {
                methodConstraint += body;
            }
            else
            {
                methodConstraint += "elif (" + body + ")";
            }
        }

        return methodConstraint;
    }

    private string FindConstraintForSendingElement(string id)
    {
        var methodConstraint = "";
        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == id);
        if (senderOccurrenceSpecification == null) return methodConstraint;

        var operand = InteractionOperands.Find(operand =>
            operand.Fragments.Any(fragment => fragment == senderOccurrenceSpecification.Id));
        if (operand == null) return methodConstraint;

        var constraint = InteractionConstraints.Find(constraint => constraint.Id == operand.GuardId);

        if (constraint == null) return methodConstraint;
        var opaque = OpaqueExpressions.Find(opaque => opaque.Id == constraint.SpecificationId);

        if (opaque == null) return methodConstraint;
        var body = opaque.Body;

        var combinedFragment = CombinedFragments.Find(combinedFragment =>
            combinedFragment.Operands.Any(fragmentOperand => fragmentOperand == operand.Id));

        if (combinedFragment == null) return methodConstraint;
        if (IsNestedFragment(combinedFragment.Id) && FragmentOperandIndex(combinedFragment, operand.Id) == 0)
        {
            var anotherConstraint = FindConstraintForAnotherConstraint(combinedFragment.Id);
            if (anotherConstraint != "")
            {
                methodConstraint += anotherConstraint + "\n";
            }
        }

        if (int.Parse(combinedFragment.InteractionOperatorId) == 7)
        {
            methodConstraint += "while (" + body + ")";
        }
        else if (int.Parse(combinedFragment.InteractionOperatorId) == 2 ||
                 int.Parse(combinedFragment.InteractionOperatorId) == 3)
        {
            var index = FragmentOperandIndex(combinedFragment, operand.Id);
            if (index == 0)
            {
                methodConstraint += "if (" + body + ")";
            }
            else if (body == "else")
            {
                methodConstraint += body;
            }
            else
            {
                methodConstraint += "elif (" + body + ")";
            }
        }

        return methodConstraint;
    }

    private string ClearMethodCreateData(int currentIndex, List<string> creations,
        List<string> constraints, out OalClassMethod method,
        out string methodCode, out string wholeMethodCode, bool usedConstraint, bool nested)
    {
        string name;
        name = OalMethodCalls[currentIndex - 1].Name;
        method = new OalClassMethod();
        if (!usedConstraint && !nested)
        {
            creations.Clear();
            constraints.Clear();
        }

        methodCode = "";
        wholeMethodCode = "";
        return name;
    }

    private void SaveSenderMethod(OalClassMethod method, string name, string methodCode,
        List<string> constraints, OalClass senderClass)
    {
        method.Name = name;

        method.Code = methodCode;
        method.SenderOalClass = senderClass;
        if (ReplaceOldMethodInMethods(method, senderClass))
        {
            return;
        }

        OalClassMethods.Add(method);
        var senderOalClass = OalClasses.Find(oalClass => oalClass.Id == senderClass.Id);
        senderOalClass?.OalClassMethods.Add(method);
    }

    private bool ReplaceOldMethodInMethods(OalClassMethod method, OalClass senderClass)
    {
        var oldMethod = OalClassMethods.Find(it => it.Name == method.Name);
        // nahradit metodu v OalClassMethods a v metodach danej classy
        if (oldMethod == null)
        {
            return false;
        }

        var index = OalClassMethods.IndexOf(oldMethod);

        if (index == -1)
        {
            return false;
        }

        var senderOalClass = OalClasses.Find(oalClass => oalClass.Id == senderClass.Id);
        if (senderOalClass == null)
        {
            return false;
        }

        var indexInClass = senderOalClass.OalClassMethods.IndexOf(oldMethod);

        if (indexInClass == -1)
        {
            return false;
        }

        OalClassMethods.RemoveAt(index);
        OalClassMethods.Insert(index, method);
        senderOalClass.OalClassMethods.RemoveAt(indexInClass);
        senderOalClass.OalClassMethods.Insert(indexInClass, method);
        return true;
    }

    private bool ClearPreviousMethod(int currentIndex)
    {
        var previousMethodCall = OalMethodCalls[currentIndex - 1];
        var recOalClassOfPrevMethodCall =
            OalClasses.Find(oalClass => oalClass.Id == previousMethodCall.ReceiverOalClass.Id);
        if (recOalClassOfPrevMethodCall == null)
        {
            return true;
        }

        if (recOalClassOfPrevMethodCall.OalClassMethods.Count == 0 || OalClassMethods.Count == 0)
        {
            return true;
        }

        recOalClassOfPrevMethodCall.OalClassMethods.RemoveAt(
            recOalClassOfPrevMethodCall.OalClassMethods.Count - 1);
        OalClassMethods.RemoveAt(OalClassMethods.Count - 1);
        return false;
    }

    private void HandleReceiverOalMethod(OalMethodCall oalMethodCall)
    {
        var receiverOalClassMethod = new OalClassMethod
        {
            Name = oalMethodCall.Name,
            Code = ""
        };

        var receiverOalClass = OalClasses.Find(oalClass => oalClass.Id == oalMethodCall.ReceiverOalClass.Id);
        receiverOalClass?.OalClassMethods.Add(receiverOalClassMethod);
        OalClassMethods.Add(receiverOalClassMethod);
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
        else if (int.Parse(combinedFragment.InteractionOperatorId) == 3)
        {
            methodConstraint = "if (" + body + ")";
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

    private void SetSenderOalClassInMethodCall(OalMethodCall methodCall)
    {
        var senderOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == methodCall.SenderOccurrenceId);
        if (senderOccurrenceSpecification == null) return;

        var senderOalClassId = senderOccurrenceSpecification.RefrenceIdOfCoveredObject;
        var senderOalClass = OalClasses.Find(oalClass => oalClass.Id == senderOalClassId);
        if (senderOalClass == null) return;
        methodCall.SenderOalClass = senderOalClass;
        senderOalClass.OalMethodCalls.Add(methodCall);
    }

    private void SetReceiverOalClassInMethodCall(OalMethodCall methodCall)
    {
        var receiverOccurrenceSpecification =
            OccurrenceSpecifications.Find(specification => specification.Id == methodCall.ReceiverOccurrenceId);
        if (receiverOccurrenceSpecification == null) return;

        var receiverOalClassId = receiverOccurrenceSpecification.RefrenceIdOfCoveredObject;
        var receiverOalClass = OalClasses.Find(oalClass => oalClass.Id == receiverOalClassId);
        if (receiverOalClass == null) return;
        methodCall.ReceiverOalClass = receiverOalClass;
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
        if (nameOfClassInstance == "")
        {
            return nameOfMethod + "();\n";
        }

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

    private List<AnimationMethodCode> CreateAnimMethodsCodes()
    {
        var methodCodes = new List<AnimationMethodCode>();
        foreach (var oalClass in OalClasses)
        {
            var methodCode = new AnimationMethodCode
            {
                Name = oalClass.Name
            };
            var animMethods = new List<AnimationMethod>();
            foreach (var oalClassMethod in oalClass.OalClassMethods)
            {
                var animMethod = new AnimationMethod()
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