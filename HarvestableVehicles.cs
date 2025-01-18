/*
 * Copyright (C) 2024 Game4Freak.io
 * This mod is provided under the Game4Freak EULA.
 * Full legal terms can be found at https://game4freak.io/eula/
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("Harvestable Vehicles", "VisEntities", "1.6.0")]
    [Description("Lets players gather materials from vehicles.")]
    public class HarvestableVehicles : RustPlugin
    {
        #region Fields

        private static HarvestableVehicles _plugin;
        private static Configuration _config;
        private System.Random _random = new System.Random();

        #endregion Fields

        #region Configuration

        private class Configuration
        {
            [JsonProperty("Version")]
            public string Version { get; set; }

            [JsonProperty("Gathering Tools")]
            public List<GatheringToolConfig> GatheringTools { get; set; }

            [JsonProperty("Harvestable Vehicles")]
            public List<HarvestableVehicleConfig> HarvestableVehicles { get; set; }

            [JsonProperty("Rarity Weights")]
            public Dictionary<Rarity, int> RarityWeights { get; set; }
        }

        public class GatheringToolConfig
        {
            [JsonProperty("Item Short Name")]
            public string ItemShortName { get; set; }

            [JsonProperty("Skin Id")]
            public ulong SkinId { get; set; }
        }

        public class HarvestableVehicleConfig
        {
            [JsonProperty("Vehicle Short Prefab Names")]
            public List<string> VehicleShortPrefabNames { get; set; }

            [JsonProperty("Resources")]
            public List<ResourceConfig> Resources { get; set; }

            [JsonProperty("Damage Increase Factor")]
            public float DamageIncreaseFactor { get; set; }
        }

        public class ResourceConfig
        {
            [JsonProperty("Item Short Name")]
            public string ItemShortName { get; set; }

            [JsonProperty("Display Name")]
            public string DisplayName { get; set; }

            [JsonProperty("Skin Id")]
            public ulong SkinId { get; set; }

            [JsonProperty("Minimum Amount")]
            public int MinimumAmount { get; set; }

            [JsonProperty("Maximum Amount")]
            public int MaximumAmount { get; set; }

            [JsonProperty("Rarity")]
            [JsonConverter(typeof(StringEnumConverter))]
            public Rarity Rarity { get; set; }
        }

        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            VeryRare
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<Configuration>();

            if (string.Compare(_config.Version, Version.ToString()) < 0)
                UpdateConfig();

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            _config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(_config, true);
        }

        private void UpdateConfig()
        {
            PrintWarning("Config changes detected! Updating...");

            Configuration defaultConfig = GetDefaultConfig();

            if (string.Compare(_config.Version, "1.0.0") < 0)
                _config = defaultConfig;

            if (string.Compare(_config.Version, "1.3.0") < 0)
            {
                for (int i = 0; i < _config.HarvestableVehicles.Count; i++)
                {
                    HarvestableVehicleConfig vehicleConfig = _config.HarvestableVehicles[i];
                    HarvestableVehicleConfig defaultVehicleConfig = defaultConfig.HarvestableVehicles[i];

                    for (int j = 0; j < vehicleConfig.Resources.Count; j++)
                    {
                        ResourceConfig resourceConfig = vehicleConfig.Resources[j];
                        ResourceConfig defaultResourceConfig = defaultVehicleConfig.Resources[j];

                        resourceConfig.MinimumAmount = defaultResourceConfig.MinimumAmount;
                        resourceConfig.MaximumAmount = defaultResourceConfig.MaximumAmount;
                    }
                }
            }

            if (string.Compare(_config.Version, "1.4.0") < 0)
            {
                _config.GatheringTools = defaultConfig.GatheringTools;
            }

            if (string.Compare(_config.Version, "1.5.0") < 0)
            {
                _config.RarityWeights = defaultConfig.RarityWeights;

                foreach (HarvestableVehicleConfig vehicleConfig in _config.HarvestableVehicles)
                {
                    foreach (ResourceConfig resourceConfig in vehicleConfig.Resources)
                    {
                        resourceConfig.Rarity = Rarity.Common;
                    }
                }
            }

            PrintWarning("Config update complete! Updated from version " + _config.Version + " to " + Version.ToString());
            _config.Version = Version.ToString();
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                Version = Version.ToString(),
                GatheringTools = new List<GatheringToolConfig>
                {
                    new GatheringToolConfig
                    {
                        ItemShortName = "hammer.salvaged",
                        SkinId = 0
                    },
                    new GatheringToolConfig
                    {
                        ItemShortName = "jackhammer",
                        SkinId = 0
                    }
                },
                HarvestableVehicles = new List<HarvestableVehicleConfig>
                {
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "minicopter.entity",
                            "scraptransporthelicopter"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                MinimumAmount = 5,
                                MaximumAmount = 10,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                MinimumAmount = 1,
                                MaximumAmount = 5,
                                Rarity = Rarity.Rare
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "hotairballoon",
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "cloth",
                                SkinId = 0,
                                MinimumAmount = 10,
                                MaximumAmount = 15,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 10,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "rope",
                                SkinId = 0,
                                MinimumAmount = 1,
                                MaximumAmount = 3,
                                Rarity = Rarity.Common
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "rhib",
                            "rowboat"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 15,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                SkinId = 0,
                                MinimumAmount = 1,
                                MaximumAmount = 5,
                                Rarity = Rarity.Rare
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "kayak"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "wood",
                                SkinId = 0,
                                MinimumAmount = 10,
                                MaximumAmount = 20,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 15,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "cloth",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 10,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "rope",
                                SkinId = 0,
                                MinimumAmount = 1,
                                MaximumAmount = 3,
                                Rarity = Rarity.Common
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "motorbike",
                            "motorbike_sidecar",
                            "pedalbike",
                            "pedaltrike"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 15,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                SkinId = 0,
                                MinimumAmount = 1,
                                MaximumAmount = 5,
                                Rarity = Rarity.Rare
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "1module_cockpit",
                            "1module_cockpit_armored",
                            "1module_cockpit_with_engine",
                            "1module_engine",
                            "1module_flatbed",
                            "1module_passengers_armored",
                            "1module_rear_seats",
                            "1module_storage",
                            "1module_taxi",
                            "2module_camper",
                            "2module_flatbed",
                            "2module_fuel_tank",
                            "2module_passengers"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                SkinId = 0,
                                MinimumAmount = 5,
                                MaximumAmount = 10,
                                Rarity = Rarity.Common
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                SkinId = 0,
                                MinimumAmount = 1,
                                MaximumAmount = 5,
                                Rarity = Rarity.Rare
                            }
                        },
                        DamageIncreaseFactor = 5
                    },
                },
                RarityWeights = new Dictionary<Rarity, int>
                {
                    { Rarity.Common, 60 },
                    { Rarity.Uncommon, 25 },
                    { Rarity.Rare, 10 },
                    { Rarity.VeryRare, 5 }
                }
            };
        }

        #endregion Configuration

        #region Oxide Hooks

        private void Init()
        {
            _plugin = this;
            PermissionUtil.RegisterPermissions();
        }

        private void Unload()
        {
            _config = null;
            _plugin = null;
        }

        private void OnEntityTakeDamage(BaseEntity entity, HitInfo hitInfo)
        {
            if (entity == null || hitInfo == null)
                return;

            if (!(entity is BaseVehicle) && !(entity is HotAirBalloon))
                return;

            if (entity.Health() <= 0)
                return;

            BasePlayer player = hitInfo.InitiatorPlayer;
            if (player == null || player.IsNpc)
                return;

            Item item = player.GetActiveItem();
            if (item == null || item.info.category != ItemCategory.Tool)
                return;

            GatheringToolConfig toolConfig = _config.GatheringTools
                .FirstOrDefault(t => t.ItemShortName == item.info.shortname);

            if (toolConfig == null || !_config.GatheringTools.Exists(t => t.ItemShortName == item.info.shortname))
                return;

            if (toolConfig.SkinId != 0 && item.skin != toolConfig.SkinId)
                return;

            BaseLock baseLock = entity.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
            if (baseLock != null)
            {
                CodeLock codeLock = baseLock as CodeLock;
                if (codeLock != null && !codeLock.whitelistPlayers.Contains(player.userID))
                    return;
                else if (baseLock.OwnerID != player.userID)
                    return;
            }

            if (!PermissionUtil.HasPermission(player, PermissionUtil.USE))
                return;

            foreach (HarvestableVehicleConfig vehicleConfig in _config.HarvestableVehicles)
            {
                if (vehicleConfig.VehicleShortPrefabNames.Contains(entity.ShortPrefabName))
                {
                    DoGather(entity, player, vehicleConfig);
                    ScaleDamage(hitInfo, vehicleConfig.DamageIncreaseFactor);
                    break;
                }
            }
        }

        #endregion Oxide Hooks

        #region Resource Gathering

        private void DoGather(BaseEntity entity, BasePlayer player, HarvestableVehicleConfig vehicleConfig)
        {
            List<ResourceConfig> availableResources = vehicleConfig.Resources;
            if (availableResources.Count == 0)
                return;

            ResourceConfig selectedResource = SelectResourceByRarity(availableResources);
            if (selectedResource == null)
                return;

            int amountToGive = _random.Next(selectedResource.MinimumAmount, selectedResource.MaximumAmount + 1);
            if (amountToGive <= 0)
                return;

            Item item = ItemManager.CreateByName(selectedResource.ItemShortName, amountToGive, selectedResource.SkinId);
            if (selectedResource.DisplayName != null)
                item.name = selectedResource.DisplayName;

            if (item != null)
            {
                player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
            }
        }

        private ResourceConfig SelectResourceByRarity(List<ResourceConfig> resources)
        {
            int totalWeight = 0;
            foreach (ResourceConfig resource in resources)
            {
                if (_config.RarityWeights.TryGetValue(resource.Rarity, out int weight))
                    totalWeight += weight;
            }

            if (totalWeight <= 0)
                return null;

            int randomValue = _random.Next(0, totalWeight);
            int cumulativeWeight = 0;

            foreach (var resource in resources)
            {
                if (_config.RarityWeights.TryGetValue(resource.Rarity, out int weight))
                {
                    cumulativeWeight += weight;
                    if (randomValue < cumulativeWeight)
                    {
                        return resource;
                    }
                }
            }

            return null;
        }

        #endregion Resource Gathering

        #region Damage Scaling

        private void ScaleDamage(HitInfo hitInfo, float damageIncreaseFactor)
        {
            if (damageIncreaseFactor != 1.0f)
            {
                hitInfo.damageTypes.ScaleAll(damageIncreaseFactor);
            }
        }

        #endregion Damage Scaling

        #region Permissions

        private static class PermissionUtil
        {
            public const string USE = "harvestablevehicles.use";
            private static readonly List<string> _permissions = new List<string>
            {
                USE,
            };

            public static void RegisterPermissions()
            {
                foreach (var permission in _permissions)
                {
                    _plugin.permission.RegisterPermission(permission, _plugin);
                }
            }

            public static bool HasPermission(BasePlayer player, string permissionName)
            {
                return _plugin.permission.UserHasPermission(player.UserIDString, permissionName);
            }
        }

        #endregion Permissions
    }
}