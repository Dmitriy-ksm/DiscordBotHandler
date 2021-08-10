using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Drawing;
using SystemFonts = SixLabors.Fonts.SystemFonts;
using FontStyle = SixLabors.Fonts.FontStyle;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.Fonts;
using DiscordBotHandler.Helpers;
using DiscordBotHandler.Helpers.Dota;

namespace DiscordBotHandler.Services
{
    public class DotaImageDraw : IDraw<DotaGameResult>
    {
        private readonly ILogger _logger;
        private readonly IDotaAssistans _dota;
        private readonly IStorage<Image> _heroImageStorage;
        private readonly IStorage<Image> _itemImageStorage;
        public DotaImageDraw(IServiceProvider service)
        {
            var providerDelegat = service.GetRequiredService<Func<StorageContains, IStorage<Image>>>();
            _logger = service.GetRequiredService<ILogger>();
            _dota = service.GetRequiredService<IDotaAssistans>();
            _heroImageStorage = providerDelegat(StorageContains.DotaHero);
            _itemImageStorage = providerDelegat(StorageContains.DotaItem);
        }
        struct Sizes
        {
            public int fullWidth;
            public int fullHeight;
            public int headerHeight;
            public int heroesColumnWidth;
            public int heroesColumnHeight;
            public int heroPortraitWidth;
            public int heroPortraitHeight;
            public int heroesColumnIndent;
            public int itemBackpackSquare;
            public int itemSquare;
            public int itemHorizontalIntent;
            public int itemColumnHeight;
            public int _fontHeight;
            public int heroStatsHeight;
            public int footerHeight;
            public int towerRadius;
            public int barackSize;
            public int ancientSize;
            public int heroPortraitPickBanWidth;
            public int heroPortraitPickBanHeight;
            public int pickBanHeight;
        }
        private readonly Sizes _size = new Sizes()
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
        private Font _font = SystemFonts.CreateFont("Arial", 18, FontStyle.Regular);
        private Font _fontHeader = SystemFonts.CreateFont("Arial", 36, FontStyle.Regular);
        private bool IsPlayerIdEqualSteamId(uint playerId, ulong steamId)
        {
            return steamId - Consts.SteamAccount3264BitConst == playerId;
        }

