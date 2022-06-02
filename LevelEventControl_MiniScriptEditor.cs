using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
    public class LevelEventControl_MiniScriptEditor : LevelEventControl_Base
    {
        public new void Awake()
        {
            RDVertPlugin.Vert.Log.LogInfo("Awake");
			//base.Awake();
			RDVertPlugin.Vert.Log.LogInfo("1");

			if (this.image == null)
			{
				this.image = base.GetComponent<Image>();
			}
			RDVertPlugin.Vert.Log.LogInfo("2");

			this.rt = base.GetComponent<RectTransform>();
			RDVertPlugin.Vert.Log.LogInfo("3");
			RDVertPlugin.Vert.Log.LogInfo(this.image.name);
			RDVertPlugin.Vert.Log.LogInfo("4");
			this.initialColor = RDConstants.data.colorPalette[this.paletteColor].WithAlpha(this.image.color.a);
			RDVertPlugin.Vert.Log.LogInfo("5");
			this.go = base.gameObject;
		}

        public override void UpdateUIInternal()
        {
            RDVertPlugin.Vert.Log.LogInfo("UpdateUIInternal");
        }

        public override void ShowAsDeselected()
        {
			RDVertPlugin.Vert.Log.LogInfo("1");
			this.selected = false;
			RDVertPlugin.Vert.Log.LogInfo("2");
			this.isShownAsSelected = false;
			RDVertPlugin.Vert.Log.LogInfo("3");
			this.image.DOKill(true);
			RDVertPlugin.Vert.Log.LogInfo("4");
			Color color = (this.levelEvent.active ? this.initialColor : Color.gray).WithAlpha(this.deselectedAlpha);
			RDVertPlugin.Vert.Log.LogInfo("5");
			this.image.color = color;
			RDVertPlugin.Vert.Log.LogInfo("6");
			if (this.levelEvent.active)
			{
				float h;
				float s;
				float num;
				RDVertPlugin.Vert.Log.LogInfo("7");
				Color.RGBToHSV(this.image.color, out h, out s, out num);
				RDVertPlugin.Vert.Log.LogInfo("8");
				s = 0.05882353f;
				num *= 0.78431374f;
				this.border.color = Color.HSVToRGB(h, s, num);
				RDVertPlugin.Vert.Log.LogInfo("9");
				return;
			}
			RDVertPlugin.Vert.Log.LogInfo("10");
			this.border.color = Color.gray;
		}

	}
}
