using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using StackExchange.Redis;
using Xunit.Sdk;

namespace Carbon.Redis.UnitTests.DataShares
{
    public class ConvertObjectToJsonValidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { new object() };
        }
    }

    public class SendNullInvalidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { null };
        }
    }

    public class SendKeyValidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "key:" };
        }
    }
    public class SendEmptyKeyData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "" };
        }
    }

    public class SetValidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            object obj = "\"test\": \"test\"";
            yield return new object[] { "key:", obj };
        }
    }

    public class SendHugeKeyInvalidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {

            yield return new object[]
            {
                "eA5HeCwvajTgME0y8cAocD6TdPa5qGGy6N4QJnisX7qs53DQGv8QTkOLqbFv25bIId7ccomIKtVd5dZptFfv0lsYAGJCRdZpj3p17ZxGua04YpDtwoTVn3HaPUqcs5vLjxq6ewk9inR03NfSeXLGx0Q4c1L4UBsJA1TY0R4ojy1up5zbKOmzXJbudqMDcDoQUgy19BwvffeoPvxWV5IWRjo7MTHP011ESOw3eJ03AnCOwDDJvCQEmSxWrdLl0ZMRDkFOuHGjsx7l718SKiof5HgU4aCgckUCPcLcdmzoSdajFKRzkpIJginm2G4HC66FmcCda6SQfnVZ1nsiVTEztZA5ZbMH9fqL2nHww5TgP6MYsq9RUdAz7mYLHsaXfUJ9NkEtiS6NmFTgXOTzRFxSUvffW8xidQICIjZNpkm5vCVMvyBSXdXxtC91eUT8hf4rg5Y8CKZ42tsd2YmKaBWa4ijvQoKC0S0LEhTFQEt2mFXhBsJv9Uo43:"
            };
        }
    }
    public class ConvertJsonToObjectExceptionalData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { null, "key:" };
        }
    }

    public class SendWrongSyntaxedKeyInvalidData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "key" };
        }
    }

    public class RemoveKeysByPatternData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            IDatabase db = new RedisDatabase();
            IConnectionMultiplexer connection = new DummyConnectionMultiplexer();
            yield return new object[] { db, "keyPattern", connection };
        }
    }
    
    public class AddRedisPersisterServicesIsNullData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "services" };
        }
    }

    public class AddRedisPersisterConfigurationIsNullData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "configuration" };
        }
    }

    public class ConvertToMD5Data : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "123456" };
        }
    }
}