        private List<Image<Rgba32>> GetHeroColumns(DotaGameResult match,out Dictionary<ulong, Image> pickedHeores)
        {
            pickedHeores = new Dictionary<ulong, Image>();
            List<Image<Rgba32>> heroColumns = new List<Image<Rgba32>>();
            foreach (var heroes in match.Players)
            {
                Image<Rgba32> hero = new Image<Rgba32>(_size.heroesColumnWidth, _size.heroesColumnHeight);
                Image heroPortrait = _heroImageStorage.GetObject(_dota.GetHeroById(heroes.HeroId).Name);
                heroPortrait.Mutate(o => o.Resize(new Size(_size.heroPortraitWidth, _size.heroPortraitHeight)));
                pickedHeores.Add(heroes.HeroId, heroPortrait);
                hero.Mutate(o => o.DrawImage(heroPortrait, new Point(_size.heroesColumnIndent, 0), 1f));

                foreach (var item in heroes.Items)
                {
                    if (item.ItemId != 0)
                    {
                        using Image heroItem = _itemImageStorage.GetObject(_dota.GetItemById(item.ItemId).Name);

                        heroItem.Mutate(o => o.Resize(new Size(_size.itemSquare, _size.itemSquare)));
                        int intentWidth = _size.heroesColumnIndent + ((item.Slot + 1) % 2 == 0 ? (_size.itemSquare + _size.heroesColumnIndent * 2) : 0);
                        int row = (int)Math.Floor((double)item.Slot / 2);
                        int intentHeight = _size.itemHorizontalIntent + _size.itemHorizontalIntent * row + _size.itemSquare * row;
                        hero.Mutate(o => o.DrawImage(heroItem, new Point(intentWidth, intentHeight + _size.heroPortraitHeight), 1f));

                    }
                }
                foreach(var item in heroes.BackPacks)
                {
                    if(item.ItemId != 0)
                    {
                        using Image heroBackpackItem = _itemImageStorage.GetObject(_dota.GetItemById(item.ItemId).Name);

                        heroBackpackItem.Mutate(o => o.Resize(new Size(_size.itemBackpackSquare, _size.itemBackpackSquare)));
                        int intentWidth = _size.itemBackpackSquare * item.Slot;
                        int intentHeight = _size.itemColumnHeight - _size.itemBackpackSquare;
                        hero.Mutate(o => o.DrawImage(heroBackpackItem, new Point(intentWidth, intentHeight + _size.heroPortraitHeight), 1f));
                    }
                }
                IPen pen = new Pen(Color.Yellow, 5);
                hero.Mutate(o => o
                    .DrawText(heroes.Level.ToString(),
                        _font,
                        Color.Yellow,
                        new Point(_size.itemSquare + _size.heroesColumnIndent * 2 + _size.itemSquare / 2, _size.heroPortraitHeight + _size.itemColumnHeight - _size.itemSquare))
                    .DrawText(heroes.Kills + "/" + heroes.Deaths + "/" + heroes.Assists,
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight))
                    .DrawText(heroes.HeroHealing.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight))
                    .DrawText(heroes.HeroDamage.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 2))
                    .DrawText(heroes.TowerDamage.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 3))
                     .DrawText(heroes.NetWorth.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 4))
                     .DrawText(heroes.LastHits + "/" + heroes.Denies,
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 5))
                     .DrawText(heroes.GoldPerMinute.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 6))
                     .DrawText(heroes.ExperiencePerMinute.ToString(),
                        _font,
                        Color.White,
                        new PointF(_size.heroesColumnIndent, _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 7))
                    );
                if (match.PlayerId != null && IsPlayerIdEqualSteamId(heroes.AccountId, match.PlayerId.Value))
                {
                    hero.Mutate(o => o
                        .DrawPolygon(new Pen(Color.Red, 5), new PointF[] { new PointF(0, 0), new PointF(_size.heroesColumnWidth, 0), new PointF(_size.heroesColumnWidth, _size.heroesColumnHeight), new PointF(0, _size.heroesColumnHeight) }));
                }
                heroColumns.Add(hero);
            }
            return heroColumns;
        }
        
        private void DrawHeroesColumnStat(DotaGameResult match, out Dictionary<ulong, Image> pickedHeores, Image<Rgba32> outputImage)
        {
            var heroColumns = GetHeroColumns(match,out pickedHeores);
            int counter = 0;
            foreach (var heroIm in heroColumns)
            {
                if (counter == 5)
                {
                    outputImage.Mutate(o => o.DrawText("Герои",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight / 2))
                    .DrawText("Предметы",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight))
                    .DrawText("КДА",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight))
                    .DrawText("Исцеление",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight))
                    .DrawText("Урон по героям",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 2))
                     .DrawText("Урон по строениям",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 3))
                    .DrawText("Общая ценность",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 4))
                    .DrawText("Ластхит/Денай",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 5))
                     .DrawText("Золото в минуту",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 6))
                     .DrawText("Опыт в минуту",
                        _font,
                        Color.White,
                        new Point(_size.heroesColumnWidth * counter, _size.headerHeight + _size.heroPortraitHeight + _size.itemColumnHeight + _size._fontHeight * 7))
                    );
                    counter++;
                }
                outputImage.Mutate(o => o.DrawImage(heroIm, new Point(_size.heroesColumnWidth * counter++, _size.headerHeight), 1f));
                heroIm.Dispose();
            }
        }

        private Image<Rgba32> DrawMiniMap(DotaGameResult match)
        {
            var miniMap = new Image<Rgba32>(_size.footerHeight, _size.footerHeight);
            using Image backgroundMap = Image.Load("map.png");
            backgroundMap.Mutate(o => o.Resize(_size.footerHeight, _size.footerHeight));
            float oneWidthBlock = (float)miniMap.Width / 100;
            float oneHeightBlock = (float)miniMap.Height / 100;
            Color radiantLive = Color.Blue;
            Color direLive = Color.Red;
            Color dead = Color.Black;
            Dictionary<string, float> RadiantStrcutPositionW = new Dictionary<string, float>()
                    {
                        { "Top",10 },
                        { "MBTop",7 },
                        { "RBTop",12 },
                        { "T1Mid",40 },
                        { "T2Mid",30 },
                        { "T3Mid",23 },
                        { "MBMid",19 },
                        { "RBMid",22 },
                        { "T1Bottom",80 },
                        { "T2Bottom",40 },
                        { "T3Bottom",28 },
                        { "BBottom",23 },
                        { "T4Top",12 },
                        { "T4Bottom",17 },
                        { "Ancient",10 }
                    };
            Dictionary<string, float> RadiantStrcutPositionH = new Dictionary<string, float>()
                    {
                        { "T1Top",35 },
                        { "T2Top",55 },
                        { "T3Top",75 },
                        { "BTop",80 },
                        { "T1Mid",60 },
                        { "T2Mid",70 },
                        { "T3Mid",80 },
                        { "MBMid",82 },
                        { "RBMid",83 },
                        { "Bottom",95 },
                        { "MBBottom",93 },
                        { "RBBottom",97 },
                        { "T4Top",92 },
                        { "T4Bottom",93 },
                        { "Ancient",92 }
                    };
            var T1RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T1Top"], _size.towerRadius);
            var T2RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T2Top"], _size.towerRadius);
            var T3RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T3Top"], _size.towerRadius);
            var MiliRadiantTop = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["MBTop"], oneHeightBlock * RadiantStrcutPositionH["BTop"], _size.barackSize, _size.barackSize);
            var RangeRadiantTop = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["RBTop"], oneHeightBlock * RadiantStrcutPositionH["BTop"], _size.barackSize, _size.barackSize);

            var T1DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T1Top"]), _size.towerRadius);
            var T2DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T2Top"]), _size.towerRadius);
            var T3DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T3Top"]), _size.towerRadius);
            var MiliDireBottom = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["MBTop"]), oneHeightBlock * (100 - RadiantStrcutPositionH["BTop"]), _size.barackSize, _size.barackSize);
            var RangeDireBottom = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["RBTop"]), oneHeightBlock * (100 - RadiantStrcutPositionH["BTop"]), _size.barackSize, _size.barackSize);

            var T1RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T1Mid"], oneHeightBlock * RadiantStrcutPositionH["T1Mid"], _size.towerRadius);
            var T2RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T2Mid"], oneHeightBlock * RadiantStrcutPositionH["T2Mid"], _size.towerRadius);
            var T3RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T3Mid"], oneHeightBlock * RadiantStrcutPositionH["T3Mid"], _size.towerRadius);
            var MiliRadiantMid = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["MBMid"], oneHeightBlock * RadiantStrcutPositionH["MBMid"], _size.barackSize, _size.barackSize);
            var RangeRadiantMid = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["RBMid"], oneHeightBlock * RadiantStrcutPositionH["RBMid"], _size.barackSize, _size.barackSize);

            var T1DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T1Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T1Mid"]), _size.towerRadius);
            var T2DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T2Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T2Mid"]), _size.towerRadius);
            var T3DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T3Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T3Mid"]), _size.towerRadius);
            var MiliDireMid = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["MBMid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["MBMid"]), _size.barackSize, _size.barackSize);
            var RangeDireMid = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["RBMid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["RBMid"]), _size.barackSize, _size.barackSize);

            var T1RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T1Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], _size.towerRadius);
            var T2RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T2Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], _size.towerRadius);
            var T3RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T3Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], _size.towerRadius);
            var MiliRadiantBottom = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["BBottom"], oneHeightBlock * RadiantStrcutPositionH["MBBottom"], _size.barackSize, _size.barackSize);
            var RangeRadiantBottom = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["BBottom"], oneHeightBlock * RadiantStrcutPositionH["RBBottom"], _size.barackSize, _size.barackSize);

            var T1DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T1Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), _size.towerRadius);
            var T2DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T2Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), _size.towerRadius);
            var T3DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T3Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), _size.towerRadius);
            var MiliDireTop = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["BBottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["MBBottom"]), _size.barackSize, _size.barackSize);
            var RangeDireTop = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["BBottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["RBBottom"]), _size.barackSize, _size.barackSize);

            var T4RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T4Top"], oneHeightBlock * RadiantStrcutPositionH["T4Top"] - 5, _size.towerRadius);
            var T4RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T4Bottom"], oneHeightBlock * RadiantStrcutPositionH["T4Bottom"] - 5, _size.towerRadius);
            var RadiantAncient = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["Ancient"], oneHeightBlock * RadiantStrcutPositionH["Ancient"], _size.ancientSize, _size.ancientSize);

            var T4DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T4Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T4Top"]) + 20, _size.towerRadius);
            var T4DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T4Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T4Bottom"]) + 20, _size.towerRadius);
            var DireAncient = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Ancient"]) - 10, oneHeightBlock * (100 - RadiantStrcutPositionH["Ancient"]), _size.ancientSize, _size.ancientSize);

            miniMap.Mutate(o => o.DrawImage(backgroundMap, 1f)
                .Fill(match.TowerStatesRadiant.IsTopTier1Alive ? radiantLive : dead, T1RadiantTop)
                .Fill(match.TowerStatesRadiant.IsTopTier2Alive ? radiantLive : dead, T2RadiantTop)
                .Fill(match.TowerStatesRadiant.IsTopTier3Alive ? radiantLive : dead, T3RadiantTop)
                .Fill(match.TowerStatesRadiant.IsBottomTier1Alive ? radiantLive : dead, T1RadiantBottom)
                .Fill(match.TowerStatesRadiant.IsBottomTier2Alive ? radiantLive : dead, T2RadiantBottom)
                .Fill(match.TowerStatesRadiant.IsBottomTier3Alive ? radiantLive : dead, T3RadiantBottom)
                .Fill(match.TowerStatesRadiant.IsMiddleTier1Alive ? radiantLive : dead, T1RadiantMid)
                .Fill(match.TowerStatesRadiant.IsMiddleTier2Alive ? radiantLive : dead, T2RadiantMid)
                .Fill(match.TowerStatesRadiant.IsMiddleTier3Alive ? radiantLive : dead, T3RadiantMid)
                .Fill(match.TowerStatesRadiant.IsAncientTopAlive ? radiantLive : dead, T4RadiantTop)
                .Fill(match.TowerStatesRadiant.IsAncientBottomAlive ? radiantLive : dead, T4RadiantBottom)

                .Fill(match.TowerStatesDire.IsTopTier1Alive ? direLive : dead, T1DireTop)
                .Fill(match.TowerStatesDire.IsTopTier2Alive ? direLive : dead, T2DireTop)
                .Fill(match.TowerStatesDire.IsTopTier3Alive ? direLive : dead, T3DireTop)
                .Fill(match.TowerStatesDire.IsBottomTier1Alive ? direLive : dead, T1DireBottom)
                .Fill(match.TowerStatesDire.IsBottomTier2Alive ? direLive : dead, T2DireBottom)
                .Fill(match.TowerStatesDire.IsBottomTier3Alive ? direLive : dead, T3DireBottom)
                .Fill(match.TowerStatesDire.IsMiddleTier1Alive ? direLive : dead, T1DireMid)
                .Fill(match.TowerStatesDire.IsMiddleTier2Alive ? direLive : dead, T2DireMid)
                .Fill(match.TowerStatesDire.IsMiddleTier3Alive ? direLive : dead, T3DireMid)
                .Fill(match.TowerStatesDire.IsAncientTopAlive ? direLive : dead, T4DireTop)
                .Fill(match.TowerStatesDire.IsAncientBottomAlive ? direLive : dead, T4DireBottom)

                .Fill(match.BarracksStatesRadiant.IsBottomMeleeAlive ? radiantLive : dead, MiliRadiantBottom)
                .Fill(match.BarracksStatesRadiant.IsBottomRangedAlive ? radiantLive : dead, RangeRadiantBottom)
                .Fill(match.BarracksStatesRadiant.IsMiddleMeleeAlive ? radiantLive : dead, MiliRadiantMid)
                .Fill(match.BarracksStatesRadiant.IsMiddleRangedAlive ? radiantLive : dead, RangeRadiantMid)
                .Fill(match.BarracksStatesRadiant.IsTopMeleeAlive ? radiantLive : dead, MiliRadiantTop)
                .Fill(match.BarracksStatesRadiant.IsTopRangedAlive ? radiantLive : dead, RangeRadiantTop)

                .Fill(match.BarracksStatesDire.IsBottomMeleeAlive ? direLive : dead, MiliDireBottom)
                .Fill(match.BarracksStatesDire.IsBottomRangedAlive ? direLive : dead, RangeDireBottom)
                .Fill(match.BarracksStatesDire.IsMiddleMeleeAlive ? direLive : dead, MiliDireMid)
                .Fill(match.BarracksStatesDire.IsMiddleRangedAlive ? direLive : dead, RangeDireMid)
                .Fill(match.BarracksStatesDire.IsTopMeleeAlive ? direLive : dead, MiliDireTop)
                .Fill(match.BarracksStatesDire.IsTopRangedAlive ? direLive : dead, RangeDireTop)

                .Fill(match.RadiantWin ? radiantLive : dead, RadiantAncient)

                .Fill(match.RadiantWin ? dead : direLive, DireAncient)
            );
            return miniMap;
        }

        private Image<Rgba32> DrawPicksAndBan(DotaGameResult match, Dictionary<ulong, Image> pickedHeores, int height)
        {
            var picksAndBans = new Image<Rgba32>(_size.heroesColumnWidth * 8, height);
            int CounterRadiant = 0;
            int CounterDire = 0;
            int bonusCounter = 0;
            foreach (var pb in match.PicksAndBans)
            {
                Image ImageToDraw;
                if (pb.IsPick && pickedHeores.ContainsKey(pb.HeroId))
                {
                    ImageToDraw = pickedHeores[pb.HeroId];
                }
                else
                {
                    ImageToDraw = _heroImageStorage.GetObject(_dota.GetHeroById(pb.HeroId).Name);
                }
                ImageToDraw.Mutate(o => o.Resize(_size.heroPortraitPickBanWidth, _size.heroPortraitPickBanHeight));
                if (pb.Team == 0)
                {
                    if (CounterRadiant < 12)
                    {
                        picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(_size.heroPortraitPickBanWidth * CounterRadiant++, 0), 1f));
                    }
                    else
                    {
                        picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(_size.heroPortraitPickBanWidth * bonusCounter++, _size.heroPortraitPickBanHeight * 2 + _size.itemHorizontalIntent), 1f));
                    }
                }
                else
                {
                    if (CounterDire < 12)
                    {
                        picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(_size.heroPortraitPickBanWidth * CounterDire++, _size.heroPortraitPickBanHeight + _size.itemHorizontalIntent), 1f));
                    }
                    else
                    {
                        picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(_size.heroPortraitPickBanWidth * bonusCounter++, _size.heroPortraitPickBanHeight * 2 + _size.itemHorizontalIntent), 1f));
                    }
                }
                ImageToDraw.Dispose();
                if (bonusCounter > 12)
                {
                    break;
                }
            }
            return picksAndBans;
        }
        
        private void DrawFooter(DotaGameResult match, Dictionary<ulong, Image> pickedHeores, Image<Rgba32> outputImage)
        {
            using Image<Rgba32> footer = new Image<Rgba32>(_size.fullWidth, _size.footerHeight);

            using Image<Rgba32> miniMap = DrawMiniMap(match);

            using Image<Rgba32> picksAndBans = DrawPicksAndBan(match, pickedHeores, footer.Height);

            using Image<Rgba32> gameInfo = new Image<Rgba32>(_size.heroesColumnWidth * 8, _size.footerHeight - _size.pickBanHeight);
            //TODO gameinfo
            gameInfo.Mutate(o => o.DrawText("Match Id:" + match.MatchId + " Duration:" + TimeSpan.FromSeconds(match.Duration).ToString() + " Game start at:" + match.StartTime.ToString(),
                _font, Color.White, new Point(0, 0)));
            footer.Mutate(o => o.DrawImage(miniMap, new Point(0, 0), 1f)
                .DrawImage(picksAndBans, new Point(_size.heroesColumnWidth * 3, 0), 1f)
                .DrawImage(gameInfo, new Point(_size.heroesColumnWidth * 3, _size.pickBanHeight), 1f));
            outputImage.Mutate(o => o.DrawImage(footer, new Point(0, _size.headerHeight + _size.heroesColumnHeight), 1f));
        }
        
        public void DrawImage(DotaGameResult match, MemoryStream stream)
        {
            try
            {
                using (Image<Rgba32> outputImage = new Image<Rgba32>(_size.fullWidth, _size.fullHeight))
                {
                    
                    using Image<Rgba32> header = new Image<Rgba32>(_size.fullWidth, _size.headerHeight);

                    header.Mutate(o => o.DrawText(match.RadiantWin ? " Radiant win " : " Dire win ", _fontHeader, Color.White, new PointF(_size.fullWidth / 2 - 100, 0)));
                    
                    using Image<Rgba32> midle = new Image<Rgba32>(_size.fullWidth, _size.heroesColumnHeight);

                    outputImage.Mutate(o => o.BackgroundColor(Color.Black)
                        .DrawImage(header, new Point(0, 0), 1f));

                    DrawHeroesColumnStat(match,out Dictionary<ulong, Image> pickedHeores, outputImage);

                    DrawFooter(match, pickedHeores, outputImage);

                    outputImage.Save(stream, new JpegEncoder());
                }
            }
            catch (Exception e)
            {
                _logger.LogMessage(e.Message);
            }
        }
    }
}
