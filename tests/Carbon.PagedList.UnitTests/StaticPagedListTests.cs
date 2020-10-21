using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Carbon.PagedList.UnitTests
{
    public class StaticPagedListTests
    {
        private readonly IEnumerable<string> dataList;

        public StaticPagedListTests()
        {
            var tmpList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                tmpList.Add("testString" + i.ToString());
            }

            dataList = tmpList.AsEnumerable();
        }

        [Fact]
        public void CreateWithArguments_CreateSuccessfully_ReturnStaticPagedList()
        {
            // Act
            var spl = new StaticPagedList<string>(dataList, 1, 5, dataList.Count());

            // Arrange
            Assert.IsType<StaticPagedList<string>>(spl);
        }

        [Fact]
        public void CreateWithMetadata_CreateSuccessfully_ReturnStaticPagedList()
        {
            // Arrange
            var mData = new PagedListMetaData(dataList.ToPagedList(1, 5));

            // Act
            var spl = new StaticPagedList<string>(dataList, mData);

            // Assert
            Assert.IsType<StaticPagedList<string>>(spl);
            Assert.Equal(dataList.Count(), spl.Count());
        }
    }
}
