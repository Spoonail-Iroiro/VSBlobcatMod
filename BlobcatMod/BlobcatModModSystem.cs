using BlobcatMod.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace BlobcatMod {
    public class BlobcatModModSystem : ModSystem {

        Harmony harmony;

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api) {
            api.Logger.Notification("Hello from template mod: " + api.Side);

            if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll();
            }
        }

        public override void StartServerSide(ICoreServerAPI api) {
            api.RegisterAiTask<AiTaskPetLookAtEntity>("petlookatentity");

# if DEBUG

            var baseCommand = api.ChatCommands
                .Create("blobcat")
                .RequiresPrivilege(Privilege.chat)
                .RequiresPlayer()
                .HandleWith((args) => {
                    return TextCommandResult.Success("See `.chb` for usage", null);
                });

            baseCommand.BeginSubCommand("de")
                .WithDescription("Debug entity")
                .RequiresPrivilege(Privilege.chat)
                .RequiresPlayer()
                .HandleWith(args => {
                    if (args.Caller.Player is IServerPlayer splr) {
                        var ent = splr.CurrentEntitySelection?.Entity;
                        if (ent == null) {
                            return TextCommandResult.Error("No entity selected");
                        }

                        return TextCommandResult.Success($"{ent.Alive}");
                    }
                    return TextCommandResult.Success($"");
                });
#endif
        }

        public override void StartClientSide(ICoreClientAPI api) {
            api.Logger.Notification("Hello from template mod client side: " + Lang.Get("blobcatmod:hello"));
        }

        public override void Dispose() {
            base.Dispose();

            harmony?.UnpatchAll(Mod.Info.ModID);
        }



    }
}
