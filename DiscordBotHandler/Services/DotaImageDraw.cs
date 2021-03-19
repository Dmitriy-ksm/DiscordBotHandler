//using Discord;
using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using SystemFonts = SixLabors.Fonts.SystemFonts;
using FontStyle = SixLabors.Fonts.FontStyle;
using Steam.Models.DOTA2;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Runtime.Serialization.Formatters;

namespace DiscordBotHandler.Services
{
    public class DotaImageDraw : IDraw
    {
        private readonly ILogger _logger;
        public DotaImageDraw(IServiceProvider service)
        {
            _logger = service.GetRequiredService<ILogger>();
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
            public int itemSquare;
            public int itemHorizontalIntent;
            public int itemColumnHeight;
            public int fontHeight;
            public int heroStatsHeight;
            public int footerHeight;
            public int towerRadius;
            public int barackSize;
            public int ancientSize;
            public int heroPortraitPickBanWidth;
            public int heroPortraitPickBanHeight;
            public int pickBanHeight;
        }
        public void DrawImage(object match, MemoryStream stream)
        {
            Sizes Size = new Sizes()
            {
                fullWidth = 1920,
                fullHeight = 1080,
                headerHeight = 55,
                heroesColumnWidth = 170,
                heroesColumnHeight = 650,
                heroPortraitWidth = 150,
                heroPortraitHeight = 100,
                heroesColumnIndent = (170 - 150) / 2,
                itemSquare = 65,
                itemHorizontalIntent = 15,
                itemColumnHeight = 320,
                fontHeight = 25,
                heroStatsHeight = 225,
                footerHeight = 375,
                towerRadius = 6,
                barackSize = 10,
                ancientSize = 20,
                heroPortraitPickBanWidth = 100,
                heroPortraitPickBanHeight = 75,
                pickBanHeight = 255,
            };
            try
            {
                var matchDota = match as DotaGameResult;
                using (Image<Rgba32> outputImage = new Image<Rgba32>(Size.fullWidth, Size.fullHeight))
                {
                    Dictionary<ulong, Image> pickedHeores = new Dictionary<ulong, Image>();
                    var font = SystemFonts.CreateFont("Arial", 18, FontStyle.Regular);
                    var fontHeader = SystemFonts.CreateFont("Arial", 36, FontStyle.Regular);
                    using Image<Rgba32> header = new Image<Rgba32>(Size.fullWidth, Size.headerHeight);
                    header.Mutate(o => o.DrawText(matchDota.RadiantWin ? " Radiant win " : " Dire win ", fontHeader, Color.White, new PointF(Size.fullWidth / 2 - 100, 0)));
                    using Image<Rgba32> midle = new Image<Rgba32>(Size.fullWidth, Size.heroesColumnHeight);
                    List<Image<Rgba32>> heroColumns = new List<Image<Rgba32>>();
                    foreach (var heroes in matchDota.Players)
                    {
                        Image<Rgba32> hero = new Image<Rgba32>(Size.heroesColumnWidth, Size.heroesColumnHeight);
                        var requestPortrait = WebRequest.Create(heroes.HeroImageUrl);

                        using (var responsePortrait = requestPortrait.GetResponse())
                        using (var streamPortrait = responsePortrait.GetResponseStream())
                        {
                            Image heroPortrait = Image.Load(streamPortrait);
                            heroPortrait.Mutate(o => o.Resize(new SixLabors.ImageSharp.Size(Size.heroPortraitWidth, Size.heroPortraitHeight)));
                            pickedHeores.Add(heroes.HeroId, heroPortrait);
                            hero.Mutate(o => o.DrawImage(heroPortrait, new Point(Size.heroesColumnIndent, 0), 1f));
                        }
                        foreach (var item in heroes.Items)
                        {
                            if (item.ItemId != 0)
                            {
                                var requestItem = WebRequest.Create(item.ItemImageUrl);

                                using (var responseItem = requestItem.GetResponse())
                                using (var streamItem = responseItem.GetResponseStream())
                                {
                                    using Image heroItem = Image.Load(streamItem);
                                    heroItem.Mutate(o => o.Resize(new Size(Size.itemSquare, Size.itemSquare)));
                                    int intentWidth = Size.heroesColumnIndent + ((item.Slot + 1) % 2 == 0 ? (Size.itemSquare + Size.heroesColumnIndent * 2) : 0);
                                    int row = (int)Math.Floor((double)item.Slot / 2);
                                    int intentHeight = Size.itemHorizontalIntent + Size.itemHorizontalIntent * row + Size.itemSquare * row;
                                    hero.Mutate(o => o.DrawImage(heroItem, new Point(intentWidth, intentHeight + Size.heroPortraitHeight), 1f));
                                }
                            }
                        }
                        IPen pen = new Pen(Color.Yellow, 5);
                        hero.Mutate(o => o
                            /*.Draw(pen, (IPath)new SixLabors.Shapes.EllipsePolygon(Size.itemSquare + Size.heroesColumnIndent * 2, Size.heroPortraitHeight + Size.itemColumnHeight - (int)Math.Floor((float)Size.itemSquare / 2), (float)Size.itemSquare / 2))*/
                            .DrawText(heroes.Level.ToString(),
                                font,
                                Color.Yellow,
                                new Point(Size.itemSquare + Size.heroesColumnIndent * 2 + Size.itemSquare / 2, Size.heroPortraitHeight + Size.itemColumnHeight - Size.itemSquare))
                            .DrawText(heroes.Kills + "/" + heroes.Deaths + "/" + heroes.Assists,
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight))
                            .DrawText(heroes.HeroHealing.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight))
                            .DrawText(heroes.HeroDamage.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 2))
                            .DrawText(heroes.TowerDamage.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 3))
                             .DrawText(heroes.NetWorth.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 4))
                             .DrawText(heroes.LastHits + "/" + heroes.Denies,
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 5))
                             .DrawText(heroes.GoldPerMinute.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 6))
                             .DrawText(heroes.ExperiencePerMinute.ToString(),
                                font,
                                Color.White,
                                new PointF(Size.heroesColumnIndent, Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 7))
                            );
                        heroColumns.Add(hero);
                    }
                    outputImage.Mutate(o => o.BackgroundColor(Color.Black)
                        .DrawImage(header, new Point(0, 0), 1f));
                    int counter = 0;
                    foreach (var heroIm in heroColumns)
                    {
                        if (counter == 5)
                        {
                            outputImage.Mutate(o => o.DrawText("Герои",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight / 2))
                            .DrawText("Предметы",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight))
                            .DrawText("КДА",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight))
                            .DrawText("Исцеление",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight))
                            .DrawText("Урон по героям",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 2))
                             .DrawText("Урон по строениям",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 3))
                            .DrawText("Общая ценность",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 4))
                            .DrawText("Ластхит/Денай",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 5))
                             .DrawText("Золото в минуту",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 6))
                             .DrawText("Опыт в минуту",
                                font,
                                Color.White,
                                new Point(Size.heroesColumnWidth * counter, Size.headerHeight + Size.heroPortraitHeight + Size.itemColumnHeight + Size.fontHeight * 7))
                            );
                            counter++;
                        }
                        outputImage.Mutate(o => o.DrawImage(heroIm, new Point(Size.heroesColumnWidth * counter++, Size.headerHeight), 1f));
                        heroIm.Dispose();
                    }
                    using Image<Rgba32> footer = new Image<Rgba32>(Size.fullWidth, Size.footerHeight);
                    using Image<Rgba32> miniMap = new Image<Rgba32>(Size.footerHeight, Size.footerHeight);
                    using Image backgroundMap = Image.Load("map.png");
                    backgroundMap.Mutate(o => o.Resize(Size.footerHeight, Size.footerHeight));
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
                    var T1RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T1Top"], Size.towerRadius);
                    var T2RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T2Top"], Size.towerRadius);
                    var T3RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["Top"], oneHeightBlock * RadiantStrcutPositionH["T3Top"], Size.towerRadius);
                    var MiliRadiantTop = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["MBTop"], oneHeightBlock * RadiantStrcutPositionH["BTop"], Size.barackSize, Size.barackSize);
                    var RangeRadiantTop = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["RBTop"], oneHeightBlock * RadiantStrcutPositionH["BTop"], Size.barackSize, Size.barackSize);

                    var T1DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T1Top"]), Size.towerRadius);
                    var T2DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T2Top"]), Size.towerRadius);
                    var T3DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T3Top"]), Size.towerRadius);
                    var MiliDireBottom = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["MBTop"]), oneHeightBlock * (100 - RadiantStrcutPositionH["BTop"]), Size.barackSize, Size.barackSize);
                    var RangeDireBottom = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["RBTop"]), oneHeightBlock * (100 - RadiantStrcutPositionH["BTop"]), Size.barackSize, Size.barackSize);

                    var T1RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T1Mid"], oneHeightBlock * RadiantStrcutPositionH["T1Mid"], Size.towerRadius);
                    var T2RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T2Mid"], oneHeightBlock * RadiantStrcutPositionH["T2Mid"], Size.towerRadius);
                    var T3RadiantMid = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T3Mid"], oneHeightBlock * RadiantStrcutPositionH["T3Mid"], Size.towerRadius);
                    var MiliRadiantMid = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["MBMid"], oneHeightBlock * RadiantStrcutPositionH["MBMid"], Size.barackSize, Size.barackSize);
                    var RangeRadiantMid = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["RBMid"], oneHeightBlock * RadiantStrcutPositionH["RBMid"], Size.barackSize, Size.barackSize);

                    var T1DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T1Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T1Mid"]), Size.towerRadius);
                    var T2DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T2Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T2Mid"]), Size.towerRadius);
                    var T3DireMid = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T3Mid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T3Mid"]), Size.towerRadius);
                    var MiliDireMid = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["MBMid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["MBMid"]), Size.barackSize, Size.barackSize);
                    var RangeDireMid = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["RBMid"]), oneHeightBlock * (100 - RadiantStrcutPositionH["RBMid"]), Size.barackSize, Size.barackSize);

                    var T1RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T1Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], Size.towerRadius);
                    var T2RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T2Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], Size.towerRadius);
                    var T3RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T3Bottom"], oneHeightBlock * RadiantStrcutPositionH["Bottom"], Size.towerRadius);
                    var MiliRadiantBottom = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["BBottom"], oneHeightBlock * RadiantStrcutPositionH["MBBottom"], Size.barackSize, Size.barackSize);
                    var RangeRadiantBottom = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["BBottom"], oneHeightBlock * RadiantStrcutPositionH["RBBottom"], Size.barackSize, Size.barackSize);

                    var T1DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T1Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), Size.towerRadius);
                    var T2DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T2Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), Size.towerRadius);
                    var T3DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T3Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["Bottom"]), Size.towerRadius);
                    var MiliDireTop = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["BBottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["MBBottom"]), Size.barackSize, Size.barackSize);
                    var RangeDireTop = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["BBottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["RBBottom"]), Size.barackSize, Size.barackSize);

                    var T4RadiantTop = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T4Top"], oneHeightBlock * RadiantStrcutPositionH["T4Top"] - 5, Size.towerRadius);
                    var T4RadiantBottom = new EllipsePolygon(oneWidthBlock * RadiantStrcutPositionW["T4Bottom"], oneHeightBlock * RadiantStrcutPositionH["T4Bottom"] - 5, Size.towerRadius);
                    var RadiantAncient = new RectangularPolygon(oneWidthBlock * RadiantStrcutPositionW["Ancient"], oneHeightBlock * RadiantStrcutPositionH["Ancient"], Size.ancientSize, Size.ancientSize);

                    var T4DireTop = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T4Top"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T4Top"]) + 20, Size.towerRadius);
                    var T4DireBottom = new EllipsePolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["T4Bottom"]), oneHeightBlock * (100 - RadiantStrcutPositionH["T4Bottom"]) + 20, Size.towerRadius);
                    var DireAncient = new RectangularPolygon(oneWidthBlock * (100 - RadiantStrcutPositionW["Ancient"]) - 10, oneHeightBlock * (100 - RadiantStrcutPositionH["Ancient"]), Size.ancientSize, Size.ancientSize);

                    miniMap.Mutate(o => o.DrawImage(backgroundMap, 1f)
                        .Fill(matchDota.TowerStatesRadiant.IsTopTier1Alive ? radiantLive : dead, T1RadiantTop)
                        .Fill(matchDota.TowerStatesRadiant.IsTopTier2Alive ? radiantLive : dead, T2RadiantTop)
                        .Fill(matchDota.TowerStatesRadiant.IsTopTier3Alive ? radiantLive : dead, T3RadiantTop)
                        .Fill(matchDota.TowerStatesRadiant.IsBottomTier1Alive ? radiantLive : dead, T1RadiantBottom)
                        .Fill(matchDota.TowerStatesRadiant.IsBottomTier2Alive ? radiantLive : dead, T2RadiantBottom)
                        .Fill(matchDota.TowerStatesRadiant.IsBottomTier3Alive ? radiantLive : dead, T3RadiantBottom)
                        .Fill(matchDota.TowerStatesRadiant.IsMiddleTier1Alive ? radiantLive : dead, T1RadiantMid)
                        .Fill(matchDota.TowerStatesRadiant.IsMiddleTier2Alive ? radiantLive : dead, T2RadiantMid)
                        .Fill(matchDota.TowerStatesRadiant.IsMiddleTier3Alive ? radiantLive : dead, T3RadiantMid)
                        .Fill(matchDota.TowerStatesRadiant.IsAncientTopAlive ? radiantLive : dead, T4RadiantTop)
                        .Fill(matchDota.TowerStatesRadiant.IsAncientBottomAlive ? radiantLive : dead, T4RadiantBottom)

                        .Fill(matchDota.TowerStatesDire.IsTopTier1Alive ? direLive : dead, T1DireTop)
                        .Fill(matchDota.TowerStatesDire.IsTopTier2Alive ? direLive : dead, T2DireTop)
                        .Fill(matchDota.TowerStatesDire.IsTopTier3Alive ? direLive : dead, T3DireTop)
                        .Fill(matchDota.TowerStatesDire.IsBottomTier1Alive ? direLive : dead, T1DireBottom)
                        .Fill(matchDota.TowerStatesDire.IsBottomTier2Alive ? direLive : dead, T2DireBottom)
                        .Fill(matchDota.TowerStatesDire.IsBottomTier3Alive ? direLive : dead, T3DireBottom)
                        .Fill(matchDota.TowerStatesDire.IsMiddleTier1Alive ? direLive : dead, T1DireMid)
                        .Fill(matchDota.TowerStatesDire.IsMiddleTier2Alive ? direLive : dead, T2DireMid)
                        .Fill(matchDota.TowerStatesDire.IsMiddleTier3Alive ? direLive : dead, T3DireMid)
                        .Fill(matchDota.TowerStatesDire.IsAncientTopAlive ? direLive : dead, T4DireTop)
                        .Fill(matchDota.TowerStatesDire.IsAncientBottomAlive ? direLive : dead, T4DireBottom)

                        .Fill(matchDota.BarracksStatesRadiant.IsBottomMeleeAlive ? radiantLive : dead, MiliRadiantBottom)
                        .Fill(matchDota.BarracksStatesRadiant.IsBottomRangedAlive ? radiantLive : dead, RangeRadiantBottom)
                        .Fill(matchDota.BarracksStatesRadiant.IsMiddleMeleeAlive ? radiantLive : dead, MiliRadiantMid)
                        .Fill(matchDota.BarracksStatesRadiant.IsMiddleRangedAlive ? radiantLive : dead, RangeRadiantMid)
                        .Fill(matchDota.BarracksStatesRadiant.IsTopMeleeAlive ? radiantLive : dead, MiliRadiantTop)
                        .Fill(matchDota.BarracksStatesRadiant.IsTopRangedAlive ? radiantLive : dead, RangeRadiantTop)

                        .Fill(matchDota.BarracksStatesDire.IsBottomMeleeAlive ? direLive : dead, MiliDireBottom)
                        .Fill(matchDota.BarracksStatesDire.IsBottomRangedAlive ? direLive : dead, RangeDireBottom)
                        .Fill(matchDota.BarracksStatesDire.IsMiddleMeleeAlive ? direLive : dead, MiliDireMid)
                        .Fill(matchDota.BarracksStatesDire.IsMiddleRangedAlive ? direLive : dead, RangeDireMid)
                        .Fill(matchDota.BarracksStatesDire.IsTopMeleeAlive ? direLive : dead, MiliDireTop)
                        .Fill(matchDota.BarracksStatesDire.IsTopRangedAlive ? direLive : dead, RangeDireTop)

                        .Fill(matchDota.RadiantWin ? radiantLive : dead, RadiantAncient)

                        .Fill(matchDota.RadiantWin ? dead : direLive, DireAncient)
                    );
                    using Image<Rgba32> picksAndBans = new Image<Rgba32>(Size.heroesColumnWidth * 8, footer.Height);
                    int CounterRadiant = 0;
                    int CounterDire = 0;
                    int bonusCounter = 0;
                    foreach (var pb in matchDota.PicksAndBans)
                    {
                        Image ImageToDraw;
                        if (pb.IsPick && pickedHeores.ContainsKey(pb.HeroId))
                        {
                            ImageToDraw = pickedHeores[pb.HeroId];
                        }
                        else
                        {
                            var requestItem = WebRequest.Create(pb.HeroImageUrl);

                            using (var responseItem = requestItem.GetResponse())
                            using (var streamItem = responseItem.GetResponseStream())
                            {
                                ImageToDraw = Image.Load(streamItem);
                            }
                        }
                        ImageToDraw.Mutate(o => o.Resize(Size.heroPortraitPickBanWidth, Size.heroPortraitPickBanHeight));
                        if (pb.Team == 0)
                        {
                            if (CounterRadiant < 12)
                            {
                                picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(Size.heroPortraitPickBanWidth * CounterRadiant++, 0), 1f));
                            }
                            else
                            {
                                picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(Size.heroPortraitPickBanWidth * bonusCounter++, Size.heroPortraitPickBanHeight * 2+ Size.itemHorizontalIntent), 1f));
                            }
                        }
                        else
                        {
                            if (CounterDire < 12)
                            {
                                picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(Size.heroPortraitPickBanWidth * CounterDire++, Size.heroPortraitPickBanHeight+ Size.itemHorizontalIntent), 1f));
                            }
                            else
                            {
                                picksAndBans.Mutate(o => o.DrawImage(ImageToDraw, new Point(Size.heroPortraitPickBanWidth * bonusCounter++, Size.heroPortraitPickBanHeight * 2+ Size.itemHorizontalIntent), 1f));
                            }
                        }
                        ImageToDraw.Dispose();
                        if (bonusCounter > 12)
                        {
                            break;
                        }
                    }
                    using Image<Rgba32> gameInfo = new Image<Rgba32>(Size.heroesColumnWidth * 8, Size.footerHeight - Size.pickBanHeight);
                    //TODO gameinfo
                    gameInfo.Mutate(o => o.DrawText("Match Id:" + matchDota.MatchId + " Duration:" + TimeSpan.FromSeconds(matchDota.Duration).ToString() + " Game start at:" + matchDota.StartTime.ToString(),
                        font, Color.White, new Point(0, 0)));
                    footer.Mutate(o => o.DrawImage(miniMap, new Point(0, 0), 1f)
                        .DrawImage(picksAndBans, new Point(Size.heroesColumnWidth * 3, 0), 1f)
                        .DrawImage(gameInfo, new Point(Size.heroesColumnWidth * 3, Size.pickBanHeight), 1f));
                    outputImage.Mutate(o => o.DrawImage(footer, new Point(0, Size.headerHeight + Size.heroesColumnHeight), 1f));
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
