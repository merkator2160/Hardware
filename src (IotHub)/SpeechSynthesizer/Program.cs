using ApiClients.Http.DependencyInjection;
using ApiClients.Http.YandexCloud;
using ApiClients.Http.YandexCloud.Models.Config;
using ApiClients.Http.YandexCloud.Models.Const;
using ApiClients.Http.YandexCloud.Models.Exceptions;
using ApiClients.Http.YandexCloud.Models.Request;
using Autofac;
using Common.DependencyInjection;
using Common.DependencyInjection.Modules;
using Common.Helpers;
using System.Diagnostics;

namespace SpeechSynthesizer
{
    internal class Program
    {
        private const String _iamConfigFileName = "IamConfig.json";
        private const String _textFileName = "Text.txt";
        private const String _resultFileName = "Result.mp3";


        static void Main(String[] args)
        {
            using (var container = CreateContainer())
            {
                if (!TryGetText(out var text))
                    return;

                if(!TryCreateSpeech(container, text, out var fileBytes))
                    return;

                var resultFilePath = Path.Combine(FileHelper.DesktopDirectory, _resultFileName);

                fileBytes.SaveOnDisk(resultFilePath);
                new Process
                {
                    StartInfo = new ProcessStartInfo(resultFilePath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }


        // SUPPORT FUNCTIONS ////////////////////////////////////////////////////////////////////////////
        private static IContainer CreateContainer()
        {
            var assemblies = Collector.LoadAssemblies("SpeechSynthesizer");
            var builder = new ContainerBuilder();
            var configuration = CustomConfigurationProvider.CollectConfiguration();

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
        private static Boolean TryGetText(out String text)
        {
            var filePath = Path.Combine(FileHelper.DesktopDirectory, _textFileName);
            if (!File.Exists(filePath))
            {
                text = null;
                Console.WriteLine($"File not found: {filePath}!");
                Console.ReadKey();

                return false;
            }

            text = File.ReadAllText(filePath);

            if (String.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("File have no text for synthesis!");
                Console.ReadKey();

                return false;
            }

            return true;
        }
        private static Boolean TryCreateSpeech(IContainer container,  String text, out Byte[] fileBytes)
        {
            var iamToken = GetIamToken(container);
            var client = container.Resolve<YandexCloudHttpClient>();

            try
            {
                fileBytes = client.SynthesizeSpeechAsync(iamToken, new SynthesizeSpeechRequest()
                {
                    Text = text,
                    Format = OutputFileFormat.Mp3,
                    Voice = Voice.Ru.Ermil,
                    Speed = 0.9F
                }).Result;
                return true;
            }
            catch(Exception ex) when (ex is AggregateException or YandexCloudHttpClientException)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

            fileBytes = new Byte[] { };
            return false;
        }
    }
}