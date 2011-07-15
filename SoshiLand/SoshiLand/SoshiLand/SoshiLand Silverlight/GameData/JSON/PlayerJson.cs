using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newtonsoft.Json;

namespace SoshiLandSilverlight.GameData.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerJson
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int Money { get; set; }
        [JsonProperty]
        public int BoardPosition { get; set; }
    }
}
