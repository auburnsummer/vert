﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace RDVertPlugin
{
    public static class PatchScnMenu
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnMenu), "Awake")]
        public static bool Awake(scnMenu __instance, ref Text[] ___optionsText, ref RectTransform ___optionsContainer)
        {
            // mess with the recttransform??

            // get an existing recttransform to use as a 'template' (e.g. it has the font set, etc)
            RectTransform firstChild = (RectTransform)___optionsContainer.GetChild(0);
            // create a clone of it...
            RectTransform ourNewOption = RectTransform.Instantiate(firstChild);
            // and now get the text associated with this recttransform.
            Text ourOptionText = ourNewOption.GetComponent<Text>();
            // the text is a localisation key, not the text directly.
            // to get our own things, we patch RDString as well.
            // this lets us hook into the same localisation system RD uses.
            ourOptionText.name = "VertMenuOption";
            // now we need to attach it to the same parent, and RD takes over from here.
            ourNewOption.SetParent(___optionsContainer);

            // i need this for some reason, not sure why
            ourOptionText.rectTransform.offsetMax = ourOptionText.rectTransform.offsetMax.WithX(0f);
            ourOptionText.rectTransform.offsetMin = ourOptionText.rectTransform.offsetMin.WithX(0f);

            // lastly, since we added a new option, this shifts things down.
            // for now, we can just add a shift like this to the container...
            // in the future, if more people start doing mods, we will need a "BaseMod" that gracefully
            // handles lots of menu options.
            ___optionsContainer.offsetMax = ___optionsContainer.offsetMax.WithY(12f);

            // if the prefix returns true, this tells bepinex to move onto the canonical function.
            return true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(scnMenu), "TransitionToScene")]
        public static void TransitionToScene(scnMenu instance, string scene)
        {
            throw new NotImplementedException("It's a stub");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnMenu), "SelectOption")]
        public static void SelectOption(
            scnMenu __instance,
            int ___currentOption,
            Text[] ___optionsText
           )
        {
            string name = ___optionsText[___currentOption].gameObject.name;
            Vert.Log.LogInfo(name);
            if (String.Equals(name, "VertMenuOption"))
            {
                Vert.Log.LogInfo("Entering VERT......");
                string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string resource in resourceNames)
                {
                    Vert.Log.LogInfo(resource);
                }
                // okay so you can't make scenes at runtime
                // there is a way to make scenes by importing an "AssetBundle" but it sounds complicated
                // instead we're going to hijack scnLogo
                Vert.NowHijacking = true;
                ConfigureRDEditorConstants.Configure();
                TransitionToScene(__instance, "scnEditor");
            } else
            {
                // we still want you to be able to quit out and go to normal editor.
                Vert.NowHijacking = false;
                ConfigureRDEditorConstants.Unconfigure();
            }
        }
    }
}