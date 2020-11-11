using Xunit;
using System.Collections.Generic;
using System;

namespace Carbon.PagedList.UnitTests
{
    public class BasePagedListTests
    {
        public BasePagedListTests()
        {

        }

        [Fact]
        public void CreateBasePagedList_CreateWithNoArguments_ReturnBasePagedList()
        {
            // Act
            var x = new TestBasePagedList<string>();

            // Assert
            Assert.IsAssignableFrom<BasePagedList<string>>(x);
        }

        [Fact]
        public void CreateBasePagedList_CreateWithArguments_ReturnBagePagedList()
        {
            // Act
            var x = new TestBasePagedList<string>(2, 3, 15);

            // Assert
            Assert.IsAssignableFrom<BasePagedList<string>>(x);
        }

        [Fact]
        public void CreateBasePagedList_InvalidPageNumber_ThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestBasePagedList<string>(0, 1, 1));
        }

        [Fact]
        public void CreateBasePagedList_InvalidPageSize_ThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestBasePagedList<string>(4, 0, 1));
        }

        [Fact]
        public void GetEnumerator_GetSuccessfully_ReturnEnumerator()
        {
            // Arrange
            var x = new TestBasePagedList<string>();

            // Act
            var enumerator = x.GetEnumerator();

            // Assert
            Assert.IsAssignableFrom<IEnumerator<string>>(enumerator);
        }

        [Fact]
        public void GetCount_GetSuccessfully_ReturnInt()
        {
            // Arrange
            var x = new TestBasePagedList<string>();

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
            var x = new TestBasePagedList<string>();

            // Act
            var mData = x.GetMetaData();

            // Assert
            Assert.IsType<PagedListMetaData>(mData);
        }

        [Fact]
        public void InsertData_RetrieveSuccessfully_ReturnElement()
        {
            // Arrange
            var testItem = "testString";
            var times = 5;
            var x = new TestBasePagedList<string>(testItem, times);

            // Act
            var retrievedItem = x[times-1];

            // Assert
            Assert.Equal(testItem, retrievedItem);
        }
    }

    class TestBasePagedList<T>: BasePagedList<T>
    {
        public TestBasePagedList(): base()
        {

        }

        public TestBasePagedList(int pageNumber, int pageSize, int totalItemCount) : base(pageNumber, pageSize, totalItemCount)
        {
        }

        public TestBasePagedList(T item, int count)
        {
            while(this.Subset.Count < count) this.Subset.Add(item);
        }
    }
}
