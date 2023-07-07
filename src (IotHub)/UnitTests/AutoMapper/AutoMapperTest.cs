using AutoMapper;
using Common.DependencyInjection;
using Xunit;

namespace UnitTests.AutoMapper
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