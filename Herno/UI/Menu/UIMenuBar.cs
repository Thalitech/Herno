﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Veldrid;

namespace Herno.UI
{
    public class UIMenuBar : UIContainer
    {
        public UIMenuBar(IEnumerable<IUIComponent> children) : base(children) { }
        public UIMenuBar() : this(Enumerable.Empty<IUIComponent>()) { }

        public override void Render(CommandList cl)
        {
            if (ImGui.BeginMenuBar())
            {
                RenderChildren(cl);
                ImGui.EndMenuBar();
            }
        }
    }
}
