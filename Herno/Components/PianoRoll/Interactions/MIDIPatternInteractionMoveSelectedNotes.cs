﻿using ImGuiNET;
using Herno.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Herno.Components.PianoRoll.Interactions
{
  class MIDIPatternInteractionSelectionRectangle : MIDIPatternInteraction
  {
    Vector2d StartLocation { get; }

    public MIDIPatternInteractionSelectionRectangle(MIDIPatternConnect pianoRollPattern, Vector2d startLocation) : base(pianoRollPattern)
    {
      StartLocation = startLocation;
    }

    public override IPianoRollInteraction? DoInteraction()
    {
      base.DoInteraction();

      var pos = GetMousePos();
      var rect = new Rectangle(StartLocation, pos);
      PianoRollPattern.SetSelectionRectangle(rect);

      if(!ImGui.IsMouseDown(ImGuiMouseButton.Left))
      {
        PianoRollPattern.ClearSelectionRectangle();

        foreach(var n in PianoRollPattern.GetNotesInRectangle(rect))
        {
          PianoRollPattern.SelectNote(n);
        }

        return new MIDIPatternInteractionIdle(PianoRollPattern);
      }
      return null;
    }
  }
}
