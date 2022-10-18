using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Utilities.Dapper
{
    public static class Mapper
    {
        private static ConcurrentDictionary<string, (Type type, Dictionary<string, string> mapping)> masterMapping = new ConcurrentDictionary<string, (Type type, Dictionary<string, string> mapping)>();



        public static void AddMapping(string @namespace, Dictionary<string, string> mapping)
        {

            var types = from assem in AppDomain.CurrentDomain.GetAssemblies().ToList()
                        from type in assem.GetTypes()
                        where type.IsClass && (type.Namespace == @namespace || type.FullName == @namespace)
                        select type;

            types.ToList().ForEach(type =>
            {
                AddMapping(type, mapping);
            });
        }
        public static void AddMapping(Type type, Dictionary<string, string> mapping)
        {
            var nm = type.FullName;

            if (mapping == null)
            {
                mapping = new Dictionary<string, string>();
            }

            //nm = nm.ToUpper().Trim();
            if (!masterMapping.ContainsKey(nm))
            {
                masterMapping.TryAdd(nm, (type, mapping));
            }
            else
            {
                var map = masterMapping[nm].mapping;
                foreach (var key in mapping.Keys)
                {
                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, mapping[key]);
                    }
                }
            }
        }
        public static void AddMapping<TT>(Dictionary<string, string> mapping)
        {
            var type = typeof(TT);
            AddMapping(type, mapping);
        }



        public static void AddColumnMapping(string @namespace)
        {

            var types = from assem in AppDomain.CurrentDomain.GetAssemblies().ToList()
                        from type in assem.GetTypes()
                        where type.IsClass && (type.Namespace == @namespace || type.FullName == @namespace)
                        select type;

            types.ToList().ForEach(type =>
            {
                AddColumnMapping(type);
            });


        }
        public static void AddColumnMapping(Type type)
        {
            //AddColumnMapping(type.FullName);



            var atts = (from prop in type.GetProperties() from att in prop.GetCustomAttributes(false).OfType<ColumnAttribute>() select new { prop, att });

            var mapping = new Dictionary<string, string>();

            foreach (var it in atts)
            {
                mapping.Add(it.att.Name, it.prop.Name);
            }


            AddMapping(type, mapping);
        }
        public static void AddColumnMapping<TT>()
        {
            var type = typeof(TT);
            AddColumnMapping(type);
        }

        public static Dictionary<string, string> GetMapping(string @namespace)
        {

            var selectedtype = (from assem in AppDomain.CurrentDomain.GetAssemblies().ToList()
                                from type in assem.GetTypes()
                                where type.IsClass && type.FullName == @namespace
                                select type).FirstOrDefault();

            return GetMapping(selectedtype);



        }
        public static Dictionary<string, string> GetMapping(Type type)
        {
            var nm = type.FullName;

            if (!masterMapping.ContainsKey(nm))
            {
                masterMapping.TryAdd(nm, (type, new Dictionary<string, string>()));
            }

            return masterMapping[nm].mapping;
        }
        public static Dictionary<string, string> GetMapping<TT>()
        {

            var type = typeof(TT);
            return GetMapping(type);
        }



        public static void InitializeMapping()
        {
            foreach (var key in masterMapping.Keys)
            {

                var mapset = masterMapping[key];


                var oldMap = SqlMapper.GetTypeMap(mapset.type);
                var map = new CustomTypeMap(mapset.type, oldMap);
                foreach (var innerkey in mapset.mapping.Keys)
                {
                    map.Map(innerkey, mapset.mapping[innerkey]);
                }
                SqlMapper.SetTypeMap(map.Type, map);

            }
        }

    }
}
