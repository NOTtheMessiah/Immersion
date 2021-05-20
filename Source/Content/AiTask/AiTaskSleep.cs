﻿using System;
using System.Collections.Generic;
using Immersion.Utility;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Immersion
{
    public class AiTaskSleep : AiTaskBase
    {
        public AiTaskSleep(EntityAgent entity) : base(entity)
        {
        }

        public bool isNocturnal = true;

        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            if (taskConfig["isnocturnal"] != null)
            {
                isNocturnal = taskConfig["isnocturnal"].AsBool(true);
            }
            base.LoadConfig(taskConfig, aiConfig);
        }

        public override bool ShouldExecute()
        {
            float dls = entity.World.Calendar.GetDayLightStrength(entity.Pos.X, entity.Pos.Z);
			return (isNocturnal && dls > 0.50f || !isNocturnal && dls < 0.50f);
        }

        public override bool ContinueExecute(float dt)
        {
			return ShouldExecute();
        }

    }
}
