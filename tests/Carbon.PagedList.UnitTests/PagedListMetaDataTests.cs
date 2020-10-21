using System.Collections.Generic;
using Xunit;

namespace Carbon.PagedList.UnitTests
{
    public class PagedListMetaDataTests
    {

        private readonly IPagedList dataList;

        public PagedListMetaDataTests() 
        {
            // fill datalist
            var tmpList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                tmpList.Add("testString" + i.ToString());
            }

            dataList = tmpList.ToPagedList(1,5);
        }

        [Fact]
        public void CreateWithPagedList_CreateSuccessfully_ReturnMetaData()
        {
            // Act
            var mData = new PagedListMetaData(dataList);

            // Assert
            Assert.IsType<PagedListMetaData>(mData);
            Assert.Equal(dataList.PageCount, mData.PageCount);
        }
    }
}
