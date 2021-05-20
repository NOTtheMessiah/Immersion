﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace Immersion
{
    public static class Extensions
    {
        public static T Next<T>(this T[] array, ref uint index)
        {
            index = (uint)(++index % array.Length);
            return array[index];
        }

        public static T Next<T>(this List<T> array, ref uint index) => array.ToArray().Next(ref index);
        public static T Next<T>(this HashSet<T> array, ref uint index) => array.ToArray().Next(ref index);

        public static T Prev<T>(this T[] array, ref uint index)
        {
            index = index > 0 ? index - 1 : (uint)(array.Length - 1);
            return array[index];
        }

        public static Block GetBlock(this BlockPos Pos, IWorldAccessor world) { return world.BlockAccessor.GetBlock(Pos); }
        public static Block GetBlock(this BlockPos Pos, ICoreAPI Api) { return Pos.GetBlock(Api.World); }

        public static Block GetBlock(this AssetLocation asset, ICoreAPI Api)
        {
            if (Api.World.BlockAccessor.GetBlock(asset) != null)
            {
                return Api.World.BlockAccessor.GetBlock(asset);
            }
            return null;
        }

        public static Item GetItem(this AssetLocation asset, ICoreAPI Api)
        {
            if (Api.World.GetItem(asset) != null)
            {
                return Api.World.GetItem(asset);
            }
            return null;
        }

        public static AssetLocation ToAsset(this string asset) { return new AssetLocation(asset); }
        public static Block ToBlock(this string block, ICoreAPI Api) => block.WithDomain().ToAsset().GetBlock(Api);
        public static Block Block(this BlockSelection sel, ICoreAPI Api) => Api.World.BlockAccessor.GetBlock(sel.Position);
        public static BlockEntity BlockEntity(this BlockSelection sel, ICoreAPI Api) => Api.World.BlockAccessor.GetBlockEntity(sel.Position);

        public static AssetLocation[] ToAssets(this string[] strings)
        {
            List<AssetLocation> assets = new List<AssetLocation>();
            foreach (var val in strings)
            {
                assets.Add(val.ToAsset());
            }
            return assets.ToArray();
        }
        public static AssetLocation[] ToAssets(this List<string> strings) => strings.ToArray().ToAssets();

        public static void PlaySoundAtWithDelay(this IWorldAccessor world, AssetLocation location, BlockPos Pos, int delay)
        {
            world.RegisterCallback(dt => world.PlaySoundAt(location, Pos.X, Pos.Y, Pos.Z), delay);
        }

        public static T[] Stretch<T>(this T[] array, int amount)
        {
            if (amount < 1) return array;
            T[] parray = array;

            Array.Resize(ref array, array.Length + amount);
            for (int i = 0; i < array.Length; i++)
            {
                double scalar = (double)i / array.Length;
                array[i] = parray[(int)(scalar * parray.Length)];
            }
            return array;
        }

        public static int IndexOfMin(this IList<int> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", "self");
            }

            int min = self[0];
            int minIndex = 0;

            for (int i = 1; i < self.Count; ++i)
            {
                if (self[i] < min)
                {
                    min = self[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public static int IndexOfMin(this int[] self) => IndexOfMin(self.ToList());

        public static bool IsSurvival(this EnumGameMode gamemode) => gamemode == EnumGameMode.Survival;
        public static bool IsCreative(this EnumGameMode gamemode) => gamemode == EnumGameMode.Creative;
        public static bool IsSpectator(this EnumGameMode gamemode) => gamemode == EnumGameMode.Spectator;
        public static bool IsGuest(this EnumGameMode gamemode) => gamemode == EnumGameMode.Guest;
        public static void PlaySoundAt(this IWorldAccessor world, AssetLocation loc, BlockPos Pos) => world.PlaySoundAt(loc, Pos.X, Pos.Y, Pos.Z);
        public static int GetID(this AssetLocation loc, ICoreAPI Api) => loc.GetBlock(Api).BlockId;

        public static string WithDomain(this string a, string domain = "game") => a.IndexOf(":") == -1 ? domain + ":" + a : a;

        public static string[] WithDomain(this string[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = a[i].WithDomain();
            }
            return a;
        }

        public static Vec3d MidPoint(this BlockPos Pos) => Pos.ToVec3d().AddCopy(0.5, 0.5, 0.5);

        public static string Apd(this string a, string appended)
        {
            return a + "-" + appended;
        }

        public static string Apd(this string a, int appended)
        {
            return a + "-" + appended;
        }

        public static void SpawnItemEntity(this IWorldAccessor world, ItemStack[] stacks, Vec3d Pos, Vec3d velocity = null)
        {
            foreach (ItemStack stack in stacks)
            {
                world.SpawnItemEntity(stack, Pos, velocity);
            }
        }

        public static void SpawnItemEntity(this IWorldAccessor world, JsonItemStack[] stacks, Vec3d Pos, Vec3d velocity = null)
        {
            foreach (JsonItemStack stack in stacks)
            {
                string err = "";
                stack.Resolve(world, err);
                if (stack.ResolvedItemstack != null) world.SpawnItemEntity(stack.ResolvedItemstack, Pos, velocity);
            }
        }

        public static BlockEntity BlockEntity(this BlockPos Pos, IWorldAccessor world)
        {
            return world.BlockAccessor.GetBlockEntity(Pos);
        }

        public static BlockEntity BlockEntity(this BlockPos Pos, ICoreAPI Api) => Pos.BlockEntity(Api.World);

        public static BlockEntity BlockEntity(this BlockSelection sel, IWorldAccessor world)
        {
            return sel.Position.BlockEntity(world);
        }

        public static void InitializeAnimators(this BlockEntityAnimationUtil util, Vec3f rot, params string[] CacheDictKeys)
        {
            foreach (var val in CacheDictKeys)
            {
                util.InitializeAnimator(val, rot);
            }
        }

        public static void InitializeAnimators(this BlockEntityAnimationUtil util, Vec3f rot, List<string> CacheDictKeys)
        {
            InitializeAnimators(util, rot, CacheDictKeys.ToArray());
        }

        public static void SetUv(this MeshData mesh, TextureAtlasPosition texPos) => mesh.SetUv(new float[] { texPos.x1, texPos.y1, texPos.x2, texPos.y1, texPos.x2, texPos.y2, texPos.x1, texPos.y2 });

        public static bool TryGiveItemstack(this IPlayerInventoryManager manager, ItemStack[] stacks)
        {
            foreach (var val in stacks)
            {
                if (manager.TryGiveItemstack(val)) continue;
                return false;
            }
            return true;
        }

        public static bool TryGiveItemstack(this IPlayerInventoryManager manager, JsonItemStack[] stacks)
        {
            return manager.TryGiveItemstack(stacks.ResolvedStacks(manager.ActiveHotbarSlot.Inventory.Api.World));
        }

        public static double DistanceTo(this SyncedEntityPos Pos, Vec3d vec)
        {
            return Math.Sqrt(Pos.SquareDistanceTo(vec));
        }

        public static bool WildCardMatch(this RegistryObject obj, string a) => obj.WildCardMatch(new AssetLocation(a));

        public static ItemStack[] ResolvedStacks(this JsonItemStack[] stacks, IWorldAccessor world)
        {
            List<ItemStack> stacks1 = new List<ItemStack>();
            foreach (JsonItemStack stack in stacks)
            {
                stack.Resolve(world, null);
                stacks1.Add(stack.ResolvedItemstack);
            }
            return stacks1.ToArray();
        }

        public static K GetField<T, K>(this T instance, string fieldName)
        {
            return (K)instance.GetField(fieldName);
        }

        public static object GetField<T>(this T instance, string fieldName)
        {
            return GetInstanceField(instance.GetType(), instance, fieldName);
        }

        public static object CallMethod<T>(this T instance, string methodName) => instance?.CallMethod(methodName, null);

        public static object CallMethod<T>(this T instance, string methodName, params object[] parameters)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo info = instance?.GetType()?.GetMethod(methodName, bindFlags);
            return info?.Invoke(instance, parameters);
        }

        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        public static List<AssetLocation> GetMatches(this AssetLocation code, ICoreAPI Api)
        {
            List<AssetLocation> assets = new List<AssetLocation>();
            foreach (var block in Api.World.Blocks)
            {
                if (block?.Code == null) continue;
                if (block.WildCardMatch(code)) assets.Add(block.Code);
            }
            foreach (var item in Api.World.Items)
            {
                if (item?.Code == null) continue;
                if (item.WildCardMatch(code)) assets.Add(item.Code);
            }

            return assets;
        }

        public static bool WildCardMatch(this AssetLocation asset, AssetLocation match, EnumItemClass itemClass, ICoreAPI Api)
        {
            if (asset == null || match == null) return false;

            if (itemClass == EnumItemClass.Item)
            {
                return asset.GetItem(Api).WildCardMatch(match);
            }
            else if (itemClass == EnumItemClass.Block)
            {
                return asset.GetBlock(Api).WildCardMatch(match);
            }
            return false;
        }

        public static bool IsBlock(this JsonItemStack stack) => stack.Type == EnumItemClass.Block;
        public static bool IsItem(this JsonItemStack stack) => stack.Type == EnumItemClass.Item;

        public static TextureAtlasPosition GetTexAtlasPosition(this ICoreClientAPI capi, CollectibleObject collectible, string direction = "up")
        {
            return collectible.ItemClass == EnumItemClass.Item ?
                capi.ItemTextureAtlas.GetPosition(collectible.Code.GetItem(capi)) :
                capi.BlockTextureAtlas.GetPosition(collectible.Code.GetBlock(capi), direction);
        }
        public static double DistanceTo(this Vec3d start, Vec3d end) => Math.Sqrt(start.SquareDistanceTo(end));

        public static Vec3i ToClimateVec(this IntDataMap2D climateMap, int chunkX, int chunkZ, int regionsize, int chunksize, float lx = 0.5f, float ly = 0.5f)
        {
            int regionChunkSize = regionsize / chunksize;
            float fac = (float)climateMap.InnerSize / regionChunkSize;
            int rlX = chunkX % regionChunkSize;
            int rlZ = chunkZ % regionChunkSize;

            int climateUpLeft = climateMap.GetUnpaddedInt((int)(rlX * fac), (int)(rlZ * fac));
            int climateUpRight = climateMap.GetUnpaddedInt((int)(rlX * fac + fac), (int)(rlZ * fac));
            int climateBotLeft = climateMap.GetUnpaddedInt((int)(rlX * fac), (int)(rlZ * fac + fac));
            int climateBotRight = climateMap.GetUnpaddedInt((int)(rlX * fac + fac), (int)(rlZ * fac + fac));

            int climateMid = GameMath.BiLerpRgbColor(lx, ly, climateUpLeft, climateUpRight, climateBotLeft, climateBotRight);

            int rain = (climateMid >> 8) & 0xff;
            int humidity = climateMid & 0xff;
            int temp = (climateMid >> 16) & 0xff;

            return new Vec3i(rain, humidity, temp);
        }

        public static Block GetBlock(this int BlockId, ICoreAPI api) => BlockId.GetBlock(api.World);

        public static Block GetBlock(this int BlockId, IWorldAccessor world)
        {
            return world.GetBlock(BlockId);
        }

        public static JsonCraftingOutput[] DeepClone(this JsonCraftingOutput[] outputs)
        {
            if (outputs == null) return null;

            List<JsonCraftingOutput> outputlist = new List<JsonCraftingOutput>();
            foreach (var val in outputs)
            {
                JsonCraftingOutput output = new JsonCraftingOutput();
                output.Attributes = val.Attributes?.Token?.HasValues ?? false ? val.Attributes : null;
                output.Code = val.Code;
                output.StackSize = val.StackSize;
                output.Type = val.Type;
                outputlist.Add(output);
            }

            return outputlist.ToArray();
        }

        public static AssetLocation WithVariant(this AssetLocation location, string variantcode, string variantstate)
        {
            return new AssetLocation(location.ToString().Replace("{" + variantcode + "}", variantstate));
        }

        public static int GetSunlight(this IBlockAccessor blockAcessor, BlockPos pos) => GetSunlight(blockAcessor, pos.X, pos.Y, pos.Z);

        public static int GetSunlight(this IBlockAccessor blockAcessor, int posX, int posY, int posZ)
        {
            IWorldAccessor world = (blockAcessor as BlockAccessorBase).GetField<BlockAccessorBase, IWorldAccessor>("worldAccessor");
            WorldMap worldMap = (blockAcessor as BlockAccessorBase).GetField<BlockAccessorBase, WorldMap>("worldmap");

            IWorldChunk chunkAtBlockPos = blockAcessor.GetChunkAtBlockPos(posX, posY, posZ);
            if (chunkAtBlockPos == null || !worldMap.IsValidPos(posX, posY, posZ))
                return world.SunBrightness;
            chunkAtBlockPos.Unpack();
            int chunkSize = blockAcessor.ChunkSize;
            int index = (posY % chunkSize * chunkSize + posZ % chunkSize) * chunkSize + posX % chunkSize;
            int num = (int)chunkAtBlockPos.Light[index];
            int val2 = num >> 5 & 31;
            int val1 = num & 31;

            return (int)Math.Round((double)val1 * (double)world.Calendar.GetDayLightStrength(posX,posZ));
        }
    }
}
