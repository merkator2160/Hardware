using IotHub.Common.Extensions;
using System;
using Xunit;

namespace IotHub.UnitTests
{
	public class MapTests
	{
		[Fact]
		public void SingleMapTest()
		{
			Byte value = 100;
			var test = value.Map(0, 255, 0, 1024.345F);

			Assert.Equal(401.70391845703125, test);
		}

		[Fact]
		public void ByteMapTest()
		{
			Byte value = 100;
			var test = value.Map(0, 255, 0, 1024);

			Assert.Equal(401, test);
		}

		[Fact]
		public void Int32MapTest()
		{
			var value = 100;
			var test = value.Map(0, 255, 0, 1024);

			Assert.Equal(401, test);
		}
	}
}
