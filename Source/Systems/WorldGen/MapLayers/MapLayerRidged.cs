﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;
using Vintagestory.ServerMods;

namespace Immersion
{
    public class MapLayerRidged : MapLayerBase
    {
        RidgedSimplexNoise noisegen;

        float multiplier;
        double[] thresholds;

        public MapLayerRidged(long seed, int octaves, float persistence, int scale, int multiplier) : base(seed)
        {
            noisegen = RidgedSimplexNoise.FromDefaultOctaves(octaves, 1f / scale, persistence, seed + 12321);
            this.multiplier = multiplier;
        }

        public MapLayerRidged(long seed, int octaves, float persistence, int scale, int multiplier, double[] thresholds) : base(seed)
        {
            noisegen = RidgedSimplexNoise.FromDefaultOctaves(octaves, 1f / scale, persistence, seed + 12321);
            this.multiplier = multiplier;
            this.thresholds = thresholds;
        }


        public override int[] GenLayer(int xCoord, int zCoord, int sizeX, int sizeZ)
        {
            int[] outData = new int[sizeX * sizeZ];

            if (thresholds != null)
            {
                for (int z = 0; z < sizeZ; ++z)
                {
                    for (int x = 0; x < sizeX; ++x)
                    {
                        outData[z * sizeX + x] = (int)GameMath.Clamp(multiplier * noisegen.InvNoise(xCoord + x, zCoord + z, thresholds), 0, 255);
                    }
                }
            }
            else
            {
                for (int z = 0; z < sizeZ; ++z)
                {
                    for (int x = 0; x < sizeX; ++x)
                    {
                        outData[z * sizeX + x] = (int)GameMath.Clamp(multiplier * noisegen.InvNoise(xCoord + x, zCoord + z), 0, 255);
                    }
                }
            }



            return outData;
        }

        public int[] GenLayer(int xCoord, int zCoord, int sizeX, int sizeZ, double[] thresholds)
        {
            int[] outData = new int[sizeX * sizeZ];

            for (int z = 0; z < sizeZ; ++z)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    outData[z * sizeX + x] = (int)GameMath.Clamp(multiplier * noisegen.InvNoise(xCoord + x, zCoord + z, thresholds), 0, 255);
                }
            }

            return outData;
        }
    }
}
