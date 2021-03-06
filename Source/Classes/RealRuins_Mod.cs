﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Verse;
using RimWorld;
using UnityEngine;

namespace RealRuins {
    public class RealRuins_Mod : Mod {


        public static string Text_NetSettings_Category = "RealRuins_ModOptions_Category";
        public static string Text_MapSettings_Category = "RealRuins_MapOptions_Category";

        public static string Text_Option_AllowDownloads = "RealRuins_ModOptions_AllowDownloads";
        public static string Text_Option_AllowUploads = "RealRuins_ModOptions_AllowUploads";
        public static string Text_Option_CacheSize = "RealRuins_ModOptions_CacheSize";
        public static string Text_Option_OfflineMode = "RealRuins_ModOptions_OfflineMode";

        public static string Text_Option_CurrentCacheSize = "RealRuins_ModOptions_CurrentCacheSize";
        public static string Text_Option_RemoveAll = "RealRuins_ModOptions_RemoveAll";

        public static string Text_Option_AllowDownloadsTooltip = "RealRuins_ModOptions_AllowDownloadsTooltip";
        public static string Text_Option_AllowUploadsTooltip = "RealRuins_ModOptions_AllowUploadsTooltip";
        public static string Text_Option_CacheSizeTooltip = "RealRuins_ModOptions_CacheSizeTooltip";
        public static string Text_Option_OfflineModeTooltip = "RealRuins_ModOptions_OfflineModeTooltip";

        public static string Text_Option_DownloadMore = "RealRuins_ModOptions_DownloadMore";
        public static string Text_Option_CurrentCacheCount = "RealRuins_ModOptions_CurrentCacheCount";
        public static string Text_Option_Density = "RealRuins_MapOptions_Density";
        public static string Text_Option_Size = "RealRuins_MapOptions_Size";
        public static string Text_Option_Deterioration = "RealRuins_MapOptions_Deterioration";
        public static string Text_Option_Scavengers = "RealRuins_MapOptions_Scavengers";
        public static string Text_Option_CostLimit = "RealRuins_MapOptions_CostLimit";
        public static string Text_Option_DisableHaulables = "RealRuins_MapOptions_DisableHaulables";
        public static string Text_Option_WallsAndDoorsOnly = "RealRuins_MapOptions_WallsAndDoorsOnly";
        public static string Text_Option_DisableDecoration = "RealRuins_MapOptions_DisableDecoration";
        public static string Text_Option_DisableTraps = "RealRuins_MapOptions_DisableTraps";
        public static string Text_Option_DisableHostiles = "RealRuins_MapOptions_DisableHostiles";
        public static string Text_Option_Claimable = "RealRuins_MapOptions_Claimable";
        public static string Text_Option_Proximity = "RealRuins_MapOptions_EnableProximity";

        public static string Text_Option_DensityTT = "RealRuins_MapOptions_DensityTT";
        public static string Text_Option_SizeTT = "RealRuins_MapOptions_SizeTT";
        public static string Text_Option_DeteriorationTT = "RealRuins_MapOptions_DeteriorationTT";
        public static string Text_Option_ScavengersTT = "RealRuins_MapOptions_ScavengersTT";
        public static string Text_Option_CostLimitTT = "RealRuins_MapOptions_CostLimitTT";
        public static string Text_Option_DisableHaulablesTT = "RealRuins_MapOptions_DisableHaulablesTT";
        public static string Text_Option_WallsAndDoorsOnlyTT = "RealRuins_MapOptions_WallsAndDoorsOnlyTT";
        public static string Text_Option_DisableDecorationTT = "RealRuins_MapOptions_DisableDecorationTT";
        public static string Text_Option_DisableTrapsTT = "RealRuins_MapOptions_DisableTrapsTT";
        public static string Text_Option_DisableHostilesTT = "RealRuins_MapOptions_DisableHostilesTT";
        public static string Text_Option_ClaimableTT = "RealRuins_MapOptions_ClaimableTT";
        public static string Text_Option_ProximityTT = "RealRuins_MapOptions_EnableProximityTT";

        // fast regex from xml:
        //<RealRuins_M..Options_([^>]*)>[^<]*<\/([^>]*)>     ===>     public static string Text_Option_$1 = "$2";

        public RealRuins_Mod(ModContentPack mcp)
        : base(mcp) {
            LongEventHandler.ExecuteWhenFinished(SetTexts);
            LongEventHandler.ExecuteWhenFinished(GetSettings);
        }

        public void SetTexts() {
            FieldInfo[] fields = GetType().GetFields();

            foreach (FieldInfo fi in fields) {
                if (fi.Name.StartsWith("Text_Option")) {
                    fi.SetValue(null, ((string)fi.GetValue(null)).Translate());
                }
            }

            Text_MapSettings_Category = Text_MapSettings_Category.Translate();
            Text_NetSettings_Category = Text_NetSettings_Category.Translate();
        }



        public void GetSettings() {
            GetSettings<RealRuins_ModSettings>();
            if (RealRuins_ModSettings.defaultScatterOptions == null) {
                Debug.Message("Settings scatter is null! setting default");
                RealRuins_ModSettings.defaultScatterOptions = ScatterOptions.Default;
            }
            //Debug.Message("Settings scatter: {1}", RealRuins_ModSettings.defaultScatterOptions);
        }

        public override void WriteSettings() {
            base.WriteSettings();
            SnapshotStoreManager.Instance.CheckCacheContents();
            SnapshotStoreManager.Instance.CheckCacheSizeLimits();
        }

        public override string SettingsCategory() {
            return Text_NetSettings_Category;
        }

