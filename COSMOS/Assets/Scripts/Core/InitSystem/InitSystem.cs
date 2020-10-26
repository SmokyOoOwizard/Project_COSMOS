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

            var methods = ReflectionsKeeper.instance.GetAllWithAttributeByCondition<InitMethodAttribute>((a, mri) => a.Queue == queue);

            foreach (var member in methods)
            {
                if (member is MethodReflection)
                {
                    var method = member as MethodReflection;
                    var attributes = member.GetAllAttributes<InitMethodAttribute>();
                    if (attributes.Count > 0)
                    {
                        var att = attributes[0];
                        if (att != null)
                        {
                            if (att.Queue == queue)
                            {
                                var c = new ContainerForMarkedTypes();
                                c.Attribute = att;
                                c.Type = method.Owner;
                                c.Method = method.Method;
                                container.initTypes.Add(c);
                            }
                        }
                    }
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
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "InitSystem");
            }
        }
    }
}
