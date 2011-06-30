using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLand
{
    enum Props
    {
        None,
        LaScala,
        Bali,
        TempleMount,
        DamnoenMarket,
        GreatWall,
        TajMahal,
        StatueLiberty,
        EiffelTower,
        Parthenon,
        WhiteHouse,
        GyeongBokGoong,
        MountEverest,
        GrandCanal,
        VenetianResort,
        ChateauDeChillon,
        TokyoDome,
        Colosseum,
        BlueHouse,
        BukitTimah,
        CNTower,
        KuwaitMuseum,
        WalkOfFame,
        AngkorWat,
        Disneyland,
        AmazonRainforest,
        HongKong,
        UNHQ,
        SydneyOpera,
        GoldenGateBridge,
        MalibuBeach,
        BarcelonaAirport,
        WencelsasSquare,
        BarrierReef,
        Pisa,
        BigBen,
        GizaPyramid,
        Chance1,
        Forever9,
        CommChest,
        ShoppingSpree,
        LuxuryTax,
        Chance2,
        SoshiBond
    };

    public enum TileType
    {
        Go,
        Property,
        Chance,
        CommunityChest,
        Jail,
        SpecialLuxuryTax,
        ShoppingSpree,
        Utility,
        FanMeeting,
        GoToJail
    };

    public enum SpecialCardType
    {
        None,
        GetOutOfJailFreeCard,
        GoToJailCard,
        CanPassGo
    };
}
