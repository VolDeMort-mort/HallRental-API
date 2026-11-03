using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Bll.Common.Shared;

public interface IDependencyRegistrar
{
    void Register(IServiceCollection services, IConfiguration configuration);
}
