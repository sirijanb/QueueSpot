using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace App.Tests.TestHelpers;

/// <summary>
/// Provides a fake ProtectedSessionStorage for unit tests using a no-op data protection provider.
/// JS interop for sessionStorage must be set up in tests when storage is used.
/// </summary>
public static class FakeProtectedSessionStorage
{
    public static void AddFakeProtectedSessionStorage(this IServiceCollection services)
    {
        services.AddSingleton<ProtectedSessionStorage>(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            var protector = new NoOpDataProtector();
            var provider = new NoOpDataProtectionProvider(protector);
            return new ProtectedSessionStorage(jsRuntime, provider);
        });
    }

    private class NoOpDataProtectionProvider : IDataProtectionProvider
    {
        private readonly IDataProtector _protector;

        public NoOpDataProtectionProvider(IDataProtector protector) => _protector = protector;

        public IDataProtector CreateProtector(string purpose) => _protector;
    }

    private class NoOpDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose) => this;
        public byte[] Protect(byte[] plaintext) => plaintext;
        public byte[] Unprotect(byte[] protectedData) => protectedData;
    }
}
