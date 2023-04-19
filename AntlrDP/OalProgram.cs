namespace AntlrDP;

public class OalProgram
{
    public List<OalClass> OalClasses = new();
    public List<OalClassMethod> OalClassMethods = new();

    public List<OalOccurrenceSpecification> OccurrenceSpecifications = new();
    public string Code = "";

    public void SetOalClassesInMethods()
    {
        foreach (var classMethod in OalClassMethods)
        {
            SetSenderOalClassInMethod(classMethod);
            SetReceiverOalClassInMethod(classMethod);
        }
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
        // receiverOalClass.OalClassMethods.Add(classMethod);
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
                var methodCode = CreateMethodCall(classInstanceName, oalClassMethod.Name);
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
}