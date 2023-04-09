using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMSCommon.Model;

namespace BiblePay.BMS.DSQL
{
        public static class SessionExtensions
        {
            public static void SetObject(this ISession session, string key, object value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }

            public static T GetObject<T>(this ISession session, string key)
            {
                var value = session.GetString(key);
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }

            public static bool ObjectExists(this ISession session, string key)
            {
                var value = session.GetString(key);
                return (value == null ? false : true);
            }

            public static string GetFormValue(this ISession session, string key)
            {
                string sData = String.Empty;
                ClientToServer cts = session.GetObject<ClientToServer>("formdata");
                if (cts != null)
                {
                     sData = DOMItem.GetFormData(cts.FormData, "txtName");
                }
                return sData;
            }

    }


}
