﻿using Vorp.Core.Client.Interface;
using Vorp.Shared.Records;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterManager : Manager<CharacterManager>
    {
        CharacterCreatorManager characterCreatorManager => CharacterCreatorManager.GetModule();

        public override void Begin()
        {
            ClientGateway.Mount("vorp:character:list", new Action<Dictionary<int, Character>, int>(async (characters, maxCharcters) =>
            {
                Logger.Trace($"Received {characters.Count} characters from the server, max {maxCharcters} allowed.");

                await Screen.FadeOut(500);

                DisplayHud(false);
                DisplayRadar(false);

                await characterCreatorManager.StartCharacterCreator();

                if (characters.Count == 0)
                {
                    Logger.Trace($"No characters, lets goto character creator.");
                    return;
                }

                foreach (var kvp in characters)
                {
                    int characterId = kvp.Key;
                    Character character = kvp.Value;
                }

                await Screen.FadeIn(500);
            }));
        }
    }
}