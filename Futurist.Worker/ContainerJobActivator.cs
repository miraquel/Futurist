using Hangfire;

namespace Futurist.Worker;

public class ContainerJobActivator : JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    public ContainerJobActivator(IServiceCollection serviceCollection)
    {
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override object ActivateJob(Type jobType)
    {
        return _serviceProvider.GetRequiredService(jobType);
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
        return new ContainerJobScope(_serviceProvider.CreateScope());
    }

    private class ContainerJobScope : JobActivatorScope
    {
        private readonly IServiceScope _serviceScope;

        public ContainerJobScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public override object Resolve(Type type)
        {
            return _serviceScope.ServiceProvider.GetRequiredService(type);
        }

        public override void DisposeScope()
        {
            _serviceScope.Dispose();
        }
    }
}