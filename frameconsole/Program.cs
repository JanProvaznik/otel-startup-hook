// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;
using System.Linq;


public class Program{
    public static void Main(string[] args){
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.GetName().Name?.StartsWith("System", StringComparison.OrdinalIgnoreCase) == true))
{
    try 
    {
        var staticClasses = assembly.GetTypes()
            .Where(t => t.IsAbstract && t.IsSealed);

        foreach (var staticClass in staticClasses)
        {
            Console.WriteLine($"Assembly: {assembly.GetName().Name}");
            Console.WriteLine($"Static Class: {staticClass.FullName}");
            
            var staticMembers = staticClass.GetMembers(BindingFlags.Public | 
                                                      BindingFlags.NonPublic | 
                                                      BindingFlags.Static)
                                         .Where(m => m.DeclaringType == staticClass);

            foreach (var member in staticMembers)
            {
                if (member.Name.StartsWith("<"))
                    continue;

                string memberInfo;
                if (member is FieldInfo f)
                {
                    try 
                    {
                        var value = f.GetValue(null); // null because it's static
                        memberInfo = $"Field: {f.Name} ({f.FieldType.Name}) = {value ?? "null"}";
                    }
                    catch (Exception ex)
                    {
                        memberInfo = $"Field: {f.Name} ({f.FieldType.Name}) = <error reading value: {ex.Message}>";
                    }
                }
                else if (member is PropertyInfo p)
                {
                    try 
                    {
                        if (p.GetMethod != null)
                        {
                            var value = p.GetValue(null); // null because it's static
                            memberInfo = $"Property: {p.Name} ({p.PropertyType.Name}) = {value ?? "null"}";
                        }
                        else
                        {
                            memberInfo = $"Property: {p.Name} ({p.PropertyType.Name}) = <write-only>";
                        }
                    }
                    catch (Exception ex)
                    {
                        memberInfo = $"Property: {p.Name} ({p.PropertyType.Name}) = <error reading value: {ex.Message}>";
                    }
                }
                else if (member is MethodInfo m)
                    memberInfo = $"Method: {m.Name}()";
                else
                    memberInfo = $"{member.MemberType}: {member.Name}";

                Console.WriteLine($"\t{memberInfo}");
            }
            Console.WriteLine();
        }
    }
    catch (ReflectionTypeLoadException ex)
    {
        Console.WriteLine($"Could not load types from {assembly.GetName().Name}: {ex.Message}");
    }
}

        Console.WriteLine("Hello, World!");
    }
}
