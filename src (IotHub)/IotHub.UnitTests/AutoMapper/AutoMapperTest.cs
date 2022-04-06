using AutoMapper;
using IotHub.Common.DependencyInjection;
using Xunit;

namespace IotHub.UnitTests.AutoMapper
{
    public class AutoMapperTest
    {
        [Fact]
        public void AutoMapperConfigurationTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Collector.LoadAssemblies("IotHub"));
            });

            mapperConfiguration.CompileMappings();
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}