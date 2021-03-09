using System;
using LaunchPad.Mobile.Enums;
using LaunchPad.Mobile.Models;
using Xamarin.Forms;

namespace LaunchPad.Mobile.Helpers
{
    public static class ConcernHelper
    {
        public static ConcernItem GetConcernDetails(ConcernType annoType)
        {
            switch (annoType)
            {
                case ConcernType.LinesWrinkles:
                    return new ConcernItem { Color = Color.FromHex("#E77878"), Description = "Lines and Wrinkles" };
                case ConcernType.DarkSpots:
                    return new ConcernItem { Color = Color.FromHex("#E6AD76"), Description = "Dark Spots" };
                case ConcernType.EyeArea:
                    return new ConcernItem { Color = Color.FromHex("#E8D27C"), Description = "Eye Area" };
                case ConcernType.DrynessDehydration:
                    return new ConcernItem { Color = Color.FromHex("#D0E675"), Description = "Dryness/Dehydration" };
                case ConcernType.FirmingLifting:
                    return new ConcernItem { Color = Color.FromHex("#B4EB90"), Description = "Firming/Lifting" };
                case ConcernType.RednessSensitivity:
                    return new ConcernItem { Color = Color.FromHex("#85E985"), Description = "Redness/Sensitivity" };
                case ConcernType.SunDamage:
                    return new ConcernItem { Color = Color.FromHex("#77E7A4"), Description = "Sun Damage" };
                case ConcernType.VisiblePores:
                    return new ConcernItem { Color = Color.FromHex("#75E6D0"), Description = "Visible Pores" };
                case ConcernType.LackRadiance:
                    return new ConcernItem { Color = Color.FromHex("#74CFE6"), Description = "Lack of Radiance" };
                case ConcernType.ScarringTexture:
                    return new ConcernItem { Color = Color.FromHex("#74A1E6"), Description = "Scarring/Texture" };
                case ConcernType.OilControl:
                    return new ConcernItem { Color = Color.FromHex("#7C7CE7"), Description = "Oil Control" };
                case ConcernType.BlemishProne:
                    return new ConcernItem { Color = Color.FromHex("#A478E7"), Description = "Blemish Prone" };
                case ConcernType.RazorBurn:
                    return new ConcernItem { Color = Color.FromHex("#D075E6"), Description = "Razor Burn" };
                case ConcernType.IngrowingHairs:
                    return new ConcernItem { Color = Color.FromHex("#E77BD2"), Description = "Ingrowing Hairs" };
                case ConcernType.Cellulite:
                    return new ConcernItem { Color = Color.FromHex("#E778A4"), Description = "Cellulite" };
                default:
                    return new ConcernItem { Color = Color.FromHex("#000000"), Description = "Unknown" };
            }
        }

        public static ConcernItem GetConcernDetailsByText(string concernText)
        {
            switch (concernText.ToLower())
            {
                case "lines and wrinkles":
                    return new ConcernItem { Color = Color.FromHex("#E77878"), Description = "Lines and Wrinkles" };
                case "dark spots":
                    return new ConcernItem { Color = Color.FromHex("#E6AD76"), Description = "Dark Spots" };
                case "eye area":
                    return new ConcernItem { Color = Color.FromHex("#E8D27C"), Description = "Eye Area" };
                case "dryness/dehydration":
                    return new ConcernItem { Color = Color.FromHex("#D0E675"), Description = "Dryness/Dehydration" };
                case "firming/lifting":
                    return new ConcernItem { Color = Color.FromHex("#B4EB90"), Description = "Firming/Lifting" };
                case "redness/sensitivity":
                    return new ConcernItem { Color = Color.FromHex("#85E985"), Description = "Redness/Sensitivity" };
                case "sun damage":
                    return new ConcernItem { Color = Color.FromHex("#77E7A4"), Description = "Sun Damage" };
                case "visible pores":
                    return new ConcernItem { Color = Color.FromHex("#75E6D0"), Description = "Visible Pores" };
                case "lack of radiance":
                    return new ConcernItem { Color = Color.FromHex("#74CFE6"), Description = "Lack of Radiance" };
                case "scarring/texture":
                    return new ConcernItem { Color = Color.FromHex("#74A1E6"), Description = "Scarring/Texture" };
                case "oil control":
                    return new ConcernItem { Color = Color.FromHex("#7C7CE7"), Description = "Oil Control" };
                case "blemish prone":
                    return new ConcernItem { Color = Color.FromHex("#A478E7"), Description = "Blemish Prone" };
                case "razor burn":
                    return new ConcernItem { Color = Color.FromHex("#D075E6"), Description = "Razor Burn" };
                case "ingrowing hairs":
                    return new ConcernItem { Color = Color.FromHex("#E77BD2"), Description = "Ingrowing Hairs" };
                case "cellulite":
                    return new ConcernItem { Color = Color.FromHex("#E778A4"), Description = "Cellulite" };
                default:
                    return new ConcernItem { Color = Color.FromHex("#000000"), Description = "Unknown" };
            }
        }
    }
}
