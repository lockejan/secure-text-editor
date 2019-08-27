
//
//var Dict = new Dictionary<BlockMode, Padding>(){
//    {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
//    {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
//}
//
//Enum.hasFlag("keyausDict",Padding.NoPadding)
//Enum.hasFlag("keyausDict",Padding.NoPadding)
//
//enum BlockMode
//{
//    ECB,
//    CBC,
//    GCM,
//    OFB,
//    CTS
//}
//
//[Flag]
//enum Padding
//{
//    NoPadding = 1,
//    ZeroBytePadding = 1 << 1,
//    PKCS7 = 1 << 2
//}
//
//

//
//// [JsonProperty(Required = Required.Default)]
//// private string Signature { get; }
//
//// string json = JsonConvert.SerializeObject(model,Formatting.Indented,
////         new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
//
//
//
//_cipherComboBox.PropertySelectedItem.PropertyChanged += (s, e) => 
//        {
//            var selectedItem = _cipherComboBox.SelectedItem;
//var title = selectedItem?.Title;
//
//            if(title == null)
//            return;
//
//            if(title == "CBC" || title == )
//        };
//
//        public static string ToJson(this object obj, Formatting formatting = Formatting.None)
//{
//    var jsonSerializerSettings = new JsonSerializerSettings
//    {
//        NullValueHandling = NullValueHandling.Ignore,
//        TypeNameHandling = TypeNameHandling.Auto,
//        Formatting = formatting
//    };
//
//    return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
//}
//
//public static T FromJson<T>(this string json)
//{
//    var jsonSerializerSettings = new JsonSerializerSettings
//    {
//        NullValueHandling = NullValueHandling.Ignore,
//        TypeNameHandling = TypeNameHandling.Auto,
//    };
//
//    return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
//}
//
//var settings = new Settings();
//var json = settings.ToJson();
//
//
//class Settings
//{
//    CipherType Type { get; set; }
//    int KeyLength { get; set; }
//    Padding? Padding { get; set; }
//    BlockMode BlockMode { get; set; }
//
//    //[JsonIgnore]
//    char[] Password { get; set; }
//}
//
//enum Padding
//{
//    ZeroByte,
//    PKC7
//}
//
//
//enum CipherType
//{
//    AES,
//    ECB
//}
//
//enum BlockMode
//{
//    None
//}
//
