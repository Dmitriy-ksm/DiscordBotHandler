using DiscordBotHandler.Services;

namespace DiscordBotHandler.Helpers
{
    public static class Consts
    {
        public const ulong SteamAccount3264BitConst  = 76561197960265728;

        public static readonly Sizes DotaMiniMapSizes = new Sizes()
        {
            fullWidth = 1920,
            fullHeight = 1080,
            headerHeight = 55,
            heroesColumnWidth = 170,
            heroesColumnHeight = 650,
            heroPortraitWidth = 150,
            heroPortraitHeight = 100,
            heroesColumnIndent = (170 - 150) / 2,
            itemBackpackSquare = 30,
            itemSquare = 65,
            itemHorizontalIntent = 15,
            itemColumnHeight = 350,
            _fontHeight = 25,
            heroStatsHeight = 225,
            footerHeight = 375,
            towerRadius = 6,
            barackSize = 10,
            ancientSize = 20,
            heroPortraitPickBanWidth = 100,
            heroPortraitPickBanHeight = 75,
            pickBanHeight = 255,
        };
    }
}
