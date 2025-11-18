using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Identity;
using Dsw2025Tpi.Data.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VSDiagnostics;

namespace Dsw2025Tpi.Data.Benchmarks;
[CPUUsageDiagnoser]
public class UserServiceConstructorBenchmark
{
    private ServiceProvider? _serviceProvider;
    private UserManager<AppUser>? _userManager;
    private SignInManager<AppUser>? _signInManager;
    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddIdentityCore<AppUser>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<SignInManager<AppUser>>();
        _serviceProvider = services.BuildServiceProvider();
        _userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
        _signInManager = _serviceProvider.GetRequiredService<SignInManager<AppUser>>();
    }

    [Benchmark]
    public UserService ConstructUserService()
    {
        return new UserService(_userManager!, _signInManager!);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }
}