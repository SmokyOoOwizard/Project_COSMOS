using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public sealed class InitSystem
    {
        private struct ContainerForMarkedTypes
        {
            public InitMethodAttribute Attribute;
            public Type Type;
            public MethodInfo Method;
        }
        private readonly List<ContainerForMarkedTypes> initTypes = new List<ContainerForMarkedTypes>();

        public static void Init(string queue)
        {
            var system = PrepareInit(queue);
            system.Init();
        }
        public static InitSystem PrepareInit(string queue)
        {
            InitSystem container = new InitSystem();

            var asseblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = asseblies.SelectMany(a => a.GetTypes()).Where(t => t.IsClass || t.IsValueType);

            var classes = types.Where(t => t.IsClass && t.GetCustomAttribute<InitMethodAttribute>() != null).Where(t => !t.IsAbstract && !t.IsGenericType).Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0 && !c.IsStatic));
            foreach (var cl in classes)
            {
                var attribute = cl.GetCustomAttribute<InitMethodAttribute>();
                if (attribute.Queue == queue)
                {
                    var c = new ContainerForMarkedTypes();
                    c.Attribute = attribute;
                    c.Type = cl;
                    container.initTypes.Add(c);
                }
            }

            var methods = types.SelectMany(t => t.GetMethods()).Where(m => m.IsStatic && m.GetParameters().Length == 0 && m.GetCustomAttribute<InitMethodAttribute>() != null);
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<InitMethodAttribute>();
                if (attribute.Queue == queue)
                {
                    var c = new ContainerForMarkedTypes();
                    c.Attribute = attribute;
                    c.Method = method;
                    container.initTypes.Add(c);
                }
            }

            container.initTypes.Sort((x, y) => x.Attribute.Order.CompareTo(y.Attribute.Order));

            return container;
        }

        public void Init()
        {
            try
            {
                foreach (var i in initTypes)
                {
                    if (i.Method != null) // execute method
                    {
                        Log.Info($"Try init invoke method {i.Method.Name} in {i.Method.DeclaringType}", "InitSystem", "Invoke", $"Queue: {i.Attribute.Queue}");
                        i.Method.Invoke(null, null);
                    }
                    else if (i.Type != null) // create type
                    {
                        Log.Info($"Try init create instance of type {i.Type}", "InitSystem", "CreateInstance", $"Queue: {i.Attribute.Queue}");
                        Activator.CreateInstance(i.Type);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "InitSystem");
            }
        }
    }
}
