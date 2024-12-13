// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;


public class Program{
    public static void Main(string[] args){
        object tracer = null;
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

                        if (member is FieldInfo f && f.Name.Equals("tracerHolder", StringComparison.OrdinalIgnoreCase))
                        {
                            tracer = f.GetValue(null);
                        }

                        string memberInfo;
                        if (member is FieldInfo field)
                        {
                            try
                            {
                                var value = field.GetValue(null);
                                memberInfo = $"Field: {field.Name} ({field.FieldType.Name}) = {value ?? "null"}";
                            }
                            catch (Exception ex)
                            {
                                memberInfo = $"Field: {field.Name} ({field.FieldType.Name}) = <error reading value: {ex.Message}>";
                            }
                        }
                        else if (member is PropertyInfo p)
                        {
                            try
                            {
                                if (p.GetMethod != null)
                                {
                                    var value = p.GetValue(null);
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

        ActivitySource activitySource = new ActivitySource("Microsoft.Build");
        using (var activity = activitySource.StartActivity("SayHello"))
        {
            activity?.SetTag("MyTag", "MyValue");
            Console.WriteLine("Hello, World!");
        }
        
        (tracer as IDisposable)?.Dispose();
    }
}
