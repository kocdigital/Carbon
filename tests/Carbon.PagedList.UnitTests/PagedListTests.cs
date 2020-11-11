using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Carbon.PagedList.UnitTests
{
    public class PagedListTests
    {

        private readonly IEnumerable<string> dataList;

        public PagedListTests()
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
        public void CreatePagedList_CreateWithEnumerable_ReturnPagedList()
        {
            // Act
            var x = new PagedList<string>(dataList, 1, 5);

            // Assert
            Assert.IsType<PagedList<string>>(x);
        }

        [Fact]
        public void CreatePagedList_CreateWithQueryable_ReturnPagedList()
        {
            // Act
            var x = new PagedList<string>(dataList.AsQueryable(), 1, 5);

            // Assert
            Assert.IsType<PagedList<string>>(x);
        }

        [Fact]
        public void CreatePagedList_InvalidPageSize_ThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<string>(dataList.AsQueryable(), 1, 0));

        }

        [Fact]
        public void CreatePagedList_InvalidPageNumber_ThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<string>(dataList.AsQueryable(), 0, 5));
        }

        [Fact]
        public void GetEnumerator_GetSuccessfully_ReturnEnumerator()
        {
            // Arrange
            var x = new PagedList<string>(dataList, 1, 5);

            // Act
            var enumerator = x.GetEnumerator();

            // Assert
            Assert.IsAssignableFrom<IEnumerator<string>>(enumerator);
        }

        [Fact]
        public void GetCount_GetSuccessfully_ReturnInt()
        {
            // Arrange
            var emptyList = new List<string>();
            var x = new PagedList<string>(emptyList, 1, 5);

            // Act
            var count = x.Count;

            // Assert
            Assert.IsType<int>(count);
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetMetaData_GetSuccessfully_ReturnMetaData()
        {
            // Arrange
            var x = new PagedList<string>(dataList, 1, 5);

            // Act
            var mData = x.GetMetaData();

            // Assert
            Assert.IsType<PagedListMetaData>(mData);
        }

        [Fact]
        public void GetDataWithIndex_GetSuccessfully_ReturnElement()
        {
            // Arrange
            var index = 4;
            var x = new PagedList<string>(dataList, 1, 5);

            // Act
            var retrievedItem = x[index];

            // Assert
            Assert.Equal(dataList.ElementAt(index), retrievedItem);
        }
    }
}
