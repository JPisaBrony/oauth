using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace TokenSecurityTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    //Configure SSL
                    var serverCertificate = LoadCertificate();
                    options.UseHttps(serverCertificate);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("https://localhost:44318")
                .Build();

            host.Run();
        }

        private static X509Certificate2 LoadCertificate()
        {
            var socialSampleAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(socialSampleAssembly, "TokenSecurityTest");
            var certificateFileInfo = embeddedFileProvider.GetFileInfo("compiler/resources/cert.pfx");
            using (var certificateStream = certificateFileInfo.CreateReadStream())
            {
                byte[] certificatePayload;
                using (var memoryStream = new MemoryStream())
                {
                    certificateStream.CopyTo(memoryStream);
                    certificatePayload = memoryStream.ToArray();
                }

                return new X509Certificate2(certificatePayload, "testPassword");
            }
        }
    }
}
