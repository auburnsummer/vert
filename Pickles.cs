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

        public static double[] GetColorList(Color c)
        {
            return new double[] { c.r, c.g, c.b, c.a };
        }

        public static ValList PickleColor(Color c)
        {
            ValList mcolor = new ValList();
            foreach (double d in GetColorList(c))
            {
                mcolor.values.Add(new ValNumber(d));
            }
            return mcolor;
        }

        public static Color UnpickleColor(ValList v)
        {
            var values = v.values.Select(v => v.FloatValue()).ToArray();
            Color c = new Color(
                values[0],
                values[1],
                values[2],
                values[3]
            );
            return c;
        }

        public static ValMap PickleCamera(Camera c)
        {
            ValMap cameraMap = new ValMap();
            ValList backgroundColor = PickleColor(c.backgroundColor);
            cameraMap["backgroundColor"] = backgroundColor;

            cameraMap.assignOverride = (key, value) =>
            {
                switch (key.ToString())
                {
                    case "backgroundColor":
                        c.backgroundColor = UnpickleColor(value as ValList);
                        return true;
                }
                return false;
            };

            return cameraMap;
        }
    }
}
