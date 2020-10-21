using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Carbon.PagedList.UnitTests
{
    public class PagedListExtensionsTests
    {

        private readonly IEnumerable<string> dataList;

        public PagedListExtensionsTests()
        {
            // fill datalist
            var tmpList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                tmpList.Add("testString" + i.ToString());
            }

            dataList = tmpList.AsEnumerable();
        }

        [Fact]
        public void ConvertEnumerableToPagedList_ConvertSuccessfully_ReturnPagedList()
        {
            // Act
            var result = dataList.ToPagedList(1,5);

            // Assert
            Assert.IsType<PagedList<string>>(result);
        }

        [Fact]
        public void ConvertQueryableToPagedList_ConvertSuccessfully_ReturnPagedList()
        {
            // Act
            var result = dataList.AsQueryable().ToPagedList(1, 5);

            // Assert
            Assert.IsType<PagedList<string>>(result);
        }

        [Fact]
        public void SplitEnumerable_ConvertSuccessfully_ReturnEnumerable()
        {
            // Act
            var result = dataList.Split(5);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<IEnumerable<string>>>(result);
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void PartitionEnumerable_ConvertSuccessfully_ReturnEnumerable()
        {
            // Act
            var result = dataList.Partition(5);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<IEnumerable<string>>>(result);
            Assert.Equal(System.Math.Ceiling((double)dataList.Count() / 5), result.Count());
        }

        [Fact]
        public void PartitionEnumerableWithLargePageSize_ConvertSuccessfully_ReturnEnumerable()
        {
            // Act
            var result = dataList.Partition(dataList.Count() * 2);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<IEnumerable<string>>>(result);
            Assert.Single(result);
        }
    }
}
