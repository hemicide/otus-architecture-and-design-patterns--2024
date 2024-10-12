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
using System.Runtime.InteropServices.JavaScript;

namespace SpaceBattle
{
    public class AutoGenerate
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
            //MethodBuilder methGetPosition = tb.DefineMethod("getPosition",MethodAttributes.Public, typeof(Vector<int>), new Type[] { });
            var args = method.GetParameters();
            MethodBuilder methodBuilder = tb.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, (from arg in args select arg.ParameterType).ToArray());

            //MethodInfo mi = typeof(IoC).GetMethod("Resolve"); //Assembly.GetExecutingAssembly(). typeof(object).GetConstructor(Type.EmptyTypes);
            //MethodInfo miGeneric = mi.MakeGenericMethod(typeof(Vector<int>));

            var methIL = methodBuilder.GetILGenerator();
            methIL.Emit(OpCodes.Ldarg_0);
            methIL.Emit(OpCodes.Call, method);
            methIL.Emit(OpCodes.Ret);

            //methIL.Emit(OpCodes.Ldstr, "Tank.Operations.IMovable:position.get");
            //methIL.Emit(OpCodes.Ldc_I4_1);
            //methIL.Emit(OpCodes.Newarr, typeof(object));
            //methIL.Emit(OpCodes.Dup);
            //methIL.Emit(OpCodes.Ldc_I4_0);
            //methIL.Emit(OpCodes.Ldarg_0);
            //methIL.Emit(OpCodes.Ldfld, fbObj);
            //methIL.Emit(OpCodes.Stelem_Ref);
            //methIL.Emit(OpCodes.Call, miGeneric);
            //methIL.Emit(OpCodes.Ldloc_0);
            //methIL.Emit(OpCodes.Ret);



            //tb.DefineMethodOverride(stub, method);
        }

        private static void CreateMethod2(TypeBuilder type, MethodInfo method)
        {
            var args = method.GetParameters();
            var methodImpl = type.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType, args.Select(r => r.ParameterType).ToArray());

            type.DefineMethodOverride(methodImpl, method);

            var generator = methodImpl.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);

            for (var paramIndex = 0; paramIndex < args.Length; paramIndex++)
            {
                switch (paramIndex)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        generator.Emit((paramIndex < 255)
                            ? OpCodes.Ldarg_S
                            : OpCodes.Ldarg,
                            paramIndex + 1);
                        break;
                }

            }
            generator.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != typeof(void))
                generator.Emit(OpCodes.Pop); // discard N-1 return values
                
            generator.Emit(OpCodes.Ret);
        }
    }

    public class AdapterGenerator
    {
        public static T CreateAdapter<T>(params object[] args) where T : class
        {
            return (T)CreateAdapter(typeof(T), args);
        }

        public static object CreateAdapter(Type interfaceType, params object[] args)
        {
            var proxyType = CreateProxyType(interfaceType);
            return Activator.CreateInstance(proxyType, args);
        }

        public static T CreateAdapter2<T>() where T : class
        {
           
            var interfaceType = typeof(T);
            var typeSignature = $"{interfaceType.Name}Adapter";
            var assemblyName = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name ?? typeSignature);
            var typeBuilder = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public);

            typeBuilder.AddInterfaceImplementation(interfaceType);

            // Реализация методов интерфейса
            foreach (var method in interfaceType.GetMethods())
            {
                var parameters = method.GetParameters();
                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, parameterTypes);

                var ilGenerator = methodBuilder.GetILGenerator();

                var resolveMethod = typeof(IoC).GetMethod("Resolve2", new[] { typeof(string) });
                //var resolveMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

                // Здесь можно добавить логику реализации метода
                ilGenerator.Emit(OpCodes.Ldstr, "IoC.Register");
                //ilGenerator.Emit(OpCodes.Newarr, typeof(object));
                //ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
                ilGenerator.Emit(OpCodes.Call, resolveMethod);
                if (method.ReturnType != typeof(void))
                    ilGenerator.Emit(OpCodes.Ldc_I4_0); // Возвращаем 0 для int

                ilGenerator.Emit(OpCodes.Ret);
            }

            var adapterType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(adapterType);
        }

        private static Type CreateProxyType(Type interfaceType)
        {
            var assemblyName = new AssemblyName("DynamicAdapters");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(interfaceType.Name + "Adapter", TypeAttributes.Public);

            typeBuilder.AddInterfaceImplementation(interfaceType);

            // Создание поля Field
            var fbObj = typeBuilder.DefineField("field", typeof(object), FieldAttributes.Public);

            // Define a constructor that takes an integer argument and
            // stores it in the private field.
            ConstructorBuilder ctor1 = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(object) });
            ILGenerator ctor1IL = ctor1.GetILGenerator();
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ConstructorInfo? ci = typeof(object).GetConstructor(Type.EmptyTypes);
            ctor1IL.Emit(OpCodes.Call, ci!);
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Ldarg_1);
            ctor1IL.Emit(OpCodes.Stfld, fbObj);
            ctor1IL.Emit(OpCodes.Ret);

            foreach (var method in interfaceType.GetMethods())
            {
                var parameters = method.GetParameters();
                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, parameterTypes);

                var ilGenerator = methodBuilder.GetILGenerator();

                // Загрузка первого параметра (string)
                ilGenerator.Emit(OpCodes.Ldstr, $"{interfaceType.Name}.{method.Name}");

                // Загрузка второго параметра (object[])
                ilGenerator.DeclareLocal(typeof(object[])); // Объявление локальной переменной для массива объектов
                ilGenerator.Emit(OpCodes.Ldc_I4_1); // Размер массива (только один элемент)
                //ilGenerator.Emit(OpCodes.Ldc_I4, 2); // Размер массива
                ilGenerator.Emit(OpCodes.Newarr, typeof(object)); // Создание нового массива объектов
                ilGenerator.Emit(OpCodes.Dup); // Дублирование ссылки на массив для сохранения значения

                //// Установка значений в массив
                //ilGenerator.Emit(OpCodes.Ldc_I4_0); // Индекс 0
                //ilGenerator.Emit(OpCodes.Ldstr, "param2"); // Замените на нужное значение
                //ilGenerator.Emit(OpCodes.Stelem_Ref); // Установка значения в массив

                // ---

                // Установка значения поля в массив
                ilGenerator.Emit(OpCodes.Ldc_I4_0); // Индекс 0
                ilGenerator.Emit(OpCodes.Ldarg_0); // Загрузка текущего объекта (адаптера)
                ilGenerator.Emit(OpCodes.Ldfld, fbObj); // Загрузка значения поля
                ilGenerator.Emit(OpCodes.Stelem_Ref); // Установка значения в массив


                //// Загрузка второго параметра (object[])
                //ilGenerator.DeclareLocal(typeof(object[])); // Объявление локальной переменной для массива объектов
                //ilGenerator.Emit(OpCodes.Ldc_I4_1); // Размер массива (только один элемент)
                //ilGenerator.Emit(OpCodes.Newarr, typeof(object)); // Создание нового массива объектов
                //ilGenerator.Emit(OpCodes.Dup); // Дублирование ссылки на массив для сохранения значения

                //// Установка значения в массив
                //ilGenerator.Emit(OpCodes.Ldc_I4_0); // Индекс 0
                //ilGenerator.Emit(OpCodes.Ldarg_1); // Загрузка аргумента implementation
                //ilGenerator.Emit(OpCodes.Stelem_Ref); // Установка значения в массив



                // Вызов метода Resolve из IoC
                var resolveMethod = typeof(IoC).GetMethod("Resolve1", new[] { typeof(string), typeof(object[]) });
                //var resolveMethod = typeof(IoC).GetMethod("Resolve1", new[] { typeof(string) });
                //var resolveMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });
                //var resolveMethod = typeof(IoC).GetMethod("Resolve", new[] { typeof(string), typeof(object[]) });
                if (resolveMethod == null)
                    throw new InvalidOperationException("Method Resolve not found in IoC class.");
               
                if (resolveMethod.IsGenericMethod && method.ReturnType != typeof(void))
                {
                    // Создание обобщенного метода
                    var genericMethod = resolveMethod.MakeGenericMethod(method.ReturnType);
                    ilGenerator.Emit(OpCodes.Call, genericMethod); // Вызов обобщенного метода
                }
                else
                {
                    // Вызов обычного метода
                    ilGenerator.Emit(OpCodes.Call, resolveMethod);
                }

                //if (method.ReturnType != typeof(void))
                //    ilGenerator.Emit(OpCodes.Ldc_I4_0); // Возвращаем 0 для int

                ilGenerator.Emit(OpCodes.Ret);

            }

            return typeBuilder.CreateType();
        }
    }
}

