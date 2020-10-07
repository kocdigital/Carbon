using Carbon.PagedList;
using Carbon.Test.Common.DataShares;
using Carbon.Test.Common.Fixtures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Carbon.PageList.Mapster.UnitTests.DataShares
{

        public class AdaptQueryableExtensions : DataAttribute
        {
            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                
                var list = new List<CarbonContextTestClass>();
                var item = new CarbonContextTestClass();
                item.Id = Guid.NewGuid();
                item.TenantId = Guid.NewGuid();
            
                list.Add(item);
                IPagedList<CarbonContextTestClass> pagedList = new PagedList<CarbonContextTestClass>(list,1,3);


                //data
            yield return new object[] { pagedList };

            }
        }


    
}
