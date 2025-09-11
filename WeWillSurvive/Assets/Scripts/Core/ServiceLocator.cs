using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WeWillSurvive.Core
{
    public interface IService
    {
        public UniTask InitializeAsync();
    }

    public class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static bool _initialized;

        public async static UniTask AutoRegisterServices()
        {
            if (_initialized)
            {
                Debug.LogWarning("ServiceLocator is already initialized.");
                return;
            }

            var serviceTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .OrderBy(t => t.Namespace != null && t.Namespace.StartsWith("WeWillSurvive.Core") ? 0 : 1);

            foreach (var serviceType in serviceTypes)
            {
                try
                {
                    var serviceInstance = Activator.CreateInstance(serviceType);

                    var interfaces = serviceType.GetInterfaces();

                    foreach (var interfaceType in interfaces)
                    {
                        if (interfaceType == typeof(IService) || interfaceType.GetInterfaces().Contains(typeof(IService)))
                        {
                            RegisterServiceWithInterface(serviceType, serviceInstance);
                        }
                    }

                    await ((IService)serviceInstance).InitializeAsync();

                    Debug.Log($"Auto-registered service: {serviceType.Name}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to auto-register service {serviceType.Name}: {ex.Message}");
                }
            }

            _initialized = true;
            Debug.Log("ServiceLocator initialization completed.");
        }

        public static void Register<TInterface, TImplementation>(TImplementation service) where TImplementation : class, TInterface
        {
            Type interfaceType = typeof(TInterface);

            if (_services.ContainsKey(interfaceType))
            {
                Debug.LogWarning($"Service {interfaceType.Name} already registered. Overwriting...");
                _services[interfaceType] = service;
            }
            else
            {
                _services.Add(interfaceType, service);
                Debug.Log($"Service {interfaceType.Name} registered.");
            }
        }

        /// <summary>
        /// ���� ��������
        /// </summary>
        /// <typeparam name="TInterface">���� �������̽� Ÿ��</typeparam>
        /// <returns>��ϵ� ���� �ν��Ͻ� �Ǵ� �⺻��</returns>
        public static TInterface Get<TInterface>()
        {
            Type interfaceType = typeof(TInterface);

            if (_services.TryGetValue(interfaceType, out object service))
            {
                return (TInterface)service;
            }

            Debug.Log($"Service {interfaceType.Name} not found!");
            return default;
        }

        /// <summary>
        /// ���� ���� ���� Ȯ��
        /// </summary>
        /// <typeparam name="TInterface">���� �������̽� Ÿ��</typeparam>
        /// <returns>���� ���� ����</returns>
        public static bool HasService<TInterface>()
        {
            return _services.ContainsKey(typeof(TInterface));
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        /// <typeparam name="TInterface">���� �������̽� Ÿ��</typeparam>
        /// <returns>���� ���� ����</returns>
        public static bool Unregister<TInterface>()
        {
            Type interfaceType = typeof(TInterface);

            if (_services.ContainsKey(interfaceType))
            {
                _services.Remove(interfaceType);
                Debug.Log($"Service {interfaceType.Name} unregistered.");
                return true;
            }

            return false;
        }

        public static void ClearAll()
        {
            _services.Clear();
        }

        private static void RegisterServiceWithInterface(Type interfaceType, object serviceInstance)
        {
            if (_services.ContainsKey(interfaceType))
            {
                Debug.LogWarning($"Service {interfaceType.Name} already registered. Overwriting...");
                _services[interfaceType] = serviceInstance;
            }
            else
            {
                _services.Add(interfaceType, serviceInstance);
            }
        }
    }
}
