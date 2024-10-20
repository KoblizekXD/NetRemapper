namespace NetRemapper.Tests
{
    public class MappingsReaderTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(File.Exists("./Resources/Main.exe"));
        }
    }
}