﻿using Vorp.Core.Client.Interface;
using Vorp.Core.Client.Interface.Menu;
using Vorp.Shared.Enums;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterEditorManager : Manager<CharacterEditorManager>
    {
        /*
         * Requirements
         * -> Character options for selected sex needs to update the screen
         * -> NUI Updates
         * 
         * 
         * */

        private static PluginManager Instance => PluginManager.Instance;

        static Vector3 _pedPosition = new Vector3(-558.3258f, -3781.111f, 237.60f);
        static float _pedHeading = 93.2f;
        static Ped _ped;

        static Camera _cameraFace;
        static Camera _cameraBody;
        static Camera _cameraLegs;
        static bool isTransitioning = false;

        static Camera _currentCamera;
        static string _camera;

        static bool _hideNui;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));

            Instance.NuiManager.RegisterCallback("CharacterRandomise", new Action(async () =>
            {
                _ped.RandomiseClothingAsync(true);
                await BaseScript.Delay(100);
                // Need to review updating values on the NUI Menu
                CreateBaseMenu(true);
            }));

            Instance.NuiManager.RegisterCallback("CharacterCamera", new Action<List<string>>(async args =>
            {
                Dictionary<string, dynamic> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(args[0]);
                if (!valuePairs.ContainsKey("camera")) return;
                string camera = valuePairs["camera"];

                if (camera == _camera) return;

                if (isTransitioning) return;
                isTransitioning = true;

                if (_cameraFace.IsActive)
                {
                    _currentCamera = _cameraFace;
                }
                else if (_cameraBody.IsActive)
                {
                    _currentCamera = _cameraBody;
                }
                else if (_cameraLegs.IsActive)
                {
                    _currentCamera = _cameraLegs;
                }

                Camera nextCamera = _cameraBody;
                if (camera == "Face")
                    nextCamera = _cameraFace;
                else if (camera == "Legs")
                    nextCamera = _cameraLegs;

                _camera = camera;

                if (_currentCamera.Handle == nextCamera.Handle)
                {
                    isTransitioning = false;
                    return;
                }

                Logger.Trace($"Current Camera: {_currentCamera.Handle} / Next Camera: {nextCamera.Handle}");

                SetCamActiveWithInterp(nextCamera.Handle, _currentCamera.Handle, 2000, 250, 250);
                await BaseScript.Delay(2000);
                nextCamera.IsActive = true;
                _currentCamera.IsActive = false;
                isTransitioning = false;
            }));

            Instance.NuiManager.RegisterCallback("CharacterSetComponent", new Action<List<string>>(args =>
            {
                try
                {
                    Dictionary<string, dynamic> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(args[0]);
                    if (!valuePairs.ContainsKey("selectedValue")) return;

                    int.TryParse($"{valuePairs["selectedValue"]}", out int selectIndex);
                    string component = valuePairs["component"];

                    VorpPedComponents vorpPedComponents = _ped.PedComponents;

                    switch (component)
                    {
                        case "Eyes":
                            UpdateComponent(selectIndex, _ped.Eyes, vorpPedComponents.Eyes);
                            break;
                        case "Head":
                            UpdateComponent(selectIndex, _ped.Heads, vorpPedComponents.Head);
                            break;
                        case "BodyUpper":
                            UpdateComponent(selectIndex, _ped.BodiesUpper, vorpPedComponents.BodyUpper);
                            break;
                        case "BodyLower":
                            UpdateComponent(selectIndex, _ped.BodiesLower, vorpPedComponents.BodyLower);
                            break;
                        case "Accessory":
                            UpdateComponent(selectIndex, _ped.Accessories, vorpPedComponents.Accessory);
                            break;
                        case "Armor":
                            UpdateComponent(selectIndex, _ped.Armor, vorpPedComponents.Armor);
                            break;
                        case "Badge":
                            UpdateComponent(selectIndex, _ped.Badges, vorpPedComponents.Badge);
                            break;
                        case "Beard":
                            UpdateComponent(selectIndex, _ped.Beards, vorpPedComponents.Beard);
                            break;
                        case "Belt":
                            UpdateComponent(selectIndex, _ped.Belts, vorpPedComponents.Belt);
                            break;
                        case "BeltBuckle":
                            UpdateComponent(selectIndex, _ped.BeltBuckles, vorpPedComponents.BeltBuckle);
                            break;
                        case "BootAccessory":
                            UpdateComponent(selectIndex, _ped.BootAccessories, vorpPedComponents.BootAccessory);
                            break;
                        case "Boots":
                            UpdateComponent(selectIndex, _ped.Boots, vorpPedComponents.Boots);
                            break;
                        case "Chaps":
                            UpdateComponent(selectIndex, _ped.Chaps, vorpPedComponents.Chaps);
                            break;
                        case "Cloaks":
                            UpdateComponent(selectIndex, _ped.Cloaks, vorpPedComponents.Cloak);
                            break;
                        case "Coats":
                            UpdateComponent(selectIndex, _ped.Coats, vorpPedComponents.Coat);
                            break;
                        case "CoatsClosed":
                            UpdateComponent(selectIndex, _ped.CoatsClosed, vorpPedComponents.CoatClosed);
                            break;
                        case "Dresses":
                            UpdateComponent(selectIndex, _ped.Dresses, vorpPedComponents.Dresses);
                            break;
                        case "EyeWear":
                            UpdateComponent(selectIndex, _ped.EyeWear, vorpPedComponents.EyeWear);
                            break;
                        case "Gauntlets":
                            UpdateComponent(selectIndex, _ped.Gauntlets, vorpPedComponents.Gauntlet);
                            break;
                        case "Gloves":
                            UpdateComponent(selectIndex, _ped.Gloves, vorpPedComponents.Gloves);
                            break;
                        case "Gunbelts":
                            UpdateComponent(selectIndex, _ped.Gunbelts, vorpPedComponents.Gunbelt);
                            break;
                        case "GunbeltAccessories":
                            UpdateComponent(selectIndex, _ped.GunbeltAccessories, vorpPedComponents.GunbeltAccessory);
                            break;
                        case "Hair":
                            UpdateComponent(selectIndex, _ped.Hairs, vorpPedComponents.Hair);
                            break;
                        case "HairAccessories":
                            UpdateComponent(selectIndex, _ped.HairAccessories, vorpPedComponents.HairAccessory);
                            break;
                        case "Hats":
                            UpdateComponent(selectIndex, _ped.Hats, vorpPedComponents.Hat);
                            break;
                        case "HolstersLeft":
                            UpdateComponent(selectIndex, _ped.HolstersLeft, vorpPedComponents.HolstersLeft);
                            break;
                        case "JewelryBracelets":
                            UpdateComponent(selectIndex, _ped.JewelryBracelets, vorpPedComponents.JewelryBracelets);
                            break;
                        case "JewelryRingsLeft":
                            UpdateComponent(selectIndex, _ped.JewelryRingsLeft, vorpPedComponents.JewelryRingsLeft);
                            break;
                        case "JewelryRingsRight":
                            UpdateComponent(selectIndex, _ped.JewelryRingsRight, vorpPedComponents.JewelryRingsRight);
                            break;
                        case "Loadouts":
                            UpdateComponent(selectIndex, _ped.Loadouts, vorpPedComponents.Loadout);
                            break;
                        case "Masks":
                            UpdateComponent(selectIndex, _ped.Masks, vorpPedComponents.Masks);
                            break;
                        case "Neckties":
                            UpdateComponent(selectIndex, _ped.Neckties, vorpPedComponents.Neckties);
                            break;
                        case "Neckwear":
                            UpdateComponent(selectIndex, _ped.Neckwear, vorpPedComponents.Neckwear);
                            break;
                        case "Pants":
                            UpdateComponent(selectIndex, _ped.Pants, vorpPedComponents.Pants);
                            vorpPedComponents.Skirt.Value = 0;
                            break;
                        case "Ponchos":
                            UpdateComponent(selectIndex, _ped.Ponchos, vorpPedComponents.Poncho);
                            break;
                        case "Satchels":
                            UpdateComponent(selectIndex, _ped.Satchels, vorpPedComponents.Satchel);
                            break;
                        case "Shirt":
                            UpdateComponent(selectIndex, _ped.Shirts, vorpPedComponents.Shirt);
                            break;
                        case "Skirts":
                            UpdateComponent(selectIndex, _ped.Skirts, vorpPedComponents.Skirt);
                            vorpPedComponents.Pants.Value = 0;
                            break;
                        case "Spats":
                            UpdateComponent(selectIndex, _ped.Spats, vorpPedComponents.Spats);
                            break;
                        case "Suspenders":
                            UpdateComponent(selectIndex, _ped.Suspenders, vorpPedComponents.Suspenders);
                            break;
                        case "Teeth":
                            UpdateComponent(selectIndex, _ped.Teeth, vorpPedComponents.Teeth);
                            break;
                        case "Vests":
                            UpdateComponent(selectIndex, _ped.Vests, vorpPedComponents.Vest);
                            break;
                        default:
                            Logger.Error($"Component '{component}' not configured");
                            break;
                    }

                    _ped.PedComponents = vorpPedComponents;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error occurred when trying to change item");
                }
            }));
        }

        internal static void UpdateComponent(int index, List<long> items, PedComponent pedComponent)
        {
            index--; // minus as UI sends from list starting at 1
            
            if (index > items.Count) index = items.Count - 1;

            pedComponent.Value = index <= 0 ? 0 : items[index];
        }

        internal static async void Init(string pedModel, VorpPedComponents components)
        {
            _ped = await VorpAPI.CreatePed(pedModel, _pedPosition, _pedHeading);
            _ped.IsPositionFrozen = true;
            SetEntityInvincible(_ped.Handle, true);
            _ped.PedComponents = components;

            _ped.TaskStartScenarioInPlace("GENERIC_STANDING_SCENARIO", _pedHeading);

            Vector3 rot = new Vector3(0f, 0f, -90f);
            float fov = 37f;

            RenderScriptCams(true, true, 250, true, true, 0);
            _cameraFace = VorpAPI.CreateCameraWithParams(new Vector3(-559.4195f, -3780.841f, 239.1749f), rot, fov);
            _cameraBody = VorpAPI.CreateCameraWithParams(new Vector3(-561.569f, -3780.841f, 238.5f), rot, fov);
            _cameraLegs = VorpAPI.CreateCameraWithParams(new Vector3(-559.4195f, -3780.841f, 238.9249f), rot, fov);

            Logger.Trace($"Cameras: Face {_cameraFace.Handle} / Body {_cameraBody.Handle} / Legs {_cameraLegs.Handle}");

            Instance.AttachTickHandler(OnNuiHandling);

            DisplayHud(false);
            DisplayRadar(false);

            await BaseScript.Delay(500);
            _cameraFace.IsActive = false;
            _cameraBody.IsActive = true;
            _cameraLegs.IsActive = false;

            World.SetWeather(eWeatherType.SUNNY);
            World.SetWeatherFrozen(true);
            World.WindSpeed = 0f;

            await BaseScript.Delay(1000);

            await Screen.FadeIn(500);

            CreateBaseMenu();
            Instance.NuiManager.SetFocus(true, true);
        }

        private static void CreateBaseMenu(bool update = false)
        {
            MenuBase menuBase = new();
            menuBase.Title = ClientConfiguration.Translation("mainMenu.label");

            MenuOption moCharacterRandomise = MenuOption.MenuOptionButton(ClientConfiguration.Translation("character.randomise.label"), ClientConfiguration.Translation("character.randomise.description"), "CharacterRandomise");
            menuBase.AddOption(moCharacterRandomise);

            MenuOption moDivider = MenuOption.MenuOptionDivider();
            menuBase.AddOption(moDivider);

            MenuOption moCharacterName = MenuOption.MenuOptionButton(ClientConfiguration.Translation("character.name.label"), ClientConfiguration.Translation("character.name.description"), "CharacterChangeName", ClientConfiguration.Translation("character.name.label.right"));
            menuBase.AddOption(moCharacterName);

            MenuOption moCharacterAppearance = MenuOption.MenuOptionMenu(ClientConfiguration.Translation("character.appearance.label"), ClientConfiguration.Translation("character.appearance.subtitle"), ClientConfiguration.Translation("character.appearance.description"));
            menuBase.AddOption(moCharacterAppearance);

            MenuOption moCharacterFace = MenuOption.MenuOptionMenu(ClientConfiguration.Translation("character.face.label"), ClientConfiguration.Translation("character.face.subtitle"), ClientConfiguration.Translation("character.face.description"), "CharacterCamera/Body");
            moCharacterAppearance.AddOption(moCharacterFace);

            MenuOption moCharacterBody = MenuOption.MenuOptionMenu(ClientConfiguration.Translation("character.body.label"), ClientConfiguration.Translation("character.body.subtitle"), ClientConfiguration.Translation("character.body.description"), "CharacterCamera/Body");
            moCharacterAppearance.AddOption(moCharacterBody);

            MenuOption moCharacterClothes = MenuOption.MenuOptionMenu(ClientConfiguration.Translation("character.clothes.label"), ClientConfiguration.Translation("character.clothes.subtitle"), ClientConfiguration.Translation("character.clothes.description"), "CharacterCamera/Body");
            moCharacterAppearance.AddOption(moCharacterClothes);

            Dictionary<string, Tuple<string, string, List<long>, long>> characterClothesOptions = new();
            Dictionary<string, Tuple<string, string, List<long>, long>> characterFaceOptions = new();
            Dictionary<string, Tuple<string, string, List<long>, long>> characterBodyOptions = new();

            characterFaceOptions.Add("CharacterSetComponent/Eyes", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.eyes.label"), ClientConfiguration.Translation("character.face.eyes.description"), _ped.Eyes, _ped.PedComponents.Eyes.Value));
            characterFaceOptions.Add("CharacterSetComponent/Head", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.head.label"), ClientConfiguration.Translation("character.face.head.description"), _ped.Heads, _ped.PedComponents.Head.Value));
            characterFaceOptions.Add("CharacterSetComponent/Teeth", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.teeth.label"), ClientConfiguration.Translation("character.face.teeth.description"), _ped.Teeth, _ped.PedComponents.Teeth.Value));
            
            if (_ped.IsMale)
                characterFaceOptions.Add("CharacterSetComponent/Beard", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.beard.label"), ClientConfiguration.Translation("character.face.beard.description"), _ped.CreatorBeards, _ped.PedComponents.Beard.Value));
            
            characterFaceOptions.Add("CharacterSetComponent/Hair", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.hair.label"), ClientConfiguration.Translation("character.face.hair.description"), _ped.CreatorHairs, _ped.PedComponents.Hair.Value));

            if (!_ped.IsMale)
                characterFaceOptions.Add("CharacterSetComponent/HairAccessories", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.face.hairAccessories.label"), ClientConfiguration.Translation("character.face.hairAccessories.description"), _ped.CreatorHairAccessories, _ped.PedComponents.HairAccessory.Value));

            characterBodyOptions.Add("CharacterSetComponent/BodyUpper", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.body.upper.label"), ClientConfiguration.Translation("character.body.upper.description"), _ped.BodiesUpper, _ped.PedComponents.BodyUpper.Value));
            characterBodyOptions.Add("CharacterSetComponent/BodyLower", new Tuple<string, string, List<long>, long>(ClientConfiguration.Translation("character.body.lower.label"), ClientConfiguration.Translation("character.body.lower.description"), _ped.BodiesLower, _ped.PedComponents.BodyLower.Value));
            
            characterClothesOptions.Add("CharacterSetComponent/Accessory", new Tuple<string, string, List<long>, long>("Accessory", "Change your character's accessories.", _ped.CreatorAccessories, _ped.PedComponents.Accessory.Value));
            characterClothesOptions.Add("CharacterSetComponent/Armor", new Tuple<string, string, List<long>, long>("Armor", "Change your character's armor.", _ped.CreatorArmor, _ped.PedComponents.Armor.Value));
            characterClothesOptions.Add("CharacterSetComponent/Badge", new Tuple<string, string, List<long>, long>("Badge", "Change your character's badge.", _ped.CreatorBadges, _ped.PedComponents.Badge.Value));
            characterClothesOptions.Add("CharacterSetComponent/Belt", new Tuple<string, string, List<long>, long>("Belt", "Change your character's belt.", _ped.CreatorBelts, _ped.PedComponents.Belt.Value));
            characterClothesOptions.Add("CharacterSetComponent/BeltBuckle", new Tuple<string, string, List<long>, long>("Belt Buckle", "Change your character's belt buckle.", _ped.CreatorBeltBuckles, _ped.PedComponents.BeltBuckle.Value));
            characterClothesOptions.Add("CharacterSetComponent/BootAccessory", new Tuple<string, string, List<long>, long>("Boot Accessory", "Change your character's boot accessory.", _ped.CreatorBootAccessories, _ped.PedComponents.BootAccessory.Value));
            characterClothesOptions.Add("CharacterSetComponent/Boots", new Tuple<string, string, List<long>, long>("Boots", "Change your character's boots.", _ped.CreatorBoots, _ped.PedComponents.Boots.Value));
            characterClothesOptions.Add("CharacterSetComponent/Chaps", new Tuple<string, string, List<long>, long>("Chaps", "Change your character's chaps.", _ped.CreatorChaps, _ped.PedComponents.Chaps.Value));
            characterClothesOptions.Add("CharacterSetComponent/Cloaks", new Tuple<string, string, List<long>, long>("Cloaks", "Change your character's cloak.", _ped.CreatorCloaks, _ped.PedComponents.Cloak.Value));
            characterClothesOptions.Add("CharacterSetComponent/Coats", new Tuple<string, string, List<long>, long>("Coats", "Change your character's coat.", _ped.CreatorCoats, _ped.PedComponents.Coat.Value));
            characterClothesOptions.Add("CharacterSetComponent/CoatsClosed", new Tuple<string, string, List<long>, long>("Coats Closed", "Change your character's closed coat.", _ped.CreatorCoatsClosed, _ped.PedComponents.CoatClosed.Value));

            if (_ped.IsMale)
                characterClothesOptions.Add("CharacterSetComponent/Dresses", new Tuple<string, string, List<long>, long>("Dresses", "Change your character's dresses.", _ped.CreatorDresses, _ped.PedComponents.Dresses.Value));

            characterClothesOptions.Add("CharacterSetComponent/EyeWear", new Tuple<string, string, List<long>, long>("Eye Wear", "Change your character's eye wear.", _ped.CreatorEyeWear, _ped.PedComponents.EyeWear.Value));
            characterClothesOptions.Add("CharacterSetComponent/Gauntlets", new Tuple<string, string, List<long>, long>("Gauntlets", "Change your character's gauntlets.", _ped.CreatorGauntlets, _ped.PedComponents.Gauntlet.Value));
            characterClothesOptions.Add("CharacterSetComponent/Gloves", new Tuple<string, string, List<long>, long>("Gloves", "Change your character's gloves.", _ped.CreatorGloves, _ped.PedComponents.Gloves.Value));
            characterClothesOptions.Add("CharacterSetComponent/Gunbelts", new Tuple<string, string, List<long>, long>("Gunbelts", "Change your character's gunbelts.", _ped.CreatorGunbelts, _ped.PedComponents.Gunbelt.Value));
            characterClothesOptions.Add("CharacterSetComponent/GunbeltAccessories", new Tuple<string, string, List<long>, long>("Gunbelt Accessories", "Change your character's gunbelt accessories.", _ped.CreatorGunbeltAccessories, _ped.PedComponents.GunbeltAccessory.Value));

            characterClothesOptions.Add("CharacterSetComponent/Hats", new Tuple<string, string, List<long>, long>("Hats", "Change your character's hats.", _ped.CreatorHats, _ped.PedComponents.Hat.Value));
            characterClothesOptions.Add("CharacterSetComponent/HolstersLeft", new Tuple<string, string, List<long>, long>("Holsters", "Change your character's holster.", _ped.CreatorHolstersLeft, _ped.PedComponents.HolstersLeft.Value));
            characterClothesOptions.Add("CharacterSetComponent/JewelryBracelets", new Tuple<string, string, List<long>, long>("Bracelets", "Change your character's bracelets.", _ped.CreatorJewelryBracelets, _ped.PedComponents.JewelryBracelets.Value));
            characterClothesOptions.Add("CharacterSetComponent/JewelryRingsLeft", new Tuple<string, string, List<long>, long>("Rings Left Hand", "Change your character's rings.", _ped.CreatorJewelryRingsLeft, _ped.PedComponents.JewelryRingsLeft.Value));
            characterClothesOptions.Add("CharacterSetComponent/JewelryRingsRight", new Tuple<string, string, List<long>, long>("Rings Right Hand", "Change your character's rings.", _ped.CreatorJewelryRingsRight, _ped.PedComponents.JewelryRingsRight.Value));
            characterClothesOptions.Add("CharacterSetComponent/Loadouts", new Tuple<string, string, List<long>, long>("Loadouts", "Change your character's loadouts.", _ped.CreatorLoadouts, _ped.PedComponents.Loadout.Value));
            characterClothesOptions.Add("CharacterSetComponent/Masks", new Tuple<string, string, List<long>, long>("Masks", "Change your character's masks.", _ped.CreatorMasks, _ped.PedComponents.Masks.Value));
            characterClothesOptions.Add("CharacterSetComponent/Neckties", new Tuple<string, string, List<long>, long>("Neckties", "Change your character's necktie.", _ped.CreatorNeckties, _ped.PedComponents.Neckties.Value));
            characterClothesOptions.Add("CharacterSetComponent/Neckwear", new Tuple<string, string, List<long>, long>("Neckwear", "Change your character's neckwear.", _ped.CreatorNeckwear, _ped.PedComponents.Neckwear.Value));
            characterClothesOptions.Add("CharacterSetComponent/Pants", new Tuple<string, string, List<long>, long>("Pants", "Change your character's pants.", _ped.CreatorPants, _ped.PedComponents.Pants.Value));
            characterClothesOptions.Add("CharacterSetComponent/Ponchos", new Tuple<string, string, List<long>, long>("Ponchos", "Change your character's poncho.", _ped.CreatorPonchos, _ped.PedComponents.Poncho.Value));
            characterClothesOptions.Add("CharacterSetComponent/Satchels", new Tuple<string, string, List<long>, long>("Satchels", "Change your character's satchels.", _ped.Satchels, _ped.PedComponents.Satchel.Value));
            characterClothesOptions.Add("CharacterSetComponent/Shirt", new Tuple<string, string, List<long>, long>("Shirt", "Change your character's shirt.", _ped.CreatorShirts, _ped.PedComponents.Shirt.Value));

            if (!_ped.IsMale)
                characterClothesOptions.Add("CharacterSetComponent/Skirts", new Tuple<string, string, List<long>, long>("Skirt", "Change your character's skirt.", _ped.CreatorSkirts, _ped.PedComponents.Skirt.Value));

            characterClothesOptions.Add("CharacterSetComponent/Spats", new Tuple<string, string, List<long>, long>("Spats", "Change your character's spats.", _ped.CreatorSpats, _ped.PedComponents.Spats.Value));
            characterClothesOptions.Add("CharacterSetComponent/Suspenders", new Tuple<string, string, List<long>, long>("Suspenders", "Change your character's suspenders.", _ped.CreatorSuspenders, _ped.PedComponents.Suspenders.Value));
            characterClothesOptions.Add("CharacterSetComponent/Vests", new Tuple<string, string, List<long>, long>("Vests", "Change your character's Vests.", _ped.CreatorVests, _ped.PedComponents.Vest.Value));

            foreach(KeyValuePair<string, Tuple<string, string, List<long>, long>> kvp in characterClothesOptions)
            {
                string endpoint = kvp.Key;
                string label = kvp.Value.Item1;
                string description = kvp.Value.Item2;
                List<long> list = kvp.Value.Item3;
                long currentValue = kvp.Value.Item4;
                MenuOption menuOption = MenuOption.MenuOptionList(label, description, endpoint, list, currentValue, "CharacterCamera/Main");
                moCharacterClothes.AddOption(menuOption);
            }

            foreach (KeyValuePair<string, Tuple<string, string, List<long>, long>> kvp in characterFaceOptions)
            {
                string endpoint = kvp.Key;
                string label = kvp.Value.Item1;
                string description = kvp.Value.Item2;
                List<long> list = kvp.Value.Item3;
                long currentValue = kvp.Value.Item4;
                MenuOption menuOption = MenuOption.MenuOptionList(label, description, endpoint, list, currentValue, "CharacterCamera/Face");
                moCharacterFace.AddOption(menuOption);
            }

            foreach (KeyValuePair<string, Tuple<string, string, List<long>, long>> kvp in characterBodyOptions)
            {
                string endpoint = kvp.Key;
                string label = kvp.Value.Item1;
                string description = kvp.Value.Item2;
                List<long> list = kvp.Value.Item3;
                long currentValue = kvp.Value.Item4;
                MenuOption menuOption = MenuOption.MenuOptionList(label, description, endpoint, list, currentValue, "CharacterCamera/Body");
                moCharacterBody.AddOption(menuOption);
            }


            MenuOption moDivider2 = MenuOption.MenuOptionDivider();
            menuBase.AddOption(moDivider2);

            MenuOption moCharacterSave = MenuOption.MenuOptionButton("Confirm", "Confirm and save your character.", "CharacterConfirm");
            menuBase.AddOption(moCharacterSave);

            if (!update)
                Instance.NuiManager.Set("character/CREATOR", menuBase);
            else
                Instance.NuiManager.Patch("character/CREATOR", menuBase);

            if (!update)
                Instance.NuiManager.Toggle("character/VISIBLE");
        }

        private static async Task OnNuiHandling()
        {
            Instance.WorldTime.ClockTimeOverride_2(7, 0, pauseClock: true);
            //if (IsPauseMenuActive() && !_hideNui)
            //{
            //    _hideNui = true;
            //    Instance.NuiManager.Toggle("character/VISIBLE");
            //}
            //else if (!IsPauseMenuActive() && _hideNui)
            //{
            //    _hideNui = false;
            //    Instance.NuiManager.Toggle("character/VISIBLE");
            //}
        }

        void Dispose()
        {
            Instance.DetachTickHandler(OnNuiHandling);
            Instance.WorldTime.ClearClockTimeOverride();

            DisplayHud(false);
            DisplayRadar(false);

            if (_ped is not null) _ped.Delete();

            Instance.NuiManager.SetFocus(false, false);

            Instance.NuiManager.Toggle("character/VISIBLE");

            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraLegs.Delete();

            World.SetWeatherFrozen(false);

            VorpAPI.SetStreamedTextureDictAsNoLongerNeeded("generic_textures");
        }
    }
}
