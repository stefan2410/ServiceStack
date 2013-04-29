﻿using System.Collections.Generic;
using System.Dynamic;
using ServiceStack.Html;
using ServiceStack.ServiceHost;

namespace ServiceStack.Razor
{
    public class DynamicRequestObject : DynamicDictionary
    {
        public IHttpRequest httpReq { get; set; }

        public DynamicRequestObject() { }
        public DynamicRequestObject(IHttpRequest httpReq)
        {
            this.httpReq = httpReq;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = httpReq.GetParam(binder.Name);
            return result != null || base.TryGetMember(binder, out result);
        }
    }

    public class DynamicDictionary : System.Dynamic.DynamicObject
    {
        readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();
            if (!dictionary.TryGetValue(name, out result))
                result = null;

            return true;
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }
    }

    public static class DynamicUtils
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}