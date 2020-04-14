using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
internal static class DefineAst
{

    public static void Build(string outputDir, string baseName, List<string> types)
    {
        //var exprFile = File.Open($"{outputDir}/{baseName}.cs", FileMode.CreateNew);
        var sb = new StringBuilder();
        var line = "{0};\n";
        sb.AppendFormat(line, "using System");
        sb.AppendFormat(line, "using System.Collection.Generic");
        sb.AppendLine("namespace Expressions {");
        sb.AppendLine($"internal abstract class {baseName} {{");
        sb.AppendLine("abstract T Accept<T>(Visitor<T> visitor);");
        sb.AppendLine(DefineVisitor(baseName, types));
        
        sb.AppendLine(BuildTypes(baseName, types));
        sb.AppendLine();
        sb.AppendLine("}");
        sb.AppendLine("}");
        File.WriteAllText($"{outputDir}/{baseName}.cs", sb.ToString());
    }

    private static string DefineVisitor(string baseName, List<string> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine("interface Visitor<T>{");
        foreach (var itm in types)
        {
            var typeName = itm.Split(":")[0].Trim();
            sb.AppendLine($"\t\tT Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }
        sb.AppendLine("\t}");
        return sb.ToString();
    }
    private static string DefineType(string baseName, string className, string fieldList)
    {
        StringBuilder newClass = new StringBuilder();
        newClass.AppendLine($"static class {className} : {baseName} {{");
        // Constructor
        newClass.AppendLine($"\t{className}({fieldList}){{");
        // Store paramters in fields
        var fields = fieldList.Split(", ");
        foreach (var fld in fields)
        {
            var name = fld.Split(" ")[1];
            newClass.AppendLine($"\tthis.{name}={name};");
        }
        newClass.AppendLine("\t}\n");

        newClass.AppendLine();
        newClass.AppendLine("override T Accept<T>(Visitor<T> visitor){");
        newClass.AppendLine($"\t\treturn visitor.visit{className}{baseName}(this);");
        newClass.AppendLine("}");

        // Class Fields
        foreach (var fld in fields)
        {
            newClass.AppendLine($"\tpublic {fld}{{get;}}");
        }
        newClass.AppendLine("}");
        return newClass.ToString();
    }

    private static string BuildTypes(string baseName, IEnumerable<string> types)
    {
        var results = new List<string>();
        foreach (var item in types)
        {
            var fullDetail = item.Split(":");
            var className = fullDetail[0].Trim();
            var fields = fullDetail[1].Trim();
            results.Add(DefineType(baseName, className, fields));
        }
        return string.Join("\n", results);
    }

}