using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BlobcatMod.Tasks {
    public class AiTaskPetLookAtEntity : AiTaskBaseTargetable {
        public float seekingRange = 25f;

        protected float minTurnAnglePerSec;
        protected float maxTurnAnglePerSec;

        protected float turnRadPerSec;

        public AiTaskPetLookAtEntity(EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig) : base(entity, taskConfig, aiConfig) {
            seekingRange = taskConfig["seekingRange"].AsFloat(25.0f);
        }

        public override bool ShouldExecute() {
            if (!PreconditionsSatisfied()) return false;
            targetEntity = partitionUtil.GetNearestEntity(entity.Pos.XYZ, seekingRange, (e) => IsTargetableEntity(e, seekingRange), EnumEntitySearchType.Creatures);
            return targetEntity != null;
        }

        public override void StartExecute() {
            base.StartExecute();

            var pathfinder = entity?.Properties.Server?.Attributes?.GetTreeAttribute("pathfinder");

            minTurnAnglePerSec = pathfinder?.GetFloat("minTurnAnglePerSec", 250) ?? 250;
            maxTurnAnglePerSec = pathfinder?.GetFloat("maxTurnAnglePerSec", 450) ?? 450;

            var turnAnglePerSec = minTurnAnglePerSec + entity.World.Rand.NextDouble() * (maxTurnAnglePerSec - minTurnAnglePerSec);
            turnRadPerSec = (float)turnAnglePerSec * GameMath.DEG2RAD;
        }

        public override bool ContinueExecute(float dt) {
            if (entity.Pos.SquareDistanceTo(targetEntity.Pos) > seekingRange * seekingRange) {
                return false;
            }

            Vec3f targetVec = new Vec3f(
                (float)(targetEntity.Pos.X - entity.Pos.X),
                (float)(targetEntity.Pos.Y - entity.Pos.Y),
                (float)(targetEntity.Pos.Z - entity.Pos.Z)
            );

            float targetYaw = (float)Math.Atan2(targetVec.X, targetVec.Z);

            float yawDistance = GameMath.AngleRadDistance(entity.Pos.Yaw, targetYaw);
            if (Math.Abs(yawDistance) > 0.01) {
                entity.Pos.Yaw += GameMath.Clamp(yawDistance, -turnRadPerSec * dt, turnRadPerSec * dt);
                entity.Pos.Yaw = entity.Pos.Yaw % GameMath.TWOPI;
            }

            return true;
        }



        public override bool Notify(string key, object data) {
            return false;
        }
    }
}
