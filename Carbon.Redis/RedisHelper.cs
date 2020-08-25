using FastMember;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Carbon.Redis
{
    public static class RedisHelper
    {
        public static string ConvertObjectToJson(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// Converts json to given object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        public static async Task<T> ConvertJsonToObjectAsync<T>(this IDatabase database, string key)
        {
            try
            {
                var value = await database.StringGetAsync(key);
                if (!value.IsNull)
                    return JsonConvert.DeserializeObject<T>(value);
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {

                throw new RedisException("The object couldn't deserialized",ex);
            }

        }
        /// <summary>
        /// Converts instance of an object to hash entry list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        /// <seealso cref="https://gist.github.com/ajai8085/364d076e7070c1677d1075467ae720e2#file-stackexchangeredisextensions-cs"/>
        public static IEnumerable<HashEntry> ConvertToHashEntryList<T>(this T instance) where T : new()
        {
            var acessor = TypeAccessor.Create(typeof(T));
            var members = acessor.GetMembers();

            for (var index = 0; index < members.Count; index++)
            {
                var member = members[index];
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute)))
                {
                    continue;
                }
                var type = member.Type;

                if (!type.IsValueType)//ATM supports only value types
                {
                    if (type != typeof(string))
                    {
                        continue;
                    }
                }

                var underlyingType = Nullable.GetUnderlyingType(type);
                var effectiveType = underlyingType ?? type;

                var val = acessor[instance, member.Name];

                if (val != null)
                {
                    if (effectiveType == typeof(DateTime))
                    {
                        var date = (DateTime)val;
                        if (date.Kind == DateTimeKind.Utc)
                        {

                            yield return new HashEntry(member.Name, $"{date.Ticks}|UTC");
                        }
                        else
                        {
                            yield return new HashEntry(member.Name, $"{date.Ticks}|LOC");
                        }
                    }
                    else
                    {
                        yield return new HashEntry(member.Name, val.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Converts from hash entry list and create instance of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entries">The entries returned from StackExchange.redis.</param>
        /// <returns>Instance of Type T </returns>
        /// <see cref="https://gist.github.com/ajai8085/364d076e7070c1677d1075467ae720e2#file-stackexchangeredisextensions-cs"/>
        public static T ConvertFromHashEntryList<T>(this IEnumerable<HashEntry> entries) where T : new()
        {
            var acessor = TypeAccessor.Create(typeof(T));
            var instance = new T();
            var hashEntries = entries as HashEntry[] ?? entries.ToArray();
            var members = acessor.GetMembers();
            for (var index = 0; index < members.Count; index++)
            {
                var member = members[index];

                if (member.IsDefined(typeof(IgnoreDataMemberAttribute)))
                {
                    continue;
                }

                var type = member.Type;

                if (!type.IsValueType) //ATM supports only value types
                {
                    if (type != typeof(string))
                    {
                        continue;
                    }
                }
                var underlyingType = Nullable.GetUnderlyingType(type);
                var effectiveType = underlyingType ?? type;


                var entry = hashEntries.FirstOrDefault(e => e.Name.ToString().Equals(member.Name));

                if (entry.Equals(new HashEntry()))
                {
                    continue;
                }

                var value = entry.Value.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (effectiveType == typeof(DateTime))
                {
                    if (value.EndsWith("|UTC"))
                    {
                        value = value.TrimEnd("|UTC".ToCharArray());
                        DateTime date;

                        long ticks;
                        if (long.TryParse(value, out ticks))
                        {
                            date = new DateTime(ticks);
                            acessor[instance, member.Name] = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                        }
                    }
                    else
                    {
                        value = value.TrimEnd("|LOC".ToCharArray());
                        DateTime date;
                        long ticks;
                        if (long.TryParse(value, out ticks))
                        {
                            date = new DateTime(ticks);
                            acessor[instance, member.Name] = DateTime.SpecifyKind(date, DateTimeKind.Local);
                        }
                    }

                }
                else
                {

                    if (member.Type == typeof(Guid))
                    {
                        acessor[instance, member.Name] = Guid.Parse(entry.Value.ToString());
                    }
                    else if (member.Type.BaseType == typeof(Enum))
                    {
                        acessor[instance, member.Name] = Enum.Parse(member.Type, entry.Value.ToString());
                    }
                    else
                        acessor[instance, member.Name] = Convert.ChangeType(entry.Value.ToString(), member.Type);
                }
            }
            return instance;
        }

        public static (bool isValid, string error) IsKeyValid(this string key, IDatabase _redisDb)
        {
            var (keyLength, error) = GetKeyLength(_redisDb);
            if (error !=null)
            {
                return (false, error);
            }
            if (key.Length > keyLength)
            {
                return (false, $"key lenght should be smaller than {keyLength}");
            }
            if (!key.Contains(":"))
            {
                return (false, "key should contains ':' character between meaningful seperations. The sample key is object-type:id:field(user:100:password)");
            }
            return (true, null);
        }
        private static (int keyLength, string error) GetKeyLength(this IDatabase db)
        {
            var keyLength = RedisSettingConstants.KeyLength;
            try
            {
                var value = db.StringGet(RedisSettingConstants.RedisKeyLengthKey);
                if (value.HasValue && !value.IsNullOrEmpty && value != 0)
                {
                    keyLength = (int)value;
                }
            }
            catch (Exception ex)
            {

                return (default, ex.InnerException?.Message??ex.Message);
            }

            return (keyLength,null);
        }
        public static bool IsRedisDisabled(this IDatabase db)
        {
            if (db.Multiplexer == null)
            {
                return true;
            }
            return false;
        }
    }
}
