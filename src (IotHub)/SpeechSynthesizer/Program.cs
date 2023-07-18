using ApiClients.Http.DependencyInjection;
using ApiClients.Http.YandexCloud;
using ApiClients.Http.YandexCloud.Models.Config;
using ApiClients.Http.YandexCloud.Models.Const;
using ApiClients.Http.YandexCloud.Models.Request;
using Autofac;
using Common.DependencyInjection;
using Common.DependencyInjection.Modules;
using Common.Helpers;

namespace SpeechSynthesizer
{
    internal class Program
    {
        private const String _iamConfigFileName = "IamConfig.json";


        static void Main(String[] args)
        {
            using (var container = CreateContainer())
            {
                var iamToken = GetIamToken(container);
                var client = container.Resolve<YandexCloudHttpClient>();

                //var cloudDirectories = client.GetFoldersAsync(iamToken).Result;

                var text = File.ReadAllText(Path.Combine(FileHelper.DesktopDirectory, "Text.txt"));
                var fileBytes = client.SynthesizeSpeechAsync(iamToken, new SynthesizeSpeechRequest()
                {
                    Text = text,
                    Format = OutputFileFormat.Mp3,
                    Voice = Voice.Ru.Zahar
                }).Result;
                fileBytes.SaveOnDisk(Path.Combine(FileHelper.DesktopDirectory, "Result.mp3"));
            }
        }


        // SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
        private static IContainer CreateContainer()
        {
            var assemblies = Collector.LoadAssemblies("SpeechSynthesizer");
            var builder = new ContainerBuilder();
            var configuration = CustomConfigurationProvider.CollectEnvironmentRelatedConfiguration();

            builder.RegisterInstance(configuration).AsSelf().AsImplementedInterfaces();
            builder.RegisterServices(assemblies);
            builder.RegisterConfiguration(configuration, assemblies);

            builder.RegisterModule(new AutoMapperModule(assemblies));
            builder.RegisterModule(new HttpClientModule(configuration));

            return builder.Build();
        }
        private static String GetIamToken(IContainer container)
        {
            var yandexCloudHttpClientConfig = container.Resolve<YandexCloudHttpClientConfig>();
            var iamConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), _iamConfigFileName);

            if (!File.Exists(iamConfigFilePath))
                return GetNewIamToken(container, iamConfigFilePath);

            var iamTokenConfig = FileHelper.GetFromJsonFile<IamTokenConfig>(iamConfigFilePath);
            var threshold = iamTokenConfig.CreationDateUtc.AddSeconds(yandexCloudHttpClientConfig.IamTokenRefreshThresholdSec);
            var now = DateTime.UtcNow;

            if (now > iamTokenConfig.ExpirationDateUtc)
                return GetNewIamToken(container, iamConfigFilePath);
                
            if (now > threshold)        // Token lifetime is about 12 hours, but recommended refresh rate is once per hour
                return GetNewIamToken(container, iamConfigFilePath);

            return iamTokenConfig.IamToken;
        }
        private static String GetNewIamToken(IContainer container, String iamConfigFilePath)
        {
            var client = container.Resolve<YandexCloudHttpClient>();
            var iamTokenResponse = client.GetIamTokenAsync().Result;

            var iamTokenConfig = new IamTokenConfig()
            {
                IamToken = iamTokenResponse.IamToken,
                CreationDateUtc = DateTime.UtcNow,
                ExpirationDateUtc = iamTokenResponse.ExpiresAt
            };
            iamTokenConfig.SaveOnDiskAsJson(iamConfigFilePath);

            return iamTokenConfig.IamToken;
        }
    }
}