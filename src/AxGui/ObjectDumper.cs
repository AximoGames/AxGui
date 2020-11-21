// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SkiaSharp;

namespace AxGui
{
    /// <summary>
    /// For internal debugging only.
    /// TODO: Exclude in Relase-Build
    /// </summary>
    internal static class ObjectDumper
    {

        public static string Dump(object obj)
        {
            if (obj == null)
                return "";

            var settings = new JsonSerializerSettings() {
                ContractResolver = new MyContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        private class MyContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f=>!typeof(Delegate).IsAssignableFrom(f.FieldType))
                    .Select(f => base.CreateProperty(f, memberSerialization))
                    .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }

    }

}
