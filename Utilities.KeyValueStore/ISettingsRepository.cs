using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.KeyValueStore
{
    public interface ISettingsRepository
    {
        void SetValue(string name, string value);
        void SetValue(string name, bool value);
        void SetValue<TT>(string name, TT value) where TT : class;
        void SetValueSeperate<TT>(string name, TT value) where TT : class;
        TT GetValue<TT>(string name, TT defaultValue = null) where TT : class;
        TT GetValueSeperate<TT>(string name, TT defaultValue = null) where TT : class;
        string GetValueString(string name, string defaultValue = "");
        bool GetValueBool(string name, bool defaultValue = false);

        decimal GetValueDecimal(string name, decimal defaultValue = 0);

    }
}
