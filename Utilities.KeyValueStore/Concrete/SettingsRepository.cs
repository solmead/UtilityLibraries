using Utilities.SerializeExtensions;
using Utilities.Poco;

namespace Utilities.KeyValueStore.Concrete
{
    public class SettingsRepository : ISettingsRepository
    {
        private IKeyValueRepository? _keyValueRepository;

        public IKeyValueRepository? KeyValueRepository
        {
            get { return _keyValueRepository; }
            set { _keyValueRepository = value; }
        }

        public SettingsRepository()
        {

        }
        public SettingsRepository(IKeyValueRepository keyValueRepository)
        {
            _keyValueRepository = keyValueRepository;
        }


        public virtual void SetValue(string name, string value)
        {
            _keyValueRepository.SetSettingValue(name, value);
        }
        public virtual void SetValue<TT>(string name, TT value) where TT : class
        {
            var s = new Serializer();
            var sData = s.Serialize(value);
            _keyValueRepository.SetSettingValue(name, sData);
        }

        public virtual void SetValue(string name, bool value)
        {
            _keyValueRepository.SetSettingValue(name, value.ToString());
        }

        public virtual void SetValueSeperate<TT>(string name, TT value) where TT : class
        {
            foreach (var p in value.GetPropertyNames())
            {
                _keyValueRepository.SetSettingValue(name + "_" + p, value?.GetValue(p)?.ToString() ?? "");
            }
        }


        public virtual TT GetValue<TT>(string name, TT defaultValue = null) where TT : class
        {
            var s = new Serializer();
            var sData = s.Serialize(defaultValue);
            var sd = _keyValueRepository.GetSettingValue(name, sData);
            var nData = s.Deserialize<TT>(sd);
            return nData;
        }


        public virtual TT GetValueSeperate<TT>(string name, TT defaultValue = null) where TT : class
        {
            var newObj = Extensions.Create<TT>();
            foreach (var p in newObj.GetPropertyNames())
            {
                var dt = _keyValueRepository.GetSettingValue(name + "_" + p, defaultValue?.GetValue(p)?.ToString() ?? "");

                newObj.SetValue(p, dt);
            }

            return newObj;
        }


        public virtual string GetValueString(string name, string defaultValue = "")
        {
            return _keyValueRepository.GetSettingValue(name, defaultValue);
        }
        public virtual bool GetValueBool(string name, bool defaultValue = false)
        {
            var s = GetValueString(name, defaultValue.ToString());

            return bool.TryParse(s, out bool b) ? b : defaultValue;
        }

    }
}
