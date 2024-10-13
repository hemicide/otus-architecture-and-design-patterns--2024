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
using System.Data;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.CodeAnalysis;
namespace SpaceBattle
{

    // https://stackoverflow.com/questions/29131117/using-ilgenerator-emit-to-call-a-method-in-another-assembly-that-has-an-out-para

    // NOT USED
    public class AdapterGenerator
    {
        public static T CreateAdapter<T>(params object[] args) where T : class
        {
            return CreateAdapter(typeof(T), new object[] { args }) as T;
        }

        public static object CreateAdapter(Type interfaceType, params object[] args)
        {
            var proxyType = CreateProxyType(interfaceType);
            return Activator.CreateInstance(proxyType, new object[] { args } );
        }

        private static Type CreateProxyType(Type interfaceType)
        {
            var typeSignature = $"{interfaceType.Name}Adapter";
            var assemblyName = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name ?? typeSignature);
            var typeBuilder = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public);

            // Наследование от интерфейса
            typeBuilder.AddInterfaceImplementation(interfaceType);

            // Определение поля для хранения аргументов конструктора
            var argsField = typeBuilder.DefineField("args", typeof(object[]), FieldAttributes.Public);
            //var args = typeBuilder.DefineField("args", typeof(object[]), FieldAttributes.Public);

            // Определение конструктора
            ConstructorBuilder ctor1 = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(object[]) });
            //ConstructorBuilder ctor1 = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(object[]) });
            ILGenerator ctor1IL = ctor1.GetILGenerator();

            // Вызов конструктора по умолчанию
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ConstructorInfo? ci = typeof(object).GetConstructor(Type.EmptyTypes);
            ctor1IL.Emit(OpCodes.Call, ci!);

            // Сохранение аргументов конструктора в поле
            ctor1IL.Emit(OpCodes.Ldarg_0); // Загрузка текущего объекта
            ctor1IL.Emit(OpCodes.Ldarg_1); // Загрузка аргумента типа object[]
            ctor1IL.Emit(OpCodes.Stfld, argsField); // Сохранение в поле

            ctor1IL.Emit(OpCodes.Ret); // Завершение конструктора

            foreach (var method in interfaceType.GetMethods())
            {
                var parameters = method.GetParameters();
                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, parameterTypes);

                var il = methodBuilder.GetILGenerator();

                //ilGenerator.Emit(OpCodes.Ldarg_0); // Загрузка текущего объекта
                // Загрузка первого параметра (string)
                il.Emit(OpCodes.Ldstr, $"{interfaceType.Name}.{method.Name}");

                // Загрузка поля args на стек
                il.Emit(OpCodes.Ldarg_0); // Загрузка текущего объекта
                il.Emit(OpCodes.Ldfld, argsField); // Загрузка значения поля args


                // TODO: не получается вытащить из параметров метода аргумент и доабавить его в стек 
                // GetVelocity(new Vector2(10, 10));

                // нужно параметры из стека добавить в argsField (расширить)

                // Вызов метода Resolve из IoC
                var resolveMethod = typeof(IoC).GetMethod("Resolve", new[] { typeof(string), typeof(object[]) });
                if (resolveMethod == null)
                    throw new InvalidOperationException("Method Resolve not found in IoC class.");
               
                if (resolveMethod.IsGenericMethod && method.ReturnType != typeof(void))
                {
                    // Создание обобщенного метода
                    var genericMethod = resolveMethod.MakeGenericMethod(method.ReturnType);
                    il.Emit(OpCodes.Call, genericMethod); // Вызов обобщенного метода
                }
                else
                {
                    // Вызов обычного метода
                    il.Emit(OpCodes.Call, resolveMethod);
                }

                //if (method.ReturnType != typeof(void))
                //    ilGenerator.Emit(OpCodes.Ldc_I4_0); // Возвращаем 0 для int

                il.Emit(OpCodes.Ret);
            }

            return typeBuilder.CreateType();
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
    }
}

