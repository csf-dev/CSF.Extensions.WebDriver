using System.Reflection;
using AutoFixture;
using Castle.DynamicProxy;

namespace CSF.Extensions.WebDriver.Proxies;

public class ProxyFactoryAttribute : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
        => new ProxyFactoryCustomization();
}

public class ProxyFactoryCustomization : ICustomization
{
    /// <summary>
    /// Singleton proxy generator, following advice in DynamicProxy docco.
    /// </summary>
    static readonly IProxyGenerator singletonProxyGenerator = new ProxyGenerator();

    public void Customize(IFixture fixture)
    {
        fixture.Customize<IServiceProvider>(c => c.FromFactory((IProxyGenerator generator, IAugmentsProxyContext augmenter) =>
        {
            var services = new Mock<IServiceProvider>();
            services.Setup(x => x.GetService(It.Is<Type>(t => typeof(IAugmentsProxyContext).IsAssignableFrom(t)))).Returns(augmenter);
            services.Setup(x => x.GetService(typeof(IProxyGenerator))).Returns(generator);
            return services.Object;
        }));
        fixture.Inject(singletonProxyGenerator);
        fixture.Freeze<IAugmentsProxyContext>();
    }
}