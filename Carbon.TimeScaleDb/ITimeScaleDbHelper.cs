using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.TimeScaleDb
{
    public interface ITimeScaleDbHelper
    {
        public bool CheckTimeScaleDbSupport();
        public bool ConvertTableToTimeSeriesDb(string tableName, string timeColumnName);
        public bool AddTimeScaleDbExtensionToDatabase();
    }
}
