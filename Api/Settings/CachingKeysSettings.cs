namespace Api.Settings
{
    public class CachingKeysSettings
    {
        public  List<KeyItem> Keys {get;set;}
    }

    public class KeyItem {

        public string Key {get;set;}
        public string Val {get;set;} 
    }
}