using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Player;
using PokerHand.Common.Helpers.Present;
using System;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static string ToCombinationString(this HandType handType)
    {
        switch (handType)
        {
            case HandType.None:         return string.Empty;
            case HandType.HighCard:     return GameConstants.HighValue;
            case HandType.OnePair:      return GameConstants.Pair;
            case HandType.TwoPairs:     return GameConstants.TwoPairs;
            case HandType.ThreeOfAKind: return GameConstants.ThreeOfAKind;
            case HandType.Straight:     return GameConstants.Straight;
            case HandType.Flush:        return GameConstants.Flush;
            case HandType.FullHouse:    return GameConstants.FullHouse;
            case HandType.FourOfAKind:  return GameConstants.FourOfAKind;
            case HandType.StraightFlush:return GameConstants.StraightFlush;
            case HandType.RoyalFlush:   return GameConstants.RoyalFlush;
            case HandType.FiveOfAKind:   return GameConstants.FiveOfAKind;
            default: return string.Empty;
        }
    }

    public static int CardsCombinationCount(this HandType handType)
    {
        switch (handType)
        {
            case HandType.None:             return 0;
            case HandType.HighCard:         return 1;
            case HandType.OnePair:          return 2;
            case HandType.TwoPairs:         return 4;
            case HandType.ThreeOfAKind:     return 3;
            case HandType.Straight:         return 5;
            case HandType.Flush:            return 5;
            case HandType.FullHouse:        return 5;
            case HandType.FourOfAKind:      return 4;
            case HandType.StraightFlush:    return 5;
            case HandType.RoyalFlush:       return 5;
            case HandType.FiveOfAKind:      return 5;
            default:                        return 0;
        }
    }

    public static CardDto ToCardDto(this Card card)
    {
        var cardDto = new CardDto();
        cardDto.Suit = (CardSuitType)card.suite;
        cardDto.Rank = (CardRankType)card.value;
        return cardDto;
    }

    public static bool IsActive(this PlayerDto player) => Client.TableData.ActivePlayers.Any(t => t.Id == player.Id);

    public static CountryCode ToCountryCode(this string country)
    {
        switch(country)
        {
            case "ad": return CountryCode.AD;
            case "ae": return CountryCode.AE;
            case "af": return CountryCode.AF;
            case "ag": return CountryCode.AG;
            case "ai": return CountryCode.AI;
            case "al": return CountryCode.AL;
            case "am": return CountryCode.AM;
            case "ao": return CountryCode.AO;
            case "aq": return CountryCode.AQ;
            case "ar": return CountryCode.AR;
            case "as": return CountryCode.AS;
            case "at": return CountryCode.AT;
            case "au": return CountryCode.AU;
            case "aw": return CountryCode.AW;
            case "ax": return CountryCode.AX;
            case "az": return CountryCode.AZ;
            case "ba": return CountryCode.BA;
            case "bb": return CountryCode.BB;
            case "bd": return CountryCode.BD;
            case "be": return CountryCode.BE;
            case "bf": return CountryCode.BF;
            case "bg": return CountryCode.BG;
            case "bh": return CountryCode.BH;
            case "bi": return CountryCode.BI;
            case "bj": return CountryCode.BJ;
            case "bl": return CountryCode.BL;
            case "bm": return CountryCode.BM;
            case "bn": return CountryCode.BN;
            case "bo": return CountryCode.BO;
            case "bq": return CountryCode.BQ;
            case "br": return CountryCode.BR;
            case "bs": return CountryCode.BS;
            case "bt": return CountryCode.BT;
            case "bv": return CountryCode.BV;
            case "bw": return CountryCode.BW;
            case "by": return CountryCode.BY;
            case "bz": return CountryCode.BZ;
            case "ca": return CountryCode.CA;
            case "cc": return CountryCode.CC;
            case "cd": return CountryCode.CD;
            case "cf": return CountryCode.CF;
            case "cg": return CountryCode.CG;
            case "ch": return CountryCode.CH;
            case "ci": return CountryCode.CI;
            case "ck": return CountryCode.CK;
            case "cl": return CountryCode.CL;
            case "cm": return CountryCode.CM;
            case "cn": return CountryCode.CN;
            case "co": return CountryCode.CO;
            case "cr": return CountryCode.CR;
            case "cu": return CountryCode.CU;
            case "cv": return CountryCode.CV;
            case "cw": return CountryCode.CW;
            case "cx": return CountryCode.CX;
            case "cy": return CountryCode.CY;
            case "cz": return CountryCode.CZ;
            case "de": return CountryCode.DE;
            case "dj": return CountryCode.DJ;
            case "dk": return CountryCode.DK;
            case "dm": return CountryCode.DM;
            case "do": return CountryCode.DO;
            case "dz": return CountryCode.DZ;
            case "ec": return CountryCode.EC;
            case "ee": return CountryCode.EE;
            case "eg": return CountryCode.EG;
            case "eh": return CountryCode.EH;
            case "er": return CountryCode.ER;
            case "es": return CountryCode.ES;
            case "et": return CountryCode.ET;
            case "fi": return CountryCode.FI;
            case "fj": return CountryCode.FJ;
            case "fk": return CountryCode.FK;
            case "fm": return CountryCode.FM;
            case "fo": return CountryCode.FO;
            case "fr": return CountryCode.FR;
            case "ga": return CountryCode.GA;
            case "gb_eng": return CountryCode.GB_ENG;
            case "gb_nir": return CountryCode.GB_NIR;
            case "gb_sct": return CountryCode.GB_SCT;
            case "gb_wls": return CountryCode.GB_WLS;
            case "gb": return CountryCode.GB;
            case "gd": return CountryCode.GD;
            case "ge": return CountryCode.GE;
            case "gf": return CountryCode.GF;
            case "gg": return CountryCode.GG;
            case "gh": return CountryCode.GH;
            case "gi": return CountryCode.GI;
            case "gl": return CountryCode.GL;
            case "gm": return CountryCode.GM;
            case "gn": return CountryCode.GN;
            case "gp": return CountryCode.GP;
            case "gq": return CountryCode.GQ;
            case "gr": return CountryCode.GR;
            case "gs": return CountryCode.GS;
            case "gt": return CountryCode.GT;
            case "gu": return CountryCode.GU;
            case "gw": return CountryCode.GW;
            case "gy": return CountryCode.GY;
            case "hk": return CountryCode.HK;
            case "hm": return CountryCode.HM;
            case "hn": return CountryCode.HN;
            case "hr": return CountryCode.HR;
            case "ht": return CountryCode.HT;
            case "hu": return CountryCode.HU;
            case "id": return CountryCode.ID;
            case "ie": return CountryCode.IE;
            case "il": return CountryCode.IL;
            case "im": return CountryCode.IM;
            case "in": return CountryCode.IN;
            case "io": return CountryCode.IO;
            case "iq": return CountryCode.IQ;
            case "ir": return CountryCode.IR;
            case "is": return CountryCode.IS;
            case "it": return CountryCode.IT;
            case "je": return CountryCode.JE;
            case "jm": return CountryCode.JM;
            case "jo": return CountryCode.JO;
            case "jp": return CountryCode.JP;
            case "ke": return CountryCode.KE;
            case "kg": return CountryCode.KG;
            case "kh": return CountryCode.KH;
            case "ki": return CountryCode.KI;
            case "km": return CountryCode.KM;
            case "kn": return CountryCode.KN;
            case "kp": return CountryCode.KP;
            case "kr": return CountryCode.KR;
            case "kw": return CountryCode.KW;
            case "ky": return CountryCode.KY;
            case "kz": return CountryCode.KZ;
            case "la": return CountryCode.LA;
            case "lb": return CountryCode.LB;
            case "lc": return CountryCode.LC;
            case "li": return CountryCode.LI;
            case "lk": return CountryCode.LK;
            case "lr": return CountryCode.LR;
            case "ls": return CountryCode.LS;
            case "lt": return CountryCode.LT;
            case "lu": return CountryCode.LU;
            case "lv": return CountryCode.LV;
            case "ly": return CountryCode.LY;
            case "ma": return CountryCode.MA;
            case "mc": return CountryCode.MC;
            case "md": return CountryCode.MD;
            case "me": return CountryCode.ME;
            case "mf": return CountryCode.MF;
            case "mg": return CountryCode.MG;
            case "mh": return CountryCode.MH;
            case "mk": return CountryCode.MK;
            case "ml": return CountryCode.ML;
            case "mm": return CountryCode.MM;
            case "mn": return CountryCode.MN;
            case "mo": return CountryCode.MO;
            case "mp": return CountryCode.MP;
            case "mq": return CountryCode.MQ;
            case "mr": return CountryCode.MR;
            case "ms": return CountryCode.MS;
            case "mt": return CountryCode.MT;
            case "mu": return CountryCode.MU;
            case "mv": return CountryCode.MV;
            case "mw": return CountryCode.MW;
            case "mx": return CountryCode.MX;
            case "my": return CountryCode.MY;
            case "mz": return CountryCode.MZ;
            case "na": return CountryCode.NA;
            case "nc": return CountryCode.NC;
            case "ne": return CountryCode.NE;
            case "nf": return CountryCode.NF;
            case "ng": return CountryCode.NG;
            case "ni": return CountryCode.NI;
            case "nl": return CountryCode.NL;
            case "no": return CountryCode.NO;
            case "np": return CountryCode.NP;
            case "nr": return CountryCode.NR;
            case "nu": return CountryCode.NU;
            case "nz": return CountryCode.NZ;
            case "om": return CountryCode.OM;
            case "pa": return CountryCode.PA;
            case "pe": return CountryCode.PE;
            case "pf": return CountryCode.PF;
            case "pg": return CountryCode.PG;
            case "ph": return CountryCode.PH;
            case "pk": return CountryCode.PK;
            case "pl": return CountryCode.PL;
            case "pm": return CountryCode.PM;
            case "pn": return CountryCode.PN;
            case "pr": return CountryCode.PR;
            case "ps": return CountryCode.PS;
            case "pt": return CountryCode.PT;
            case "pw": return CountryCode.PW;
            case "py": return CountryCode.PY;
            case "qa": return CountryCode.QA;
            case "re": return CountryCode.RE;
            case "ro": return CountryCode.RO;
            case "rs": return CountryCode.RS;
            case "ru": return CountryCode.RU;
            case "rw": return CountryCode.RW;
            case "sa": return CountryCode.SA;
            case "sb": return CountryCode.SB;
            case "sc": return CountryCode.SC;
            case "sd": return CountryCode.SD;
            case "se": return CountryCode.SE;
            case "sg": return CountryCode.SG;
            case "sh": return CountryCode.SH;
            case "si": return CountryCode.SI;
            case "sj": return CountryCode.SJ;
            case "sk": return CountryCode.SK;
            case "sl": return CountryCode.SL;
            case "sm": return CountryCode.SM;
            case "sn": return CountryCode.SN;
            case "so": return CountryCode.SO;
            case "sr": return CountryCode.SR;
            case "ss": return CountryCode.SS;
            case "st": return CountryCode.ST;
            case "sv": return CountryCode.SV;
            case "sx": return CountryCode.SX;
            case "sy": return CountryCode.SY;
            case "sz": return CountryCode.SZ;
            case "tc": return CountryCode.TC;
            case "td": return CountryCode.TD;
            case "tf": return CountryCode.TF;
            case "tg": return CountryCode.TG;
            case "th": return CountryCode.TH;
            case "tj": return CountryCode.TJ;
            case "tk": return CountryCode.TK;
            case "tl": return CountryCode.TL;
            case "tm": return CountryCode.TM;
            case "tn": return CountryCode.TN;
            case "to": return CountryCode.TO;
            case "tr": return CountryCode.TR;
            case "tt": return CountryCode.TT;
            case "tv": return CountryCode.TV;
            case "tw": return CountryCode.TW;
            case "tz": return CountryCode.TZ;
            case "ua": return CountryCode.UA;
            case "ug": return CountryCode.UG;
            case "um": return CountryCode.UM;
            case "us": return CountryCode.US;
            case "uy": return CountryCode.UY;
            case "uz": return CountryCode.UZ;
            case "va": return CountryCode.VA;
            case "vc": return CountryCode.VC;
            case "ve": return CountryCode.VE;
            case "vg": return CountryCode.VG;
            case "vi": return CountryCode.VI;
            case "vn": return CountryCode.VN;
            case "vu": return CountryCode.VU;
            case "wf": return CountryCode.WF;
            case "wo": return CountryCode.WO;
            case "ws": return CountryCode.WS;
            case "xk": return CountryCode.XK;
            case "ye": return CountryCode.YE;
            case "yt": return CountryCode.YT;
            case "za": return CountryCode.ZA;
            case "zm": return CountryCode.ZM;
            case "zw": return CountryCode.ZW;
            default: return CountryCode.None;
        }
    }

    public static bool TryGetPlayerProfileLocalIndex(this PlayerProfileDto playerProfileDto, out int localIndex)
    {
        if (Client.TableData.Players.Any(t => t.Id == playerProfileDto.Id))
        {
            localIndex = Client.TableData.Players.First(t => t.Id == playerProfileDto.Id).LocalIndex;
            return true;
        }
        else
        {
            localIndex = -1;
            return false;
        }
    }    

    public static Sprite GetGiftSpriteByName(this GiftInfo[] giftsInfo, PresentName presentName)
    {
        for (var i = 0; i < giftsInfo.Length; i++)
        {
            if (giftsInfo[i].Name == presentName)
                return giftsInfo[i].Sprite;
        }
        return null;
    }

    // Other extensions
    public static Texture2D ScaleTexture(this Texture2D source, int targetWidth, int targetHeight)
    {
        var result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        var incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        var incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (var px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                            incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    public static PlayerWithRankDto ExpandToPlayerRank(this RankDto rankDto)
    {
        return new PlayerWithRankDto()
        {
            Id = PlayerProfileData.Instance.Id,
            UserName = PlayerProfileData.Instance.Nickname,
            Country = PlayerProfileData.Instance.Country,
            Experience = PlayerProfileData.Instance.Experience,
            TotalMoney = PlayerProfileData.Instance.TotalMoney,
            Rank = rankDto.Rank != -1 ? rankDto.Rank : -rankDto.RankPercentage,
            BinaryImage = Convert.ToBase64String(Client.ProfileImages[0].EncodeToPNG())
        };
    }

    public static Sprite LoadCountryFlagSprite(this CountryCode countryCode)
    {
        string countryPucturePath;
        if (countryCode == CountryCode.None)
            countryPucturePath = "Images/Flags/wo";
        else
            countryPucturePath = $"Images/Flags/{countryCode.ToString().ToLower()}";
        return Resources.Load<Sprite>(countryPucturePath);
    }

    public static string LoadCountryCodeString(this CountryCode countryCode)
    {
        if (countryCode != CountryCode.None)
            return countryCode.ToString();
        else
            return "WO";
    }
}