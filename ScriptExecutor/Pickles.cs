using Miniscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RDVertPlugin
{
    static class Pickles
    {


        public static ValString PickleColor(Color c)
        {
            string hex = c.a == 1 ? ColorUtility.ToHtmlStringRGB(c) : ColorUtility.ToHtmlStringRGBA(c);
            return new ValString("#" + hex);
        }

        public static Color UnpickleColor(ValString s)
        {
            Color c = new Color(0, 0, 0, 1);
            bool ok = ColorUtility.TryParseHtmlString(s.value, out c);
            if (!ok)
            {
                Vert.Log.LogWarning(string.Format("Color {0} not parsable, defaulting to black", s.value));
            }
            return c;
        }

        public static ValMap PickleCamera(Camera c)
        {
            // return the camera's bg colour.
            // this is an anonymous intrinsic, only accessible via camera.backgroundColor
            var __cameraBgColor = Intrinsic.Create("");
            __cameraBgColor.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(PickleColor(c.backgroundColor), true);
            };
            ValMap cameraMap = new ValMap();
            cameraMap["backgroundColor"] = __cameraBgColor.GetFunc();

            cameraMap.assignOverride = (key, value) =>
            {
                switch (key.ToString())
                {
                    case "backgroundColor":
                        c.backgroundColor = UnpickleColor(value as ValString);
                        return true;
                }
                return false;
            };

            return cameraMap;
        }
    }
}
