/*
 * Copyright (C) 2024 Game4Freak.io
 * This mod is provided under the Game4Freak EULA.
 * Full legal terms can be found at https://game4freak.io/eula/
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Harvestable Vehicles", "VisEntities", "1.0.0")]
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

            [JsonProperty("Gathering Tool Short Names")]
            public List<string> GatheringToolShortNames { get; set; }

            [JsonProperty("Harvestable Vehicles")]
            public List<HarvestableVehicleConfig> HarvestableVehicles { get; set; }
        }

        public class HarvestableVehicleConfig
        {
            [JsonProperty("Vehicle Short Prefab Names")]
            public List<string> VehicleShortPrefabNames { get; set; }

            [JsonProperty("Resources")]
            public List<ResourceConfig> Resources { get; set; }

            [JsonProperty("Yield")]
            public YieldConfig Yield { get; set; }

            [JsonProperty("Damage Increase Factor")]
            public float DamageIncreaseFactor { get; set; }
        }

        public class ResourceConfig
        {
            [JsonProperty("Item Short Name")]
            public string ItemShortName { get; set; }

            [JsonProperty("Amount")]
            public int Amount { get; set; }
        }

        public class YieldConfig
        {
            [JsonProperty("No Yield Probability")]
            public int NoYieldProbability { get; set; }

            [JsonProperty("Normal Yield Probability")]
            public int NormalYieldProbability { get; set; }

            [JsonProperty("High Yield Probability")]
            public int HighYieldProbability { get; set; }

            [JsonProperty("High Yield Bonus")]
            public float HighYieldBonus { get; set; }
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

            PrintWarning("Config update complete! Updated from version " + _config.Version + " to " + Version.ToString());
            _config.Version = Version.ToString();
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                Version = Version.ToString(),
                GatheringToolShortNames = new List<string>
                {
                    "hammer.salvaged",
                    "jackhammer"
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
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
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
                                Amount = 10
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "rope",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
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
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
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
                                Amount = 10
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "cloth",
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "rope",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
                        },
                        DamageIncreaseFactor = 5
                    },
                    new HarvestableVehicleConfig
                    {
                        VehicleShortPrefabNames = new List<string>
                        {
                            "motorbike",
                            "motorbike_sidecar",
                            "pedalbike"
                        },
                        Resources = new List<ResourceConfig>
                        {
                            new ResourceConfig
                            {
                                ItemShortName = "metal.fragments",
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
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
                                Amount = 5
                            },
                            new ResourceConfig
                            {
                                ItemShortName = "metal.refined",
                                Amount = 1
                            }
                        },
                        Yield = new YieldConfig
                        {
                            NoYieldProbability = 70,
                            NormalYieldProbability = 25,
                            HighYieldProbability = 5,
                            HighYieldBonus = 1.5f
                        },
                        DamageIncreaseFactor = 5
                    },
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
            if (item == null || item.info.category != ItemCategory.Tool || !_config.GatheringToolShortNames.Contains(item.info.shortname))
                return;

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
            foreach (ResourceConfig resource in vehicleConfig.Resources)
            {
                int amountToGive = GetRandomizedAmount(resource.Amount, vehicleConfig.Yield);

                if (amountToGive > 0)
                {
                    Item item = ItemManager.CreateByName(resource.ItemShortName, amountToGive);
                    if (item != null)
                    {
                        player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
                    }
                }
            }
        }

        private int GetRandomizedAmount(int baseAmount, YieldConfig yieldConfig)
        {
            int chance = _random.Next(0, 100);

            if (chance < yieldConfig.NoYieldProbability)
            {
                return 0;
            }
            else if (chance < yieldConfig.NoYieldProbability + yieldConfig.NormalYieldProbability)
            {
                return baseAmount;
            }
            else
            {
                return Mathf.CeilToInt(baseAmount * yieldConfig.HighYieldBonus);
            }
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