        public override void DoSettingsWindowContents(Rect rect) {
            Rect rect2 = rect.LeftPart(0.45f).Rounded();
            Rect rect3 = rect.RightPart(0.45f).Rounded();
            Listing_Standard left = new Listing_Standard();
            Listing_Standard right = new Listing_Standard();

            left.Begin(rect2);
            //networking settings
            left.CheckboxLabeled(Text_Option_OfflineMode, ref RealRuins_ModSettings.offlineMode, Text_Option_OfflineModeTooltip);
            left.Label(Text_Option_CurrentCacheSize + " " + SnapshotStoreManager.Instance.TotalSize() / (1024 * 1024) + " MB");
            left.Label(Text_Option_CurrentCacheCount + " " + SnapshotStoreManager.Instance.StoredSnapshotsCount());
            left.Label(Text_Option_CacheSize + "  " + ((int)(RealRuins_ModSettings.diskCacheLimit)).ToString() + " MB", -1f, Text_Option_CacheSizeTooltip);
            if (left.ButtonText(Text_Option_DownloadMore, null)) {
                SnapshotManager.Instance.LoadSomeSnapshots();
            }
            left.Gap(25f);

            //generation settings
            int sizeMin = (int)(RealRuins_ModSettings.defaultScatterOptions.referenceRadiusAverage * 0.8f);
            int sizeMax = (int)(RealRuins_ModSettings.defaultScatterOptions.referenceRadiusAverage * 1.2f);
            string costStr = "∞"; if (RealRuins_ModSettings.defaultScatterOptions.itemCostLimit < 1000) {
                costStr = RealRuins_ModSettings.defaultScatterOptions.itemCostLimit.ToString();
            }

            left.Label(Text_Option_Density + ": x" + RealRuins_ModSettings.defaultScatterOptions.densityMultiplier, -1, Text_Option_DensityTT);
            left.Label(Text_Option_Size + ": " + sizeMin + " - " + sizeMax, -1, Text_Option_SizeTT);
            left.Gap(15);
            left.Label(Text_Option_Deterioration + ": " + RealRuins_ModSettings.defaultScatterOptions.deteriorationMultiplier, -1, Text_Option_DeteriorationTT);
            left.Label(Text_Option_Scavengers + ": " + RealRuins_ModSettings.defaultScatterOptions.scavengingMultiplier, -1, Text_Option_ScavengersTT);
            left.Label(Text_Option_CostLimit + ": " + costStr, -1, Text_Option_CostLimitTT);
            left.Gap(15);
            left.Label(Text_Option_DisableDecoration, -1, Text_Option_DisableDecorationTT);
            left.Label(Text_Option_DisableTraps, -1, Text_Option_DisableTrapsTT);
            left.Label(Text_Option_DisableHostiles, -1, Text_Option_DisableHostilesTT);
            left.Gap(15);
            left.CheckboxLabeled(Text_Option_DisableHaulables, ref RealRuins_ModSettings.defaultScatterOptions.disableSpawnItems, Text_Option_DisableHaulablesTT);
            left.CheckboxLabeled(Text_Option_WallsAndDoorsOnly, ref RealRuins_ModSettings.defaultScatterOptions.wallsDoorsOnly, Text_Option_WallsAndDoorsOnlyTT);
            left.CheckboxLabeled(Text_Option_Proximity, ref RealRuins_ModSettings.defaultScatterOptions.enableProximity, Text_Option_ProximityTT);
            left.End();
            right.Begin(rect3);
            right.CheckboxLabeled(Text_Option_AllowDownloads, ref RealRuins_ModSettings.allowDownloads, Text_Option_AllowDownloadsTooltip);
            right.CheckboxLabeled(Text_Option_AllowUploads, ref RealRuins_ModSettings.allowUploads, Text_Option_AllowUploadsTooltip);
            right.Gap(25f);
            RealRuins_ModSettings.diskCacheLimit = right.Slider(RealRuins_ModSettings.diskCacheLimit, 20.0f, 2048.0f);
            if (right.ButtonText(Text_Option_RemoveAll, null)) {
                SnapshotStoreManager.Instance.ClearCache();
            }
            right.Gap(25);

            //generation settings
            RealRuins_ModSettings.defaultScatterOptions.densityMultiplier = right.Slider(RealRuins_ModSettings.defaultScatterOptions.densityMultiplier, 0.0f, 20.0f);
            RealRuins_ModSettings.defaultScatterOptions.referenceRadiusAverage = (int)right.Slider(RealRuins_ModSettings.defaultScatterOptions.referenceRadiusAverage, 4.0f, 64.0f);
            right.Gap(15);
            RealRuins_ModSettings.defaultScatterOptions.deteriorationMultiplier = right.Slider(RealRuins_ModSettings.defaultScatterOptions.deteriorationMultiplier, 0.0f, 1.0f);
            RealRuins_ModSettings.defaultScatterOptions.scavengingMultiplier = right.Slider(RealRuins_ModSettings.defaultScatterOptions.scavengingMultiplier, 0.0f, 5.0f);
            RealRuins_ModSettings.defaultScatterOptions.itemCostLimit = (int)right.Slider(RealRuins_ModSettings.defaultScatterOptions.itemCostLimit, 0.0f, 1000.0f);
            right.Gap(15);
            RealRuins_ModSettings.defaultScatterOptions.decorationChance = right.Slider(RealRuins_ModSettings.defaultScatterOptions.decorationChance, 0.0f, 0.01f);
            RealRuins_ModSettings.defaultScatterOptions.trapChance = right.Slider(RealRuins_ModSettings.defaultScatterOptions.trapChance, 0.0f, 0.01f);
            RealRuins_ModSettings.defaultScatterOptions.hostileChance = right.Slider(RealRuins_ModSettings.defaultScatterOptions.hostileChance, 0.0f, 1.0f);

            right.End();
        }
    }
}
