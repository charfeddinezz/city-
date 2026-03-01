using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.IO;
using System.Linq;
using UnityEditor;

namespace FCG
{
    public class CityGenerator : MonoBehaviour
    {

        private int nB = 0;
        private Vector3 center;
        private int residential = 0;
        private bool _residential = false;

        GameObject cityMaker;

        [HideInInspector]
        public GameObject[] miniBorder;

        [HideInInspector]
        public GameObject[] smallBorder;

        [HideInInspector]
        public GameObject[] mediumBorder;

        [HideInInspector]
        public GameObject[] largeBorder;
        
        [HideInInspector]
        public GameObject[] miniBorderFlat;

        [HideInInspector]
        public GameObject[] smallBorderFlat;

        [HideInInspector]
        public GameObject[] mediumBorderFlat;

        [HideInInspector]
        public GameObject[] largeBorderFlat;
         
        [HideInInspector]
        public GameObject[] miniBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] smallBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] mediumBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] largeBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] largeBlocks;

        private bool[] _largeBlocks;



        [HideInInspector]
        public GameObject[] bigLargeBlocks;


        [HideInInspector]
        public GameObject[] forward50;
        [HideInInspector]
        public GameObject[] forward100;
        [HideInInspector]
        public GameObject[] forward300;
        [HideInInspector]
        public GameObject[] forward400;
        [HideInInspector]
        public GameObject[] forwardLeft400;
        [HideInInspector]
        public GameObject[] forwardRight400;
        [HideInInspector]
        public GameObject[] left200;
        [HideInInspector]
        public GameObject[] left300;
        [HideInInspector]
        public GameObject[] right200;
        [HideInInspector]
        public GameObject[] right300;



        private bool[] _bigLargeBlocks;


        [HideInInspector]
        public GameObject[] BB;  // Buildings in suburban areas (not in the corner)
        [HideInInspector]
        public GameObject[] BC;  // Down Town Buildings(Not in the corner)
        [HideInInspector]
        public GameObject[] BR;  // Residential buildings in suburban areas (not in the corner)
        [HideInInspector]
        public GameObject[] DC;  // Corner buildings that occupy both sides of the block
        [HideInInspector]
        public GameObject[] EB;  // Corner buildings in suburban areas
        [HideInInspector]
        public GameObject[] EC;  // Down Town Corner Buildings 
        [HideInInspector]
        public GameObject[] MB;  //  Buildings that occupy both sides of the block 
        [HideInInspector]
        public GameObject[] BK;  //  Buildings that occupy an entire block
        [HideInInspector]
        public GameObject[] SB;  //  Large buildings that occupy larger blocks 
        [HideInInspector]
        public GameObject[] BBS;  //  Buildings on slopes (neighborhood)
        [HideInInspector]
        public GameObject[] BCS;  //  Down Town Buildings on slopes

        private int[] _BB;
        private int[] _BC;
        private int[] _BR;
        //private int[] _DC;
        private int[] _EB;
        private int[] _EC;

        private int[] _EBS;
        private int[] _ECS;

        private int[] _MB;
        private int[] _BK;
        private int[] _SB;
        private int[] _BBS;
        private int[] _BCS;

        private GameObject[] tempArray;
        private int numB;


        float distCenter = 300;
        bool withDowntownArea = true;
        float downTownSize = 100;

        [Header("Spider Network")]
        public bool generateSpiderNetwork = false;
        public bool smartChainCityRoadNetwork = true;
        public bool automaticSpiderCounts = true;
        [Range(1, 10000)]
        public int mainCitiesCount = 1;
        [Range(0, 10000)]
        public int subCitiesPerMain = 1;
        [Range(0, 10000)]
        public int extraRoadLinks = 2;

        [Header("Advanced Spider Options")]
        public bool generateCityByCity = true;
        public bool automaticAdvancedRoadOptions = true;
        public bool automaticBridgeRouting = true;
        public bool preventRoadCrossing = true;
        public bool enforceRoadContinuity = true;
        [Range(0f, 1f)]
        public float continuitySnapBlend = 0.55f;
        [Range(5f, 120f)]
        public float intersectionSafetyDistance = 30f;
        [Range(5f, 60f)]
        public float maxRoadSlopeDegrees = 26f;
        [Range(800f, 30000f)]
        public float cityAreaRadius = 6000f;
        [Range(500f, 3500f)]
        public float minMainCityDistance = 1500f;
        [Range(50f, 800f)]
        public float roadSegmentSpacing = 240f;
        [Range(1f, 32f)]
        public float roadClearanceRadius = 10f;
        [Range(5f, 80f)]
        public float roadSnapDistance = 18f;
        [Range(3f, 100f)]
        public float bridgeHeightThreshold = 16f;
        [Range(0f, 1f)]
        public float bridgeSmoothing = 0.65f;

        [Header("Intelligent Road Typing")]
        public bool smartRoadTypeSelection = true;
        [Range(350f, 4000f)]
        public float intercityDistanceThreshold = 900f;
        [Range(120f, 1200f)]
        public float cityInfluenceRadius = 420f;
        [Range(0f, 1f)]
        public float roadTypeBlend = 0.45f;

        private readonly System.Collections.Generic.HashSet<string> roadLinks = new System.Collections.Generic.HashSet<string>();
        private readonly Collider[] roadOverlapBuffer = new Collider[96];
        private readonly System.Collections.Generic.List<Vector3> cityInfluencePoints = new System.Collections.Generic.List<Vector3>();
        private readonly System.Collections.Generic.List<RoadSegment2D> reservedRoadSegments = new System.Collections.Generic.List<RoadSegment2D>();
        private readonly System.Collections.Generic.List<Vector3> roadNodeCache = new System.Collections.Generic.List<Vector3>();

        [Header("Memory Guard")]
        public bool enableMemoryGuard = true;
        public bool smartAdaptiveBudget = true;

        [Header("Generation Quality")]
        [Range(0, 2)]
        public int roadQualityPreset = 1; // 0 Fast, 1 Balanced, 2 High Quality
        public bool enableGenerationAnalytics = true;
        public bool reinforceLowDegreeAnchors = true;
        [Range(1, 4)]
        public int targetAnchorDegree = 2;

        private int placedRoadPrefabsInCurrentNetwork = 0;
        private int adaptiveRoadPrefabBudget = 0;
        private const int DefaultAutomaticRoadPrefabBudget = 20000;

        private int analyticsLinksAttempted = 0;
        private int analyticsLinksAccepted = 0;
        private int analyticsLinksRejectedDuplicate = 0;
        private int analyticsLinksRejectedCrossing = 0;
        private int analyticsLinksRejectedTooClose = 0;
        private int analyticsPlacementResolveSuccess = 0;
        private int analyticsReinforcedLinks = 0;

        private struct RoadSegment2D
        {
            public Vector2 a;
            public Vector2 b;

            public RoadSegment2D(Vector2 start, Vector2 end)
            {
                a = start;
                b = end;
            }
        }

        public void ClearCity()
        {
            if (!cityMaker)
                cityMaker = GameObject.Find("City-Maker");

            if (cityMaker)
                DestroyImmediate(cityMaker);

            roadNodeCache.Clear();

        }

        public void GenerateCity(int size, bool withSatteliteCity = false, bool borderFlat = false, int satteliteCitiesCount = 1)
        {

            if (generateSpiderNetwork && mainCitiesCount > 1)
            {
                GenerateSpiderNetwork(size, borderFlat, satteliteCitiesCount);
                return;
            }

            bool satCity = false;

            if (size == 1)
            {
                // Very Small City
                satCity = GenerateStreetsVerySmall(borderFlat, withSatteliteCity);
            }
            else if (size == 2)
            {
                // Small City
                satCity = GenerateStreetsSmall(borderFlat, withSatteliteCity );
            }
            else if (size == 3)
            {
                // Medium City
                satCity = GenerateStreets(borderFlat, withSatteliteCity);
            }
            else if (size == 4)
            {
                // Large City
                satCity = GenerateStreetsBig(borderFlat, withSatteliteCity);
            }
            else
            {
                // Very Large City
                satCity = GenerateStreetsVeryLarge(borderFlat, withSatteliteCity);
            }


            if (satCity)
            {
                int satellitesToGenerate = Mathf.Max(1, satteliteCitiesCount);

                for (int satelliteIndex = 0; satelliteIndex < satellitesToGenerate; satelliteIndex++)
                {
                    Transform exitPositipon = CityExitPosition();

                    if (exitPositipon != null)
                    {
                        GenerateSatteliteCityFromExit(exitPositipon);
                    }
                    else
                    {
                        Debug.Log("ExitCity gameobject not found");
                        break;
                    }
                }
            }



            DayNight dayNight = FindObjectOfType<DayNight>();
            if (dayNight)
                dayNight.ChangeMaterial();

        }

        private void ResolveAutomaticSpiderOptions(int primaryCitySize)
        {
            int safeCitySize = Mathf.Clamp(primaryCitySize, 1, 5);

            if (automaticSpiderCounts)
            {
                int targetMainCities = Mathf.Clamp(mainCitiesCount, 1, 10000);
                float sizeFactor = Mathf.Lerp(0.8f, 1.7f, (safeCitySize - 1f) / 4f);

                subCitiesPerMain = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(1f, targetMainCities * 0.45f * sizeFactor)), 0, 10000);
                extraRoadLinks = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(1f, targetMainCities * 0.75f * sizeFactor)), 0, 10000);
            }

            if (automaticAdvancedRoadOptions)
            {
                float crowdedScale = Mathf.Clamp01(mainCitiesCount / 12f);
                minMainCityDistance = Mathf.Clamp(1200f + (safeCitySize * 240f) + (crowdedScale * 700f), 500f, 3500f);
                cityAreaRadius = Mathf.Clamp(minMainCityDistance * (2.8f + (mainCitiesCount * 0.08f)), 800f, 30000f);
                continuitySnapBlend = Mathf.Clamp01(0.45f + (crowdedScale * 0.3f));
                intersectionSafetyDistance = Mathf.Clamp(24f + (crowdedScale * 24f), 5f, 120f);
                maxRoadSlopeDegrees = Mathf.Clamp(28f - (safeCitySize * 1.8f), 5f, 60f);
                roadSegmentSpacing = Mathf.Clamp(220f - (crowdedScale * 40f) - (safeCitySize * 10f), 50f, 800f);
                roadClearanceRadius = Mathf.Clamp(10f + (crowdedScale * 8f), 1f, 32f);
                roadSnapDistance = Mathf.Clamp(16f + (safeCitySize * 2f) + (crowdedScale * 10f), 5f, 80f);
                intercityDistanceThreshold = Mathf.Clamp(minMainCityDistance * 0.62f, 350f, 4000f);
                cityInfluenceRadius = Mathf.Clamp(260f + (safeCitySize * 60f) + (crowdedScale * 180f), 120f, 1200f);
                roadTypeBlend = Mathf.Clamp01(0.33f + (safeCitySize * 0.06f));
            }

            if (automaticBridgeRouting)
            {
                float topologyScale = Mathf.Clamp01(mainCitiesCount / 10f);
                bridgeHeightThreshold = Mathf.Clamp(12f + (safeCitySize * 2f) + (topologyScale * 14f), 3f, 100f);
                bridgeSmoothing = Mathf.Clamp01(0.55f + (safeCitySize * 0.05f));
            }

            ApplyRoadQualityPreset();
        }

        private void ApplyRoadQualityPreset()
        {
            int preset = Mathf.Clamp(roadQualityPreset, 0, 2);
            if (preset == 0)
            {
                continuitySnapBlend = Mathf.Clamp01(continuitySnapBlend * 0.8f);
                roadSegmentSpacing = Mathf.Clamp(roadSegmentSpacing * 1.2f, 50f, 800f);
                intersectionSafetyDistance = Mathf.Clamp(intersectionSafetyDistance * 0.8f, 5f, 120f);
                targetAnchorDegree = 1;
            }
            else if (preset == 2)
            {
                continuitySnapBlend = Mathf.Clamp01(continuitySnapBlend * 1.15f);
                roadSegmentSpacing = Mathf.Clamp(roadSegmentSpacing * 0.85f, 50f, 800f);
                intersectionSafetyDistance = Mathf.Clamp(intersectionSafetyDistance * 1.2f, 5f, 120f);
                maxRoadSlopeDegrees = Mathf.Clamp(maxRoadSlopeDegrees * 0.85f, 5f, 60f);
                targetAnchorDegree = Mathf.Clamp(targetAnchorDegree + 1, 1, 4);
            }
        }

        private int CalculateAdaptiveRoadBudget(int primaryCitySize)
        {
            int baseBudget = Mathf.Max(500, DefaultAutomaticRoadPrefabBudget);
            if (!smartAdaptiveBudget)
                return baseBudget;

            int safeCitySize = Mathf.Clamp(primaryCitySize, 1, 5);
            float memorySizeMB = (float)SystemInfo.systemMemorySize;
            float memoryScale = Mathf.Clamp(memorySizeMB / 8192f, 0.65f, 1.8f);
            float complexity = Mathf.Max(1f, mainCitiesCount + (subCitiesPerMain * 0.5f) + (extraRoadLinks * 0.35f));
            float complexityPenalty = Mathf.Clamp(250f / complexity, 0.35f, 1f);
            float sizeBoost = Mathf.Lerp(0.85f, 1.35f, (safeCitySize - 1f) / 4f);

            int adaptiveBudget = Mathf.RoundToInt(baseBudget * memoryScale * complexityPenalty * sizeBoost);
            return Mathf.Clamp(adaptiveBudget, 500, 200000);
        }

        private float CalculateAdaptiveSpacingBoost()
        {
            if (!smartAdaptiveBudget || adaptiveRoadPrefabBudget <= 0)
                return 1f;

            float usage = placedRoadPrefabsInCurrentNetwork / (float)adaptiveRoadPrefabBudget;
            if (usage < 0.55f)
                return 1f;

            return Mathf.Lerp(1f, 1.45f, Mathf.InverseLerp(0.55f, 1f, Mathf.Clamp01(usage)));
        }


        private int CalculateAutomaticChainBranchFactor(int targetCities)
        {
            int safeCities = Mathf.Clamp(targetCities, 1, 10000);
            if (safeCities <= 2)
                return 1;
            if (safeCities <= 6)
                return 2;
            if (safeCities <= 14)
                return 3;
            if (safeCities <= 32)
                return 4;

            return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(4f, 6f, Mathf.Clamp01((safeCities - 32f) / 128f))), 1, 8);
        }

        private int CalculateAutomaticChainStepLimit(int targetCities, int branchFactor)
        {
            int safeCities = Mathf.Clamp(targetCities, 1, 10000);
            int safeBranch = Mathf.Clamp(branchFactor, 1, 8);
            int estimatedSteps = Mathf.CeilToInt(safeCities / (float)safeBranch);
            int bufferedSteps = Mathf.CeilToInt(estimatedSteps * 1.35f);
            return Mathf.Clamp(bufferedSteps + 4, 1, 10000);
        }

        private void GenerateSpiderNetwork(int primaryCitySize, bool borderFlat, int defaultSatelliteCount)
        {
            ResolveAutomaticSpiderOptions(primaryCitySize);

            ClearCity();
            cityMaker = new GameObject("City-Maker");
            roadLinks.Clear();
            reservedRoadSegments.Clear();
            cityInfluencePoints.Clear();
            roadNodeCache.Clear();
            analyticsLinksAttempted = 0;
            analyticsLinksAccepted = 0;
            analyticsLinksRejectedDuplicate = 0;
            analyticsLinksRejectedCrossing = 0;
            analyticsLinksRejectedTooClose = 0;
            analyticsPlacementResolveSuccess = 0;
            analyticsReinforcedLinks = 0;
            placedRoadPrefabsInCurrentNetwork = 0;
            adaptiveRoadPrefabBudget = CalculateAdaptiveRoadBudget(primaryCitySize);

            if (enableMemoryGuard && smartAdaptiveBudget)
                Debug.Log("[FCG] Adaptive road budget: " + adaptiveRoadPrefabBudget + " prefabs");

            int maxMainCities = Mathf.Max(1, Mathf.Min(mainCitiesCount, 10000));
            int satellitesPerMainCity = Mathf.Max(defaultSatelliteCount, subCitiesPerMain);

            var cityAnchors = new System.Collections.Generic.List<Vector3>(maxMainCities);

            if (smartChainCityRoadNetwork)
            {
                GenerateSmartChainNetwork(primaryCitySize, borderFlat, satellitesPerMainCity, maxMainCities, cityAnchors);
            }
            else
            {
                for (int mainIndex = 0; mainIndex < maxMainCities; mainIndex++)
                {
                    int citySize = (mainIndex == 0) ? primaryCitySize : Random.Range(1, 6);
                    Vector3 cityOffset = (mainIndex == 0) ? Vector3.zero : RandomCityOffset(cityAnchors);
                    cityAnchors.Add(cityOffset);
                    RegisterCityInfluencePoint(cityOffset, true);

                    int borderType = Random.Range(0, 4);
                    GenerateCityCluster(citySize, cityOffset, borderFlat, satellitesPerMainCity, borderType);

                    if (generateCityByCity && mainIndex > 0)
                    {
                        CreateRoadConnection(cityAnchors[mainIndex - 1], cityOffset, 1f, true);
                    }
                }

                BuildSpiderRoadLinks(cityAnchors);
            }

            if (enableGenerationAnalytics)
                Debug.Log(GetLastGenerationReport());

            DayNight dayNight = FindObjectOfType<DayNight>();
            if (dayNight)
                dayNight.ChangeMaterial();
        }

        private void GenerateSmartChainNetwork(int primaryCitySize, bool borderFlat, int satellitesPerMainCity, int maxMainCities, System.Collections.Generic.List<Vector3> cityAnchors)
        {
            int safeBranchFactor = Mathf.Clamp(CalculateAutomaticChainBranchFactor(maxMainCities), 1, 8);
            int safeStepLimit = Mathf.Clamp(CalculateAutomaticChainStepLimit(maxMainCities, safeBranchFactor), 1, 10000);
            int safeSatelliteCount = Mathf.Clamp(satellitesPerMainCity, 0, Mathf.Min(24, subCitiesPerMain + 4));

            cityAnchors.Add(Vector3.zero);
            RegisterCityInfluencePoint(Vector3.zero, true);
            GenerateCityCluster(primaryCitySize, Vector3.zero, borderFlat, safeSatelliteCount, Random.Range(0, 4));

            var frontier = new System.Collections.Generic.List<int>(maxMainCities);
            frontier.Add(0);
            int frontierHead = 0;
            int chainSteps = 0;

            while (cityAnchors.Count < maxMainCities && frontierHead < frontier.Count && chainSteps < safeStepLimit)
            {
                int parentIndex = frontier[frontierHead++];
                Vector3 parent = cityAnchors[parentIndex];

                for (int branch = 0; branch < safeBranchFactor && cityAnchors.Count < maxMainCities; branch++)
                {
                    Vector3 child = RandomChainedCityOffset(parent, cityAnchors);
                    int childIndex = cityAnchors.Count;
                    cityAnchors.Add(child);
                    RegisterCityInfluencePoint(child, true);
                    frontier.Add(childIndex);

                    int citySize = Random.Range(1, 6);
                    GenerateCityCluster(citySize, child, borderFlat, safeSatelliteCount, Random.Range(0, 4));

                    CreateRoadConnection(parent, child, 0.95f, true);

                    int nearest = FindNearestAnchorIndex(cityAnchors, childIndex, 1);
                    if (nearest >= 0 && nearest != parentIndex)
                        CreateRoadConnection(child, cityAnchors[nearest], 0.7f, true);

                    if (branch > 0)
                    {
                        int neighborRank = Mathf.Clamp(branch, 1, 3);
                        int sideNeighbor = FindNearestAnchorIndex(cityAnchors, childIndex, neighborRank);
                        if (sideNeighbor >= 0 && sideNeighbor != parentIndex)
                            CreateRoadConnection(child, cityAnchors[sideNeighbor], 0.6f, true);
                    }
                }

                chainSteps++;
                if (enableMemoryGuard && placedRoadPrefabsInCurrentNetwork >= Mathf.Max(500, adaptiveRoadPrefabBudget))
                    break;
            }

            BuildSpiderRoadLinks(cityAnchors);
        }

        private Vector3 RandomChainedCityOffset(Vector3 source, System.Collections.Generic.List<Vector3> existingAnchors)
        {
            float minRadius = minMainCityDistance * 0.82f;
            float maxRadius = minMainCityDistance * 1.35f;

            for (int attempt = 0; attempt < 40; attempt++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float radius = Random.Range(minRadius, maxRadius);
                Vector3 candidate = source + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                candidate = ClampToCityArea(candidate);

                bool valid = true;
                for (int i = 0; i < existingAnchors.Count; i++)
                {
                    if (Vector3.Distance(existingAnchors[i], candidate) < minMainCityDistance * 0.72f)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    return candidate;
            }

            return ClampToCityArea(source + new Vector3(Random.Range(minRadius, maxRadius), 0, Random.Range(minRadius, maxRadius)));
        }

        private Vector3 RandomCityOffset(System.Collections.Generic.List<Vector3> existingAnchors)
        {
            int safeAttempts = 0;
            while (safeAttempts < 45)
            {
                safeAttempts++;
                float radius = minMainCityDistance * Mathf.Sqrt(safeAttempts + Random.Range(0.3f, 1f));
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector3 candidate = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                candidate = ClampToCityArea(candidate);

                bool isFarFromOthers = true;
                for (int i = 0; i < existingAnchors.Count; i++)
                {
                    if (Vector3.Distance(existingAnchors[i], candidate) < minMainCityDistance)
                    {
                        isFarFromOthers = false;
                        break;
                    }
                }

                if (isFarFromOthers)
                    return candidate;
            }

            float fallbackRadius = minMainCityDistance * (existingAnchors.Count + 1);
            float fallbackAngle = existingAnchors.Count * 0.95f;
            return ClampToCityArea(new Vector3(Mathf.Cos(fallbackAngle) * fallbackRadius, 0, Mathf.Sin(fallbackAngle) * fallbackRadius));
        }

        private Vector3 ClampToCityArea(Vector3 candidate)
        {
            float maxRadius = Mathf.Max(minMainCityDistance * 1.2f, cityAreaRadius);
            Vector2 flat = new Vector2(candidate.x, candidate.z);
            if (flat.sqrMagnitude <= maxRadius * maxRadius)
                return candidate;

            flat = flat.normalized * maxRadius;
            candidate.x = flat.x;
            candidate.z = flat.y;
            return candidate;
        }

        private void GenerateCityCluster(int citySize, Vector3 offset, bool borderFlat, int satelliteCount, int borderType)
        {
            GameObject[] selectedBorder = GetBorderBySize(citySize, borderFlat, false);
            GameObject[] selectedBlocks = (citySize == 4 && bigLargeBlocks.Length > 0 && Random.value > 0.4f) ? bigLargeBlocks : largeBlocks;
            if (selectedBorder == null || selectedBorder.Length == 0 || selectedBlocks == null || selectedBlocks.Length == 0)
                return;

            int blockCount = Mathf.Clamp(citySize + 1, 2, 7);
            for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
            {
                Vector3 blockOffset = new Vector3((blockIndex % 3) * 300 - 300, 0, (blockIndex / 3) * 300 - 150);
                Vector3 jitter = new Vector3(Random.Range(-60f, 60f), 0, Random.Range(-60f, 60f));
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
                Instantiate(selectedBlocks[Random.Range(0, selectedBlocks.Length)], offset + blockOffset + jitter, rotation, cityMaker.transform);
            }

            Quaternion borderRotation = Quaternion.Euler(0, borderType * 90f, 0);
            Instantiate(selectedBorder[Random.Range(0, selectedBorder.Length)], offset, borderRotation, cityMaker.transform);

            int satellitesToSpawn = Mathf.Min(Mathf.Max(0, satelliteCount), 10000);
            for (int satelliteIndex = 0; satelliteIndex < satellitesToSpawn; satelliteIndex++)
            {
                Vector3 satOffset = offset + Quaternion.Euler(0, satelliteIndex * (360f / Mathf.Max(1, satellitesToSpawn)), 0) * new Vector3(0, 0, Random.Range(700f, 1200f));
                RegisterCityInfluencePoint(satOffset, false);
                GenerateSubCity(satOffset, borderFlat);
                CreateRoadConnection(offset, satOffset, 0.65f, true);
            }
        }

        private void GenerateSubCity(Vector3 offset, bool borderFlat)
        {
            GameObject[] subBorder = (Random.value > 0.5f) ? (borderFlat ? miniBorderFlat : miniBorder) : (borderFlat ? smallBorderFlat : smallBorder);
            if (subBorder == null || subBorder.Length == 0 || largeBlocks.Length == 0)
                return;

            int blockCount = Random.Range(1, 4);
            for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
            {
                Vector3 blockOffset = new Vector3((blockIndex - 1) * 250, 0, Random.Range(-180f, 180f));
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
                Instantiate(largeBlocks[Random.Range(0, largeBlocks.Length)], offset + blockOffset, rotation, cityMaker.transform);
            }

            Instantiate(subBorder[Random.Range(0, subBorder.Length)], offset, Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0), cityMaker.transform);
            RegisterCityInfluencePoint(offset, false);
        }

        private void RegisterCityInfluencePoint(Vector3 point, bool isMainCity)
        {
            if (cityInfluencePoints.Count >= 20000)
                return;

            float mergeDistance = isMainCity ? cityInfluenceRadius * 0.7f : cityInfluenceRadius * 0.45f;
            mergeDistance = Mathf.Max(60f, mergeDistance);

            for (int i = 0; i < cityInfluencePoints.Count; i++)
            {
                if (Vector3.Distance(cityInfluencePoints[i], point) <= mergeDistance)
                    return;
            }

            cityInfluencePoints.Add(point);
        }

        private float SampleCityInfluence(Vector3 position)
        {
            if (!smartRoadTypeSelection || cityInfluencePoints.Count == 0)
                return 0f;

            float nearest = float.MaxValue;
            float secondary = float.MaxValue;

            for (int i = 0; i < cityInfluencePoints.Count; i++)
            {
                float d = Vector3.Distance(position, cityInfluencePoints[i]);
                if (d < nearest)
                {
                    secondary = nearest;
                    nearest = d;
                }
                else if (d < secondary)
                {
                    secondary = d;
                }
            }

            float nearestInfluence = 1f - Mathf.Clamp01(nearest / Mathf.Max(120f, cityInfluenceRadius));
            float secondaryInfluence = secondary < float.MaxValue ? 1f - Mathf.Clamp01(secondary / Mathf.Max(180f, cityInfluenceRadius * 1.5f)) : 0f;

            return Mathf.Clamp01(Mathf.Lerp(nearestInfluence, Mathf.Max(nearestInfluence, secondaryInfluence), roadTypeBlend));
        }

        private GameObject[] GetRoadPrefabSetByTier(int tier)
        {
            if (tier <= 0)
            {
                if (forward50 != null && forward50.Length > 0)
                    return forward50;
                if (forward100 != null && forward100.Length > 0)
                    return forward100;
            }
            else if (tier == 1)
            {
                if (forward100 != null && forward100.Length > 0)
                    return forward100;
                if (forward300 != null && forward300.Length > 0)
                    return forward300;
            }
            else if (tier == 2)
            {
                if (forward300 != null && forward300.Length > 0)
                    return forward300;
                if (forward400 != null && forward400.Length > 0)
                    return forward400;
            }
            else
            {
                if (forward400 != null && forward400.Length > 0)
                    return forward400;
                if (forward300 != null && forward300.Length > 0)
                    return forward300;
            }

            if (forward50 != null && forward50.Length > 0)
                return forward50;
            if (forward100 != null && forward100.Length > 0)
                return forward100;
            if (forward300 != null && forward300.Length > 0)
                return forward300;
            return (forward400 != null && forward400.Length > 0) ? forward400 : null;
        }

        private GameObject[] GetBorderBySize(int citySize, bool borderFlat, bool withExit)
        {
            switch (citySize)
            {
                case 1:
                    if (withExit && miniBorderWithExitOfCity.Length > 0) return miniBorderWithExitOfCity;
                    return borderFlat ? miniBorderFlat : miniBorder;
                case 2:
                    if (withExit && smallBorderWithExitOfCity.Length > 0) return smallBorderWithExitOfCity;
                    return borderFlat ? smallBorderFlat : smallBorder;
                case 3:
                    if (withExit && mediumBorderWithExitOfCity.Length > 0) return mediumBorderWithExitOfCity;
                    return borderFlat ? mediumBorderFlat : mediumBorder;
                default:
                    if (withExit && largeBorderWithExitOfCity.Length > 0) return largeBorderWithExitOfCity;
                    return borderFlat ? largeBorderFlat : largeBorder;
            }
        }

        private void BuildSpiderRoadLinks(System.Collections.Generic.List<Vector3> cityAnchors)
        {
            if (cityAnchors.Count < 2)
                return;

            Vector3 center = cityAnchors[0];
            for (int i = 1; i < cityAnchors.Count; i++)
            {
                CreateRoadConnection(center, cityAnchors[i], 1f, true);
            }

            // Smart side-road linking: each city tries to connect with its nearest neighbors.
            int nearestNeighborCount = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(1f, 3f, Mathf.Clamp01(cityAnchors.Count / 10f))), 1, 3);
            for (int i = 0; i < cityAnchors.Count; i++)
            {
                for (int linkIndex = 0; linkIndex < nearestNeighborCount; linkIndex++)
                {
                    int nearest = FindNearestAnchorIndex(cityAnchors, i, linkIndex + 1);
                    if (nearest >= 0)
                        CreateRoadConnection(cityAnchors[i], cityAnchors[nearest], 0.7f, true);
                }
            }

            int linksToBuild = Mathf.Min(Mathf.Max(0, extraRoadLinks), 10000);
            BuildCostBasedExtraLinks(cityAnchors, linksToBuild);
            ConnectSparseAnchors(cityAnchors);
            ReinforceLowDegreeAnchorConnectivity(cityAnchors);
        }


        private void BuildCostBasedExtraLinks(System.Collections.Generic.List<Vector3> cityAnchors, int linksToBuild)
        {
            if (linksToBuild <= 0 || cityAnchors.Count < 3)
                return;

            var usedPairs = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < linksToBuild; i++)
            {
                int bestA = -1;
                int bestB = -1;
                float bestScore = float.MinValue;

                int tries = Mathf.Min(64, cityAnchors.Count * 2);
                for (int attempt = 0; attempt < tries; attempt++)
                {
                    int a = Random.Range(0, cityAnchors.Count);
                    int b = Random.Range(0, cityAnchors.Count);
                    if (a == b)
                        continue;

                    string key = a < b ? a + "_" + b : b + "_" + a;
                    if (usedPairs.Contains(key))
                        continue;

                    Vector3 pa = cityAnchors[a];
                    Vector3 pb = cityAnchors[b];
                    float distance = Vector3.Distance(pa, pb);
                    if (distance < minMainCityDistance * 0.35f || distance > minMainCityDistance * 2.2f)
                        continue;

                    Vector2 sa = new Vector2(pa.x, pa.z);
                    Vector2 sb = new Vector2(pb.x, pb.z);
                    if (preventRoadCrossing && (WouldCrossExistingRoad(sa, sb) || IsSegmentTooCloseToExisting(sa, sb, Mathf.Max(12f, roadSnapDistance * 1.1f))))
                        continue;

                    float normalized = Mathf.Clamp01((distance - minMainCityDistance * 0.35f) / Mathf.Max(1f, minMainCityDistance * 1.85f));
                    float score = 1f - Mathf.Abs(normalized - 0.55f);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestA = a;
                        bestB = b;
                    }
                }

                if (bestA >= 0)
                {
                    string finalKey = bestA < bestB ? bestA + "_" + bestB : bestB + "_" + bestA;
                    usedPairs.Add(finalKey);
                    CreateRoadConnection(cityAnchors[bestA], cityAnchors[bestB], 0.55f, true);
                }
            }
        }

        private void ConnectSparseAnchors(System.Collections.Generic.List<Vector3> cityAnchors)
        {
            if (cityAnchors.Count < 3)
                return;

            float sparseThreshold = minMainCityDistance * 1.55f;
            for (int i = 0; i < cityAnchors.Count; i++)
            {
                int nearest = FindNearestAnchorIndex(cityAnchors, i, 1);
                if (nearest < 0)
                    continue;

                float nearestDistance = Vector3.Distance(cityAnchors[i], cityAnchors[nearest]);
                if (nearestDistance > sparseThreshold)
                {
                    int secondNearest = FindNearestAnchorIndex(cityAnchors, i, 2);
                    if (secondNearest >= 0)
                        CreateRoadConnection(cityAnchors[i], cityAnchors[secondNearest], 0.75f, true);
                }
            }
        }


        private void ReinforceLowDegreeAnchorConnectivity(System.Collections.Generic.List<Vector3> cityAnchors)
        {
            if (!reinforceLowDegreeAnchors || cityAnchors.Count < 3)
                return;

            int desiredDegree = Mathf.Clamp(targetAnchorDegree, 1, 4);
            int maxAddedLinks = Mathf.Min(cityAnchors.Count * 2, 1000);

            for (int i = 0; i < cityAnchors.Count && maxAddedLinks > 0; i++)
            {
                int degree = CountAnchorDegree(cityAnchors[i]);
                if (degree >= desiredDegree)
                    continue;

                for (int rank = 1; rank <= 4 && degree < desiredDegree && maxAddedLinks > 0; rank++)
                {
                    int nearest = FindNearestAnchorIndex(cityAnchors, i, rank);
                    if (nearest < 0 || nearest == i)
                        continue;

                    int before = analyticsLinksAccepted;
                    CreateRoadConnection(cityAnchors[i], cityAnchors[nearest], 0.8f, true);
                    if (analyticsLinksAccepted > before)
                    {
                        analyticsReinforcedLinks++;
                        maxAddedLinks--;
                        degree = CountAnchorDegree(cityAnchors[i]);
                    }
                }
            }
        }

        private int CountAnchorDegree(Vector3 anchor)
        {
            int degree = 0;
            Vector2 point = new Vector2(anchor.x, anchor.z);
            float tolerance = Mathf.Max(20f, roadSnapDistance * 1.2f);

            for (int i = 0; i < reservedRoadSegments.Count; i++)
            {
                RoadSegment2D segment = reservedRoadSegments[i];
                if (Vector2.Distance(point, segment.a) <= tolerance || Vector2.Distance(point, segment.b) <= tolerance)
                    degree++;
            }

            return degree;
        }

        private int FindNearestAnchorIndex(System.Collections.Generic.List<Vector3> anchors, int sourceIndex, int rank)
        {
            int nearestIndex = -1;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < anchors.Count; i++)
            {
                if (i == sourceIndex)
                    continue;

                float candidateDistance = Vector3.Distance(anchors[sourceIndex], anchors[i]);
                if (candidateDistance < nearestDistance)
                {
                    nearestDistance = candidateDistance;
                    nearestIndex = i;
                }
            }

            if (rank <= 1 || nearestIndex < 0)
                return nearestIndex;

            for (int iteration = 2; iteration <= rank; iteration++)
            {
                float nextDistance = float.MaxValue;
                int nextIndex = -1;

                for (int i = 0; i < anchors.Count; i++)
                {
                    if (i == sourceIndex)
                        continue;

                    float candidateDistance = Vector3.Distance(anchors[sourceIndex], anchors[i]);
                    if (candidateDistance > nearestDistance && candidateDistance < nextDistance)
                    {
                        nextDistance = candidateDistance;
                        nextIndex = i;
                    }
                }

                if (nextIndex < 0)
                    break;

                nearestDistance = nextDistance;
                nearestIndex = nextIndex;
            }

            return nearestIndex;
        }

        private void CreateRoadConnection(Vector3 start, Vector3 end, float density, bool preventDuplicates = false)
        {
            float estimatedDistance = Vector3.Distance(start, end);

            if (preventDuplicates && !ReserveRoadLink(start, end))
                return;

            Vector3 delta = end - start;
            float distance = delta.magnitude;
            if (distance < 10f)
                return;

            Vector3 direction = delta.normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            float densitySpacing = Mathf.Lerp(360f, 180f, Mathf.Clamp01(density));
            float adaptiveSpacing = Mathf.Min(800f, roadSegmentSpacing * CalculateAdaptiveSpacingBoost());
            float spacing = Mathf.Max(90f, Mathf.Min(adaptiveSpacing, densitySpacing));
            int segments = Mathf.Max(1, Mathf.FloorToInt(distance / spacing));

            float startGround = SampleGroundHeight(start);
            float endGround = SampleGroundHeight(end);
            float endPointHeightDelta = Mathf.Abs(endGround - startGround);
            bool canBridge = automaticBridgeRouting && endPointHeightDelta >= bridgeHeightThreshold;
            float intercityFactor = Mathf.Clamp01((estimatedDistance - Mathf.Max(250f, intercityDistanceThreshold * 0.45f)) / Mathf.Max(250f, intercityDistanceThreshold));

            for (int segmentIndex = 0; segmentIndex <= segments; segmentIndex++)
            {
                float t = segmentIndex / (float)segments;
                Vector3 roadPos = Vector3.Lerp(start, end, t);
                float desiredHeight = Mathf.Lerp(startGround, endGround, t);

                if (segmentIndex > 0 && segmentIndex < segments)
                {
                    Vector3 snapped = SnapRoadToGround(roadPos);

                    if (canBridge)
                    {
                        float terrainHeight = snapped.y;
                        float smoothedHeight = Mathf.Lerp(terrainHeight, desiredHeight, bridgeSmoothing);

                        if (Mathf.Abs(desiredHeight - terrainHeight) >= bridgeHeightThreshold * 0.35f)
                            roadPos.y = smoothedHeight;
                        else
                            roadPos.y = terrainHeight;
                    }
                    else
                    {
                        roadPos = snapped;
                    }

                    roadPos = SnapRoadToExistingRoad(roadPos, Mathf.Max(6f, roadSnapDistance), continuitySnapBlend);
                }
                else
                {
                    roadPos.y = desiredHeight;
                }

                roadPos = LimitRoadSlope(roadPos, desiredHeight);

                float cityInfluence = SampleCityInfluence(roadPos);
                GameObject[] roadChoices = SelectRoadPrefabsForConnection(density, estimatedDistance, cityInfluence, intercityFactor, t);
                if (roadChoices == null || roadChoices.Length == 0)
                    continue;

                float collisionRadius = Mathf.Lerp(roadClearanceRadius * 0.8f, roadClearanceRadius * 1.2f, Mathf.Clamp01(1f - cityInfluence));
                Vector3 placementPos = roadPos;
                if (!TryResolveRoadPlacement(placementPos, direction, collisionRadius, out placementPos))
                    continue;

                if (enableMemoryGuard && placedRoadPrefabsInCurrentNetwork >= Mathf.Max(500, adaptiveRoadPrefabBudget))
                    break;

                Instantiate(roadChoices[Random.Range(0, roadChoices.Length)], placementPos, rotation, cityMaker.transform);
                RegisterRoadNode(placementPos);
                placedRoadPrefabsInCurrentNetwork++;
            }
        }

        private GameObject[] SelectRoadPrefabsForConnection(float density, float distance, float cityInfluence, float intercityFactor, float segmentT)
        {
            bool longDistance = distance >= Mathf.Max(1000f, intercityDistanceThreshold * 1.15f);
            bool mediumDistance = distance >= Mathf.Max(500f, intercityDistanceThreshold * 0.7f);
            bool highDensity = density >= 0.85f;
            bool mediumDensity = density >= 0.65f;

            if (!smartRoadTypeSelection)
            {
                if (longDistance)
                    return GetRoadPrefabSetByTier(3);
                if (mediumDistance || highDensity)
                    return GetRoadPrefabSetByTier(2);
                if (mediumDensity)
                    return GetRoadPrefabSetByTier(1);
                return GetRoadPrefabSetByTier(0);
            }

            float corridor = 1f - Mathf.Abs((segmentT * 2f) - 1f);
            float corridorIntercityBoost = corridor * intercityFactor * 0.35f;
            float outsideScore = Mathf.Clamp01((1f - cityInfluence) * 0.65f + intercityFactor * 0.55f + corridorIntercityBoost);
            float insideScore = Mathf.Clamp01(cityInfluence * 0.8f + density * 0.2f);

            if (outsideScore >= 0.78f || longDistance)
                return GetRoadPrefabSetByTier(3);

            if (outsideScore >= 0.55f || (mediumDistance && cityInfluence < 0.45f))
                return GetRoadPrefabSetByTier(2);

            if (insideScore >= 0.62f || mediumDensity || highDensity)
                return GetRoadPrefabSetByTier(1);

            return GetRoadPrefabSetByTier(0);
        }

        private bool ReserveRoadLink(Vector3 a, Vector3 b)
        {
            string first = Mathf.RoundToInt(a.x) + "_" + Mathf.RoundToInt(a.z);
            string second = Mathf.RoundToInt(b.x) + "_" + Mathf.RoundToInt(b.z);
            string key = string.CompareOrdinal(first, second) < 0 ? first + "-" + second : second + "-" + first;

            analyticsLinksAttempted++;
            if (roadLinks.Contains(key))
            {
                analyticsLinksRejectedDuplicate++;
                return false;
            }

            if (preventRoadCrossing)
            {
                Vector2 start = new Vector2(a.x, a.z);
                Vector2 end = new Vector2(b.x, b.z);
                if (WouldCrossExistingRoad(start, end))
                {
                    analyticsLinksRejectedCrossing++;
                    return false;
                }

                if (IsSegmentTooCloseToExisting(start, end, Mathf.Max(10f, roadSnapDistance * 0.9f)))
                {
                    analyticsLinksRejectedTooClose++;
                    return false;
                }

                reservedRoadSegments.Add(new RoadSegment2D(start, end));
            }

            roadLinks.Add(key);
            analyticsLinksAccepted++;
            return true;
        }

        private bool WouldCrossExistingRoad(Vector2 start, Vector2 end)
        {
            if (reservedRoadSegments.Count == 0)
                return false;

            float endpointTolerance = Mathf.Max(intersectionSafetyDistance, roadSnapDistance * 1.8f);
            for (int i = 0; i < reservedRoadSegments.Count; i++)
            {
                RoadSegment2D segment = reservedRoadSegments[i];
                bool sharesEndpoint = Vector2.Distance(start, segment.a) <= endpointTolerance
                                      || Vector2.Distance(start, segment.b) <= endpointTolerance
                                      || Vector2.Distance(end, segment.a) <= endpointTolerance
                                      || Vector2.Distance(end, segment.b) <= endpointTolerance;

                if (sharesEndpoint)
                    continue;

                if (SegmentsIntersect2D(start, end, segment.a, segment.b))
                    return true;
            }

            return false;
        }

        private bool IsSegmentTooCloseToExisting(Vector2 start, Vector2 end, float minDistance)
        {
            float sqMin = minDistance * minDistance;
            for (int i = 0; i < reservedRoadSegments.Count; i++)
            {
                RoadSegment2D segment = reservedRoadSegments[i];
                if (DistancePointToSegmentSquared(start, segment.a, segment.b) < sqMin) return true;
                if (DistancePointToSegmentSquared(end, segment.a, segment.b) < sqMin) return true;
                if (DistancePointToSegmentSquared(segment.a, start, end) < sqMin) return true;
                if (DistancePointToSegmentSquared(segment.b, start, end) < sqMin) return true;
            }

            return false;
        }

        private static float DistancePointToSegmentSquared(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float denom = Mathf.Max(0.0001f, Vector2.Dot(ab, ab));
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / denom);
            Vector2 projection = a + ab * t;
            return (point - projection).sqrMagnitude;
        }

        public string GetLastGenerationReport()
        {
            return "[FCG] Roads report => Links attempted: " + analyticsLinksAttempted
                   + ", accepted: " + analyticsLinksAccepted
                   + ", rejected duplicate: " + analyticsLinksRejectedDuplicate
                   + ", rejected crossing: " + analyticsLinksRejectedCrossing
                   + ", rejected near-parallel: " + analyticsLinksRejectedTooClose
                   + ", resolve retries succeeded: " + analyticsPlacementResolveSuccess
                   + ", reinforced links: " + analyticsReinforcedLinks
                   + ", spawned prefabs: " + placedRoadPrefabsInCurrentNetwork;
        }

        private static float Orientation2D(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        private static bool SegmentsIntersect2D(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float o1 = Orientation2D(a, b, c);
            float o2 = Orientation2D(a, b, d);
            float o3 = Orientation2D(c, d, a);
            float o4 = Orientation2D(c, d, b);
            const float eps = 0.0001f;

            if ((o1 * o2 < 0f) && (o3 * o4 < 0f))
                return true;

            if (Mathf.Abs(o1) <= eps && IsPointOnSegment2D(c, a, b)) return true;
            if (Mathf.Abs(o2) <= eps && IsPointOnSegment2D(d, a, b)) return true;
            if (Mathf.Abs(o3) <= eps && IsPointOnSegment2D(a, c, d)) return true;
            if (Mathf.Abs(o4) <= eps && IsPointOnSegment2D(b, c, d)) return true;

            return false;
        }

        private static bool IsPointOnSegment2D(Vector2 p, Vector2 a, Vector2 b)
        {
            return p.x <= Mathf.Max(a.x, b.x) + 0.001f && p.x >= Mathf.Min(a.x, b.x) - 0.001f
                   && p.y <= Mathf.Max(a.y, b.y) + 0.001f && p.y >= Mathf.Min(a.y, b.y) - 0.001f;
        }

        private Vector3 SnapRoadToGround(Vector3 position)
        {
            RaycastHit hit;
            Vector3 top = position + Vector3.up * 600f;
            if (Physics.Raycast(top, Vector3.down, out hit, 1500f))
            {
                position.y = hit.point.y;
                return position;
            }

            return position;
        }

        private Vector3 SnapRoadToExistingRoad(Vector3 position, float maxDistance, float blend)
        {
            if (!enforceRoadContinuity || roadNodeCache.Count == 0)
                return position;

            float bestDistance = maxDistance;
            Vector3 best = position;

            for (int i = 0; i < roadNodeCache.Count; i++)
            {
                Vector3 node = roadNodeCache[i];
                float d = Vector2.Distance(new Vector2(node.x, node.z), new Vector2(position.x, position.z));
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = node;
                }
            }

            if (bestDistance >= maxDistance)
                return position;

            Vector3 snapped = Vector3.Lerp(position, new Vector3(best.x, position.y, best.z), Mathf.Clamp01(blend));
            return snapped;
        }

        private Vector3 LimitRoadSlope(Vector3 position, float desiredHeight)
        {
            float maxSlopeTan = Mathf.Tan(maxRoadSlopeDegrees * Mathf.Deg2Rad);
            float maxHeightDelta = Mathf.Max(2f, roadSegmentSpacing * maxSlopeTan);
            position.y = Mathf.Clamp(position.y, desiredHeight - maxHeightDelta, desiredHeight + maxHeightDelta);
            return position;
        }


        private void RegisterRoadNode(Vector3 position)
        {
            if (roadNodeCache.Count >= 250000)
                return;

            float minDistance = Mathf.Max(2.5f, roadSnapDistance * 0.25f);
            for (int i = roadNodeCache.Count - 1; i >= 0 && i >= roadNodeCache.Count - 64; i--)
            {
                if (Vector2.Distance(new Vector2(roadNodeCache[i].x, roadNodeCache[i].z), new Vector2(position.x, position.z)) < minDistance)
                    return;
            }

            roadNodeCache.Add(position);
        }

        private bool TryResolveRoadPlacement(Vector3 basePosition, Vector3 direction, float clearanceRadius, out Vector3 resolved)
        {
            resolved = basePosition;

            if (!RoadPointHasCollision(basePosition, clearanceRadius) && !IsRoadNearBridge(basePosition))
                return true;

            Vector3 side = Vector3.Cross(Vector3.up, direction).normalized;
            float sideStep = Mathf.Max(2f, roadSnapDistance * 0.45f);

            for (int i = 1; i <= 2; i++)
            {
                float offset = sideStep * i;
                Vector3 candidateA = basePosition + side * offset;
                Vector3 candidateB = basePosition - side * offset;

                candidateA = SnapRoadToGround(candidateA);
                candidateB = SnapRoadToGround(candidateB);

                if (!RoadPointHasCollision(candidateA, clearanceRadius) && !IsRoadNearBridge(candidateA))
                {
                    resolved = candidateA;
                    analyticsPlacementResolveSuccess++;
                    return true;
                }

                if (!RoadPointHasCollision(candidateB, clearanceRadius) && !IsRoadNearBridge(candidateB))
                {
                    resolved = candidateB;
                    analyticsPlacementResolveSuccess++;
                    return true;
                }
            }

            return false;
        }

        private float SampleGroundHeight(Vector3 position)
        {
            return SnapRoadToGround(position).y;
        }

        private bool RoadPointHasCollision(Vector3 position, float clearanceRadius)
        {
            int overlapCount = Physics.OverlapSphereNonAlloc(position + Vector3.up * 2f, Mathf.Max(1f, clearanceRadius), roadOverlapBuffer);
            for (int i = 0; i < overlapCount; i++)
            {
                Collider current = roadOverlapBuffer[i];
                if (!current)
                    continue;

                string lowerName = current.name.ToLowerInvariant();
                if (lowerName.Contains("road") || lowerName.Contains("street") || lowerName.Contains("waypoint"))
                    return true;

                if (current.gameObject != cityMaker && (cityMaker == null || !current.transform.IsChildOf(cityMaker.transform)))
                    return true;
            }

            if (cityMaker)
            {
                int childCount = cityMaker.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = cityMaker.transform.GetChild(i);
                    if (!child)
                        continue;

                    if (Vector3.Distance(child.position, position) < Mathf.Max(4f, roadSnapDistance * 0.85f))
                    {
                        string lowerName = child.name.ToLowerInvariant();
                        if (!lowerName.Contains("road") && !lowerName.Contains("street") && !lowerName.Contains("exit"))
                            return true;
                    }
                }
            }

            return false;
        }



        private bool IsRoadNearBridge(Vector3 position)
        {
            int overlapCount = Physics.OverlapSphereNonAlloc(position + Vector3.up * 4f, Mathf.Max(12f, roadClearanceRadius * 1.5f), roadOverlapBuffer);
            for (int i = 0; i < overlapCount; i++)
            {
                Collider current = roadOverlapBuffer[i];
                if (!current)
                    continue;

                string lowerName = current.name.ToLowerInvariant();
                if (lowerName.Contains("bridge") || lowerName.Contains("overpass"))
                    return true;
            }

            return false;
        }

        private void GenerateSatteliteCityFromExit(Transform exitPositipon)
        {
            int i = (int)Random.Range(1, 10f);

            GameObject block;

            switch (i)
            {
                case 8:

                    GenerateStreetsVerySmall(false, false, true, 0, -1516);
                    block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position + exitPositipon.forward * 400, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position + exitPositipon.forward * 800, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 7:

                    GenerateStreetsVerySmall(false, false, true, -300, -1516);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 400 + exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 800 + exitPositipon.right * 200, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 6:

                    GenerateStreetsVerySmall(false, false, true, 200, -1516);
                    block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 400, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 800 - exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 5:

                    GenerateStreetsVerySmall(false, false, true, -100, -1516);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 400 + exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 800 + exitPositipon.right * 200, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 4:

                    GenerateStreetsVerySmall(false, false, true, 700, -1316);
                    block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position + exitPositipon.forward * 300 - exitPositipon.right * 300, Quaternion.Euler(0, 270, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 - exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 3:

                    GenerateStreetsVerySmall(false, false, true, 500, -1316);
                    block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position + exitPositipon.forward * 300 - exitPositipon.right * 300, Quaternion.Euler(0, 270, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 - exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                case 2:

                    GenerateStreetsVerySmall(false, false, true, -700, -1316);
                    block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position + exitPositipon.forward * 300 + exitPositipon.right * 300, Quaternion.Euler(0, 90, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 + exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;

                default:

                    GenerateStreetsVerySmall(false, false, true, -500, -1316);
                    block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position + exitPositipon.forward * 300 + exitPositipon.right * 300, Quaternion.Euler(0, 90, 0), cityMaker.transform);
                    block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 600 + exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                    break;
            }
        }
        private Transform CityExitPosition()
        {

            if (GameObject.Find("ExitCity"))
                return GameObject.Find("ExitCity").transform;
            else
                return null;

        }

        private GameObject InstantiatePrefab (GameObject gameObject, Vector3 pos, Quaternion rot, Transform parent)
        {

            GameObject obj;

#if UNITY_EDITOR
            obj = PrefabUtility.InstantiatePrefab(gameObject, parent) as GameObject;
#else
            obj = Instantiate(gameObject, parent) as GameObject;
#endif
            
            obj.transform.position = pos;
            obj.transform.rotation = rot;

            return obj;

        }

        private bool GenerateStreetsVerySmall(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false, float satteliteCityPositionX = 0, float satteliteCityPositionZ = 0)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();
                cityMaker = new GameObject("City-Maker");
            }

            GameObject block;

            if (!satteliteCity)
                distCenter = 150;

            int nb = 0;

            int le = largeBlocks.Length;
            nb = Random.Range(0, le);

            if (satteliteCity && smallBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(largeBlocks[nb], CityExitPosition().position + new Vector3(satteliteCityPositionX, 0, satteliteCityPositionZ) - new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
                block = (GameObject)Instantiate(largeBlocks[nb], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);


            if ((withSatteliteCity || satteliteCity) && miniBorderWithExitOfCity.Length > 0)
            {
                if (satteliteCity)
                    block = (GameObject)Instantiate(miniBorderWithExitOfCity[Random.Range(0, miniBorderWithExitOfCity.Length)], CityExitPosition().position + new Vector3(satteliteCityPositionX, 0, satteliteCityPositionZ), Quaternion.Euler(0, 180, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(miniBorderWithExitOfCity[Random.Range(0, miniBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(miniBorderFlat[Random.Range(0, miniBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(miniBorder[Random.Range(0, miniBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && miniBorderWithExitOfCity.Length > 0);

        }

        private bool GenerateStreetsSmall(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();

                cityMaker = new GameObject("City-Maker");

            }

            if (!satteliteCity)
                distCenter = 200;

            int nb = 0;

            int le = largeBlocks.Length;
            _largeBlocks = new bool[largeBlocks.Length];

            //Position and Rotation
            Vector3[] ps = new Vector3[3];

            int[] rt = new int[3];

            float s = Random.Range(0, 6f);

            if (s < 3)
            {
                ps[1] = new Vector3(0, 0, 0); rt[1] = 0;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0;
            }
            else
            {
                ps[1] = new Vector3(-150, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(150, 0, 150); rt[2] = 90;
            }


            for (int qt = 1; qt < 3; qt++)
            {

                for (int lp = 0; lp < 100; lp++)
                {
                    nb = Random.Range(0, le);
                    if (!_largeBlocks[nb]) break;
                }
                _largeBlocks[nb] = true;

                if (satteliteCity && smallBorderWithExitOfCity.Length > 0)
                    Instantiate(largeBlocks[nb], ps[qt] + CityExitPosition().position + new Vector3(-0, 0, -1516) - new Vector3(0, 0, 300), Quaternion.Euler(0, rt[qt] + 180, 0), cityMaker.transform);
                else
                    Instantiate(largeBlocks[nb], ps[qt], Quaternion.Euler(0, rt[qt], 0), cityMaker.transform);

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && smallBorderWithExitOfCity.Length > 0)
            {
                if (satteliteCity)
                    block = (GameObject)Instantiate(smallBorderWithExitOfCity[Random.Range(0, smallBorderWithExitOfCity.Length)], CityExitPosition().position + new Vector3(-0, 0, -1516), Quaternion.Euler(0, 180, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(smallBorderWithExitOfCity[Random.Range(0, smallBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);

            }
            else
            {
                if (borderFlat)
                    block = (GameObject)Instantiate(smallBorderFlat[Random.Range(0, smallBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(smallBorder[Random.Range(0, smallBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && smallBorderWithExitOfCity.Length > 0);

        }



        private bool GenerateStreets(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {

                ClearCity();

                cityMaker = new GameObject("City-Maker");
            }

            if (!satteliteCity)
                distCenter = 300;

            int nb = 0;

            int le = largeBlocks.Length;
            _largeBlocks = new bool[largeBlocks.Length];

            //Position and Rotation
            Vector3[] ps = new Vector3[5];

            int[] rt = new int[5];

            float s = Random.Range(0, 6f);

            if (s < 2)
            {

                ps[1] = new Vector3(0, 0, 0); rt[1] = 0;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0;
                ps[3] = new Vector3(450, 0, 150); rt[3] = 90;
                ps[4] = new Vector3(-450, 0, 150); rt[4] = 90;

            }
            else if (s < 3)
            {

                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90;

            }
            else if (s < 4)
            {

                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(300, 0, 0); rt[3] = 0;
                ps[4] = new Vector3(300, 0, 300); rt[4] = 0;

            }
            else
            {

                ps[1] = new Vector3(450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(-300, 0, 0); rt[3] = 0;
                ps[4] = new Vector3(-300, 0, 300); rt[4] = 0;

            }


            for (int qt = 1; qt < 5; qt++)
            {

                for (int lp = 0; lp < 100; lp++)
                {
                    nb = Random.Range(0, le);
                    if (!_largeBlocks[nb]) break;
                }
                _largeBlocks[nb] = true;

                Instantiate(largeBlocks[nb], ps[qt], Quaternion.Euler(0, rt[qt], 0), cityMaker.transform);

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && mediumBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(mediumBorderWithExitOfCity[Random.Range(0, mediumBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(mediumBorderFlat[Random.Range(0, mediumBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(mediumBorder[Random.Range(0, mediumBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }
                

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && mediumBorderWithExitOfCity.Length > 0);

        }


        private bool GenerateStreetsBig(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();
                cityMaker = new GameObject("City-Maker");
            }

            distCenter = 350;

            int nb = 0;

            int le = largeBlocks.Length;
            int lebig = bigLargeBlocks.Length;

            _largeBlocks = new bool[largeBlocks.Length];
            _bigLargeBlocks = new bool[bigLargeBlocks.Length];

            //Position 
            Vector3[] ps = new Vector3[7];

            //Rotation
            int[] rt = new int[7];

            //Type
            int[] tb = new int[7];   // 1->Large - 2->BigLarge

            int qt;

            float s = Random.Range(0, 7f);


            if (s < 3)
            {
                qt = 6;

                ps[1] = new Vector3(0, 0, 0); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(450, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(-450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;


            }
            else if (s < 3)
            {
                qt = 6;
                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90; tb[1] = 1;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90; tb[2] = 1;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;

            }
            else if (s < 4)
            {
                qt = 6;
                ps[1] = new Vector3(-300, 0, 300); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(-300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;


            }
            else if (s < 5)
            {
                qt = 5;
                ps[1] = new Vector3(-300, 0, 0); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(-300, 0, 600); rt[3] = 0; tb[3] = 1;
                ps[4] = new Vector3(300, 0, 600); rt[4] = 0; tb[4] = 1;
                ps[5] = new Vector3(0, 0, 300); rt[5] = 0; tb[5] = 2;



            }
            else
            {
                qt = 6;
                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90; tb[1] = 1;
                ps[2] = new Vector3(300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(-150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 450); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(150, 0, 450); rt[6] = 90; tb[6] = 1;

            }


            for (int count = 1; count <= qt; count++)
            {

                if (tb[count] == 1)
                {
                    for (int lp = 0; lp < 100; lp++)
                    {
                        nb = Random.Range(0, le);
                        if (!_largeBlocks[nb]) break;
                    }
                    _largeBlocks[nb] = true;

                    Instantiate(largeBlocks[nb], ps[count], Quaternion.Euler(0, rt[count], 0), cityMaker.transform);
                }
                else if (tb[count] == 2)
                {
                    for (int lp = 0; lp < 100; lp++)
                    {
                        nb = Random.Range(0, lebig);
                        if (!_bigLargeBlocks[nb]) break;
                    }
                    _bigLargeBlocks[nb] = true;

                    Instantiate(bigLargeBlocks[nb], ps[count], Quaternion.Euler(0, rt[count], 0), cityMaker.transform);
                }

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && largeBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(largeBorderWithExitOfCity[Random.Range(0, largeBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(largeBorderFlat[Random.Range(0, largeBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(largeBorder[Random.Range(0, largeBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);

            }
                

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && largeBorderWithExitOfCity.Length > 0);

        }

        private bool GenerateStreetsVeryLarge(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {
            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();
                cityMaker = new GameObject("City-Maker");
            }

            distCenter = 500;

            int largeLength = largeBlocks != null ? largeBlocks.Length : 0;
            int bigLength = bigLargeBlocks != null ? bigLargeBlocks.Length : 0;
            if (largeLength == 0 && bigLength == 0)
                return false;

            _largeBlocks = new bool[Mathf.Max(1, largeLength)];
            _bigLargeBlocks = new bool[Mathf.Max(1, bigLength)];

            Vector3[] placements = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 300),
                new Vector3(0, 0, 600),
                new Vector3(-300, 0, 0),
                new Vector3(300, 0, 0),
                new Vector3(-300, 0, 300),
                new Vector3(300, 0, 300),
                new Vector3(-300, 0, 600),
                new Vector3(300, 0, 600),
                new Vector3(-600, 0, 300),
                new Vector3(600, 0, 300),
                new Vector3(-450, 0, 150),
                new Vector3(450, 0, 150),
                new Vector3(-450, 0, 450),
                new Vector3(450, 0, 450)
            };

            for (int i = 0; i < placements.Length; i++)
            {
                bool placeBigBlock = (i % 4 == 0 || i % 7 == 0) && bigLength > 0;
                int rot = Random.Range(0, 4) * 90;

                if (placeBigBlock)
                {
                    int idx = Random.Range(0, bigLength);
                    for (int attempts = 0; attempts < 80; attempts++)
                    {
                        if (!_bigLargeBlocks[idx])
                            break;
                        idx = Random.Range(0, bigLength);
                    }
                    _bigLargeBlocks[idx] = true;
                    Instantiate(bigLargeBlocks[idx], placements[i], Quaternion.Euler(0, rot, 0), cityMaker.transform);
                }
                else if (largeLength > 0)
                {
                    int idx = Random.Range(0, largeLength);
                    for (int attempts = 0; attempts < 80; attempts++)
                    {
                        if (!_largeBlocks[idx])
                            break;
                        idx = Random.Range(0, largeLength);
                    }
                    _largeBlocks[idx] = true;
                    Instantiate(largeBlocks[idx], placements[i], Quaternion.Euler(0, rot, 0), cityMaker.transform);
                }
            }

            GameObject border;
            if ((withSatteliteCity || satteliteCity) && largeBorderWithExitOfCity.Length > 0)
            {
                border = Instantiate(largeBorderWithExitOfCity[Random.Range(0, largeBorderWithExitOfCity.Length)], Vector3.zero, Quaternion.identity, cityMaker.transform);
            }
            else
            {
                GameObject[] borderSet = borderFlat ? largeBorderFlat : largeBorder;
                if (borderSet == null || borderSet.Length == 0)
                    return false;

                border = Instantiate(borderSet[Random.Range(0, borderSet.Length)], Vector3.zero, Quaternion.identity, cityMaker.transform);
            }

            PopulateSmartRoadGridForVeryLarge();

            border.transform.SetParent(cityMaker.transform);
            return (withSatteliteCity && largeBorderWithExitOfCity.Length > 0);
        }

        private void PopulateSmartRoadGridForVeryLarge()
        {
            if (cityMaker == null)
                return;

            Vector3[] roadNodes = new Vector3[]
            {
                new Vector3(0, 0, 150),
                new Vector3(0, 0, 450),
                new Vector3(-300, 0, 150),
                new Vector3(300, 0, 150),
                new Vector3(-300, 0, 450),
                new Vector3(300, 0, 450),
                new Vector3(-600, 0, 300),
                new Vector3(600, 0, 300),
                new Vector3(-150, 0, 300),
                new Vector3(150, 0, 300)
            };

            for (int i = 0; i < roadNodes.Length; i++)
            {
                Vector3 roadPos = SnapRoadToGround(roadNodes[i]);
                float distance = Vector3.Distance(Vector3.zero, roadPos);
                float influence = Mathf.Clamp01(1f - (distance / Mathf.Max(500f, cityInfluenceRadius)));
                float intercityFactor = Mathf.Clamp01(distance / Mathf.Max(300f, intercityDistanceThreshold));
                GameObject[] roadPrefabs = SelectRoadPrefabsForConnection(0.9f, distance, influence, intercityFactor, 0.5f);
                if (roadPrefabs == null || roadPrefabs.Length == 0)
                    continue;

                Quaternion rotation = (Mathf.Abs(roadPos.x) > Mathf.Abs(roadPos.z)) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
                Instantiate(roadPrefabs[Random.Range(0, roadPrefabs.Length)], roadPos, rotation, cityMaker.transform);
            }
        }

        private GameObject pB;

        public void GenerateAllBuildings(bool _withDowntownArea, float _downTownSize)
        {

            downTownSize = _downTownSize;

            withDowntownArea = _withDowntownArea;

            if (withDowntownArea)
            {
                GameObject[] tArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Marcador")).ToArray();

                if (tArray.Length == 1)
                    center = tArray[0].transform.position;
                else
                    center = tArray[Random.Range(1, tArray.Length - 1)].transform.position;

                if (GameObject.Find("DownTownPosition") && Random.Range(1, 10) < 5)
                    center = GameObject.Find("DownTownPosition").transform.position;


            }


            _BB = new int[BB.Length];
            _BC = new int[BC.Length];
            _BR = new int[BR.Length];
            //_DC = new int[DC.Length];
            _EB = new int[EB.Length];
            _EC = new int[EC.Length];
            _MB = new int[MB.Length];
            _BK = new int[BK.Length];
            _SB = new int[SB.Length];

            _EBS = new int[EB.Length];
            _ECS = new int[EC.Length];


            _BBS = new int[BBS.Length];
            _BCS = new int[BCS.Length];

            residential = 0;

            DestroyBuildings();

            GameObject pB = new GameObject();

            nB = 0;

            CreateBuildingsInSuperBlocks();
            CreateBuildingsInBlocks();
            CreateBuildingsInLines();
            CreateBuildingsInDouble();



            //Debug.ClearDeveloperConsole();
            Debug.Log(nB + " buildings were created");


            DestroyImmediate(pB);

            DayNight dayNight = FindObjectOfType<DayNight>();
            if(dayNight)
            {
                dayNight.ChangeMaterial();
            }



        }



        public void CreateBuildingsInLines()
        {


            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Marcador")).ToArray();

            foreach (GameObject lines in tempArray)
            {

                _residential = (residential < 15 && Vector3.Distance(center, lines.transform.position) > 400 && Random.Range(0, 100) < 30);

                foreach (Transform child in lines.transform)
                {

                    if (child.name == "E")
                        CreateBuildingsInCorners(child.gameObject);
                    else if (child.name == "EL")
                    {
                        int ct = 0;
                        do
                        {
                            ct++;
                            if (CreateBuildingsInCorners(child.gameObject, true))
                                break;

                        } while (ct < 300);

                    }
                    else if (child.name.Substring(0, 1) == "S")
                        CreateBuildingsInLine(child.gameObject, 90f, true);
                    else
                        CreateBuildingsInLine(child.gameObject, 90f);

                }

                _residential = false;


            }

        }

        public bool CreateBuildingsInCorners(GameObject child, bool notAnyone = false)
        {

            GameObject pBuilding;

            pB = null;
            int numB = 0;
            int t = 0;
            float pWidth = 0;
            float wComprimento;

            float pScale;
            float remainingMeters;
            GameObject newMarcador;

            float distancia = Vector3.Distance(center, child.transform.position);

            int lp;
            lp = 0;
            int lt = 0;

            float _distCenter = distCenter * (Mathf.Clamp(downTownSize, 50, 200) / 100);

            while (t < 100)
            {

                t++;

                if (distancia < _distCenter && withDowntownArea)
                {

                    do
                    {
                        lp++;
                        lt = 0;
                        do
                        {
                            lt++;
                            numB = Random.Range(0, EC.Length);
                        } while (notAnyone && _ECS[numB] > 0 && lt < 2000);

                        if (_EC[numB] == 0) break;
                        if (lp > 100 && _EC[numB] <= 1) break;
                        if (lp > 150 && _EC[numB] <= 2) break;
                        if (lp > 200 && _EC[numB] <= 3) break;
                        if (lp > 250) break;

                    } while (lp < 300);

                    pWidth = GetWith(EC[numB]);

                    if (pWidth <= 0)
                    {
                        Debug.LogWarning("Error: EC: " + numB);
                        _EC[numB] = 100;
                        return false;
                    }
                    else if (pWidth <= 36.3f)
                    {
                        _EC[numB] += 1;
                        pB = EC[numB];
                        break;
                    }

                }
                else
                {

                    do
                    {
                        lp++;
                        do
                        {
                            lt++;
                            numB = Random.Range(0, EB.Length);
                        } while (notAnyone && _EBS[numB] >= 100 && lt < 2000);

                        if (_EB[numB] == 0) break;
                        if (lp > 100 && _EB[numB] <= 1) break;
                        if (lp > 150 && _EB[numB] <= 2) break;
                        if (lp > 200 && _EB[numB] <= 3) break;
                        if (lp > 250) break;

                    } while (lp < 300);


                    pWidth = GetWith(EB[numB]);

                    if (pWidth <= 0)
                    {
                        Debug.LogWarning("Error: EB: " + numB);
                        _EB[numB] = 100;
                        return false;
                    }
                    else if (pWidth <= 36.3f)
                    {
                        _EB[numB] += 1;
                        pB = EB[numB];
                        break;
                    }

                }



            }



            pBuilding = (GameObject)Instantiate(pB, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            

            if (notAnyone && !TestBaseBuildindCornerOnTheSlope(pBuilding.transform))
            {

                if (distancia < _distCenter && withDowntownArea)
                {
                    _ECS[numB] = 100;
                    _EC[numB] -= 1;
                }
                else
                {
                    _EBS[numB] = 100;
                    _EB[numB] -= 1;
                }


                DestroyImmediate(pBuilding);

                return false;
            }

            pBuilding.name = pBuilding.name;
            pBuilding.transform.SetParent(child.transform);
            pBuilding.transform.localPosition = new Vector3(-(pWidth * 0.5f), 0, 0);
            pBuilding.transform.localRotation = Quaternion.Euler(0, 0, 0);

            nB++;

            // Check space behind the corner building -------------------------------------------------------------------------------------------------------------------
            wComprimento = GetHeight(pB);
            if (wComprimento < 29.9f)
            {

                newMarcador = new GameObject("Marcador");

                newMarcador.transform.SetParent(child.transform);
                newMarcador.transform.localPosition = new Vector3(0, 0, -36);
                newMarcador.transform.localRotation = Quaternion.Euler(0, 0, 0);
                newMarcador.name = (36 - wComprimento).ToString();
                CreateBuildingsInLine(newMarcador, 90);

            }
            else
            {
                remainingMeters = 36 - wComprimento;
                pScale = 1 + (remainingMeters / wComprimento);
                pBuilding.transform.localScale = new Vector3(1, 1, pScale);

            }


            // Check space on the corner building -------------------------------------------------------------------------------------------------------------------


            if (pWidth < 29.9f)
            {

                newMarcador = new GameObject("Marcador");



                newMarcador.transform.SetParent(child.transform);
                newMarcador.transform.localPosition = new Vector3(-pWidth, 0, 0);
                newMarcador.transform.localRotation = Quaternion.Euler(0, 270, 0);
                newMarcador.name = (36 - pWidth).ToString();
                CreateBuildingsInLine(newMarcador, 90);

            }
            else
            {

                remainingMeters = 36 - pWidth;
                pScale = 1 + (remainingMeters / pWidth);
                pBuilding.transform.localScale = new Vector3(pScale, 1, 1);

            }

            return true;

        }

        bool TestBaseBuildindCornerOnTheSlope(Transform buildingCornerOnTheSlope)
        {
            return (buildingCornerOnTheSlope.Find("Base-Corner-0-Collider") || buildingCornerOnTheSlope.Find("Base-Corner-03-Collider") || buildingCornerOnTheSlope.Find("Base-Corner-06-Collider"));
        }

        int RandRotation()
        {
            int r = 0;
            int i = Random.Range(0, 4);
            if (i == 3) r = 180;
            else if (i == 2) r = 90;
            else if (i == 1) r = 270;
            else r = 0;

            return r;


        }


        public void CreateBuildingsInBlocks()
        {

            int numB = 0;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Blocks")).ToArray();

            foreach (GameObject bks in tempArray)
            {

                foreach (Transform bk in bks.transform)
                {

                    if (Random.Range(0, 20) > 5)
                    {

                        int lp = 0;
                        do
                        {
                            lp++;
                            numB = Random.Range(0, BK.Length);
                            if (_BK[numB] == 0) break;
                            if (lp > 125 && _BK[numB] <= 1) break;
                            if (lp > 150 && _BK[numB] <= 2) break;
                            if (lp > 200 && _BK[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        _BK[numB] += 1;


                        Instantiate(BK[numB], bk.position, bk.rotation, bk);
                        
                        nB++;

                    }
                    else
                    {

                        for (int i = 1; i <= 4; i++)
                        {
                            GameObject nc = new GameObject("E");
                            nc.transform.SetParent(bk);
                            if (i == 1)
                            {
                                nc.transform.localPosition = new Vector3(-36, 0, -36);
                                nc.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            }
                            if (i == 2)
                            {
                                nc.transform.localPosition = new Vector3(-36, 0, 36);
                                nc.transform.localRotation = Quaternion.Euler(0, 270, 0);
                            }
                            if (i == 3)
                            {
                                nc.transform.localPosition = new Vector3(36, 0, 36);
                                nc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            if (i == 4)
                            {
                                nc.transform.localPosition = new Vector3(36, 0, -36);
                                nc.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            }
                            CreateBuildingsInCorners(nc);

                        }
                    }


                }

            }

        }

        public void CreateBuildingsInSuperBlocks()
        {

            int numB = 0;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("SuperBlocks")).ToArray();

            foreach (GameObject bks in tempArray)
            {

                foreach (Transform bk in bks.transform)
                {


                    int lp = 0;
                    do
                    {
                        lp++;
                        numB = Random.Range(0, SB.Length);
                        if (_SB[numB] == 0) break;
                        if (lp > 125 && _SB[numB] <= 1) break;
                        if (lp > 150 && _SB[numB] <= 2) break;
                        if (lp > 200 && _SB[numB] <= 3) break;
                        if (lp > 250) break;
                    } while (lp < 300);

                    _SB[numB] += 1;

                    Instantiate(SB[numB], bk.position, bk.rotation, bk);
                    
                    nB++;



                }

            }

        }

        private void CreateBuildingsInLine(GameObject line, float angulo, bool slope = false)
        {


            int index = -1;
            GameObject[] pBuilding;
            pBuilding = new GameObject[50];

            float limit;
            string _name = line.name;

            _name = (slope) ? line.name.Substring(1) : line.name;

            if (_name.Contains("."))
                limit = float.Parse(_name.Split('.')[0]) + float.Parse(_name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, _name.Split('.')[1].Length));
            else
                limit = float.Parse(_name);

            float init = 0;
            float pWidth = 0;

            int tt = 0;
            int t;

            int lp;


            float distancia = Vector3.Distance(center, line.transform.position);

            float _distCenter = distCenter * (Mathf.Clamp(downTownSize, 50, 200) / 100);

            while (tt < 100)
            {

                tt++;
                t = 0;


                lp = 0;
                while (t < 200 && init <= limit - 4)
                {

                    t++;

                    if (slope)
                    {
                        if (distancia < _distCenter && withDowntownArea)
                        {
                            do
                            {
                                lp++;
                                numB = Random.Range(0, BCS.Length);
                                if (_BCS[numB] == 0) break;
                                if (lp > 125 && _BCS[numB] <= 1) break;
                                if (lp > 150 && _BCS[numB] <= 2) break;
                                if (lp > 200 && _BCS[numB] <= 3) break;
                                if (lp > 250) break;

                            } while (lp < 300);

                            pWidth = GetWith(BCS[numB]);

                            if (pWidth > 0)
                                if ((init + pWidth) <= (limit + 4))
                                {
                                    pB = BCS[numB];
                                    _BCS[numB] += 1;
                                    break;
                                }
                        }
                        else
                        {

                            do
                            {
                                lp++;
                                numB = Random.Range(0, BBS.Length);
                                if (_BBS[numB] == 0) break;
                                if (lp > 125 && _BBS[numB] <= 1) break;
                                if (lp > 150 && _BBS[numB] <= 2) break;
                                if (lp > 200 && _BBS[numB] <= 3) break;
                                if (lp > 250) break;

                            } while (lp < 300);

                            pWidth = GetWith(BBS[numB]);

                            if (pWidth > 0)
                                if ((init + pWidth) <= (limit + 4))
                                {
                                    pB = BBS[numB];
                                    _BBS[numB] += 1;
                                    break;
                                }

                        }

                    }
                    else if (distancia < _distCenter && withDowntownArea)
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BC.Length);
                            if (_BC[numB] == 0) break;
                            if (lp > 125 && _BC[numB] <= 1) break;
                            if (lp > 150 && _BC[numB] <= 2) break;
                            if (lp > 200 && _BC[numB] <= 3) break;
                            if (lp > 250) break;

                        } while (lp < 300);

                        pWidth = GetWith(BC[numB]);

                        if (pWidth > 0)
                            if ((init + pWidth) <= (limit + 4))
                            {
                                pB = BC[numB];
                                _BC[numB] += 1;
                                break;
                            }

                    }
                    else if (_residential)
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BR.Length);
                            if (_BR[numB] == 0) break;
                            if (lp > 100 && _BR[numB] <= 1) break;
                            if (lp > 150 && _BR[numB] <= 2) break;
                            if (lp > 200 && _BR[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        pWidth = GetWith(BR[numB]);

                        if (pWidth <= 0) { Debug.LogWarning("Error: BR: " + numB); _BR[numB] += 1; }
                        else
                        if ((init + pWidth) <= (limit + 4))
                        {
                            pB = BR[numB];
                            _BR[numB] += 1;
                            residential += 1;
                            break;
                        }
                    }
                    else
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BB.Length);
                            if (_BB[numB] == 0) break;
                            if (lp > 100 && _BB[numB] <= 1) break;
                            if (lp > 150 && _BB[numB] <= 2) break;
                            if (lp > 200 && _BB[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        pWidth = GetWith(BB[numB]);

                        if (pWidth <= 0) { Debug.LogWarning("Error: BB: " + numB); _BB[numB] += 1; }
                        if ((init + pWidth) <= (limit + 4))
                        {
                            pB = BB[numB];
                            _BB[numB] += 1;
                            break;
                        }

                    }


                }


                if (t >= 200 || init > limit - 4)
                {
                    // Didn't find one that fits in the remaining space

                    AdjustsWidth(pBuilding, index + 1, limit - init, 0, slope);
                    break;

                }
                else
                {

                    index++;


                    nB++;

                    //pBuilding[index].name = pBuilding[index].name;

                    pBuilding[index] = (GameObject)Instantiate(pB, new Vector3(0, 0, init + (pWidth * 0.5f)), Quaternion.Euler(0, angulo, 0), line.transform);
                    

                    pBuilding[index].transform.SetParent(line.transform);
                    pBuilding[index].transform.localPosition = new Vector3(0, 0, init + (pWidth * 0.5f));
                    pBuilding[index].transform.localRotation = Quaternion.Euler(0, angulo, 0);

                    init += pWidth;

                    if (init > limit - 6)
                    {
                        AdjustsWidth(pBuilding, index + 1, limit - init, 0, slope);
                        break;
                    }

                }



            }



        }


        private float GetY(Transform pos, float width)
        {

            RaycastHit hit;

            Vector3 pp = pos.transform.position + pos.transform.forward * 2 + pos.transform.up * 20;

            float l = 20;
            float r = 20;

            if (Physics.Raycast(pp + pos.transform.right * width, Vector3.down, out hit, 40))
                r = hit.distance;

            if (Physics.Raycast(pp - (pos.transform.right * width), Vector3.down, out hit, 40))
                l = hit.distance;

            return (pos.transform.localPosition.y + 20) - ((r < l) ? r : l);

        }

        private void CreateBuildingsInDoubleLine(GameObject line)
        {

            int index = -1;
            GameObject[] pBuilding;
            pBuilding = new GameObject[20];

            float limit;
            string _name = line.name;

            if (_name.Contains("."))
                limit = float.Parse(_name.Split('.')[0]) + float.Parse(_name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, _name.Split('.')[1].Length));
            else
                limit = float.Parse(_name);


            float init = 0;
            float pWidth = 0;

            int tt = 0;
            int t;
            int lp;

            while (tt < 100)
            {

                tt++;
                t = 0;

                lp = 0;

                while (t < 200 && init <= limit - 4)
                {

                    t++;

                    do
                    {
                        lp++;
                        numB = Random.Range(0, MB.Length);
                        if (_MB[numB] == 0) break;
                        if (lp > 100 && _MB[numB] <= 1) break;
                        if (lp > 150 && _MB[numB] <= 2) break;
                        if (lp > 200) break;
                    } while (lp < 300);

                    pWidth = GetWith(MB[numB]);


                    if (pWidth <= 0) { Debug.LogWarning("Error: MB: " + numB); _MB[numB] += 1; }
                    else
                    if ((init + pWidth) <= (limit + 4))
                    {
                        _MB[numB] += 1;
                        break;
                    }

                }

                if (t >= 200 || init > limit - 4)
                {
                    AdjustsWidth(pBuilding, index + 1, (limit - init), 0);
                    break;

                }
                else
                {

                    index++;

                    pBuilding[index] = (GameObject)Instantiate(MB[numB], new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0), line.transform);
                    

                    nB++;

                    pBuilding[index].name = "building";
                    pBuilding[index].transform.SetParent(line.transform);
                    pBuilding[index].transform.localPosition = new Vector3(0, 0, (init + (pWidth * 0.5f)));
                    pBuilding[index].transform.localRotation = Quaternion.Euler(0, 90, 0);

                    init += pWidth;

                    if (init > limit - 6)
                    {
                        AdjustsWidth(pBuilding, index + 1, (limit - init), 0);
                    }

                }


            }

        }

        private void CreateBuildingsInDouble()
        {
            float limit;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Double")).ToArray();

            GameObject DB;
            GameObject mc2;
            GameObject mc;


            foreach (GameObject dbCross in tempArray)
            {

                foreach (Transform line in dbCross.transform)
                {

                    if (line.name.Contains("."))
                        limit = float.Parse(line.name.Split('.')[0]) + float.Parse(line.name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, line.name.Split('.')[1].Length));
                    else
                        limit = float.Parse(line.name);



                    if (Random.Range(0, 10) < 5)
                    {
                        //Bloks

                        float wl;
                        float wl2;

                        do
                        {
                            numB = Random.Range(0, DC.Length);
                            wl = GetHeight(DC[numB]);
                        } while (wl > limit / 2);

                        GameObject e = (GameObject)Instantiate(DC[numB], line.transform.position, line.transform.rotation, line.transform);
                        

                        nB++;

                        do
                        {
                            numB = Random.Range(0, DC.Length);
                            wl2 = GetHeight(DC[numB]);
                        } while (wl2 > limit - (wl + 26));

                        e = (GameObject)Instantiate(DC[numB], line.transform.position, line.rotation, line.transform);


                        e.transform.SetParent(line.transform);
                        e.transform.localPosition = new Vector3(0, 0, -limit);
                        e.transform.localRotation = Quaternion.Euler(0, 180, 0);

                        DB = new GameObject("" + ((limit - wl - wl2)));
                        DB.transform.SetParent(line.transform);
                        DB.transform.localPosition = new Vector3(0, 0, -(limit - wl2));
                        DB.transform.localRotation = Quaternion.Euler(0, 0, 0);

                        DB.name = "" + ((limit - wl - wl2));

                        CreateBuildingsInDoubleLine(DB);

                    }
                    else
                    {
                        //Lines and Corners

                        mc = new GameObject("Marcador");
                        mc.transform.SetParent(line);
                        mc.transform.localPosition = new Vector3(0, 0, 0);
                        mc.transform.localRotation = Quaternion.Euler(0, 0, 0);


                        for (int i = 1; i <= 4; i++)
                        {
                            mc2 = new GameObject("E");
                            mc2.transform.SetParent(mc.transform);

                            if (i == 1)
                            {
                                mc2.transform.localPosition = new Vector3(36, 0, -limit);
                                mc2.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            }
                            if (i == 2)
                            {
                                mc2.transform.localPosition = new Vector3(36, 0, 0);
                                mc2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            if (i == 3)
                            {
                                mc2.transform.localPosition = new Vector3(-36, 0, 0);
                                mc2.transform.localRotation = Quaternion.Euler(0, 270, 0);
                            }
                            if (i == 4)
                            {
                                mc2.transform.localPosition = new Vector3(-36, 0, -limit);
                                mc2.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            }

                            CreateBuildingsInCorners(mc2);

                        }

                        mc2 = new GameObject("" + (limit - 72));
                        mc2.transform.SetParent(mc.transform);
                        mc2.transform.localPosition = new Vector3(-36, 0.001f, -36);
                        mc2.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        CreateBuildingsInLine(mc2, 90f);

                        mc2 = new GameObject("" + (limit - 72));
                        mc2.transform.SetParent(mc.transform);
                        mc2.transform.localPosition = new Vector3(36, 0.001f, -(limit - 36));
                        mc2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        CreateBuildingsInLine(mc2, 90f);

                    }




                }



            }


        }


        private void AdjustsWidth(GameObject[] tBuildings, int quantity, float remainingMeters, float init, bool slope = false)
        {

            if (remainingMeters == 0)
                return;

            float ajuste = remainingMeters / quantity;

            float zInit = init;
            float pWidth;
            float pScale;
            float gw;


            for (int i = 0; i < quantity; i++)
            {

                gw = GetWith(tBuildings[i]);

                if (gw > 0)
                {
                    pScale = 1 + (ajuste / gw);
                    pWidth = gw + ajuste;

                    tBuildings[i].transform.localPosition = new Vector3(tBuildings[i].transform.localPosition.x, tBuildings[i].transform.localPosition.y, zInit + (pWidth * 0.5f));
                    tBuildings[i].transform.localScale = new Vector3(pScale, 1, 1);
                    zInit += pWidth;


                    if (slope)
                    {
                        float p;

                        p = GetY(tBuildings[i].transform, (gw * pScale) * 0.5f);
                        tBuildings[i].transform.position += new Vector3(0, p, 0);


                    }


                }

            }

        }


        private float GetWith(GameObject building)
        {

            if (!building)
                return 0;


            if (building.transform.GetComponent<MeshFilter>() != null)
            {

                if (building.transform.GetComponent<MeshFilter>().sharedMesh == null)
                {
                    Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                    //return 0;
                }


                return building.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            }
            else
            {
                Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                return 0;
            }
        }

        private float GetHeight(GameObject building)
        {

            if (building.GetComponent<MeshFilter>() != null)
                return building.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
            else
            {
                Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                return 0;
            }

        }


        public void DestroyBuildings()
        {

            DestryObjetcs("Marcador");
            DestryObjetcs("Blocks");
            DestryObjetcs("SuperBlocks");
            DestryObjetcs("Double");

        }


        private void DestryObjetcs(string tag)
        {
            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == (tag)).ToArray();

            foreach (GameObject objt in tempArray)
                foreach (Transform child in objt.transform)
                    for (int k = child.childCount - 1; k >= 0; k--)
                        DestroyImmediate(child.GetChild(k).gameObject);


        }



    }
}
