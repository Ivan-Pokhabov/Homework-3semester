namespace test2;

using System.Reflection;
using System.Text;

public class Reflector
{
    static public void PrintStructure(Type someClass, string filePath)
    {
        var typeInfo = someClass.GetTypeInfo();

        if (!typeInfo.IsClass)
        {
            throw new ArgumentException("This is not a class");
        }
        

        using (File.Create($"{filePath}/{typeInfo.Name}.cs")) { }

        using StreamWriter writer = new ($"{filePath}/{typeInfo.Name}.cs");

        writer.WriteLine($"namespace {typeInfo.Namespace};");

        var attributes = typeInfo.GetCustomAttributes().ToArray();
        
        if (attributes.Length > 0)
        {
            writer.Write("[");
            for (var i = 0; i < attributes.Length - 1; ++i)
            {
                writer.Write($"{attributes[i].GetType().Name}, ");
            }

            writer.WriteLine($"{attributes[^1].GetType().Name}" + "]");
        }

        if (typeInfo.IsPublic)
        {
            writer.Write("public ");
        }

        if (typeInfo.IsSealed || typeInfo.IsAbstract)
        {
            writer.Write("static ");
        }

        writer.WriteLine($"class {typeInfo.Name}");
        writer.WriteLine("{");

        PrintFields(writer, typeInfo.DeclaredFields.ToArray());
        PrintProperies(writer, typeInfo.DeclaredProperties.ToArray());
        PrintMethods(writer, typeInfo.DeclaredMethods.ToArray());
        PrintClassesAndIntefaces(writer, typeInfo.DeclaredNestedTypes.ToArray());
        writer.WriteLine("}");

    }

    static private void PrintClassesAndIntefaces(StreamWriter writer, TypeInfo[] nestedTypes)
    {
        foreach (var nestedType in nestedTypes)
        {
            string accessModifier = GetAccessModifier(nestedType);
            if (nestedType.IsInterface)
            {
                writer.WriteLine($"- {accessModifier} interface {nestedType.Name};");
            }

            writer.WriteLine($"{accessModifier} class {nestedType.Name};");
        }
    }

    public static string DiffClasses(Type classType1, Type classType2)
    {
        var methodsClass1 = GetMethodSignatures(classType1.GetTypeInfo());
        var methodsClass2 = GetMethodSignatures(classType2.GetTypeInfo());

        var onlyInClass1 = methodsClass1.Except(methodsClass2).ToList();
        var answer = new StringBuilder();
        answer.AppendLine($"\nМетоды, присутствующие только в {classType1.Name}:");
        foreach (var method in onlyInClass1)
            answer.AppendLine($"- {method}");

        var onlyInClass2 = methodsClass2.Except(methodsClass1).ToList();
        answer.AppendLine($"\nМетоды, присутствующие только в {classType2.Name}:");
        foreach (var method in onlyInClass2)
            answer.AppendLine($"- {method}");

        return answer.ToString();
    }

    private static List<string> GetMethodSignatures(TypeInfo typeInfo)
        => typeInfo.DeclaredMethods.ToArray()
                   .Select(m => $"{(m.IsStatic ? "static " : "")}{m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})")
                   .ToList();

    static private void PrintFields(StreamWriter writer, FieldInfo[] fieldInfos)
    {
        foreach (var field in fieldInfos)
        {
            var modifiers = field.IsStatic ? "static " : "";
            writer.WriteLine($"{modifiers}{field.FieldType.Name} {field.Name};");
        }
    }

    static private void PrintProperies(StreamWriter writer, PropertyInfo[] propertyInfos)
    {
        foreach (var property in propertyInfos)
        {
            writer.WriteLine($"{property.PropertyType.Name} {property.Name} {{ get; set; }}");
        }
    }

    static private void PrintMethods(StreamWriter writer, MethodInfo[] methodInfos)
    {
        foreach (var method in methodInfos)
        {
            string modifiers = method.IsStatic ? "static " : "";
            writer.WriteLine($"{modifiers}{method.ReturnType.Name} {method.Name}({string.Join(", ", GetParameterInfo(method))})" + "{}");
        }
    }

    static private string[] GetParameterInfo(MethodInfo method)
    {
        var parameters = method.GetParameters();
        var parameterDescriptions = new string[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameterDescriptions[i] = $"{parameters[i].ParameterType.Name} {parameters[i].Name}";
        }

        return parameterDescriptions;
    }

    static private string GetAccessModifier(Type nestedType)
    {
        if (nestedType.IsNestedPublic)
            return "public";
        if (nestedType.IsNestedPrivate)
            return "private";
        if (nestedType.IsNestedFamily)
            return "protected";
        if (nestedType.IsNestedAssembly)
            return "internal";
        return "protected internal";
    }
}
