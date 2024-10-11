using factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpaceBattle.Interfaces;
using System.Data;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace SpaceBattle
{
    class AutoGenerate
    {
        public static object? CreateNewObject(Type type)
        {
            var myType = CompileResultType(type);
            return Activator.CreateInstance(myType);
        }
        public static Type CompileResultType(Type type)
        {
            TypeBuilder tb = GetTypeBuilder(type);
            //tb.AddInterfaceImplementation(type);

            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var method in type.GetMethods())
                CreateMethod(tb, method);

            foreach (var field in type.GetFields())
                CreateProperty(tb, field);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(Type type)
        {
            var typeSignature = $"{type.Name}Adapter";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(an.Name ?? typeSignature);
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, FieldInfo field)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + field.Name, field.FieldType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(field.Name, PropertyAttributes.HasDefault, field.FieldType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + field.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, field.FieldType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + field.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { field.FieldType });
            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static void CreateMethod(TypeBuilder tb, MethodInfo method)
        {
            var methodBuilder = tb.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, Type.EmptyTypes);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ret);

            //tb.DefineMethodOverride(stub, method);
        }
    }
}
